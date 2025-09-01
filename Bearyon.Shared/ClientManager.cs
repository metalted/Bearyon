using Lidgren.Network;
using System;
using System.Collections.Generic;

namespace Bearyon.Shared
{
    public class ClientManager
    {
        private int _nextClientId = 1;
        private readonly Dictionary<int, ClientInfo> _clients = new Dictionary<int, ClientInfo>();

        public ClientInfo RegisterClient(NetConnection conn)
        {
            ClientInfo client = new ClientInfo
            {
                ClientId = _nextClientId++,
                Connection = conn,
                CurrentRoomId = null
            };

            _clients.Add(client.ClientId, client);
            return client;
        }

        public ClientInfo GetClient(int id)
        {
            if (_clients.TryGetValue(id, out var client))
                return client;
            return null;
        }

        public ClientInfo GetByConnection(NetConnection conn)
        {
            foreach (var client in _clients.Values)
            {
                if (client.Connection == conn)
                    return client;
            }
            return null;
        }

        public void RemoveClient(int clientId)
        {
            Console.WriteLine("[CLIENT_MANAGER] Removed Client " + clientId);
            _clients.Remove(clientId);
        }
    }
}
