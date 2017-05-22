using System.Threading;
namespace Genesyslab.PS.Platform.Threading
{
    public interface ThreadFactory
    {
        Thread CreateThread(ThreadStart threadStart);
    }

    class SimpleThreadFactory : ThreadFactory
    {
        readonly string threadName;

        public SimpleThreadFactory() { }

        public SimpleThreadFactory(string threadName)
        {
            this.threadName = threadName;
        }

        public Thread CreateThread(ThreadStart threadStart)
        {
            var thread = new Thread(threadStart);

            if (threadName != null)
                thread.Name = threadName;
            return thread;
        }
    }
}
