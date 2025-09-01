using Bearyon.Shared;
using Bearyon.Shared.Packets.Connection;
using Bearyon.Shared.Packets.Lobby;
using Bearyon.Shared.Packets.Utilities;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;

namespace Bearyon.Network.Client
{    
    public class Client
    {
        private ClientEndpoint _endpoint;

        public Client(ClientPacketHandler handler)
        {
            _endpoint = new ClientEndpoint(handler);
            handler.Attach(_endpoint);
            PacketRegistry.RegisterAllPackets();            
        }

        public void Start()
        {
            _endpoint.Start();
        }     
        
        public void ConnectTo(string appIdentifier, string host, int port)
        {
            if(!_endpoint.IsRunning())
            {
                Console.WriteLine("Client endpoint is not running. Use Client.Start()");
                return;
            }

            _endpoint.ConnectTo(appIdentifier, host, port);
        }

        public void Disconnect()
        {
            _endpoint.Disconnect();
        }

        public ClientLocation GetLocation()
        {
            return _endpoint.Location;
        }
    }
}
