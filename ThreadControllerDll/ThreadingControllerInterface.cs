using System.Threading;
using ThreadControllerDll.Components;
using ThreadControllerDll.OtherComponents.ExtendedClasses;
using ThreadControllerDll.OtherComponents.LoggerTasks;

namespace ThreadControllerDll
{
    /// <summary>
    /// Interface to interact with the thread controller
    /// </summary>
    public class ThreadingControllerInterface
    {
        private readonly ThreadQueue threadQueue;
        private readonly ThreadMonitor threadMonitor;

        // Use on entering and exiting of methods to keep track of running methods
        // TODO: maybe should be replaced by queue size
        public readonly InterlockInt userMaintinedRunningMethods = new InterlockInt();

        /// <summary>
        /// Interface to interact with the thread controller
        /// </summary>
        /// <param name="name">Name to give to controlling interface for log recognition</param>
        /// <param name="loggerLevel">Log output level</param>
        /// <param name="maxThreads">Max concurrent threads allowed to run</param>
        public ThreadingControllerInterface(string name, Logger.Level loggerLevel, int maxThreads)
        {
            threadMonitor = new ThreadMonitor(name);
            threadQueue = new ThreadQueue(name, loggerLevel, maxThreads);
        }

        /// <summary>
        /// Adds thread to the monitor and queue
        /// </summary>
        /// <param name="thread">thread to add</param>
        public void AddThread(Thread thread)
        {
            threadQueue.AddThread(thread);
            threadMonitor.AddThread(thread);
        }

        /// <summary>
        /// Checks if the Monitor is still running
        /// </summary>
        /// <returns>bool to describe running state</returns>
        public bool IsStillRunning()
        {
            return threadMonitor.IsStillRunning();
        }

        /// <summary>
        /// Clears monitor list
        /// </summary>
        public void ClearMonitor()
        {
            threadMonitor.ClearList();
        }

        /// <summary>
        /// Gets current number of running methods as marked by the containing program
        /// </summary>
        /// <returns>count of running methods</returns>
        public int GetRunningMethodsIICount()
        {
            return userMaintinedRunningMethods.Get();
        }

        /// <summary>
        /// Increments the running methods
        /// </summary>
        public void RunningMethodsIIIncrement()
        {
            userMaintinedRunningMethods.Increment();
        }

        /// <summary>
        /// Decrements the running methods
        /// </summary>
        public void RunningMethodsIIDecrement()
        {
            userMaintinedRunningMethods.Decrement();
        }
    }
}