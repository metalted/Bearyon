using Bearyon.Shared;
using Bearyon.Shared.Packets.Connection;
using Bearyon.Shared.Packets.Lobby;
using Bearyon.Shared.Packets.Utilities;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Bearyon.Network.RoomServer
{
    public class RoomClientPacketHandler : PacketHandler
    {
        public override void HandlePacket(IPacket packet, NetConnection conn)
        {
            if(packet is RoomConnectionResponsePacket roomConnectionResponse)
            {
                HandleRoomConnectionResponse(roomConnectionResponse);
            }
        }

        public override void OnConnected(NetConnection conn)
        {
            SendRoomConnectionRequest(RoomServer.RoomId);
        }

        public virtual void SendRoomConnectionRequest(string roomId)
        {
            Send(new RoomConnectionRequestPacket()
            {
                RoomId = roomId
            });
        }

        public virtual void HandleRoomConnectionResponse(RoomConnectionResponsePacket packet)
        {
            if (packet.Success)
            {
                _clientEndpoint.SetCurrentConnection(packet.Data.Get("AppIdentifier"));
                Console.WriteLine($"[CLIENT] Connection accepted from {packet.Data.Get("AppIdentifier")}");
                RoomServer.Instance.Stop(3);
            }
            else
            {
                Console.WriteLine("[CLIENT] Connection refused: " + packet.ErrorMessage);
                _clientEndpoint.Disconnect();                
            }
        }
    }
}
