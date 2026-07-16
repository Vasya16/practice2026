using System;
using System.IO;
using CommandLib;
using task07;

namespace FileSystemCommands;

[DisplayName("Поиск файлов по маске")]
[Version(1, 0)]

public class FindFilesCommand : ICommand
{
    private readonly string _directoryPath;
    private readonly string _searchPattern;

    public FindFilesCommand(string directoryPath, string searchPattern)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
        {
            throw new ArgumentException("Путь к каталогу не может быть пустым или состоять из пробелов.", nameof(directoryPath));
        }
        if (string.IsNullOrWhiteSpace(searchPattern))
        {
            throw new ArgumentException("Маска поиска файлов не может быть пустой или состоять из пробелов.", nameof(searchPattern));
        }
        _directoryPath = directoryPath;
        _searchPattern = searchPattern;
    }

    public void Execute()
    {
        if (!Directory.Exists(_directoryPath))
        {
            Console.WriteLine($"Ошибка: Каталог {_directoryPath} не существует.");
            return;
        }

        var di = new DirectoryInfo(_directoryPath);
        var files = di.GetFiles(_searchPattern);
        foreach (var file in files)
        {
            Console.WriteLine(file.Name);
        }
    }
}