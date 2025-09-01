using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bearyon.Shared
{
    public class CommandManager
    {
        private class CommandDefinition
        {
            public string Name { get; }
            public (string Type, string Name)[] Params { get; }
            public Action<Dictionary<string, object>> Handler { get; }

            public CommandDefinition(string def, Action<Dictionary<string, object>> handler)
            {
                Handler = handler;

                // definition example: "/createroom string:name int:maxPlayers"
                var parts = def.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
                Name = parts[0].TrimStart('/');

                Params = parts.Skip(1)
                    .Select(p =>
                    {
                        var pair = p.Split(':');
                        return (pair[0], pair[1]); // (type, name)
                    })
                    .ToArray();
            }

            public bool TryExecute(string[] args)
            {
                if (args.Length != Params.Length)
                {
                    Console.WriteLine("Usage: /" + Name + " " + string.Join(" ", Params.Select(p => p.Type + ":" + p.Name)));
                    return false;
                }

                var dict = new Dictionary<string, object>();
                for (int i = 0; i < Params.Length; i++)
                {
                    string type = Params[i].Type;
                    string key = Params[i].Name;
                    string raw = args[i];

                    object parsed;
                    switch (type)
                    {
                        case "string": parsed = raw; break;
                        case "int":
                            if (!int.TryParse(raw, out var intVal))
                            {
                                Console.WriteLine($"Parameter {key} must be an int.");
                                return false;
                            }
                            parsed = intVal;
                            break;
                        case "float":
                            if (!float.TryParse(raw, out var floatVal))
                            {
                                Console.WriteLine($"Parameter {key} must be a float.");
                                return false;
                            }
                            parsed = floatVal;
                            break;
                        case "bool":
                            if (!bool.TryParse(raw, out var boolVal))
                            {
                                Console.WriteLine($"Parameter {key} must be a bool (true/false).");
                                return false;
                            }
                            parsed = boolVal;
                            break;
                        default:
                            Console.WriteLine($"Unknown parameter type {type}");
                            return false;
                    }

                    dict[key] = parsed;
                }

                Handler(dict);
                return true;
            }
        }

        private readonly Dictionary<string, CommandDefinition> _commands = new Dictionary<string, CommandDefinition>(StringComparer.OrdinalIgnoreCase);

        public void Register(string def, Action<Dictionary<string, object>> handler)
        {
            var cmd = new CommandDefinition(def, handler);
            _commands[cmd.Name] = cmd;
        }

        public void Execute(string input)
        {
            if (!input.StartsWith("/")) return;

            string raw = input.Substring(1);
            string[] parts = raw.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return;

            string cmdName = parts[0];
            string[] args = parts.Skip(1).ToArray();

            if (_commands.TryGetValue(cmdName, out var cmd))
            {
                cmd.TryExecute(args);
            }
            else
            {
                Console.WriteLine("Unknown command: " + cmdName);
            }
        }

        public void ListCommands()
        {
            foreach (var cmd in _commands.Values)
            {
                Console.WriteLine("/" + cmd.Name + " " + string.Join(" ", cmd.Params.Select(p => p.Type + ":" + p.Name)));
            }
        }
    }
}
