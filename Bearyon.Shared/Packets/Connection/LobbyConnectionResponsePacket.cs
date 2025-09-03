using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bearyon.Shared.Packets.Connection
{
    public struct LobbyConnectionResponsePacket : IPacket
    {
        public bool Success;
        public string ErrorMessage;
        public PacketMetadata Data;

        public void Serialize(NetOutgoingMessage om)
        {
            om.Write(Success);
            om.Write(ErrorMessage ?? string.Empty);

            bool hasData = Data != null && Data.Data.Count > 0;
            om.Write(hasData);

            if (hasData)
            {
                Data.Serialize(om);
            }
        }

        public void Deserialize(NetIncomingMessage im)
        {
            Success = im.ReadBoolean();
            ErrorMessage = im.ReadString();

            bool hasData = im.ReadBoolean();
            if (hasData)
            {
                Data = new PacketMetadata();
                Data.Deserialize(im);
            }
            else
            {
                Data = null;
            }
        }
    }
}
