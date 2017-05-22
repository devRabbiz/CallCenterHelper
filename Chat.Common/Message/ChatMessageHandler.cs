using System;
using System.Threading;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Commons.Threading;
using Genesyslab.PS.Platform.Threading;

namespace Genesyslab.PS.Platform
{
    /// <summary>
    /// Can be started before the protocol is opened. The message-handling loop
    /// blocks on the Received method in every case.
    /// </summary>
    public class MessageHandler : IDisposable
    {
        readonly IMessageReceiver protocol;
        readonly Action<IMessage> messageHandlerDelegate;
        readonly Thread handlingThread;

        public MessageHandler(
            IMessageReceiver protocol,
            Action<IMessage> messageHandlerDelegate,
            ThreadFactory threadFactory)
        {
            this.protocol = protocol;
            this.messageHandlerDelegate = messageHandlerDelegate;
            this.handlingThread = threadFactory.CreateThread(new ThreadStart(ReceiverLoop));
        }

        public MessageHandler(
            IMessageReceiver protocol,
            Action<IMessage> messageHandlerDelegate,
            string internalThreadName)
            : this(protocol, messageHandlerDelegate, new SimpleThreadFactory(internalThreadName))
        {
        }

        public void Start()
        {
            if (handlingThread.IsAlive)
                throw new InvalidOperationException("already started");

            handlingThread.Start();
        }

        void ReceiverLoop()
        {
            try
            {
                while (protocol != null)
                {
                    try
                    {
                        IMessage message = protocol.Receive();
                        if (message != null)
                        {
                            messageHandlerDelegate(message);
                        }
                    }
                    catch { }
                }
            }
            catch (ThreadInterruptedException) { /* expected */ }
        }

        public void StopAndWait()
        {
            if (handlingThread.IsAlive)
            {
                handlingThread.Interrupt();
                handlingThread.Join();
            }
        }

        public void Stop()
        {
            if (handlingThread.IsAlive)
            {
                handlingThread.Interrupt();
            }
        }

        /// <summary>
        /// Always stop or dispose the MessageHandler before disposing the protocol.
        /// </summary>
        public void Dispose()
        {
            Stop();
        }
    }
}
