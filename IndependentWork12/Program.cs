using System.Diagnostics;
using System.Text;

namespace IndependentWork12;

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("Порівняння LINQ і PLINQ");

        _ = Enumerable.Range(1, 2_000).Sum(ExpensiveOperation);
        foreach (var size in new[] { 1_000_000, 5_000_000, 10_000_000 })
            RunBenchmark(size);

        Console.WriteLine("\nПобічні ефекти PLINQ:");
        DemonstrateUnsafeCounter();
        DemonstrateSafeCounter();
    }

    private static void RunBenchmark(int size)
    {
        var random = new Random(2026);
        var values = Enumerable.Range(0, size)
            .Select(_ => random.Next(1, 100_000))
            .ToList();

        var sequential = Measure("LINQ", () => values
            .Where(value => value % 2 == 0)
            .Select(ExpensiveOperation)
            .Sum());
        var parallel = Measure("PLINQ", () => values
            .AsParallel()
            .WithDegreeOfParallelism(Environment.ProcessorCount)
            .Where(value => value % 2 == 0)
            .Select(ExpensiveOperation)
            .Sum());

        Console.WriteLine($"{size:N0}: LINQ {sequential.ElapsedMilliseconds} мс, " +
            $"PLINQ {parallel.ElapsedMilliseconds} мс");
    }

    private static (long ElapsedMilliseconds, long Result) Measure(
        string name,
        Func<long> operation)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = operation();
        stopwatch.Stop();
        return (stopwatch.ElapsedMilliseconds, result);
    }

    private static long ExpensiveOperation(int initial)
    {
        var value = (long)initial;
        for (var i = 0; i < 160; i++)
        {
            value = (value * 48271 + i * 17) % 1_000_000_007;
            value ^= value >> 7;
        }
        return value;
    }

    private static void DemonstrateUnsafeCounter()
    {
        var unsafeCount = 0;
        _ = Enumerable.Range(1, 200_000)
            .AsParallel()
            .Select(value =>
            {
                unsafeCount++;
                return value;
            })
            .ToList();
        Console.WriteLine($"Unsafe Count: {unsafeCount}, очікувалося 200000");
    }

    private static void DemonstrateSafeCounter()
    {
        var safeCount = 0;
        _ = Enumerable.Range(1, 200_000)
            .AsParallel()
            .Select(value =>
            {
                Interlocked.Increment(ref safeCount);
                return value;
            })
            .ToList();
        Console.WriteLine($"Interlocked Count: {safeCount}, очікувалося 200000");
    }
}
