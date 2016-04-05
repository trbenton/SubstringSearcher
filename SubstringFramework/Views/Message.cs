using ProtoBuf;
using SubstringFramework.Enums;

namespace SubstringFramework.Views
{
    [ProtoContract]
    public class Message
    {
        [ProtoMember(1, IsRequired = true)]
        public MessageType Type { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public string Contents { get; set; }

        public Message(MessageType type, string contents)
        {
            Type = type;
            Contents = contents;
        }

        public Message()
        {
            
        }
    }
}
