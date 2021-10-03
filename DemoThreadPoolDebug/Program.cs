using DemoThreadPoolLibrary;
using System;
using System.Threading;

namespace DemoThreadPoolDebug
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Environment.ProcessorCount);
            Run(2, 100);
            Run(5, 100);
            Run(100, 100);
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

            Console.WriteLine("Starting the jobs just in a moment...");
            while (threadPool.WaitingQueueCount > 0)
            {
                Thread.Sleep(500);
            }           

            Console.WriteLine($"Thread Count-{threadPool.ThreadCount}");
            Console.WriteLine($"Total Time - {threadPool.LastThreadCompleted.Subtract(startTime)}");
            //Console.ReadLine();
        }
    }
}
