using System;
using System.Collections.Generic;
using System.Text;

namespace Bearyon.Shared
{
    public class ServerProfile
    {
        public string Name { get; private set; }
        public string AppIdentifier { get; private set; }
        public string Host { get; private set; }
        public int Port { get; private set; }

        public ServerProfile(string name, string appIdentifier, string host, int port)
        {
            Name = name;
            AppIdentifier = appIdentifier; 
            Host = host; 
            Port = port;
        }
    }
}
