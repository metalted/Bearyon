using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bearyon.Shared.Packets.Utilities
{
    public struct ConsoleCommandPacket : IPacket
    {
        public string Command;
        public string[] Parameters;

        public void Serialize(NetOutgoingMessage om)
        {
            om.Write(Command ?? string.Empty);
            om.Write(Parameters.Length);
            foreach (string p in Parameters)
                om.Write(p ?? string.Empty);
        }

        public void Deserialize(NetIncomingMessage im)
        {
            Command = im.ReadString();
            int count = im.ReadInt32();
            Parameters = new string[count];
            for (int i = 0; i < count; i++)
                Parameters[i] = im.ReadString();
        }
    }
}
