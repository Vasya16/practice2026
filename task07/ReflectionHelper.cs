using System;
using System.Reflection;

namespace task07;

public static class ReflectionHelper
{
    public static void PrintTypeInfo(Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type), "Переданный тип не может быть null.");
        }

        var classDisplayNameAttr = type.GetCustomAttribute<DisplayNameAttribute>();
        if (classDisplayNameAttr != null)
        {
            Console.WriteLine($"Класс: {classDisplayNameAttr.DisplayName}");
        }

        var classVersionAttr = type.GetCustomAttribute<VersionAttribute>();
        if (classVersionAttr != null)
        {
            Console.WriteLine($"Версия: {classVersionAttr.Major}.{classVersionAttr.Minor}");
        }

        Console.WriteLine("Свойства:");
        foreach (var prop in type.GetProperties())
        {
            var propAttr = prop.GetCustomAttribute<DisplayNameAttribute>();
            if (propAttr != null)
            {
                Console.WriteLine($" - {prop.Name}: {propAttr.DisplayName}");
            }
        }
        Console.WriteLine("Методы:");
        foreach (var meth in type.GetMethods())
        {
            var methAttr = meth.GetCustomAttribute<DisplayNameAttribute>();
            if (methAttr != null)
            {
                Console.WriteLine($" - {meth.Name}: {methAttr.DisplayName}");
            }
        }
    }
}