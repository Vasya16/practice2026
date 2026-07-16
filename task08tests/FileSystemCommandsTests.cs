using System;
using System.IO;
using Xunit;
using FileSystemCommands;

namespace task08tests;

public class FileSystemCommandsTests
{
    [Fact]
    public void DirectorySizeCommand_ShouldCalculateSize()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDirSize");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "test1.txt"), "Hello");
        File.WriteAllText(Path.Combine(testDir, "test2.txt"), "World");

        using var sw = new StringWriter();
        Console.SetOut(sw);

        var command = new DirectorySizeCommand(testDir);
        command.Execute();

        var output = sw.ToString();

        Assert.Contains("10", output);

        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void FindFilesCommand_ShouldFindMatchingFiles()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDirFind");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "file1.txt"), "Text");
        File.WriteAllText(Path.Combine(testDir, "file2.log"), "Log");

        using var sw = new StringWriter();
        Console.SetOut(sw);

        var command = new FindFilesCommand(testDir, "*.txt");
        command.Execute();

        var output = sw.ToString();

        Assert.Contains("file1.txt", output);
        Assert.DoesNotContain("file2.log", output);

        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);

        Directory.Delete(testDir, true);
    }
}