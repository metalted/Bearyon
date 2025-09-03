using Lidgren.Network;
using System;
using System.Collections.Generic;

namespace Bearyon.Shared
{
    public class ClientManager
    {
        private int _nextClientId = 1;
        private readonly Dictionary<string, ClientInfo> _clients = new Dictionary<string, ClientInfo>();

        public bool RegisterClient(string clientUID, NetConnection conn, out ClientInfo clientInfo, out string errorMessage)
        {
            if(_clients.ContainsKey(clientUID))
            {
                clientInfo = null;
                errorMessage = "Client already registered";
                return false;
            }

            ClientInfo client = new ClientInfo
            {
                ClientUID = clientUID,
                Connection = conn,
                CurrentRoomId = null
            };

            _clients.Add(clientUID, client);
            clientInfo = client;
            errorMessage = "";
            return true;
        }

        public ClientInfo GetClient(string uid)
        {
            if (_clients.TryGetValue(uid, out var client))
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

        public void RemoveClient(string uid)
        {
            if(!_clients.ContainsKey(uid))
            {
                Console.WriteLine("[CLIENT_MANAGER] Player to remove is not registered.");
                return;
            }

            Console.WriteLine("[CLIENT_MANAGER] Removing Client " + uid);
            _clients.Remove(uid);              
        }
    }
}
