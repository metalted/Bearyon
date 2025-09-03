using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bearyon.Shared.Packets.Lobby
{
    public struct RoomSummaryListPacket : IPacket
    {        
        public List<RoomSummary> Rooms;

        public void Serialize(NetOutgoingMessage om)
        {
            om.Write(Rooms.Count);
            foreach (RoomSummary room in Rooms)
            {
                om.Write(room.RoomId);
                om.Write(room.Name ?? string.Empty);
                om.Write(room.ClientCount);
                om.Write(room.MaxClients);
            }
        }

        public void Deserialize(NetIncomingMessage im)
        {
            int count = im.ReadInt32();
            Rooms = new List<RoomSummary>(count);
            for (int i = 0; i < count; i++)
            {
                RoomSummary s = new RoomSummary();
                s.RoomId = im.ReadString();
                s.Name = im.ReadString();
                s.ClientCount = im.ReadInt32();
                s.MaxClients = im.ReadInt32();
                Rooms.Add(s);
            }
        }
    }
}

