using System;
using System.IO;

namespace task10;
internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Система Динамических Плагинов С Графом Зависимостей");
        
        string pluginsDir = AppContext.BaseDirectory;
        PluginManager.DiscoverAndRunPlugins(pluginsDir);
    }
}