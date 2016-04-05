using SubstringFramework.Enums;

namespace SubstringFramework.Interfaces
{
    public interface IResponseHandler
    {
        OpCode Operation { get; }
        Packet Handle(Packet msg);
    }
}
