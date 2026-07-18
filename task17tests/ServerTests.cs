using System;
using Xunit;
using task17;

namespace task17tests;

public class ServerTests
{
    [Fact]
    public void HardStop_ShouldStopImmediately()
    {
        var server = new ServerThread();
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
        var server = new ServerThread();
        server.Start();
        bool wasExecuted = false;
        server.Enqueue(new SoftStopCommand(server));
        server.Enqueue(new ActionCommand(() => wasExecuted = true));
        server.Join();
        Assert.True(wasExecuted);
    }

    [Fact]
    public void StopCommands_FromWrongThread_ShouldThrowException()
    {
        var server = new ServerThread();
        Assert.Throws<InvalidOperationException>(() => server.StopHard());
        Assert.Throws<InvalidOperationException>(() => server.StopSoft());
    }
}

public class ActionCommand : ICommand
{
    private readonly Action _action;
    public ActionCommand(Action action) => _action = action;
    public void Execute() => _action();
}
