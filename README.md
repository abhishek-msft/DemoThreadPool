# DemoThreadPool Docs
This is a test repo to learn and work on a custom Thread Pool in C#

This ThreadPool is based on a ConcurrentDictionary and sync locks to manage the queued work items. It is very basic so far and currently doesn't have a way to stop the threads.

# Usage
```
using DemoThreadPoolLibrary;
....

public void SomeMethod()
{
  var threadPool = new ThreadPool(4);
  for (int i=0; i<100; i++)
  {
    pool.QueueWorkItem((data) =>
    {
      Console.WriteLine($"Running {i}th thread.");
    }, i);
  }
}
...

```


