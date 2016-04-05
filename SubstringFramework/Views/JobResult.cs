using System;
using ProtoBuf;

namespace SubstringFramework.Views
{
    [ProtoContract]
    public class JobResult
    {
        [ProtoMember(1, IsRequired = true)]
        public int Results { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public TimeSpan ProcessingTime { get; set; }

        public JobResult(int results, TimeSpan processingTime)
        {
            Results = results;
            ProcessingTime = processingTime;
        }

        public JobResult()
        {
            
        }
    }
}
