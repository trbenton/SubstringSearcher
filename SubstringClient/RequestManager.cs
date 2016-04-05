using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.ServiceProcess;
using SubstringFramework;
using SubstringFramework.Enums;
using SubstringFramework.Interfaces;
using SubstringFramework.Util;

namespace SubstringClient
{
    public static class RequestManager
    {
        private static readonly Dictionary<OpCode, IResponseHandler> ResponseHandlers = new Dictionary<OpCode, IResponseHandler>();
        private static int _jobCounter;
        private const string ServerId = "ade41e61-724d-4d9a-ad42-a7a6929fd24c";
        private const string ServiceName = "SubstringSearch";
        private const int BufferSize = 4096;

        static RequestManager()
        {
            RegisterMessageHandlers();
        }

        private static void RegisterMessageHandlers()
        {
            var interfaces = InterfaceRegistrar.RegisterInterfaces<IResponseHandler>();
            foreach (var instance in interfaces)
            {
                ResponseHandlers.Add(instance.Operation, instance);
            }
        }

        public static void CreateRequest(Packet packet)
        {
            ++_jobCounter;
            Task.Run(() => 
            {
                SendRequest(_jobCounter, packet);
            });
        }

        private static void SendRequest(int jobId, Packet packet)
        {
            try
            {
                var sc = new ServiceController(ServiceName);
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    var pipeStream = new NamedPipeClientStream(".", ServerId, PipeDirection.InOut);
                    pipeStream.Connect(int.MaxValue);
                    pipeStream.Write(packet.Data, 0, packet.Data.Length);
                    Console.WriteLine("Starting job {0}.", jobId);

                    var buffer = new byte[BufferSize];
                    pipeStream.Read(buffer, 0, BufferSize);
                    Console.WriteLine("Job {0} complete.", jobId);

                    var msg = new IncomingPacket(buffer);
                    HandleReceivedMessage(msg);

                    pipeStream.Close();
                }
                else
                {
                    Console.WriteLine("Failed to locate service.");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to process request. Is the service installed and running?");
            }
        }

        private static void HandleReceivedMessage(Packet msg)
        {
            if (msg.HasOpCode() && ResponseHandlers.ContainsKey(msg.Operation))
            {
                ResponseHandlers[msg.Operation].Handle(msg);
            }
        }
    }
}
