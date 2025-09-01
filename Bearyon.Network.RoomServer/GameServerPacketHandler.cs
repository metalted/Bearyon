using Bearyon.Shared;
using Bearyon.Shared.Packets.Connection;
using Bearyon.Shared.Packets.Lobby;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bearyon.Network.RoomServer
{
    public class GameServerPacketHandler : PacketHandler
    {
        public override void HandlePacket(IPacket packet, NetConnection conn)
        {
            Console.WriteLine("[GAME_SERVER] Received packet of type: " + packet.GetType().Name);

            if (packet is ClientConnectionRequestPacket clientConnectionRequest)
            {
                HandleClientConnectionRequest(clientConnectionRequest, conn);
            }           
            else
            {
                HandleCustomPacket(packet, conn);
            }
        }

        public override void OnDisconnected(NetConnection conn)
        {
            ClientInfo ci = RoomServer.ClientManager.GetByConnection(conn);
            RoomServer.ClientManager.RemoveClient(ci.ClientId);
        }

        protected virtual void HandleClientConnectionRequest(ClientConnectionRequestPacket hello, NetConnection conn)
        {
            //Save and check the user here.
            ClientInfo ci = RoomServer.ClientManager.RegisterClient(conn);

            Send(new GameConnectionResponsePacket()
            {
                Success = true,
                ClientId = ci.ClientId,
                ErrorMessage = "",
            }, conn);
        }
    }
}
