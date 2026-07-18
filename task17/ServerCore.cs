using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17;

public interface ICommand
{
    void Execute();
}

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue = new();
    private readonly Thread _thread;
    private bool _stopRequested;
    private bool _hardStop;

    public ServerThread()
    {
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
            if (_stopRequested && _queue.Count == 0) break;

            try
            {
                ICommand cmd = _queue.Take();
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
