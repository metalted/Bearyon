using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bearyon.Network.Server
{
    public class Room
    {
        public int RoomId;
        public string Name;
        public int MaxPlayers;
        public int CurrentPlayers;
        public int GamePort { get; set; }
        public Process Process { get; set; }
        public bool Connected;
        public NetConnection Connection { get; set; }
    }
}
