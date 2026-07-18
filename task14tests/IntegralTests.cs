using System;
using Xunit;
using task14;

namespace task14tests;

public class IntegralTests
{
    [Fact]
    public void Solve_LinearFunction_ShouldReturnZero()
    {
        Func<double, double> X = (double x) => x;
        double result = DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2);
        Assert.Equal(0, result, 1e-4);
    }

    [Fact]
    public void Solve_SinFunction_ShouldReturnZero()
    {
        Func<double, double> SIN = (double x) => Math.Sin(x);
        double result = DefiniteIntegral.Solve(-1, 1, SIN, 1e-5, 8);
        Assert.Equal(0, result, 1e-4);
    }

    [Fact]
    public void Solve_LinearFunction_ShouldReturnArea()
    {
        Func<double, double> X = (double x) => x;
        double result = DefiniteIntegral.Solve(0, 5, X, 1e-6, 8);
        Assert.Equal(12.5, result, 1e-5);
    }
}
