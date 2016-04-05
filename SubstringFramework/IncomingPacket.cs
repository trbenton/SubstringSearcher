using System;
using SubstringFramework.Enums;

namespace SubstringFramework
{
    public class IncomingPacket : Packet
    {
        public IncomingPacket(byte[] data)
        {
            if (data != null && data.Length >= HeaderSize)
            {
                Operation = (OpCode)data[0];
                var dataSize = BitConverter.ToInt16(data, 1);

                if (dataSize > 0 && dataSize <= (data.Length - HeaderSize))
                {
                    Data = new byte[dataSize];
                    Buffer.BlockCopy(data, HeaderSize, Data, 0, dataSize);
                }
            }
        }
    }
}
