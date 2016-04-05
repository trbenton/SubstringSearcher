using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using NLog;
using SubstringFramework;
using SubstringFramework.Enums;
using SubstringFramework.Interfaces;
using SubstringFramework.Util;
using SubstringFramework.Views;

namespace SubstringSearch.Handlers
{
    public class PlainTextSearchHandler : IPacketHandler
    {
        public OpCode Operation => OpCode.PlainTextSearch;
        private const int BlockSize = 65536;
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

        public PlainTextSearch Parse(IncomingPacket msg)
        {
            if (msg.HasData())
            {
                PlainTextSearch req;
                if (ProtobufUtil.TryDeserialize(msg.Data, out req))
                {
                    return req;
                }
            }
            return null;
        }

        private OutgoingPacket HandleSearch(PlainTextSearch search, Guid jobId)
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

            if (string.IsNullOrEmpty(search.Pattern))
            {
                Logger.Log(LogLevel.Info, "[{0}] The pattern was null or empty.", jobId);
                return new OutgoingPacket(OpCode.Message, new Message(MessageType.Error, "The pattern is null or empty."));
            }

            if (search.UseBlocks)
            {
                return SearchBlocks(search,jobId);
            }

            return SearchLines(search, jobId);
        }

        private OutgoingPacket SearchBlocks(PlainTextSearch search, Guid jobId)
        {
            Logger.Log(LogLevel.Info, "[{0}] Starting a new text search job using blocks.", jobId);
            int count = 0;
            int edgeCases = 0;
            var bufferSlice = new char[(search.Pattern.Length - 1) * 2];
       
            var sw = new Stopwatch();
            sw.Start();
            

            using (var e = new CountdownEvent(1))
            {
                using (var reader = File.OpenText(search.Path))
                {
                    while (reader.Peek() >= 0)
                    {
                        e.AddCount();
                        var block = new char[BlockSize];
                        reader.ReadBlock(block, 0, BlockSize);

                        Buffer.BlockCopy(block, 0, bufferSlice, (search.Pattern.Length - 1) * sizeof(char), (search.Pattern.Length - 1) * sizeof(char)); //copy new contents into buffer
                        edgeCases += CSharpSearch(new string(bufferSlice), search.Pattern);
                        Buffer.BlockCopy(block, (BlockSize - search.Pattern.Length + 1) * sizeof(char), bufferSlice, 0, (search.Pattern.Length - 1) * sizeof(char));
                        
                        ThreadPool.QueueUserWorkItem(delegate
                        {
                            int results = CSharpSearch(new string(block), search.Pattern);                            
                            Interlocked.Add(ref count, results);
                            e.Signal();
                        });
                    }
                }

                //end the initial event...
                e.Signal();
                e.Wait();
            }

            sw.Stop();

            int total = count + edgeCases;
            Logger.Log(LogLevel.Info, "[{0}] Completed a new text search job using blocks. Results: {1} Runtime: {2}", jobId, count, sw.Elapsed);
            return new OutgoingPacket(OpCode.JobResult, new JobResult(total, sw.Elapsed));
        }

        private OutgoingPacket SearchLines(PlainTextSearch search, Guid jobId)
        {
            Logger.Log(LogLevel.Info, "[{0}] Starting a new text search job using lines", jobId);
            int count = 0;
            var sw = new Stopwatch();
            sw.Start();

            using (var reader = File.OpenText(search.Path))
            {
                while (reader.Peek() >= 0)
                {
                    var line = reader.ReadLine();
                    count += CSharpSearch(line, search.Pattern);
                }
            }

            sw.Stop();
            Logger.Log(LogLevel.Info, "[{0}] Completed a new text search job using lines. Results: {1} Runtime: {2}", jobId, count, sw.Elapsed);
            return new OutgoingPacket(OpCode.JobResult, new JobResult(count, sw.Elapsed));
        }

        public static int CSharpSearch(string text, string pattern)
        {
            int count = 0;
            int i = 0;
            while ((i = text.IndexOf(pattern, i, StringComparison.Ordinal)) != -1)
            {
                i += pattern.Length;
                ++count;
            }
            return count;
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
