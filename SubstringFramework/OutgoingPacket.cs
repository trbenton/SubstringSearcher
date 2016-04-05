using SubstringFramework.Enums;
using System;
using SubstringFramework.Util;

namespace SubstringFramework
{
    public class OutgoingPacket : Packet
    {
        public OutgoingPacket(OpCode operation, object data)
        {
            Operation = operation;

            if(data != null)
            {
                var bytes = ProtobufUtil.Serialize(data);
                Data = new byte[bytes.Length + HeaderSize];
                var lengthBytes = BitConverter.GetBytes(bytes.Length);

                Data[0] = (byte)operation;
                Data[1] = lengthBytes[0];
                Data[2] = lengthBytes[1];

                Buffer.BlockCopy(bytes, 0, Data, 3, bytes.Length);
            }
        }
    }
}
