using System.Reflection;
using Xunit;
using task07;

namespace task07tests;

public class AttributeReflectionTests
{
    [Fact]
    public void Class_HasDisplayNameAttribute()
    {
        var type = typeof(SampleClass);
        var attribute = type.GetCustomAttribute<DisplayNameAttribute>();
        Assert.NotNull(attribute);
        Assert.Equal("Пример класса", attribute.DisplayName);
    }

    [Fact]
    public void Method_HasDisplayNameAttribute()
    {
        var method = typeof(SampleClass).GetMethod("TestMethod")!;
        var attribute = method.GetCustomAttribute<DisplayNameAttribute>();
        Assert.NotNull(attribute);
        Assert.Equal("Тестовый метод", attribute.DisplayName);
    }

    [Fact]
    public void Property_HasDisplayNameAttribute()
    {
        var prop = typeof(SampleClass).GetProperty("Number")!;
        var attribute = prop.GetCustomAttribute<DisplayNameAttribute>();
        Assert.NotNull(attribute);
        Assert.Equal("Числовое свойство", attribute.DisplayName);
    }

    [Fact]
    public void Class_HasVersionAttribute()
    {
        var type = typeof(SampleClass);
        var attribute = type.GetCustomAttribute<VersionAttribute>();
        Assert.NotNull(attribute);
        Assert.Equal(1, attribute.Major);
        Assert.Equal(0, attribute.Minor);
    }

    [Fact]
    public void PrintTypeInfo_OutputsCorrectText()
    {
        using var sw = new System.IO.StringWriter();
        Console.SetOut(sw);

        ReflectionHelper.PrintTypeInfo(typeof(SampleClass));

        var output = sw.ToString();

        Assert.Contains("Класс: Пример класса", output);
        Assert.Contains("Версия: 1.0", output);
        Assert.Contains("Свойства:", output);
        Assert.Contains("- Number: Числовое свойство", output);
        Assert.Contains("Методы:", output);
        Assert.Contains("- TestMethod: Тестовый метод", output);
    }
}