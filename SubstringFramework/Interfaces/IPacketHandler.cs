using System;
using SubstringFramework.Enums;

namespace SubstringFramework.Interfaces
{
    public interface IPacketHandler
    {
        OpCode Operation { get; }
        OutgoingPacket Handle(IncomingPacket msg, Guid jobId);
    }
}
