using System;
using System.Threading;
using Lidgren.Network;
using Bearyon.Shared;

namespace Bearyon.Network.Server
{
    public class MasterServer
    {
        private readonly ServerEndpoint _lobbyEndpoint;
        private readonly ServerEndpoint _roomEndpoint;

        public static ClientManager ClientManager { get; private set; }
        public static Lobby Lobby { get; private set; }
        public static int LobbyPort { get; private set; }
        public static int RoomPort { get; private set; }

        public MasterServer(int lobbyPort, int roomPort, LobbyServerPacketHandler lobbyHandler, RoomServerPacketHandler roomHandler)
        {
            _lobbyEndpoint = new ServerEndpoint("bearyon_lobby_server", lobbyPort, 64, lobbyHandler);
            _roomEndpoint = new ServerEndpoint("bearyon_room_server", roomPort, 32, roomHandler);

            lobbyHandler.Attach(_lobbyEndpoint);
            roomHandler.Attach(_roomEndpoint);

            ClientManager = new ClientManager();
            Lobby = new Lobby();

            LobbyPort = lobbyPort;
            RoomPort = roomPort;

            PacketRegistry.RegisterAllPackets();
        }

        public void Start()
        {
            Console.WriteLine("[MASTER] Starting Bearyon");
            ConsoleUtils.PrintLogo();

            Task lobbyTask = _lobbyEndpoint.Start();
            Task roomTask = _roomEndpoint.Start();

            Console.WriteLine($"[LOBBY_SERVER] Listening on {LobbyPort}");
            Console.WriteLine($"[ROOM_SERVER] Listening on {RoomPort}");

            Task.WaitAll(lobbyTask, roomTask);

            Stop();
        }

        public void Stop()
        {
            _lobbyEndpoint.Stop();
            _roomEndpoint.Stop();
        }
    }
}
