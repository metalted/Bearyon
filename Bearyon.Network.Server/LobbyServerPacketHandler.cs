using Bearyon.Shared;
using Bearyon.Shared.Packets.Connection;
using Bearyon.Shared.Packets.Lobby;
using Lidgren.Network;

namespace Bearyon.Network.Server
{
    public class LobbyServerPacketHandler : PacketHandler
    {
        public override void HandlePacket(IPacket packet, NetConnection conn)
        {
            Console.WriteLine("[LOBBY_SERVER] Received packet of type: " + packet.GetType().Name);

            if (packet is ClientConnectionRequestPacket hello)
            {
                HandleClientHello(hello, conn);
            }
            else if (packet is ListRoomRequestPacket listRooms)
            {
                HandleListRoomsRequest(listRooms, conn);
            }
            else if(packet is CreateRoomRequestPacket createRoom)
            {
                HandleCreateRoomRequest(createRoom, conn);
            }
            else if(packet is JoinRoomRequestPacket joinRoom)
            {
                HandleJoinRoomRequest(joinRoom, conn);
            }
            else
            {
                HandleCustomPacket(packet, conn);
            }
        }

        public override void OnDisconnected(NetConnection conn)
        {
            ClientInfo ci = MasterServer.ClientManager.GetByConnection(conn);
            MasterServer.ClientManager.RemoveClient(ci.ClientId);
        }

        protected virtual void HandleClientHello(ClientConnectionRequestPacket hello, NetConnection conn)
        {
            //Save and check the user here.
            ClientInfo ci = MasterServer.ClientManager.RegisterClient(conn);

            Send(new LobbyConnectionResponsePacket()
            {
                Success = true,
                ClientId = ci.ClientId,
                ErrorMessage = ""
            }, conn);
        }

        protected virtual void HandleListRoomsRequest(ListRoomRequestPacket listRooms, NetConnection conn)
        {
            Send(new RoomSummaryListPacket()
            {
                Rooms = MasterServer.Lobby.GetRoomSummaries()
            }, conn);
        }

        protected virtual void HandleCreateRoomRequest(CreateRoomRequestPacket createRoom, NetConnection conn)
        {
            Room room = MasterServer.Lobby.CreateRoom(createRoom.Name, createRoom.MaxPlayers);

            Send(new CreateRoomResponsePacket()
            {
                Success = true,
                RoomId = room.RoomId
            }, conn);
        }

        protected virtual void HandleJoinRoomRequest(JoinRoomRequestPacket joinRoom, NetConnection conn)
        {
            Room roomToJoin = MasterServer.Lobby.GetRoom(joinRoom.RoomId);

            if (roomToJoin == null)
            {
                Send(new JoinRoomResponsePacket()
                {
                    Success = false,
                    Error = $"Room {joinRoom.RoomId} not found."
                }, conn);
                return;
            }

            if(!roomToJoin.Connected)
            {
                Send(new JoinRoomResponsePacket()
                {
                    Success = false,
                    Error = $"Room not ready."
                }, conn);
                return;
            }

            if(roomToJoin.CurrentPlayers >= roomToJoin.MaxPlayers)
            {
                Send(new JoinRoomResponsePacket()
                {
                    Success = false,
                    Error = $"Room is full."
                }, conn);
                return;
            }

            Send(new JoinRoomResponsePacket()
            {
               Success = true,
               RoomId = roomToJoin.RoomId,
               GamePort = roomToJoin.GamePort              
            }, conn);
        }
    }
}
