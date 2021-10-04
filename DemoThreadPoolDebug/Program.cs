using DemoThreadPoolLibrary;
using System;
using System.Threading;

namespace DemoThreadPoolDebug
{
    /// <summary>
    /// This class is being used to debug the thread pool and visually see the threads in action
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Environment.ProcessorCount);
            Run(2, 1000);
            Run(5, 1000);
            Run(100, 1000);
            Run(200, 1000);
        }

        static void Run(int maxThreads, int iterations)
        {
            var threadPool = new DemoThreadPool(maxThreads);
            var startTime = DateTime.UtcNow;
            for (int i = 0; i < iterations; ++i)
            {
                var index = i;
                threadPool.QueueUserWorkItem(() =>
                {
                    Thread.Sleep(1);
                    //Console.WriteLine($"Executing thread-{index + 1}");
                });
            }

            Console.WriteLine("Starting threads...");
            while (threadPool.WaitingQueueCount > 0)
            {
                Thread.Sleep(500);
            }           

            Console.WriteLine($"Thread Count-{threadPool.ThreadCount}");
            Console.WriteLine($"Total Time - {threadPool.LastThreadCompleted.Subtract(startTime)}");
        }
    }
}
