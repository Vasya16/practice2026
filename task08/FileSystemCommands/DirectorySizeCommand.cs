using System;
using System.IO;
using System.Linq;
using CommandLib;
using task07;

namespace FileSystemCommands;

[DisplayName("Вычисление размера каталога")]
[Version(1, 0)]

public class DirectorySizeCommand : ICommand
{
    private readonly string _directoryPath;
    
    public DirectorySizeCommand(string directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
        {
            throw new ArgumentException("Путь к каталогу не может быть пустым.", nameof(directoryPath));
        }
        _directoryPath = directoryPath;
    }

    public void Execute()
    {
        if (!Directory.Exists(_directoryPath))
        {
            Console.WriteLine($"Ошибка: Каталог {_directoryPath} не существует.");
            return;
        }

        var di = new DirectoryInfo(_directoryPath);
        long totalSize = di.GetFiles("*", SearchOption.AllDirectories).Sum(f => f.Length);
        Console.WriteLine($"Размер каталога: {totalSize} байт");
    }
}