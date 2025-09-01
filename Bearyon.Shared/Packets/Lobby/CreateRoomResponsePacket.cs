using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bearyon.Shared.Packets.Lobby
{
    public struct CreateRoomResponsePacket : IPacket
    {
        public bool Success;
        public int RoomId;
        public string Error;

        public void Serialize(NetOutgoingMessage om)
        {
            om.Write(Success);
            om.Write(RoomId);
            om.Write(Error ?? string.Empty);
        }

        public void Deserialize(NetIncomingMessage im)
        {
            Success = im.ReadBoolean();
            RoomId = im.ReadInt32();
            Error = im.ReadString();
        }
    }
}
