using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bearyon.Shared
{
    public class PacketMetadata
    {
        private readonly Dictionary<string, string> _data = new Dictionary<string, string>();

        public void Add(string key, string value)
        {
            _data[key] = value;
        }

        public bool TryGet(string key, out string value)
        {
            return _data.TryGetValue(key, out value);
        }

        public string Get(string key, string defaultValue = null)
        {
            return _data.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public IReadOnlyDictionary<string, string> Data => _data;

        public void Serialize(NetOutgoingMessage om)
        {
            // Write the number of pairs
            om.Write(_data.Count);

            foreach (var kvp in _data)
            {
                om.Write(kvp.Key ?? string.Empty);
                om.Write(kvp.Value ?? string.Empty);
            }
        }

        public void Deserialize(NetIncomingMessage im)
        {
            _data.Clear();

            int count = im.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string key = im.ReadString();
                string value = im.ReadString();
                _data[key] = value;
            }
        }
    }
}
