using System;
using System.Threading;
using Bearyon.Network.Client;
using Bearyon.Shared;
using Bearyon.Shared.Packets.Utilities;

namespace Bearyon.TestClient
{
    internal class Program
    {
        public static Client client;
        public static ClientPacketHandler handler;
        public static CommandManager commandManager;

        static void Main(string[] args)
        {
            handler = new ClientPacketHandler();
            client = new Client(handler);

            InitializeCommandManager(handler);

            // start client on a background thread
            Thread clientThread = new Thread(() => client.Start());
            clientThread.Start();            

            // --- input loop ---
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

        public static void InitializeCommandManager(ClientPacketHandler handler)
        {
            commandManager = new CommandManager();
            commandManager.Register("/connect", _ =>
            {
                client.ConnectTo("bearyon_lobby_server", "127.0.0.1", 14242);
            });
            commandManager.Register("/disconnect", _ =>
            {
                client.Disconnect();
            });
            commandManager.Register("/createroom string:name int:maxPlayers", argsDict =>
            {
                handler.SendCreateRoomRequest((string)argsDict["name"], (int)argsDict["maxPlayers"]);
            });

            commandManager.Register("/joinroom int:roomId", argsDict =>
            {
                handler.SendJoinRoomRequest((int)argsDict["roomId"]);
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
                if (client.GetLocation() == ClientLocation.Game)
                {
                    client.ConnectTo("bearyon_lobby_server", "127.0.0.1", 14242);
                }
            });
        }
    }
}
