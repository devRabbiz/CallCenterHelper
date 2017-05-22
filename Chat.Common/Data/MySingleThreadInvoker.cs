using Genesyslab.Diagnostics;
using Genesyslab.Platform.Commons.Logging;
using Genesyslab.Processing;
using Genesyslab.Threading;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

using Genesyslab.Platform.Commons.Threading;

namespace Chat.Common
{
    public class MySingleThreadInvoker : AbstractLogEnabled, IAsyncInvoker, IDisposable
    {
        private volatile bool active;
        private BlockedQueue eventQueue;
        private Thread thread;
        private static int threadNumber;

        static MySingleThreadInvoker()
        {
            threadNumber = 1;
        }

        public MySingleThreadInvoker()
            : this(null, null)
        {
        }

        public MySingleThreadInvoker(ILogger myLog)
            : this(null, myLog)
        {
        }

        public MySingleThreadInvoker(string name)
            : this(name, null)
        {
        }

        public MySingleThreadInvoker(string name, ILogger myLog)
        {
            this.eventQueue = new BlockedQueue();
            this.active = true;

            if (myLog == null)
            {
                this.eventQueue = new BlockedQueue();
                this.StartThread(name);
                return;
            }
            base.EnableLogging(myLog);
        }

        public MySingleThreadInvoker(string name, int queueSize, ILogger myLog)
        {
            this.eventQueue = new BlockedQueue();
            this.active = true;

            if (myLog == null)
            {
                this.eventQueue = new BlockedQueue(queueSize);
                this.StartThread(name);
                return;
            }
            base.EnableLogging(myLog);
        }

        public void Dispose()
        {
            BlockedQueue queue;
            this.StopInvoker();
            Monitor.Enter(queue = this.eventQueue);
            try
            {
                if (this.eventQueue == null)
                {
                    return;
                }
                if ((this.eventQueue as IDisposable) == null)
                {
                    return;
                }
                ((IDisposable)this.eventQueue).Dispose();
                this.eventQueue = null;
            }
            catch (Exception ex)
            {
                ChatLog.GetInstance().LogException(ex);
            }
            finally
            {
                Monitor.Exit(queue);
            }

            if (base.Logger.IsDebugEnabled && this.thread != null)
                base.Logger.Debug("SingleThreadInvoker [" + this.thread.Name + "] is disposing");
        }

        public void Invoke(Delegate d, params object[] args)
        {
            BlockedQueue queue;
            if (this.active)
            {
                Monitor.Enter(queue = this.eventQueue);
                try
                {
                    this.eventQueue.Enqueue(new ThreadingUtility.DelegateTask(d, args));
                    return;
                }
                catch (Exception ex)
                {
                    ChatLog.GetInstance().LogException(ex);
                }
                finally
                {
                    Monitor.Exit(queue);
                }
            }
            throw new InvalidOperationException("StopInvoker method already called!");
        }

        public void Invoke(WaitCallback callback, object state)
        {
            BlockedQueue queue;
            if (this.active)
            {
                Monitor.Enter(queue = this.eventQueue);
                try
                {
                    this.eventQueue.Enqueue(new WaitCallbackTask(callback, state));
                    return;
                }
                catch (Exception ex)
                {
                    ChatLog.GetInstance().LogException(ex);
                }
                finally
                {
                    Monitor.Exit(queue);
                }
            }
            throw new InvalidOperationException("StopInvoker method already called!");
        }

        public void Invoke(EventHandler handler, object sender, EventArgs args)
        {
            BlockedQueue queue;
            if (this.active)
            {
                Monitor.Enter(queue = this.eventQueue);
                try
                {
                    this.eventQueue.Enqueue(new ThreadingUtility.EventHandlerTask(handler, sender, args));
                    return;
                }
                finally
                {
                    Monitor.Exit(queue);
                }
            }
            throw new InvalidOperationException("StopInvoker method already called!");
        }

        private string LogMethodStartInfo(string message, MethodInfo mInfo, Stopwatch timer)
        {
            string str;
            if (base.Logger.IsDebugEnabled)
            {
                timer.Reset();
                str = mInfo.ReflectedType.FullName + "." + mInfo.Name;
                base.Logger.Debug(message + ": " + str);
                timer.Start();
                return str;
            }
            return string.Empty;
        }

        private void LogMethodStopInfo(string mInfo, Stopwatch timer)
        {
            if (base.Logger.IsDebugEnabled && !string.IsNullOrEmpty(mInfo))
            {
                timer.Stop();
                base.Logger.DebugFormat("==> Execution [{0}] completed in {1:N3} ms.", new object[] { mInfo, (double)((1000.0 * ((double)timer.ElapsedTicks)) / ((double)Stopwatch.Frequency)) });
            }
        }

        private void ProcessEvents()
        {
            try
            {
                ThreadMonitoring.Register(1, (this.thread.Name.Length < 0x40) ? this.thread.Name : ("..." + this.thread.Name.Substring(this.thread.Name.Length - 60)));
            }
            catch (Exception ex)
            {
                this.active = false;
                ChatLog.GetInstance().LogException(ex);
            }
            while (this.active)
            {
                try
                {
                    ThreadMonitoring.Heartbeat();
                    object obj2 = this.eventQueue.Dequeue(500);
                    if (obj2 is ThreadingUtility.DelegateTask)
                    {
                        ThreadingUtility.DelegateTask task = (ThreadingUtility.DelegateTask)obj2;
                        task.D.DynamicInvoke(task.Args);
                    }
                    else if (obj2 is ThreadingUtility.EventHandlerTask)
                    {
                        ThreadingUtility.EventHandlerTask task2 = (ThreadingUtility.EventHandlerTask)obj2;
                        task2.Handler(task2.Sender, task2.Args);
                    }
                    else if (obj2 is WaitCallbackTask)
                    {
                        WaitCallbackTask task3 = (WaitCallbackTask)obj2;
                        task3.callback(task3.state);
                    }
                    continue;
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch (Exception exception)
                {
                    if (base.Logger.IsDebugEnabled)
                    {
                        base.Logger.Debug("Exception in SingleThreadInvoker.ProcessEvents", exception);
                    }
                    continue;
                }
            }
            ThreadMonitoring.Unregister();
        }

        private void StartThread(string name)
        {
            string str;
            str = "Genesyslab.PCT.Invoker";
            if (!string.IsNullOrEmpty(name))
                str = str + name;
            else
            {
                int num = threadNumber++;
                str = string.Format("{0}#{1}", str, num);
            }
            this.thread = new Thread(new ThreadStart(this.ProcessEvents));
            this.thread.Name = str;
            this.thread.IsBackground = true;
            this.thread.Start();
            base.Logger.DebugFormat("Thread '{0}' is started", new object[] { str });
        }

        private void StopInvoker()
        {
            this.active = false;
            if (this.thread == null)
            {
                return;
            }
            base.Logger.DebugFormat("Trying to stop thread '{0}'", new object[] { this.thread.Name });
            if (!this.thread.IsAlive)
            {
                this.thread = null;
            }
            if (this.thread != null && this.thread != Thread.CurrentThread)
            {
                if (!this.thread.Join(600))
                {
                    base.Logger.DebugFormat("Thread '{0}' IsAlive={1}", new object[] { this.thread.Name, (bool)this.thread.IsAlive });
                }
                this.thread.Abort();
            }
        }

        private class WaitCallbackTask
        {
            public readonly WaitCallback callback;
            public readonly object state;

            public WaitCallbackTask(WaitCallback callback, object state)
            {

                this.callback = callback;
                this.state = state;
                return;
            }
        }
    }
}

