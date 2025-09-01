using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bearyon.Shared.Packets.Lobby
{
    public struct ListRoomRequestPacket : IPacket
    {
        public void Serialize(NetOutgoingMessage om) { }
        public void Deserialize(NetIncomingMessage im) { }
    }
}
