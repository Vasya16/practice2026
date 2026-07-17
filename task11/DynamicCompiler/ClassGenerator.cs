using System;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DynamicCompiler;

public static class ClassGenerator
{
    public static ICalculator CreateCalculator()
    {
        string sourceCode = @"
            using DynamicCompiler;
            public class Calculator : ICalculator
            {
                public int Add(int a, int b) => a + b;
                public int Minus(int a, int b) => a - b;
                public int Mul(int a, int b) => a * b;
                public int Div(int a, int b) => a / b;
            }";

        return CompileSource(sourceCode);
    }

    public static ICalculator CompileSource(string sourceCode)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        string assemblyName = Path.GetRandomFileName();

        var references = new MetadataReference[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ICalculator).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location)
        };

        var compilation = CSharpCompilation.Create(
            assemblyName,
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (!result.Success)
        {
            var failures = result.Diagnostics.Where(diagnostic => 
                diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
            
            string errorMessages = string.Join(Environment.NewLine, failures.Select(f => f.GetMessage()));
            throw new InvalidOperationException($"Ошибка компиляции рантайм-кода:{Environment.NewLine}{errorMessages}");
        }

        ms.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());

        var type = assembly.GetTypes().FirstOrDefault(t => typeof(ICalculator).IsAssignableFrom(t) && !t.IsInterface)
            ?? throw new InvalidOperationException("В скомпилированной сборке не найден класс, реализующий ICalculator.");
            
        return (ICalculator)Activator.CreateInstance(type)!;
    }
}
