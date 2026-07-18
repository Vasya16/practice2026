using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17;

public interface ICommand
{
    void Execute();
}

public interface IScheduler
{
    bool HasCommand();
    ICommand Select();
    void Add(ICommand cmd);
}

public class RoundRobinScheduler : IScheduler
{
    private readonly ConcurrentQueue<ICommand> _queue = new();
    public bool HasCommand() => !_queue.IsEmpty;
    public ICommand Select() => _queue.TryDequeue(out var cmd) ? cmd : throw new InvalidOperationException();
    public void Add(ICommand cmd) => _queue.Enqueue(cmd);
}

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue = new();
    private readonly Thread _thread;
    private readonly IScheduler _scheduler;
    private bool _stopRequested;
    private bool _hardStop;

    public ServerThread(IScheduler scheduler)
    {
        _scheduler = scheduler;
        _thread = new Thread(ProcessQueue);
    }

    public void Start() => _thread.Start();
    public void Enqueue(ICommand cmd) => _queue.Add(cmd);

    public void StopHard()
    {
        if (Thread.CurrentThread != _thread) throw new InvalidOperationException();
        _hardStop = true;
    }

    public void StopSoft()
    {
        if (Thread.CurrentThread != _thread) throw new InvalidOperationException();
        _stopRequested = true;
    }

    public void Join() => _thread.Join();

    private void ProcessQueue()
    {
        while (true)
        {
            if (_hardStop) break;
            if (_stopRequested && _queue.Count == 0 && !_scheduler.HasCommand()) break;

            if (_queue.TryTake(out var cmd) || (_scheduler.HasCommand() && (cmd = _scheduler.Select()) != null))
            {
                try { cmd.Execute(); } catch {}
                continue;
            }

            if (_stopRequested) break;

            try
            {
                cmd = _queue.Take();
                try { cmd.Execute(); } catch {}
            }
            catch { break; }
        }
    }
}

public class HardStopCommand : ICommand
{
    private readonly ServerThread _serverThread;
    public HardStopCommand(ServerThread serverThread) => _serverThread = serverThread;
    public void Execute() => _serverThread.StopHard();
}

public class SoftStopCommand : ICommand
{
    private readonly ServerThread _serverThread;
    public SoftStopCommand(ServerThread serverThread) => _serverThread = serverThread;
    public void Execute() => _serverThread.StopSoft();
}

public class TestCommand : ICommand
{
    private readonly int _id;
    private readonly IScheduler _scheduler;
    private int _counter;
    public static readonly System.Collections.Generic.List<(int TaskId, int Step, double StartMs, double EndMs)> Timeline = new();
    public static System.Diagnostics.Stopwatch GlobalSw = new();

    public TestCommand(int id, IScheduler scheduler)
    {
        _id = id;
        _scheduler = scheduler;
    }

    public void Execute()
    {
        if (!GlobalSw.IsRunning) GlobalSw.Start();
        double start = GlobalSw.Elapsed.TotalMilliseconds;
        _counter++;
        
        Thread.Sleep(15); 
        
        double end = GlobalSw.Elapsed.TotalMilliseconds;
        lock (Timeline) { Timeline.Add((_id, _counter, start, end)); }

        _scheduler.Add(this);
    }
}
