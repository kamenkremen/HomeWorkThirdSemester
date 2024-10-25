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
        Assert.That(threadPool.NumberOfThreads >= 8);
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