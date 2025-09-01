using System;
using System.Collections.Generic;
using Lidgren.Network;

namespace Bearyon.Shared
{
    public static class PacketUtility
    {
        private static readonly Dictionary<ushort, Type> _packetTypes = new Dictionary<ushort, Type>();

        public static void RegisterPacket<T>(ushort id) where T : struct, IPacket
        {
            Type packetType = typeof(T);
            if (_packetTypes.ContainsKey(id))
            {
                //throw new Exception("Packet ID " + id + " already registered for " + _packetTypes[id].Name);
                //As we are in the same project theres going to be overlap of packages, just return and ignore for now.
                return;
            }                

            _packetTypes[id] = packetType;
        }

        public static ushort GetPacketId<T>() where T : struct, IPacket
        {
            foreach (KeyValuePair<ushort, Type> pair in _packetTypes)
            {
                if (pair.Value == typeof(T))
                    return pair.Key;
            }
            throw new Exception("Packet type " + typeof(T).Name + " is not registered.");
        }

        public static ushort GetPacketId(Type packetType)
        {
            foreach (KeyValuePair<ushort, Type> pair in _packetTypes)
            {
                if (pair.Value == packetType)
                    return pair.Key;
            }
            throw new Exception("Packet type " + packetType.Name + " is not registered.");
        }

        // Generic pack (when you know the type at compile-time)
        public static void Pack<T>(T packet, NetOutgoingMessage om) where T : struct, IPacket
        {
            ushort id = GetPacketId<T>();
            om.Write(id);
            packet.Serialize(om);
        }

        // Non-generic pack (when you only have IPacket reference)
        public static void Pack(IPacket packet, NetOutgoingMessage om)
        {
            ushort id = GetPacketId(packet.GetType());
            om.Write(id);
            packet.Serialize(om);
        }

        public static bool TryUnpack(NetIncomingMessage im, out IPacket packet)
        {
            packet = null;
            try
            {
                ushort id = im.ReadUInt16();
                if (!_packetTypes.ContainsKey(id))
                {
                    Console.WriteLine("[PacketUtility] Unknown packet ID: " + id);
                    return false;
                }

                Type t = _packetTypes[id];
                object instance = Activator.CreateInstance(t);
                IPacket ip = (IPacket)instance;
                ip.Deserialize(im);
                packet = ip;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[PacketUtility] Failed to unpack: " + ex.Message);
                return false;
            }
        }
    }
}
