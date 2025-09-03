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
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public override void OnConnected(NetConnection conn)
        {

        }

        public override void OnDisconnected(NetConnection conn)
        {
            Room? room = MasterServer.Lobby.GetRoomByConnection(conn);
            if (room != null)
            {
                MasterServer.Lobby.RemoveRoom(room.RoomId);
            }
        }

        protected virtual void HandleRoomConnectionRequest(RoomConnectionRequestPacket roomConnectionRequest, NetConnection conn)
        {
            Room room = MasterServer.Lobby.GetRoom(roomConnectionRequest.RoomId);
            string errorMessage = "";

            if(room != null)
            {
                room.Connected = true;
                room.Connection = conn;

                PacketMetadata data = new PacketMetadata();
                data.Add("AppIdentifier", _serverEndpoint.GetProfile().AppIdentifier);

                Send(new RoomConnectionResponsePacket()
                {
                    Success = true,
                    ErrorMessage = errorMessage,
                    Data = data
                }, conn);
            }
            else
            {
                Send(new RoomConnectionResponsePacket()
                {
                    Success = false,
                    ErrorMessage = "Room is not registered"
                }, conn);
            }             
        }
    }
}
