using System;
using System.IO.Pipes;
using System.Security.AccessControl;
using NLog;
using SubstringFramework;

namespace SubstringSearch
{
    public abstract class PipeServer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private const int MaxServerInstances = 250;
        private const int BufferSize = 4096;
        private const string ServerId = "ade41e61-724d-4d9a-ad42-a7a6929fd24c";

        protected abstract OutgoingPacket HandleReceivedMessage(IncomingPacket msg, Guid jobId);

        protected PipeServer()
        {
           
        }

        public void StartListener()
        {
            try
            {
                var security = new PipeSecurity();
                security.AddAccessRule(new PipeAccessRule("Everyone", PipeAccessRights.FullControl, AccessControlType.Allow)); //TODO: In a release situation, use proper security setup

                var serverStream = new NamedPipeServerStream(ServerId, PipeDirection.InOut, MaxServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, BufferSize, BufferSize, security);
                serverStream.BeginWaitForConnection(ConnectionCallback, serverStream);
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, e);
            }
        }

        private void ConnectionCallback(IAsyncResult ar)
        {
            try
            {
                var jobId = Guid.NewGuid();
                var pipeServer = (NamedPipeServerStream)ar.AsyncState;
                pipeServer.EndWaitForConnection(ar);

                StartListener(); //Open a new listener stream and start listening again. Apparently we need a new one for each client

                var buffer = new byte[BufferSize];
                pipeServer.Read(buffer, 0, BufferSize);
                var msg = new IncomingPacket(buffer);

                Logger.Log(LogLevel.Info, "[{0}] Started new job.", jobId);
                var response = HandleReceivedMessage(msg, jobId);
                pipeServer.Write(response.Data, 0, response.Data.Length);
                Logger.Log(LogLevel.Info, "[{0}] Done processing job.", jobId);
                pipeServer.Close();
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, e);
            }
        }
    }
}
