using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bearyon.Shared.Packets.Connection
{
    public struct RoomConnectionResponsePacket : IPacket
    {
        public bool Success;
        public int RoomId;
        public int ClientId;
        public string ErrorMessage;        

        public void Serialize(NetOutgoingMessage om)
        {
            om.Write(Success);
            om.Write(RoomId);
            om.Write(ClientId);
            om.Write(ErrorMessage ?? string.Empty);            
        }

        public void Deserialize(NetIncomingMessage im)
        {
            Success = im.ReadBoolean();
            RoomId = im.ReadInt32();
            ClientId = im.ReadInt32();
            ErrorMessage = im.ReadString();           
        }
    }
}
