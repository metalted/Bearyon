using System;
using System.Threading;
using Bearyon.Network.Client;
using Bearyon.Shared;
using Bearyon.Shared.Packets.Utilities;

namespace Bearyon.TestClient
{
    //A basic client
    public  class Program
    {
        //The client that handles all communications.
        public static ClientEndpoint client;
        //The packet handler manages all the traffic.
        public static ClientPacketHandler handler;
        //A local command manager that allows for easily executing functions.
        public static CommandManager commandManager;       

        static void Main(string[] args)
        {
            //Create the packet handler (can be custom)
            handler = new ClientPacketHandler();

            //Create the client and pass it the handler.
            client = new ClientEndpoint(handler);
            client.AddServer(new ServerProfile("Lobby", "bearyon_lobby_server", "127.0.0.1", 14242));

            //Create the commands.
            InitializeCommandManager(handler);

            //Start the client on a background thread. Client.Start() starts the while loop.
            Thread clientThread = new Thread(() => client.Start());
            clientThread.Start();            

            //Loop for console inputs.
            while (true)
            {
                string input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                    break;

                if (input.StartsWith("/"))
                {
                    commandManager.Execute(input);
                }
            }
        }

        //Basic commands
        public static void InitializeCommandManager(ClientPacketHandler handler)
        {
            commandManager = new CommandManager();
            
            //Connect to the master server.            
            commandManager.Register("/connect", _ =>
            {
                client.ConnectTo("Lobby");
            });
            commandManager.Register("/disconnect", _ =>
            {
                client.Disconnect();
            });
            commandManager.Register("/createroom string:name int:maxPlayers", argsDict =>
            {
                handler.SendCreateRoomRequest((string)argsDict["name"], (int)argsDict["maxPlayers"]);
            });

            commandManager.Register("/joinroom string:roomId", argsDict =>
            {
                handler.SendJoinRoomRequest((string)argsDict["roomId"]);
            });

            commandManager.Register("/list", argsDict =>
            {
                handler.SendListLobbiesRequest();
            });

            commandManager.Register("/help", _ =>
            {
                Console.WriteLine("Available commands:");
                commandManager.ListCommands();
            });

            commandManager.Register("/leave", _ =>
            {
                if(client.currentConnection.Name == "Game")
                {
                    client.ConnectTo("Lobby");
                }
                else if(client.currentConnection.Name == "Lobby")
                {
                    client.Disconnect();
                }
            });
        }
    }
}
