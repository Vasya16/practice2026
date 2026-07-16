using System;

namespace task10;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class PluginLoadAttribute : Attribute
{
    public string PluginName { get; }
    public string[] Dependencies { get; }

    public PluginLoadAttribute(string pluginName, params string[] dependencies)
    {
        if (string.IsNullOrWhiteSpace(pluginName))
        {
            throw new ArgumentException("Имя плагина не может быть пустым.", nameof(pluginName));
        }
        PluginName = pluginName;
        Dependencies = dependencies ?? Array.Empty<string>();
    }
}
