using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using task07;

namespace task09;

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Ошибка: Не указан путь к файлу динамической библиотеки.");
            Console.WriteLine("Использование: dotnet run --project task09/task09.csproj <путь_к_dll>");
            return;
        }

        string dllPath = args[0];

        if (!File.Exists(dllPath))
        {
            Console.WriteLine($"Ошибка: Файл по пути '{dllPath}' не найден.");
            return;
        }

        try
        {
            Console.WriteLine($"Анализ Метаданных Библиотеки: {Path.GetFileName(dllPath)} \n");

            Assembly assembly = Assembly.LoadFrom(dllPath);

            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsClass && !type.Name.Contains("<>"))
                {
                    Console.WriteLine($"========================================");
                    Console.WriteLine($"Класс: {type.FullName}");

                    var displayNameAttr = type.GetCustomAttribute<task07.DisplayNameAttribute>();
                    var versionAttr = type.GetCustomAttribute<VersionAttribute>();

                    if (displayNameAttr != null)
                        Console.WriteLine($" [Отображаемое имя]: {displayNameAttr.DisplayName}");
                    if (versionAttr != null)
                        Console.WriteLine($" [Версия]: {versionAttr.Major}.{versionAttr.Minor}");

                    Console.WriteLine("\n Конструкторы:");
                    ConstructorInfo[] constructors = type.GetConstructors();
                    if (constructors.Length == 0)
                    {
                        Console.WriteLine("   (Нет публичных конструкторов)");
                    }
                    foreach (var ctor in constructors)
                    {
                        Console.Write($"   -{type.Name}(");
                        ParameterInfo[] parameters = ctor.GetParameters();
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            Console.Write($"{parameters[i].ParameterType.Name} {parameters[i].Name}");
                            if (i < parameters.Length - 1)
                                Console.Write(", ");
                        }
                        Console.WriteLine(")");
                    }

                    Console.WriteLine("\n Методы:");
                    MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);

                    if (methods.Length == 0)
                    {
                        Console.WriteLine("   (Нет пользовательских методов)");
                    }
                    foreach (var method in methods)
                    {
                        if (method.IsSpecialName) continue;

                        Console.Write($"   -{method.ReturnType.Name} {method.Name}(");
                        ParameterInfo[] parameters = method.GetParameters();
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            Console.Write($"{parameters[i].ParameterType.Name} {parameters[i].Name}");
                            if (i < parameters.Length - 1)
                                Console.Write(", ");
                        }
                        Console.WriteLine(")");

                        var methodDisplay = method.GetCustomAttribute<task07.DisplayNameAttribute>();
                        if (methodDisplay != null)
                        {
                            Console.WriteLine($"   [Описание метода]: {methodDisplay.DisplayName}");
                        }
                    }
                    Console.WriteLine($"========================================\n");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла критическая ошибка при анализе рефлексии: {ex.Message}");
        }
    }
}