# DemoThreadPool Docs
This is a test repo to learn and work on a custom Thread Pool in C#

This ThreadPool is based on a ConcurrentDictionary and sync locks to manage the queued work items. It is very basic so far and currently doesn't have a way to stop the threads.

# Usage
```
using DemoThreadPoolLibrary;
....

    public void Run(int maxThreads, int iterations)
    {
        var threadPool = new DemoThreadPool(maxThreads);
        var startTime = DateTime.UtcNow;
        for (int i = 0; i < iterations; ++i)
        {
            var index = i;
            threadPool.QueueUserWorkItem(() =>
            {
                Thread.Sleep(1);
                Console.WriteLine($"Executing thread-{index + 1}");
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
...

```


