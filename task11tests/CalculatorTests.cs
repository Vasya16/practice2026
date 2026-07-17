using System;
using Xunit;
using DynamicCompiler;

namespace task11tests;

public class CalculatorTests
{
    [Fact]
    public void CreateCalculator_ShouldPerformAllOperationsWithoutReflection()
    {
        ICalculator calc = ClassGenerator.CreateCalculator();

        Assert.Equal(12, calc.Add(7, 5));
        Assert.Equal(2, calc.Minus(7, 5));
        Assert.Equal(35, calc.Mul(7, 5));
        Assert.Equal(2, calc.Div(10, 5));
    }

    [Fact]
    public void DynamicCalculator_DivideByZero_ShouldThrowDivideByZeroException()
    {
        ICalculator calc = ClassGenerator.CreateCalculator();

        Assert.Throws<DivideByZeroException>(() => calc.Div(5, 0));
    }

    [Fact]
    public void ClassGenerator_WithInvalidCode_ShouldThrowInvalidOperationException()
    {
        string brokenCode = "using DynamicCompiler; public class Broken : ICalculator { public int Add(int a, int b) => a + b;";
        // тест на специально пропущенную скобку

        Assert.Throws<InvalidOperationException>(() => ClassGenerator.CompileSource(brokenCode));
    }
}
