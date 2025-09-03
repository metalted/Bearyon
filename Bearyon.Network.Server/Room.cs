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
        public string RoomId;
        public string Name;
        public int MaxClients;
        public int GamePort;
        public Process Process;
        public bool Connected;
        public NetConnection Connection;
        private List<string> _clientUIDs = new List<string>();

        public void AddClient(string uid)
        {
            if(!_clientUIDs.Contains(uid))
            {
                _clientUIDs.Add(uid);
            }
        }

        public void RemoveClient(string uid)
        {
            if (_clientUIDs.Contains(uid))
            {
                _clientUIDs.Remove(uid);
            }
        }

        public int GetClientCount()
        {
            return _clientUIDs.Count;
        }
    }
}
