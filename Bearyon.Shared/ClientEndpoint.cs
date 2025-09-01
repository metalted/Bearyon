using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bearyon.Shared
{    public enum ClientLocation { None, Lobby, Game }

    public class ClientEndpoint
    {        
        public ClientLocation Location = ClientLocation.None;

        private NetClient _client;
        private readonly PacketHandler _handler;
        private volatile bool _running;
        private string _host;
        private int _port;
        private Task _task;
        public int ClientID { get; set; }
        public (string, string, int) pendingConnection = ("","",0);

        public ClientEndpoint(PacketHandler handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public Task Start()
        {
            if (_running)
                return _task;

            _running = true;
            _task = Task.Run(RunLoop);

            return _task;
        }
        
        public bool IsRunning()
        {
            return _running;
        }

        public async Task Stop()
        {
            _running = false;

            if (_client != null &&
                !(_client.ConnectionStatus == NetConnectionStatus.None ||
                  _client.ConnectionStatus == NetConnectionStatus.Disconnected))
            {
                _client.Shutdown("Stopping client");
            }

            // Wait for RunLoop to finish before nuking task reference
            if (_task != null)
                await _task;

            _client = null;
            _task = null;
            _host = "";
            _port = -1;
        }

        public void ConnectTo(string appIdentifier, string host, int port)
        {
            pendingConnection = (appIdentifier, host, port);

            if (_client != null && !(_client.ConnectionStatus == NetConnectionStatus.None || _client.ConnectionStatus == NetConnectionStatus.Disconnected))
            {
                _client.Shutdown("Changing connection");
            }
            else
            {
                CheckPendingConnection();
            }                      
        }

        public void CheckPendingConnection()
        {
            Console.WriteLine("Checking pending connections...");
            if(pendingConnection.Item1 == "")
            {
                Console.WriteLine("No pending connections...");
                return;
            }

            Console.WriteLine("Connecting...");

            string appIdentifier = pendingConnection.Item1;
            string host = pendingConnection.Item2;
            int port = pendingConnection.Item3;
            pendingConnection = ("", "", 0);

            var config = new NetPeerConfiguration(appIdentifier);
            var newClient = new NetClient(config);
            newClient.Start();

            _host = host;
            _port = port;

            NetOutgoingMessage hail = newClient.CreateMessage("Client connecting...");
            newClient.Connect(_host, _port, hail);

            _client = newClient;
        }

        public void Disconnect()
        {
            //Lock client here or something ? 
            if (_client != null && !(_client.ConnectionStatus == NetConnectionStatus.None || _client.ConnectionStatus == NetConnectionStatus.Disconnected))
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

                Thread.Sleep(10); // small sleep to reduce CPU load but keep it responsive
            }
        }

        public void Send<T>(T packet) where T : struct, IPacket
        {
            if (_client == null || _client.ConnectionStatus != NetConnectionStatus.Connected)
            {
                throw new InvalidOperationException("Client is not connected.");
            }

            NetOutgoingMessage om = _client.CreateMessage();
            PacketUtility.Pack(packet, om);
            _client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
