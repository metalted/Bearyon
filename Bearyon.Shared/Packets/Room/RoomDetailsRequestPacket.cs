using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bearyon.Shared.Packets.Room
{
    public struct RoomDetailsRequestPacket : IPacket
    {
        public string RoomId;

        public void Serialize(NetOutgoingMessage om)
        {
            om.Write(RoomId);
        }

        public void Deserialize(NetIncomingMessage im)
        {
            RoomId = im.ReadString();
        }
    }
}
