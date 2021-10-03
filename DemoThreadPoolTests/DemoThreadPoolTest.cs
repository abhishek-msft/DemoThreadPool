using DemoThreadPoolLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace DemoThreadPoolTests
{
    [TestClass]
    public class DemoThreadPoolTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Assert_ThreadPool_Should_Have_Valid_MaxThreads()
        {
            _ = new DemoThreadPool(0);
        }

        [TestMethod]
        public void Assert_ThreadPool_Is_Initialized_With_LogicalProcessorCount()
        {
            var threadPool = new DemoThreadPool();
            Assert.AreEqual(Environment.ProcessorCount, threadPool.ThreadCount);
        }

        [TestMethod]
        public void Assert_ThreadPool_Is_Initialized_With_MaxThreads()
        {
            var maxThreads = 4;
            var threadPool = new DemoThreadPool(maxThreads);
            Assert.AreEqual(maxThreads, threadPool.ThreadCount);
        }

        [TestMethod]
        public void Test_ThreadPool_With_Higher_Threads_Is_Faster()
        {
            var resultWithTwoThreads = RunTest(2, 1000);
            var resultWithHundredThreads = RunTest(10, 1000);

            Assert.IsTrue(resultWithTwoThreads.Item1.Milliseconds > resultWithHundredThreads.Item1.Milliseconds);
        }

        private static Tuple<TimeSpan, int> RunTest(int maxThreads, int iterations)
        {
            var threadPool = new DemoThreadPool(maxThreads);
            var startTime = DateTime.UtcNow;
            for (int i = 0; i < iterations; ++i)
            {
                var index = i;
                threadPool.QueueUserWorkItem(() =>
                {
                    Thread.Sleep(1);
                });
            }

            while (threadPool.WaitingQueueCount > 0)
            {
                Thread.Sleep(500);
            }

            return new Tuple<TimeSpan, int>(threadPool.LastThreadCompleted.Subtract(startTime), threadPool.ThreadCount);
        }

    }
}
