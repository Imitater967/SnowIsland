using System;
using Unity.Netcode;

namespace SnowIsland.Scripts.Network
{
    [Serializable]
    public class StringContainer : INetworkSerializable
    {
        public string SomeText;
     
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsWriter)
            {
                serializer.GetFastBufferWriter().WriteValueSafe(SomeText);
            }
            else
            {
                serializer.GetFastBufferReader().ReadValueSafe(out SomeText);
            }
        }
    }
}