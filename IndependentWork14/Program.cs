using System.Text;

namespace IndependentWork14;

public sealed class RetryPolicy
{
    private readonly int _maxRetries;
    private readonly TimeSpan _initialDelay;
    private readonly double _jitterFactor;
    private readonly Random _random = new();
    private readonly object _randomLock = new();

    public RetryPolicy(
        int maxRetries,
        TimeSpan initialDelay,
        double jitterFactor = 0.2)
    {
        if (maxRetries < 0)
            throw new ArgumentOutOfRangeException(nameof(maxRetries));
        if (initialDelay < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(initialDelay));
        if (jitterFactor < 0)
            throw new ArgumentOutOfRangeException(nameof(jitterFactor));

        _maxRetries = maxRetries;
        _initialDelay = initialDelay;
        _jitterFactor = jitterFactor;
    }

    public T Execute<T>(
        Func<T> operation,
        Func<Exception, bool>? shouldRetry = null)
    {
        ArgumentNullException.ThrowIfNull(operation);
        shouldRetry ??= _ => true;

        for (var retryAttempt = 0; ; retryAttempt++)
        {
            try
            {
                Console.WriteLine(
                    $"Спроба {retryAttempt + 1}/{_maxRetries + 1}");
                var result = operation();
                Console.WriteLine("Спроба завершилась успішно.");
                return result;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Невдача: {exception.GetType().Name}: {exception.Message}");

                if (retryAttempt >= _maxRetries || !shouldRetry(exception))
                    throw;

                var delay = CalculateDelay(retryAttempt);
                Console.WriteLine(
                    $"Повтор через {delay.TotalMilliseconds:F0} мс " +
                    $"(retryAttempt={retryAttempt})");
                Thread.Sleep(delay);
            }
        }
    }

    private TimeSpan CalculateDelay(int retryAttempt)
    {
        double randomPart;
        lock (_randomLock)
            randomPart = _random.NextDouble();

        var jitterMultiplier =
            1 + randomPart * _jitterFactor * 2 - _jitterFactor;
        var milliseconds =
            _initialDelay.TotalMilliseconds *
            Math.Pow(2, retryAttempt) *
            jitterMultiplier;

        return TimeSpan.FromMilliseconds(Math.Max(0, milliseconds));
    }
}

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        var policy = new RetryPolicy(
            maxRetries: 4,
            initialDelay: TimeSpan.FromMilliseconds(80),
            jitterFactor: 0.2);

        Console.WriteLine("=== Успіх після кількох невдалих спроб ===");
        var successAttempts = 0;
        var result = policy.Execute(() =>
        {
            successAttempts++;
            if (successAttempts < 3)
                throw new IOException("тимчасова помилка вводу-виводу");
            return "Операція успішна";
        }, exception => exception is IOException);
        Console.WriteLine($"Результат: {result}");

        Console.WriteLine("\n=== Остаточна невдача ===");
        try
        {
            policy.Execute<string>(() =>
                throw new InvalidOperationException("стійка невідновна помилка"),
                exception => exception is InvalidOperationException);
        }
        catch (Exception exception)
        {
            Console.WriteLine(
                $"Після вичерпання повторів отримано: " +
                $"{exception.GetType().Name}: {exception.Message}");
        }
    }
}
