using System;
using System.Collections.Generic;
using NLog;
using SubstringFramework;
using SubstringFramework.Enums;
using SubstringFramework.Interfaces;
using SubstringFramework.Util;
using SubstringFramework.Views;

namespace SubstringSearch
{
    public class SubstringServer : PipeServer
    {
        private readonly Dictionary<OpCode, IPacketHandler> _requestHandlers = new Dictionary<OpCode, IPacketHandler>();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public SubstringServer()
        {
            RegisterMessageHandlers();
        }

        private void RegisterMessageHandlers()
        {
            var interfaces = InterfaceRegistrar.RegisterInterfaces<IPacketHandler>();
            foreach (var instance in interfaces)
            {
                _requestHandlers.Add(instance.Operation, instance);
            }
        }

        protected override OutgoingPacket HandleReceivedMessage(IncomingPacket msg, Guid jobId)
        {
            if (msg.HasOpCode() && _requestHandlers.ContainsKey(msg.Operation))
            {
                var response = _requestHandlers[msg.Operation].Handle(msg, jobId);
                return response;
            }
            Logger.Log(LogLevel.Warn, "[{0}] Received an invalid OpCode.", jobId);
            return new OutgoingPacket(OpCode.Message, new Message(MessageType.Error, "Invalid OpCode received!"));
        }
    }
}
