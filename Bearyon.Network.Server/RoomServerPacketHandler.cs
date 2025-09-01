using Bearyon.Shared;
using Bearyon.Shared.Packets.Connection;
using Bearyon.Shared.Packets.Lobby;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Bearyon.Network.Server
{
    public class RoomServerPacketHandler : PacketHandler
    {
        public override void HandlePacket(IPacket packet, NetConnection conn)
        {
            Console.WriteLine("[ROOM_SERVER] Received packet of type: " + packet.GetType().Name);

            if (packet is RoomConnectionRequestPacket roomConnectionRequest)
            {
                HandleRoomConnectionRequest(roomConnectionRequest, conn);
            }
            else
            {
                HandleCustomPacket(packet, conn);
            }
        }

        protected virtual void HandleRoomConnectionRequest(RoomConnectionRequestPacket roomConnectionRequest, NetConnection conn)
        {
            Room room = MasterServer.Lobby.GetRoom(roomConnectionRequest.RoomId);

            if (room != null)
            {
                room.Connected = true;
                room.Connection = conn;
            }
        }
    }
}
