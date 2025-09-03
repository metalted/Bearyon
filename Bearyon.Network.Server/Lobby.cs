using Bearyon.Shared;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bearyon.Network.Server
{
    public class Lobby
    {
        private int _nextPort = 20000;
        private readonly Dictionary<string, Room> _rooms = new Dictionary<string, Room>();

        public Room CreateRoom(string name, int maxClients)
        {            
            int gamePort = _nextPort++;
            string roomId = System.Guid.NewGuid().ToString();

            var psi = new ProcessStartInfo
            {
                FileName = "C:\\Users\\Ted\\source\\repos\\Bearyon\\Bearyon.Room\\bin\\Debug\\net8.0\\Bearyon.Room.exe",
                Arguments = $"{MasterServer.RoomPort} {roomId} {gamePort}",
                UseShellExecute = true,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal
            };

            Process proc = Process.Start(psi);

            Room room = new Room
            {
                RoomId = roomId,
                Name = name,
                MaxClients = maxClients,
                GamePort = gamePort,
                Process = proc
            };

            _rooms.Add(roomId, room);

            return room;
        }

        public List<Room> GetRooms()
        {
            return new List<Room>(_rooms.Values);
        }

        public Room? GetRoomByConnection(NetConnection connection)
        {
            return _rooms.Values.FirstOrDefault(r => r.Connection == connection);
        }

        public Room GetRoom(string id)
        {
            if (_rooms.ContainsKey(id))
                return _rooms[id];
            return null;
        }

        public Room GetRoomByIndex(string index)
        {
            int roomIndex;
            if(int.TryParse(index, out roomIndex))
            {
                string[] roomIds = _rooms.Keys.ToArray();
                if(roomIds.Length <= roomIndex)
                {
                    return _rooms[roomIds[roomIndex]];
                }
            }

            return null;
        }

        public List<RoomSummary> GetRoomSummaries()
        {
            List<RoomSummary> summaries = new List<RoomSummary>();
            
            foreach(Room r in _rooms.Values)
            {
                summaries.Add(new RoomSummary()
                {
                    RoomId = r.RoomId,
                    Name = r.Name,
                    MaxClients = r.MaxClients,
                    ClientCount = r.GetClientCount()
                });
            }

            return summaries;
        }

        public void RemoveRoom(string uid)
        {
            _rooms.Remove(uid);
        }
    }
}
