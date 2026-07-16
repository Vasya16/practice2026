using System;

namespace task10;

[PluginLoad("PluginA")]
public class PluginA : ICommand
{
    public void Execute()
    {
        Console.WriteLine("[PluginA] Успешно выполнен.");
    }
}

[PluginLoad("PluginB", "PluginA")]
public class PluginB : ICommand
{
    public void Execute()
    {
        Console.WriteLine("[PluginB] Успешно выполнен после PluginA.");
    }
}