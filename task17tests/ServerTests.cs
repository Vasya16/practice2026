using System;
using System.Threading;
using System.Collections.Generic;
using Xunit;
using task17;

namespace task17tests;

public class ServerTests
{
    [Fact]
    public void HardStop_ShouldStopImmediately()
    {
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);
        server.Start();
        server.Enqueue(new HardStopCommand(server));
        bool wasExecuted = false;
        server.Enqueue(new ActionCommand(() => wasExecuted = true));
        server.Join();
        Assert.False(wasExecuted);
    }

    [Fact]
    public void SoftStop_ShouldExecuteRemainingCommands()
    {
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);
        server.Start();
        bool wasExecuted = false;
        server.Enqueue(new SoftStopCommand(server));
        server.Enqueue(new ActionCommand(() => wasExecuted = true));
        server.Join();
        Assert.True(wasExecuted);
    }

    [Fact]
    public void Run_Task18_Performance_Graph()
    {
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);
        server.Start();

        List<double> taskCounts = new List<double>();
        List<double> processTimes = new List<double>();

        using var syncEvent = new AutoResetEvent(false);

        for (int i = 1; i <= 200; i++)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            
            server.Enqueue(new ActionCommand(() =>
            {
                Thread.Sleep(1); 
                sw.Stop();
                lock (processTimes)
                {
                    taskCounts.Add(i);
                    processTimes.Add(sw.Elapsed.TotalMilliseconds);
                }
                syncEvent.Set();
            }));

            syncEvent.WaitOne();
        }

        server.Enqueue(new HardStopCommand(server));
        server.Join();

        var plt = new ScottPlot.Plot();
        plt.Title("Зависимость времени от количества команд");
        plt.XLabel("Количество команд");
        plt.YLabel("Время (мс)");
        
        lock (processTimes)
        {
            plt.Add.Scatter(taskCounts.ToArray(), processTimes.ToArray());
        }
        
        plt.SavePng("task18_performance.png", 600, 400);
        Assert.True(System.IO.File.Exists("task18_performance.png"));
    }

    [Fact]
    public void Simulation_Task19_ShouldRunFiveCommandsThreeTimes()
    {
        List<double> tickCounts = new List<double> { 5, 10, 15, 20 };
        List<double> totalTimes = new List<double>();

        foreach (int maxTicks in tickCounts)
        {
            var scheduler = new RoundRobinScheduler();
            var server = new ServerThread(scheduler);
            
            lock (TestCommand.Timeline) { TestCommand.Timeline.Clear(); }
            TestCommand.GlobalSw = System.Diagnostics.Stopwatch.StartNew();
            
            server.Start();
            
            for (int i = 1; i <= 5; i++)
            {
                server.Enqueue(new TestCommand(i, scheduler));
            }

            while (true)
            {
                lock (TestCommand.Timeline)
                {
                    if (TestCommand.Timeline.Count >= 5 * maxTicks) break;
                }
                Thread.Sleep(10);
            }

            double elapsed = TestCommand.GlobalSw.Elapsed.TotalMilliseconds;
            totalTimes.Add(elapsed);

            server.Enqueue(new HardStopCommand(server));
            server.Join();
        }

        var plt = new ScottPlot.Plot();
        plt.Title("Зависимость времени от количества тиков");
        plt.XLabel("Тиков на команду");
        plt.YLabel("Время (мс)");
        
        plt.Add.Scatter(tickCounts.ToArray(), totalTimes.ToArray());
        plt.Axes.AutoScale();
        
        plt.SavePng("task19_gantt.png", 600, 400);
        Assert.True(System.IO.File.Exists("task19_gantt.png"));
    }
}

public class ActionCommand : ICommand
{
    private readonly Action _action;
    public ActionCommand(Action action) => _action = action;
    public void Execute() => _action();
}
