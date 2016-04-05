using ProtoBuf;

namespace SubstringFramework.Enums
{
    [ProtoContract]
    public enum MessageType : byte
    {
        Normal = 0x00,
        Warning = 0x01,
        Error = 0x02
    }
}
