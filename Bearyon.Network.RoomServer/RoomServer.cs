using Lidgren.Network;
using Bearyon.Shared;

namespace Bearyon.Network.RoomServer
{
    public class RoomServer
    {
        private readonly ClientEndpoint _roomEndpoint;
        private readonly ServerEndpoint _gameEndpoint;

        public static ClientManager ClientManager { get; private set; }  
        
        public static int RoomId { get; private set; }
        public static int RoomPort { get; private set; }
        public static int GamePort { get; private set; }

        public RoomServer(int roomId, int roomPort, int gamePort, RoomClientPacketHandler roomHandler, GameServerPacketHandler gameHandler)
        {
            _roomEndpoint = new ClientEndpoint(roomHandler);
            _gameEndpoint = new ServerEndpoint("bearyon_game_server", gamePort, 64, gameHandler);

            roomHandler.Attach(_roomEndpoint);
            gameHandler.Attach(_gameEndpoint);

            ClientManager = new ClientManager();

            RoomId = roomId;
            RoomPort = roomPort;
            GamePort = gamePort;

            PacketRegistry.RegisterAllPackets();            
        }

        public void Start()
        {
            Console.WriteLine("[ROOM] Starting Room");

            Task roomTask = _roomEndpoint.Start();
            Task gameTask = _gameEndpoint.Start();

            _roomEndpoint.ConnectTo("bearyon_room_server", "127.0.0.1", RoomPort);

            Console.WriteLine($"[GAME_SERVER] Listening on {GamePort}");

            Task.WaitAll(roomTask, gameTask);

            Stop();
        }

        public void Stop()
        {
            _roomEndpoint.Stop();
            _gameEndpoint.Stop();
        }        
    }
}
