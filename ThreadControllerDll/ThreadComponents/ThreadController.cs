using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ThreadControllerDll.OtherComponents.LoggerTasks;

namespace ThreadControllerDll.Components
{
    /// <summary>
    /// Maintains the collection of running threads
    /// </summary>
    public class ThreadController
    {
        private readonly string _ControllerName;
        private readonly Semaphore listLock = new Semaphore(1, 1);
        private readonly List<Thread> _threads = new List<Thread>();
        private readonly int MAX_THREADS = 5;
        private Thread controllerThread;

        // Auto implemented to 0
        private int emptyTimeoutCount;

        private readonly Logger logger;

        /// <summary>
        /// Initialized thread controller
        /// </summary>
        /// <param name="name">Name of the System</param>
        /// <param name="loggerLevel">Log output level</param>
        /// <param name="maxThreads">Max concurrent threads allowed</param>
        public ThreadController(string name, Logger.Level loggerLevel, int maxThreads)
        {
            MAX_THREADS = maxThreads;
            _ControllerName = name;
            logger = new Logger(loggerLevel);
            controllerThread = new Thread(() => RunController());
            controllerThread.Start();
        }

        /// <summary>
        /// Starts the controller
        /// </summary>
        private void RunController()
        {//Remove dead threads and shutdown if empty for 2 minutes
            logger.Log(_ControllerName + " controller is starting", Logger.Level.PROCCESS);
            emptyTimeoutCount = 0;
            // Should reach after 2 minutes without a thread to run
            while (emptyTimeoutCount != 12000)
            {
                Thread.Sleep(10);
                //If there are threads, run maintenance
                if (_threads.Count > 0)
                {
                    listLock.WaitOne();
                    // Remove any dead threads
                    // TODO: this could probably be written more succinctly
                    if (_threads.Any(thread => !thread.IsAlive))
                        _threads.RemoveAll(thread => !thread.IsAlive);
                    listLock.Release();
                    // reset counter because we processed threads
                    emptyTimeoutCount = 0;
                }
                else
                {
                    emptyTimeoutCount++;
                }
            }
            logger.Log(_ControllerName + " controller is going to sleep", Logger.Level.PROCCESS);
        }

        /// <summary>
        /// Attempts to add a non started thread to the controller.
        /// </summary>
        /// <param name="thread">Thread to be added</param>
        /// <returns>Returns a bool as whether the add was successfully added</returns>
        public bool TryAdd(Thread thread)
        {
            listLock.WaitOne();
            //If we have max thread, abort
            if (_threads.Count == MAX_THREADS)
            {
                listLock.Release();
                return false;
            }
            //Pre add the thread so if we need to restart controller it is not empty
            _threads.Add(thread);
            thread.Start();
            //Restart controller if asleep
            if (!controllerThread.IsAlive)
            {
                controllerThread = new Thread(() => RunController());
                controllerThread.Start();
            }
            listLock.Release();
            return true;
        }
    }
}