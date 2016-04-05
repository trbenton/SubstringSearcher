using System;
using SubstringFramework;
using SubstringFramework.Enums;
using SubstringFramework.Interfaces;
using SubstringFramework.Util;
using SubstringFramework.Views;

namespace SubstringClient.Handlers
{
    public class JobResultHandler : IResponseHandler
    {
        public OpCode Operation => OpCode.JobResult;

        public Packet Handle(Packet msg)
        {
            var res = Parse(msg);

            if (res != null)
            {
                Console.WriteLine("Job complete! Results: {0}  Time: {1}", res.Results, res.ProcessingTime);
            }
            return null;
        }

        public JobResult Parse(Packet msg)
        {
            if (msg.HasData())
            {
                JobResult req;
                if (ProtobufUtil.TryDeserialize(msg.Data, out req))
                {
                    return req;
                }
            }
            return null;
        }
    }
}
