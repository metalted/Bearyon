using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bearyon.Shared.Packets.Lobby
{
    public struct CreateRoomRequestPacket : IPacket
    {
        public string Name;
        public int MaxPlayers;

        public void Serialize(NetOutgoingMessage om)
        {
            om.Write(Name);
            om.Write(MaxPlayers);
        }

        public void Deserialize(NetIncomingMessage im)
        {
            Name = im.ReadString();
            MaxPlayers = im.ReadInt32();
        }
    }
}
