using System;
using System.Collections.Concurrent;
using System.Threading;
using ThreadControllerDll.OtherComponents.LoggerTasks;

namespace ThreadControllerDll.Components
{
    public class ThreadQueue
    {
        private readonly string _QueueName;
        private BlockingCollection<Thread> threadsCollection = new BlockingCollection<Thread>();
        private readonly ThreadController threadController;
        private readonly Logger logger;

        public ThreadQueue(string name, Logger.Level loggerLevel, int maxThreads)
        {
            _QueueName = name;
            logger = new Logger(loggerLevel);
            threadController = new ThreadController(_QueueName, loggerLevel, maxThreads);
            new Thread(() => ConsumeItems()).Start();
        }

        private readonly object restartLock = new object();

        public void AddThread(Thread thread)
        {
            try
            {
                //First Attempt at adding thread
                while (!threadsCollection.TryAdd(thread, TimeSpan.FromMilliseconds(10)))
                {
                    logger.Log(_QueueName + " collection is full, retrying...", Logger.Level.INFO);
                }
            }
            catch (InvalidOperationException ex)
            {
                logger.Log(_QueueName + ex.Message, Logger.Level.ERROR);
                logger.Log(_QueueName + " collection is restarting, retrying...", Logger.Level.INFO);
                //The collection was put in sleep state. Lets restart it. Use lock to prevent concurrent restarts
                lock (restartLock)
                {
                    if (!threadsCollection.IsCompleted)
                    {
                        //If collection was already restarted, just add new value
                        while (!threadsCollection.TryAdd(thread, TimeSpan.FromMilliseconds(10)))
                        {
                            logger.Log(_QueueName + " collection is full, retrying...", Logger.Level.INFO);
                        }
                    }
                    else
                    {
                        //make sure old collection has finished
                        while (!(threadsCollection.IsAddingCompleted && threadsCollection.IsCompleted))
                        {
                            logger.Log(_QueueName + " collection is waiting to restart, retrying...", Logger.Level.PROCCESS);
                            Thread.Sleep(100);
                        }
                        // Restart collection with new thread
                        threadsCollection = new BlockingCollection<Thread>
                            {
                                thread
                            };
                        new Thread(() => ConsumeItems()).Start();
                    }
                }
            }
        }

        private void ConsumeItems()
        {
            logger.Log(_QueueName + "collection is starting", Logger.Level.PROCCESS);
            //Count how many times we time out in a row
            int emptyTimeoutCount = 0;
            // This will only be true if CompleteAdding() was called AND the bin is empty.
            while (!threadsCollection.IsCompleted)
            {
                if (!threadsCollection.TryTake(out Thread item, TimeSpan.FromMilliseconds(1000)))
                {
                    //Increment if we have an empty Collection
                    logger.Log(_QueueName + "collection is empty, retrying...", Logger.Level.INFO);
                    emptyTimeoutCount++;
                }
                else
                {
                    while (!threadController.TryAdd(item))
                    { //Pass thread over to controller
                        Thread.Sleep(20);
                    }
                    emptyTimeoutCount = 0;
                }
                //End if we reach a count of 120 (should be about 2 minutes)
                if (emptyTimeoutCount == 120)
                    threadsCollection.CompleteAdding();
            }
            logger.Log(_QueueName + "collection is going to sleep", Logger.Level.PROCCESS);
        }
    }
}