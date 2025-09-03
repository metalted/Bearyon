using Bearyon.Shared;
using Bearyon.Shared.Packets.Connection;
using Bearyon.Shared.Packets.Lobby;
using Lidgren.Network;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bearyon.Network.Server
{
    public class LobbyServerPacketHandler : PacketHandler
    {
        public override void HandlePacket(IPacket packet, NetConnection conn)
        {
            Console.WriteLine("[LOBBY_SERVER] Received packet of type: " + packet.GetType().Name);

            if (packet is ClientConnectionRequestPacket clientConnectionRequest)
            {
                HandleClientConnectionRequest(clientConnectionRequest, conn);
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
            ClientInfo clientInfo = MasterServer.ClientManager.GetByConnection(conn);
            MasterServer.ClientManager.RemoveClient(clientInfo.ClientUID);
        }

        protected virtual void HandleClientConnectionRequest(ClientConnectionRequestPacket clientConnectionRequest, NetConnection conn)
        {
            ClientInfo clientInfo;
            string errorMessage;
            
            if(MasterServer.ClientManager.RegisterClient(clientConnectionRequest.ClientUID, conn, out clientInfo, out errorMessage))
            {
                PacketMetadata data = new PacketMetadata();
                data.Add("AppIdentifier", _serverEndpoint.GetProfile().AppIdentifier);

                Send(new LobbyConnectionResponsePacket()
                {
                    Success = true,
                    ErrorMessage = errorMessage,
                    Data = data
                }, conn);
            }
            else
            {
                Send(new LobbyConnectionResponsePacket()
                {
                    Success = false,
                    ErrorMessage = errorMessage
                }, conn);
            }
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
            //Room roomToJoin = MasterServer.Lobby.GetRoom(joinRoom.RoomId);
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

            if(roomToJoin.GetClientCount() >= roomToJoin.MaxClients)
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
