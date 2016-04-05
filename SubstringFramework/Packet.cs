using SubstringFramework.Enums;

namespace SubstringFramework
{
    public abstract class Packet
    {
        public OpCode Operation { get; protected set; }
        public byte[] Data { get; protected set; }

        //Packet structure: 1st byte: opcode, Second + Third bytes: size of data, all other bytes are data or empty bytes from the buffer
        protected const int OpCodeSize = sizeof(byte);
        protected const int DataLengthSize = sizeof(short);
        protected const int HeaderSize = OpCodeSize + DataLengthSize;

        public bool HasData()
        {
            if (Data == null || Data.Length == 0)
            {
                return false;
            }
            return true;
        }

        public bool HasOpCode()
        {
            return Operation != OpCode.NOP;
        }
    }
}
