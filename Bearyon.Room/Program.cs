using Bearyon.Shared;
using Bearyon.Network.RoomServer;

namespace Bearyon.Room
{
    public class Program
    {
        static void Main(string[] args)
        {
            int masterPort = int.Parse(args[0]);
            string roomId = args[1];
            int roomPort = int.Parse(args[2]);

            PacketRegistry.RegisterAllPackets();
            
            RoomClientPacketHandler roomClientPacketHandler = new RoomClientPacketHandler();
            GameServerPacketHandler gameServerPacketHandler = new GameServerPacketHandler();            

            RoomServer server = new RoomServer(masterPort, roomId, roomPort, roomClientPacketHandler, gameServerPacketHandler);
            server.Start();
        }
    }
}
