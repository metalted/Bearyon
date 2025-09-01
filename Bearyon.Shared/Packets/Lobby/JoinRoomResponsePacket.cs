using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bearyon.Shared.Packets.Lobby
{
    public struct JoinRoomResponsePacket : IPacket
    {
        public bool Success;
        public string Error;
        public int RoomId;
        public int GamePort;

        public void Serialize(NetOutgoingMessage om)
        {
            om.Write(Success);
            om.Write(Error ?? string.Empty);
            om.Write(RoomId);
            om.Write(GamePort);
        }

        public void Deserialize(NetIncomingMessage im)
        {
            Success = im.ReadBoolean();
            Error = im.ReadString();
            RoomId = im.ReadInt32();
            GamePort = im.ReadInt32();
        }
    }    
}
