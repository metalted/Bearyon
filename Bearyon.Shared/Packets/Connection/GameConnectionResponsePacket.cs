using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bearyon.Shared.Packets.Connection
{
    public struct GameConnectionResponsePacket : IPacket
    {
        public bool Success;
        public int ClientId;
        public string ErrorMessage;

        public void Serialize(NetOutgoingMessage om)
        {
            om.Write(Success);
            om.Write(ClientId);
            om.Write(ErrorMessage ?? string.Empty);
        }

        public void Deserialize(NetIncomingMessage im)
        {
            Success = im.ReadBoolean();
            ClientId = im.ReadInt32();
            ErrorMessage = im.ReadString();
        }
    }
}
