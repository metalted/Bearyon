using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bearyon.Shared
{
    public class PacketHandler
    {
        protected ServerEndpoint _serverEndpoint { get; private set; }
        protected ClientEndpoint _clientEndpoint { get; private set; }

        public virtual void OnConnected(NetConnection conn)
        {

        }

        public virtual void OnDisconnected(NetConnection conn)
        {

        }

        public void Attach(ServerEndpoint serverEndpoint)
        {
            _serverEndpoint = serverEndpoint;
        }

        public void Attach(ClientEndpoint clientEndpoint)
        {
            _clientEndpoint = clientEndpoint;
        }

        public virtual void HandlePacket(IPacket packet, NetConnection conn) { }
        protected virtual void HandleCustomPacket(IPacket packet, NetConnection conn) { }
        protected void Send<T>(T packet, NetConnection to = null) where T : struct, IPacket 
        {
            if(_serverEndpoint != null)
            {
                if(to == null)
                {
                    throw new NullReferenceException("NetConnection is required for server endpoint.");
                }
                _serverEndpoint.Send(packet, to);
            }
            else if(_clientEndpoint != null)
            {
                _clientEndpoint.Send(packet);
            }
            else
            {
                throw new NullReferenceException("No endpoint set for this handler.");
            }
        }
    }
}
