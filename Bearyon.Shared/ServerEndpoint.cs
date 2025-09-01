using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bearyon.Shared
{
    public class ServerEndpoint
    {
        private readonly NetServer _server;
        private readonly PacketHandler _handler;
        private volatile bool _running;

        public int Port { get; }

        public ServerEndpoint(string appIdentifier, int port, int maxConnections, PacketHandler handler)
        {
            Port = port;
            _handler = handler;

            var config = new NetPeerConfiguration(appIdentifier)
            {
                Port = port,
                MaximumConnections = maxConnections
            };
            _server = new NetServer(config);
        }

        public Task Start()
        {
            _server.Start();
            _running = true;
            return Task.Run(() => RunLoop());
        }

        public void Stop()
        {
            _running = false;
            _server.Shutdown("Server shutting down");
        }

        private void RunLoop()
        {
            while (_running)
            {
                NetIncomingMessage im;
                while ((im = _server.ReadMessage()) != null)
                {
                    switch (im.MessageType)
                    {
                        case NetIncomingMessageType.Data:
                            if (PacketUtility.TryUnpack(im, out IPacket packet))
                            {
                                _handler.HandlePacket(packet, im.SenderConnection);
                            }
                            break;

                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                            string reason = im.ReadString(); // Lidgren usually sends a reason after status

                            Console.WriteLine($"[SERVER] {im.SenderEndPoint} -> {status} ({reason})");

                            if (status == NetConnectionStatus.Connected)
                            {
                                // user just connected
                                // register them in your ClientManager
                                _handler.OnConnected(im.SenderConnection);
                            }
                            else if (status == NetConnectionStatus.Disconnected)
                            {
                                // user disconnected
                                // remove them from ClientManager
                                _handler.OnDisconnected(im.SenderConnection);
                            }
                            break;

                        // optionally catch other message types if you care
                        default:
                            // ignore
                            break;
                    }

                    _server.Recycle(im);
                }

                Thread.Sleep(10);
            }
        }

        public void Send<T>(T packet, NetConnection to) where T : struct, IPacket
        {
            NetOutgoingMessage om = _server.CreateMessage();
            PacketUtility.Pack(packet, om);
            _server.SendMessage(om, to, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
