using ProtoBuf;

namespace SubstringFramework.Enums
{
    [ProtoContract]
    public enum OpCode : byte
    {
        NOP = 0x00,
        Message = 0x01,
        PlainTextSearch = 0x02,
        RegexSearch = 0x03,
        JobResult = 0x04
    }
}
