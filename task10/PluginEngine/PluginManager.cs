using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace PluginEngine;

public class PluginInfo
{
    public Type PluginType { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string[] Dependencies { get; set; } = Array.Empty<string>();
}

public static class PluginManager
{
    public static void DiscoverAndRunPlugins(string pluginsDirectory)
    {
        if (!Directory.Exists(pluginsDirectory))
        {
            Console.WriteLine($"Ошибка: Директория {pluginsDirectory} не существует.");
            return;
        }

        string[] allDlls = Directory.GetFiles(pluginsDirectory, "*.dll");
        string[] dllFiles = allDlls.Where(file => !file.Contains("Microsoft") && !file.Contains("xunit") && !file.Contains("testhost")).ToArray();
        var discoveredPlugins = new List<PluginInfo>();
        var loadContext = new AssemblyLoadContext("PluginContext", isCollectible: true);

        foreach (string dllPath in dllFiles)
        {
            try
            {
                Assembly assembly = loadContext.LoadFromAssemblyPath(dllPath);

                foreach (Type type in assembly.GetTypes())
                {
                    var attr = type.GetCustomAttribute<PluginLoadAttribute>();
                    if (attr != null && typeof(ICommand).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    {
                        discoveredPlugins.Add(new PluginInfo
                        {
                            PluginType = type,
                            Name = attr.PluginName,
                            Dependencies = attr.Dependencies
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Предупреждение: Не удалось загрузить плагин из {Path.GetFileName(dllPath)}: {ex.Message}");
            }
        }

        List<PluginInfo> sortedPlugins = SortPluginsByDependencies(discoveredPlugins);

        Console.WriteLine("\n--- Запуск плагинов в порядке зависимостей ---");
        foreach (var plugin in sortedPlugins)
        {
            try
            {
                Console.WriteLine($"\n[Система] Активация плагина: {plugin.Name}");
                var command = (ICommand)Activator.CreateInstance(plugin.PluginType)!;
                command.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при выполнении плагина {plugin.Name}: {ex.Message}");
            }
        }
    }

    public static List<PluginInfo> SortPluginsByDependencies(List<PluginInfo> plugins)
    {
        var sorted = new List<PluginInfo>();
        var visited = new Dictionary<string, bool>();
        var pluginDict = plugins.ToDictionary(p => p.Name, p => p);

        void Visit(PluginInfo plugin)
        {
            if (visited.TryGetValue(plugin.Name, out bool isCompleted))
            {
                if (!isCompleted)
                {
                    throw new InvalidOperationException($"Обнаружена циклическая зависимость в плагине: {plugin.Name}");
                }
                return;
            }

            visited[plugin.Name] = false;

            foreach (string dependency in plugin.Dependencies)
            {
                if (pluginDict.TryGetValue(dependency, out var depPlugin))
                {
                    Visit(depPlugin);
                }
                else
                {
                    Console.WriteLine($"Предупреждение: Зависимость '{dependency}' для плагина '{plugin.Name}' не найдена.");
                }
            }

            visited[plugin.Name] = true;
            sorted.Add(plugin);
        }

        foreach (var plugin in plugins)
        {
            if (!visited.ContainsKey(plugin.Name))
            {
                Visit(plugin);
            }
        }

        return sorted;
    }
}