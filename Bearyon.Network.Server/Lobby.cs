using Bearyon.Shared;
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
        private int _nextRoomId = 1;
        private int _nextPort = 20000;
        private readonly Dictionary<int, Room> _rooms = new Dictionary<int, Room>();

        public Room CreateRoom(string name, int maxPlayers)
        {            
            int gamePort = _nextPort++;
            int roomId = _nextRoomId++;

            var psi = new ProcessStartInfo
            {
                FileName = "C:\\Users\\Ted\\source\\repos\\Bearyon\\Bearyon.Room\\bin\\Debug\\net8.0\\Bearyon.Room.exe", // path to your built RoomServer project
                Arguments = $"{roomId} {name} {maxPlayers} {MasterServer.RoomPort} {gamePort}",
                UseShellExecute = true,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal
            };

            Process proc = Process.Start(psi);

            Room room = new Room
            {
                RoomId = roomId,
                Name = name,
                MaxPlayers = maxPlayers,
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

        public Room GetRoom(int id)
        {
            if (_rooms.ContainsKey(id))
                return _rooms[id];
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
                    MaxPlayers = r.MaxPlayers,
                    PlayerCount = 0
                });
            }

            return summaries;
        }
    }
}
