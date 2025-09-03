using Bearyon.Shared;
using Bearyon.Shared.Packets.Connection;
using Bearyon.Shared.Packets.Lobby;
using Bearyon.Shared.Packets.Utilities;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

/*This is where we start, so here will be the todo
 
- Players need better profiles, the server needs to know about players, what they are doin.
- Players can have metadata
- ClientIds across the whole network must be the same.
- Clients need unique Ids.

- Rooms must communicate with the masterserver more to update playercount etc
- Rooms must have a status like private and open
- Room can have metadata
- Rooms must have a password protection
- Rooms need unique IDs
- Room names
- Player tracking accross rooms


- Room chat
- Global chat

- Kick and ban players from a server
- Discord login or some other form of authentication
- Built in voice chat, proximity chat is best

- Room events like player joined, player left etc.
- Auto cleanup empty rooms
- Master Server Announcements
*/

namespace Bearyon.Network.Client
{
    public class ClientPacketHandler : PacketHandler
    {
        public override void HandlePacket(IPacket packet, NetConnection conn)
        {
            if (packet is LobbyConnectionResponsePacket lobbyConnectionResponse)
            {
                HandleLobbyConnectionResponse(lobbyConnectionResponse, conn);
            }
            else if (packet is RoomSummaryListPacket lobbySummary)
            {
                HandleRoomSummaryList(lobbySummary, conn);
            }
            else if (packet is CreateRoomResponsePacket createRoom)
            {
                HandleCreateRoomResponse(createRoom, conn);
            }
            else if (packet is JoinRoomResponsePacket joinRoom)
            {
                HandleJoinRoomResponse(joinRoom, conn);
            }
            else if(packet is GameConnectionResponsePacket gameConnectionResponse)
            {
                HandleGameConnectionResponse(gameConnectionResponse, conn);
            }
            else if (packet is StringMessagePacket)
            {
                HandleStringMessage((StringMessagePacket)packet, conn);
            }
        }

        public override void OnConnected(NetConnection conn)
        {
            SendClientConnectionRequest();
        }

        public override void OnDisconnected(NetConnection conn)
        {
            Console.WriteLine($"[CLIENT] Disconnected");
            _clientEndpoint.CheckPendingConnection();
        }

        public virtual void SendClientConnectionRequest()
        {
            Send(new ClientConnectionRequestPacket()
            {
                ClientUID = _clientEndpoint.ClientUID
            });
            Console.WriteLine($"[CLIENT] Sent ClientConnectionRequest");
        }

        public virtual void SendListLobbiesRequest()
        {
            Send(new ListRoomRequestPacket());
            Console.WriteLine("[CLIENT] Sent ListLobbiesRequest");
        }

        public virtual void SendCreateRoomRequest(string name, int maxPlayers)
        {
            Send(new CreateRoomRequestPacket
            {
                Name = name,
                MaxPlayers = maxPlayers
            });

            Console.WriteLine($"[CLIENT] Requested CreateRoom: {name} ({maxPlayers} players)");
        }

        public virtual void SendJoinRoomRequest(string roomId)
        {
            Send(new JoinRoomRequestPacket
            {
                RoomId = roomId
            });
            Console.WriteLine($"[CLIENT] Requested JoinRoom: {roomId}");
        }

        protected virtual void HandleLobbyConnectionResponse(LobbyConnectionResponsePacket packet, NetConnection conn)
        {
            if (packet.Success)
            {
                _clientEndpoint.SetCurrentConnection(packet.Data.Get("AppIdentifier"));
                Console.WriteLine($"[CLIENT] Connection accepted from {packet.Data.Get("AppIdentifier")}");
            }
            else
            {
                Console.WriteLine("[CLIENT] Connection refused: " + packet.ErrorMessage);
                _clientEndpoint.Disconnect();
            }
        }

        protected virtual void HandleRoomSummaryList(RoomSummaryListPacket roomSummaryList, NetConnection conn)
        {
            Console.WriteLine("[CLIENT] Received RoomSummaryList");
            Console.WriteLine("Room Summary List:");
            if (roomSummaryList.Rooms.Count > 0)
            {
                foreach (RoomSummary s in roomSummaryList.Rooms)
                {
                    Console.WriteLine($"RoomId: {s.RoomId} | Name: {s.Name} | MaxClients: {s.MaxClients} | Clients: {s.ClientCount} )");
                }
            }
            else
            {
                Console.WriteLine("No active rooms...");
            }
        }

        protected virtual void HandleCreateRoomResponse(CreateRoomResponsePacket packet, NetConnection conn)
        {
            Console.WriteLine($"[CLIENT] Received CreateRoomResponse: Room created with id: {packet.RoomId}");
        }

        protected virtual void HandleJoinRoomResponse(JoinRoomResponsePacket joinRoomResponse, NetConnection conn)
        {
            Console.WriteLine($"[CLIENT] ReceivedJoinRoomResponse.");

            if (joinRoomResponse.Success)
            {
                Console.WriteLine($"[CLIENT] Success: Joining Room");
                _clientEndpoint.AddServer(new ServerProfile("Game", "bearyon_game_server", "127.0.0.1", joinRoomResponse.GamePort));
                _clientEndpoint.ConnectTo("Game");
            }
            else
            {
                Console.WriteLine($"[CLIENT] Error: {joinRoomResponse.Error}");
            }
        }

        protected virtual void HandleStringMessage(StringMessagePacket packet, NetConnection conn)
        {
            Console.WriteLine("[CLIENT] Received StringMessage: " + packet.Message);
        }

        protected virtual void HandleGameConnectionResponse(GameConnectionResponsePacket packet, NetConnection conn)
        {
            Console.WriteLine("[CLIENT] Received GameConnectionResponse");

            if (packet.Success)
            {
                _clientEndpoint.SetCurrentConnection(packet.Data.Get("AppIdentifier"));
                Console.WriteLine($"[CLIENT] Connection accepted from {packet.Data.Get("AppIdentifier")}");
            }
            else
            {
                Console.WriteLine("[CLIENT] Connection refused: " + packet.ErrorMessage);

                //Connect back to lobby
                _clientEndpoint.ConnectTo("Lobby");
                
            }
        }
    }
}
