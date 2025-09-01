using Bearyon.Shared.Packets;
using Bearyon.Shared.Packets.Connection;
using Bearyon.Shared.Packets.Lobby;
using Bearyon.Shared.Packets.Utilities;

namespace Bearyon.Shared
{
    public static class PacketRegistry
    {
        public static void RegisterAllPackets()
        {
            RegisterConnectionPackets();            
            RegisterLobbyPackets();
            RegisterUtilityPackets();
        }

        private static void RegisterConnectionPackets()
        {
            PacketUtility.RegisterPacket<ClientConnectionRequestPacket>(1000);
            PacketUtility.RegisterPacket<LobbyConnectionResponsePacket>(1001);
            PacketUtility.RegisterPacket<PingPacket>(1002);
            PacketUtility.RegisterPacket<PongPacket>(1003);
            PacketUtility.RegisterPacket<RoomConnectionResponsePacket>(1004);
            PacketUtility.RegisterPacket<GameConnectionResponsePacket>(1005);
            PacketUtility.RegisterPacket<RoomConnectionRequestPacket>(1006);
        }        

        private static void RegisterLobbyPackets()
        {
            PacketUtility.RegisterPacket<CreateRoomRequestPacket>(2000);
            PacketUtility.RegisterPacket<CreateRoomResponsePacket>(2001);
            PacketUtility.RegisterPacket<JoinRoomRequestPacket>(2002);
            PacketUtility.RegisterPacket<JoinRoomResponsePacket>(2003);
            PacketUtility.RegisterPacket<ListRoomRequestPacket>(2004);
            PacketUtility.RegisterPacket<RoomSummaryListPacket>(2005);
        }

        private static void RegisterUtilityPackets()
        {
            PacketUtility.RegisterPacket<StringMessagePacket>(9000);
            PacketUtility.RegisterPacket<ConsoleCommandPacket>(9001);
        }
    }
}
