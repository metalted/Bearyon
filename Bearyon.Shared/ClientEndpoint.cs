using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Bearyon.Shared
{    
    public class ClientEndpoint
    {        
        private Dictionary<string, ServerProfile> _servers;
        private NetClient _client;
        private readonly PacketHandler _handler;
        private volatile bool _running;
        private Task _task;
        public string ClientUID { get; private set; }

        public ServerProfile pendingConnection = null;
        public ServerProfile currentConnection = null;

        public ClientEndpoint(PacketHandler handler)
        {
            PacketRegistry.RegisterAllPackets();

            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            _handler.Attach(this);
            _servers = new Dictionary<string, ServerProfile>();   
            
            ClientUID = System.Guid.NewGuid().ToString();
        }

        public void AddServer(ServerProfile profile)
        {
            if (_servers.ContainsKey(profile.Name))
            {
                Console.WriteLine($"[CLIENT] Server with name {profile.Name} already exists.");
                return;
            }

            _servers.Add(profile.Name, profile);
        }

        public Task Start()
        {
            if (_running)
                return _task;

            _running = true;
            _task = Task.Run(RunLoop);

            return _task;
        }

        public async Task Stop()
        {
            _running = false;

            if(IsConnected())
            {
                _client.Shutdown("Stopping client");
            }

            if (_task != null)
                await _task;

            _client = null;
            _task = null;
            pendingConnection = null;
            currentConnection = null;
        }

        public void ConnectTo(string serverName)
        {
            if (!_servers.ContainsKey(serverName))
            {
                Console.WriteLine($"[CLIENT] Server with name {serverName} doesn't exist.");
                return;
            }

            ConnectTo(_servers[serverName]);
        }

        public void ConnectTo(ServerProfile profile)
        {
            pendingConnection = profile;

            if (IsConnected())
            {
                _client.Shutdown("Changing connection");
            }
            else
            {
                CheckPendingConnection();
            }
        }

        public bool IsConnected()
        {
            return _client != null && !(_client.ConnectionStatus == NetConnectionStatus.None || _client.ConnectionStatus == NetConnectionStatus.Disconnected);
        }        
        
        public bool IsRunning()
        {
            return _running;
        }

        public void CheckPendingConnection()
        {
            Console.WriteLine("[CLIENT] Checking pending connections...");
            if(pendingConnection == null)
            {
                Console.WriteLine("[CLIENT] No pending connections...");
                return;
            }

            Console.WriteLine($"[CLIENT] Connecting to {pendingConnection.Name}...");

            NetPeerConfiguration config = new NetPeerConfiguration(pendingConnection.AppIdentifier);
            NetClient newClient = new NetClient(config);
            newClient.Start();

            NetOutgoingMessage hail = newClient.CreateMessage($"Client connecting");
            newClient.Connect(pendingConnection.Host, pendingConnection.Port, hail);

            _client = newClient;
        }

        public void SetCurrentConnection(string appIdentifier)
        {
            foreach (var kvp in _servers)
            {
                if (kvp.Value.AppIdentifier == appIdentifier)
                {
                    pendingConnection = null;
                    currentConnection = kvp.Value;
                    return;
                }
            }

            Console.WriteLine($"[CLIENT] No server found with appIdentifier {appIdentifier}.");
        }

        public void Disconnect()
        {
            if (IsConnected())
            {
                _client.Shutdown("Disconnect");
            }
        }

        private void RunLoop()
        {
            while (_running)
            {
                NetClient client = _client;
                if (_client != null)
                {
                    NetIncomingMessage im;
                    while ((im = client.ReadMessage()) != null)
                    {
                        switch (im.MessageType)
                        {
                            case NetIncomingMessageType.StatusChanged:
                                NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                                Console.WriteLine("[CLIENT] Status: " + status);

                                if (status == NetConnectionStatus.Connected)
                                {
                                    _handler.OnConnected(im.SenderConnection);
                                }
                                else if (status == NetConnectionStatus.Disconnected ||
                                         status == NetConnectionStatus.None)
                                {
                                    _client = null;
                                    currentConnection = null;
                                    _handler.OnDisconnected(im.SenderConnection);                                    
                                }
                                break;
                            case NetIncomingMessageType.Data:
                                if (PacketUtility.TryUnpack(im, out IPacket packet))
                                {
                                    _handler.HandlePacket(packet, im.SenderConnection);
                                }
                                break;
                        }

                        client.Recycle(im);
                    }
                }

                Thread.Sleep(10);
            }
        }

        public void Send<T>(T packet) where T : struct, IPacket
        {
            if (_client == null || _client.ConnectionStatus != NetConnectionStatus.Connected)
            {
                Console.WriteLine($"[CLIENT] Can't send packet, client not connected.");
                return;
            }

            NetOutgoingMessage om = _client.CreateMessage();
            PacketUtility.Pack(packet, om);
            _client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
