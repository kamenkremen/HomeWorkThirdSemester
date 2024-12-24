using System.Collections.Concurrent;
using MyThreadPool;

namespace ThreadPoolTests;

public class Tests
{

    [Test]
    public void TestOneTask()
    {
        MyThreadPool.MyThreadPool threadPool = new(8);
        var task = threadPool.Submit<int>(() => 2 + 2);
        Assert.That(task.Result, Is.EqualTo(4));
    }

    [Test]
    public void TestManyTasks()
    {
        MyThreadPool.MyThreadPool threadPool = new(8);
        var tasks = new IMyTask<int>[100];
        for (var i = 0; i <= 99; ++i) {
            var locali = i;
            tasks[i] = threadPool.Submit<int>(() => locali * locali);
        }
        for (var i = 0; i <= 99; ++i) {
            Assert.That(tasks[i].Result, Is.EqualTo(i * i));
        }
    }

    [Test]
    public void TestManySequentialTasks()
    {
        MyThreadPool.MyThreadPool threadPool = new(8);
        var tasks = new IMyTask<int>[100];
        tasks[0] = threadPool.Submit<int>(() => 1);
        for (var i = 1; i <= 99; ++i) {
            tasks[i] = tasks[i - 1].ContinueWith<int>((x) => x + 1);
        }
        Assert.That(tasks[99].Result, Is.EqualTo(100));
    }

    [Test]
    public void TestNumberOfThreads() 
    {
        MyThreadPool.MyThreadPool threadPool = new(8);
        HashSet<Thread?> threads = [];
        List<IMyTask<Thread>> tasks = [];
        ManualResetEvent mre = new (false);
        for (int i = 0; i < 10 * threadPool.NumberOfThreads; ++i)
        {
            tasks.Add(threadPool.Submit<Thread>(() => 
            {
                mre.WaitOne();
                return Thread.CurrentThread;
            }));
        }
        
        mre.Set();
        Thread.Sleep(200);
        threadPool.Shutdown();
        
        foreach(var task in tasks)
        {
            threads.Add(task.Result);
        }

        Assert.That(threads, Has.Count.EqualTo(threadPool.NumberOfThreads));
    }

    [Test]
    public async Task TestConcurrentShutdownAndSubmit()
    {
        var threadPool = new MyThreadPool.MyThreadPool(8);
        var tasks = new ConcurrentBag<IMyTask<int>> ();
        var expected = 10;
        ManualResetEvent mre = new (false);
        var task = Task.Run(() => 
        {
            mre.WaitOne();
            return threadPool.Submit<int>(() => 10);
        });
        var shutdown = Task.Run(() => 
        {
            mre.WaitOne();
            threadPool.Shutdown();
        });
        mre.Set();
        Thread.Sleep(200);
        
        try 
        {
            Assert.That((await task).Result, Is.EqualTo(expected));
        }
        catch (TaskCanceledException)
        {
            Assert.Pass();
        }

        Assert.Fail();
    }


    [Test]
    public void TestExceptionThrowing() 
    {
        MyThreadPool.MyThreadPool threadPool = new(8);
        var task = threadPool.Submit<int>(() => throw new ArgumentException());
        var result = 0;
        Assert.Throws<AggregateException>(() => result = task.Result);
    }

    [Test]
    public void TestExceptionThrowingAfterShutdown() 
    {
        MyThreadPool.MyThreadPool threadPool = new(8);
        threadPool.Shutdown();
        Assert.Throws<TaskCanceledException>(() => threadPool.Submit<int>(() => 2 + 2));
    }
}
