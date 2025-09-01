using Bearyon.Shared;
using Bearyon.Shared.Packets.Connection;
using Bearyon.Shared.Packets.Lobby;
using Bearyon.Shared.Packets.Utilities;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

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

            _clientEndpoint.Location = ClientLocation.None;

            _clientEndpoint.CheckPendingConnection();
        }

        public virtual void SendClientConnectionRequest()
        {
            Send(new ClientConnectionRequestPacket()
            {
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

        public virtual void SendJoinRoomRequest(int roomId)
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
                _clientEndpoint.ClientID = packet.ClientId;
                _clientEndpoint.Location = ClientLocation.Lobby;
                Console.WriteLine("[CLIENT] ClientId = " + _clientEndpoint.ClientID);
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
                    Console.WriteLine($"RoomId: {s.RoomId} | Name: {s.Name} | MaxPlayers: {s.MaxPlayers} | Players: {s.PlayerCount} )");
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

        protected virtual void HandleJoinRoomResponse(JoinRoomResponsePacket packet, NetConnection conn)
        {
            Console.WriteLine($"[CLIENT] ReceivedJoinRoomResponse.");

            if (packet.Success)
            {
                Console.WriteLine($"[CLIENT] Success: Joining Room");
                _clientEndpoint.ConnectTo("bearyon_game_server", "127.0.0.1", packet.GamePort);
            }
            else
            {
                Console.WriteLine($"[CLIENT] Error: {packet.Error}");
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
                _clientEndpoint.ClientID = packet.ClientId;
                _clientEndpoint.Location = ClientLocation.Game;
                Console.WriteLine("[CLIENT] ClientID = " + _clientEndpoint.ClientID);
            }
            else
            {
                Console.WriteLine("[CLIENT] Connection refused: " + packet.ErrorMessage);
            }
        }
    }
}
