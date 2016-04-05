using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using NLog;
using SubstringFramework;
using SubstringFramework.Enums;
using SubstringFramework.Interfaces;
using SubstringFramework.Util;
using SubstringFramework.Views;

namespace SubstringSearch.Handlers
{
    public class RegexSearchHandler : IPacketHandler
    {
        public OpCode Operation => OpCode.RegexSearch;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public OutgoingPacket Handle(IncomingPacket msg, Guid jobId)
        {
            var search = Parse(msg);

            if (search != null)
            {
                return HandleSearch(search, jobId);
            }

            Logger.Log(LogLevel.Info, "[{0}] The search request was bad.", jobId);
            return new OutgoingPacket(OpCode.Message, new Message(MessageType.Error, "Bad search request!"));
        }

        public RegexSearch Parse(Packet msg)
        {
            if (msg.HasData())
            {
                RegexSearch req;
                if (ProtobufUtil.TryDeserialize(msg.Data, out req))
                {
                    return req;
                }
            }
            return null;
        }

        private OutgoingPacket HandleSearch(RegexSearch search, Guid jobId)
        {
            if (!FileExists(search.Path))
            {
                Logger.Log(LogLevel.Info, "[{0}] The file did not exist.", jobId);
                return new OutgoingPacket(OpCode.Message, new Message(MessageType.Error, "File does not exist."));
            }

            if (FileIsEmpty(search.Path))
            {
                Logger.Log(LogLevel.Info, "[{0}] The file was empty.", jobId);
                return new OutgoingPacket(OpCode.Message, new Message(MessageType.Error, "The file is empty."));
            }

            if (string.IsNullOrEmpty(search.Regex))
            {
                Logger.Log(LogLevel.Info, "[{0}] The regex was null or empty.", jobId);
                return new OutgoingPacket(OpCode.Message, new Message(MessageType.Error, "The regex is null or empty."));
            }

            return SearchLines(search, jobId);
        }

        private OutgoingPacket SearchLines(RegexSearch search, Guid jobId)
        {
            Logger.Log(LogLevel.Info, "[{0}] Starting a new regex search job.", jobId);
            int count = 0;
            var sw = new Stopwatch();
            sw.Start();

            using (var reader = File.OpenText(search.Path))
            {
                while (reader.Peek() >= 0)
                {
                    var line = reader.ReadLine();
                    if (line != null)
                    {
                        count += Regex.Matches(line, search.Regex).Count;
                    }          
                }
            }

            sw.Stop();
            Logger.Log(LogLevel.Info, "[{0}] Completed a new regex search job. Results: {1} Runtime: {2}", jobId, count, sw.Elapsed);
            return new OutgoingPacket(OpCode.JobResult, new JobResult(count, sw.Elapsed));
        }

        private bool FileExists(string path)
        {
            return File.Exists(path);
        }

        private bool FileIsEmpty(string path)
        {
            return new FileInfo(path).Length == 0;
        }
    }
}
