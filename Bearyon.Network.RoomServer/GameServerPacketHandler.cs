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
            RoomServer.ClientManager.RemoveClient(ci.ClientUID);
        }

        protected virtual void HandleClientConnectionRequest(ClientConnectionRequestPacket clientConnectionRequest, NetConnection conn)
        {
            ClientInfo clientInfo;
            string errorMessage;

            if(RoomServer.ClientManager.RegisterClient(clientConnectionRequest.ClientUID, conn, out clientInfo, out errorMessage))
            {
                PacketMetadata data = new PacketMetadata();
                data.Add("AppIdentifier", _serverEndpoint.GetProfile().AppIdentifier);

                Send(new GameConnectionResponsePacket()
                {
                    Success = true,
                    ErrorMessage = "",
                    Data = data
                }, conn);
            }
            else
            {
                Send(new GameConnectionResponsePacket()
                {
                    Success = false,
                    ErrorMessage = errorMessage
                }, conn);
            }
        }
    }
}
