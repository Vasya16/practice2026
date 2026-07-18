using System;

namespace task14;

public static class SingleThreadIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step)
    {
        if (step <= 0)
            throw new ArgumentException("Размер шага должен быть больше нуля.", nameof(step));

        double globalSum = 0.0;
        long totalSteps = (long)Math.Ceiling((b - a) / step);
        if (totalSteps <= 0) return 0.0;

        for (long stepIdx = 0; stepIdx < totalSteps; stepIdx++)
        {
            double x1 = a + stepIdx * step;
            double x2 = Math.Min(a + (stepIdx + 1) * step, b);
            globalSum += (function(x1) + function(x2)) * 0.5 * (x2 - x1);
        }

        return globalSum;
    }
}
