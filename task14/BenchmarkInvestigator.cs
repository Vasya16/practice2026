using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace task14;

public static class BenchmarkInvestigator
{
    public static (List<double> times, List<double> threads) GetResearchData()
    {
        double a = -100.0;
        double b = 100.0;
        Func<double, double> function = Math.Sin;
        double targetPrecision = 1e-4;

        double exactValue = 0.0; 
        double selectedStep = 1e-1;
        double[] stepsToTest = { 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6 };

        foreach (var step in stepsToTest)
        {
            double value = SingleThreadIntegral.Solve(a, b, function, step);
            if (Math.Abs(value - exactValue) <= targetPrecision)
            {
                selectedStep = step;
                break;
            }
        }

        int maxThreadsToTest = Environment.ProcessorCount * 2;
        List<double> threadCounts = new List<double>();
        List<double> executionTimes = new List<double>();

        StringBuilder report = new StringBuilder();
        report.AppendLine("=== ОТЧЕТ ПО ИССЛЕДОВАНИЮ ПРОИЗВОДИТЕЛЬНОСТИ ===");
        report.AppendLine($"Выбранный размер шага: {selectedStep}");

        for (int threads = 1; threads <= maxThreadsToTest; threads++)
        {
            double totalTime = 0;
            for (int run = 0; run < 5; run++)
            {
                Stopwatch sw = Stopwatch.StartNew();
                DefiniteIntegral.Solve(a, b, function, selectedStep, threads);
                sw.Stop();
                totalTime += sw.Elapsed.TotalMilliseconds;
            }
            double avgTime = totalTime / 5.0;
            report.AppendLine($"Потоков: {threads} | Время: {avgTime:F2} мс");

            threadCounts.Add(threads);
            executionTimes.Add(avgTime);
        }

        File.WriteAllText("benchmark_report.txt", report.ToString());
        return (executionTimes, threadCounts);
    }
}