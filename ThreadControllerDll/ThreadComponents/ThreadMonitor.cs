using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ThreadControllerDll.Components
{
    /// <summary>
    /// Monitor system for running threads
    /// </summary>
    public class ThreadMonitor
    {
        public string Name { get; protected set; }

        private readonly List<Thread> threads = new List<Thread>();
        private readonly Semaphore semaphore;

        /// <summary>
        /// Initialize Thread Monitor Class
        /// </summary>
        /// <param name="name">Name of Thread Monitor</param>
        public ThreadMonitor(string name)
        {
            threads = new List<Thread>();
            Name = name;
            semaphore = new Semaphore(1, 1);
        }

        /// <summary>
        /// Adds thread to monitor list
        /// </summary>
        /// <param name="thread">Thread to add</param>
        public Thread AddThread(Thread thread)
        {
            semaphore.WaitOne();
            threads.Add(thread);
            semaphore.Release();
            return thread;
        }

        /// <summary>
        /// Clears list of threads
        /// </summary>
        public void ClearList()
        {
            semaphore.WaitOne();
            threads.Clear();
            semaphore.Release();
        }

        /// <summary>
        /// Sees whether we have any running threads
        /// </summary>
        public bool IsStillRunning()
        {
            semaphore.WaitOne();
            var retVal = threads.Any(element => element.ThreadState == ThreadState.Unstarted) && threads.Any(element => !element.IsAlive);
            semaphore.Release();
            return retVal;
        }
    }
}