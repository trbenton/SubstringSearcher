using System;
using SubstringFramework;
using SubstringFramework.Enums;
using SubstringFramework.Interfaces;
using SubstringFramework.Util;
using SubstringFramework.Views;

namespace SubstringClient.Handlers
{
    public class MessageHandler : IResponseHandler
    {
        public OpCode Operation => OpCode.Message;

        public Packet Handle(Packet msg)
        {
            var serverMsg = Parse(msg);

            if (serverMsg != null)
            {
                Console.WriteLine("{0}:{1}", serverMsg.Type, serverMsg.Contents);
            }
            return null;
        }

        public Message Parse(Packet msg)
        {
            if (msg.HasData())
            {
                Message req;
                if (ProtobufUtil.TryDeserialize(msg.Data, out req))
                {
                    return req;
                }
            }
            return null;
        }
    }
}
