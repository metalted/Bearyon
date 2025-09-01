using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bearyon.Shared.Packets.Connection
{
    public struct PingPacket : IPacket
    {
        public void Serialize(NetOutgoingMessage om)
        {
        }

        public void Deserialize(NetIncomingMessage im)
        {
        }
    }
}
