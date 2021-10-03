using System;
using System.Collections.Concurrent;
using System.Threading;

namespace DemoThreadPoolLibrary
{
    public delegate void WorkItem();

    public class DemoThreadPool
    {
        /// <summary>
        /// Stores the max thread size of the pool
        /// </summary>
        private readonly int poolSize;
        
        /// <summary>
        /// Thread Pool stored in a dictionary for O(1) removal of thread if required
        /// </summary>
        private readonly ConcurrentDictionary<string, Thread> threads;

        /// <summary>
        /// Queue for holding the work items
        /// </summary>
        private readonly ConcurrentQueue<WorkItem> workItemQueue;

        /// <summary>
        /// Stores the time stamp for the last completed thread
        /// </summary>
        private DateTime lastThreadCompleted;

        /// <summary>
        /// Gets the current Thread Count of the Pool
        /// </summary>
        public int ThreadCount
        {
            get
            {
                return threads.Count;
            }
        }

        /// <summary>
        /// Gets the count of pending work items
        /// </summary>
        public int WaitingQueueCount
        {
            get
            {
                return workItemQueue.Count;
            }
        }

        /// <summary>
        /// Gets the timestamp of the last completed thread
        /// </summary>
        public DateTime LastThreadCompleted
        {
            get
            {
                return lastThreadCompleted;
            }
        }

        /// <summary>
        /// Default constructor initializes the thread pool with current Logical Processor count
        /// </summary>
        public DemoThreadPool() : this(Environment.ProcessorCount) { }

        /// <summary>
        /// Initializes the thread pool with given maxThreads
        /// </summary>
        /// <param name="maxThreads"></param>
        /// <exception cref="ArgumentException"></exception>
        public DemoThreadPool(int maxThreads)
        {
            if (maxThreads < 2)
            {
                throw new ArgumentException($"{nameof(maxThreads)} should atleast be 1. By default {nameof(maxThreads)} is set to current logical processor count", nameof(maxThreads));
            }
            poolSize = maxThreads;
            threads = new ConcurrentDictionary<string, Thread>(maxThreads, maxThreads);
            workItemQueue = new ConcurrentQueue<WorkItem>();
            InitializeThreadPool();
        }

        /// <summary>
        /// Queues User Work Item in a concurrent queue and then signals other threads
        /// </summary>
        /// <param name="workItem"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void QueueUserWorkItem(WorkItem workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem), "WorkItem cannot be null");
            }

            workItemQueue.Enqueue(workItem);

            // Signal other threads that a new item is waiting for processor
            lock (workItemQueue)
            {
                Monitor.PulseAll(workItemQueue);
            }
        }

        /// <summary>
        /// Initializes the thread pool the first time
        /// </summary>
        private void InitializeThreadPool()
        {
            if (threads.Count < poolSize)
            {
                SpawnThreads();
            }
            else
            {
                lock (workItemQueue)
                {
                    Monitor.PulseAll(workItemQueue);
                }
            }
        }

        /// <summary>
        /// Spawns threads until the max limit is reached
        /// </summary>
        private void SpawnThreads()
        {
            while (threads.Count < poolSize)
            {
                var thread = new Thread(StartThread)
                {
                    Name = $"Thread-{threads.Count + 1}"
                };
                threads.TryAdd(thread.Name, thread);
                thread.Start();
            }
        }

        /// <summary>
        /// Starts the thread
        /// </summary>
        private void StartThread()
        {
            while (true)
            {
                if (RemoveThread())
                {
                    return;
                }
                while (workItemQueue.IsEmpty && poolSize >= threads.Count)
                {
                    lock (workItemQueue)
                    {
                        Monitor.Wait(workItemQueue);
                    }
                }
                if (RemoveThread())
                {
                    return;
                }
                workItemQueue.TryDequeue(out var workItem);
                if (workItem != null)
                {
                    workItem.DynamicInvoke();
                    lastThreadCompleted = DateTime.UtcNow;
                }
            }
        }

        /// <summary>
        /// Remove the thread
        /// </summary>
        /// <returns></returns>
        private bool RemoveThread()
        {
            if (poolSize < threads.Count)
            {
                return threads.TryRemove(Thread.CurrentThread.Name, out _);
            }

            return false;
        }
    }
}
