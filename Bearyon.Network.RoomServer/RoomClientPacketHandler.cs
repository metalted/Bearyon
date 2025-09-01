using Bearyon.Shared;
using Bearyon.Shared.Packets.Connection;
using Bearyon.Shared.Packets.Lobby;
using Bearyon.Shared.Packets.Utilities;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bearyon.Network.RoomServer
{
    public class RoomClientPacketHandler : PacketHandler
    {
        public override void HandlePacket(IPacket packet, NetConnection conn)
        {

        }

        public override void OnConnected(NetConnection conn)
        {
            SendRoomConnectionRequest(RoomServer.RoomId);
        }

        public virtual void SendRoomConnectionRequest(int roomId)
        {
            Send(new RoomConnectionRequestPacket()
            {
                RoomId = roomId
            });
        }
    }
}
