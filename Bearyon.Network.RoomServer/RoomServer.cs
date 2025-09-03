using Lidgren.Network;
using Bearyon.Shared;

namespace Bearyon.Network.RoomServer
{
    public class RoomServer
    {
        private readonly ClientEndpoint _roomEndpoint;
        private readonly ServerEndpoint _gameEndpoint;

        public static ClientManager ClientManager { get; private set; }  
        
        public static string RoomId { get; private set; }
        public static int MasterPort { get; private set; }
        public static int GamePort { get; private set; }

        public static RoomServer Instance;

        public RoomServer(int masterPort, string roomId, int gamePort, RoomClientPacketHandler roomHandler, GameServerPacketHandler gameHandler)
        {
            Instance = this;

            //Setup the room client
            _roomEndpoint = new ClientEndpoint(roomHandler);
            _roomEndpoint.AddServer(new ServerProfile("Master", "bearyon_room_server", "127.0.0.1", masterPort));
            roomHandler.Attach(_roomEndpoint);

            _gameEndpoint = new ServerEndpoint("bearyon_game_server", gamePort, 64, gameHandler);            
            gameHandler.Attach(_gameEndpoint);

            ClientManager = new ClientManager();

            RoomId = roomId;
            MasterPort = masterPort;
            GamePort = gamePort;

            PacketRegistry.RegisterAllPackets();            
        }

        public void Start()
        {
            Console.WriteLine("[ROOM] Starting Room");

            Task roomTask = _roomEndpoint.Start();
            Task gameTask = _gameEndpoint.Start();

            _roomEndpoint.ConnectTo("Master");

            Console.WriteLine($"[GAME_SERVER] Listening on {GamePort}");

            Task.WaitAll(roomTask, gameTask);

            Stop();
        }

        public async Task Stop(int delaySeconds = 0)
        {
            _roomEndpoint.Disconnect();

            // Wait before stopping, if requested
            if (delaySeconds > 0)
            {
                Console.WriteLine($"[SERVER] Shutting down in {delaySeconds} seconds.");
                await Task.Delay(delaySeconds * 1000);
            }

            _roomEndpoint.Stop();
            _gameEndpoint.Stop();
        }     
    }
}
