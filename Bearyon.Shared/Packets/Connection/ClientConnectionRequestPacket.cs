using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bearyon.Shared.Packets.Connection
{
    public struct ClientConnectionRequestPacket : IPacket
    {
        public string ClientUID;

        public void Serialize(NetOutgoingMessage om)
        {
            om.Write(ClientUID);
        }

        public void Deserialize(NetIncomingMessage im)
        {  
            ClientUID = im.ReadString();
        }
    }
}
