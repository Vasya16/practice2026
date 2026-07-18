using System;
using System.Threading;

namespace task14;

public static class DefiniteIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        if (threadsNumber <= 0)
            throw new ArgumentException("Количество потоков должно быть больше нуля.", nameof(threadsNumber));
        if (step <= 0)
            throw new ArgumentException("Размер шага должен быть больше нуля.", nameof(step));

        double globalSum = 0.0;
        long totalSteps = (long)Math.Ceiling((b - a) / step);
        if (totalSteps <= 0) return 0.0;

        long stepsPerThread = totalSteps / threadsNumber;
        long remainingSteps = totalSteps % threadsNumber;

        Thread[] threads = new Thread[threadsNumber];
        using Barrier barrier = new Barrier(threadsNumber + 1);

        long currentStartStep = 0;

        for (int i = 0; i < threadsNumber; i++)
        {
            long startStep = currentStartStep;
            long endStep = startStep + stepsPerThread + (i < remainingSteps ? 1 : 0);
            currentStartStep = endStep;

            threads[i] = new Thread(() =>
            {
                double localSum = 0.0;

                for (long stepIdx = startStep; stepIdx < endStep; stepIdx++)
                {
                    double x1 = a + stepIdx * step;
                    double x2 = Math.Min(a + (stepIdx + 1) * step, b);
                    localSum += (function(x1) + function(x2)) * 0.5 * (x2 - x1);
                }

                double initialTotal, computedTotal;
                do
                {
                    initialTotal = globalSum;
                    computedTotal = initialTotal + localSum;
                } 
                while (initialTotal != Interlocked.CompareExchange(ref globalSum, computedTotal, initialTotal));

                barrier.SignalAndWait();
            });

            threads[i].Start();
        }

        barrier.SignalAndWait();

        return globalSum;
    }
}