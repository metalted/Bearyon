using Bearyon.Shared;
using Bearyon.Network.RoomServer;

namespace Bearyon.Room
{
    public class Program
    {
        static void Main(string[] args)
        {
            int roomId = int.Parse(args[0]);
            string roomName = args[1];
            int maxPlayers = int.Parse(args[2]);
            int roomPort = int.Parse(args[3]);
            int gamePort = int.Parse(args[4]);

            PacketRegistry.RegisterAllPackets();
            
            RoomClientPacketHandler roomClientPacketHandler = new RoomClientPacketHandler();
            GameServerPacketHandler gameServerPacketHandler = new GameServerPacketHandler();            

            RoomServer server = new RoomServer(roomId, roomPort, gamePort, roomClientPacketHandler, gameServerPacketHandler);
            server.Start();
        }
    }
}
