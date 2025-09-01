using System;
using Bearyon.Network.Server;
using Bearyon.Shared;

namespace Bearyon.TestServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PacketRegistry.RegisterAllPackets();

            LobbyServerPacketHandler lobbyServerPacketHandler = new LobbyServerPacketHandler();
            RoomServerPacketHandler roomServerPacketHandler = new RoomServerPacketHandler();

            MasterServer server = new MasterServer(14242, 14243, lobbyServerPacketHandler, roomServerPacketHandler);
            server.Start();
        }
    }
}
