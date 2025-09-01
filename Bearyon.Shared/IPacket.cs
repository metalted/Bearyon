using Lidgren.Network;

namespace Bearyon.Shared
{
    public interface IPacket
    {
        void Serialize(NetOutgoingMessage om);
        void Deserialize(NetIncomingMessage im);
    }
}
