using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bearyon.Shared.Packets.Utilities
{
    public struct StringMessagePacket : IPacket
    {
        public string Message;

        public StringMessagePacket(string message)
        {
            this.Message = message;
        }

        public void Serialize(NetOutgoingMessage om)
        {
            om.Write(Message ?? string.Empty);
        }

        public void Deserialize(NetIncomingMessage im)
        {
            Message = im.ReadString();
        }
    }
}
