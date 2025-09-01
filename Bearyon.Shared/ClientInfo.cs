using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bearyon.Shared
{
    public class ClientInfo
    {
        public int ClientId { get; set; }
        public string Username { get; set; }
        public NetConnection Connection { get; set; }
        public int? CurrentRoomId { get; set; }
    }
}
