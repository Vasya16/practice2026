using System;
using System.IO;
using System.Reflection;
using CommandLib;

namespace CommandRunner;

internal class Program
{
    private static void Main(string[] args)
    {
        string testDir = Path.Combine(Path.GetTempPath(), "RunnerTestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "readme.txt"), "Содержимое текстового файла");
        File.WriteAllText(Path.Combine(testDir, "notes.log"), "Лог-файл");

        string dllPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "FileSystemCommands", "bin", "Debug", "net10.0", "FileSystemCommands.dll");

        if (!File.Exists(dllPath))
        {
            dllPath = Path.Combine(AppContext.BaseDirectory, "FileSystemCommands.dll");
            if (!File.Exists(dllPath))
            {
                Console.WriteLine("Ошибка: Не удалось найти файл FileSystemCommands.dll. Сначала соберите проект команд.");
                return;
            }
        }

        try
        {
            Console.WriteLine("Запуск динамической загрузки библиотеки");
            Assembly assembly = Assembly.LoadFrom(dllPath);

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(ICommand).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    Console.WriteLine($"\nОбнаружена команда: {type.Name}");
                    ICommand? commandInstance = null;

                    if (type.Name == "DirectorySizeCommand")
                    {
                        commandInstance = (ICommand)Activator.CreateInstance(type, testDir)!;
                    }
                    else if (type.Name == "FindFilesCommand")
                    {
                        commandInstance = (ICommand)Activator.CreateInstance(type, testDir, "*.txt")!;
                    }

                    commandInstance?.Execute();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка при динамической загрузке: {ex.Message}");
        }
        finally
        {
            if (Directory.Exists(testDir))
            {
                Directory.Delete(testDir, true);
            }
            Console.WriteLine("\nРабота программы завершена");
        }
    }
}