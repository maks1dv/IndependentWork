using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using System.Net.Http;
using System.Text;

namespace IndependentWork13;

internal static class Program
{
    private static int _apiAttempts;
    private static int _databaseAttempts;
    private static int _queueAttempts;

    private static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        ExternalApiScenario();
        DatabaseScenario();
        await MessageQueueScenario();
    }

    private static void ExternalApiScenario()
    {
        Console.WriteLine("=== 1. Зовнішній API: Retry ===");
        var pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(100),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                OnRetry = args =>
                {
                    Console.WriteLine(
                        $"API retry #{args.AttemptNumber + 1}, затримка " +
                        $"{args.RetryDelay.TotalMilliseconds:F0} мс: " +
                        args.Outcome.Exception?.Message);
                    return default;
                }
            })
            .Build();

        var result = pipeline.Execute(() =>
        {
            _apiAttempts++;
            Console.WriteLine($"API спроба {_apiAttempts}");
            if (_apiAttempts <= 2)
                throw new HttpRequestException("Сервіс тимчасово недоступний");
            return "Дані API отримано";
        });
        Console.WriteLine($"Результат: {result}\n");
    }

    private static void DatabaseScenario()
    {
        Console.WriteLine("=== 2. База даних: Retry + Circuit Breaker ===");
        var pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<InvalidOperationException>(),
                MaxRetryAttempts = 2,
                Delay = TimeSpan.FromMilliseconds(100),
                BackoffType = DelayBackoffType.Constant,
                OnRetry = args =>
                {
                    Console.WriteLine(
                        $"DB retry #{args.AttemptNumber + 1}: " +
                        args.Outcome.Exception?.Message);
                    return default;
                }
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<InvalidOperationException>(),
                FailureRatio = 0.5,
                SamplingDuration = TimeSpan.FromSeconds(5),
                MinimumThroughput = 2,
                BreakDuration = TimeSpan.FromSeconds(1),
                OnOpened = args =>
                {
                    Console.WriteLine("Circuit breaker відкрито.");
                    return default;
                },
                OnClosed = args =>
                {
                    Console.WriteLine("Circuit breaker закрито.");
                    return default;
                }
            })
            .Build();

        for (var i = 0; i < 3; i++)
        {
            try
            {
                var result = pipeline.Execute(() =>
                {
                    _databaseAttempts++;
                    Console.WriteLine($"DB спроба {_databaseAttempts}");
                    if (_databaseAttempts <= 3)
                        throw new InvalidOperationException("Тимчасова втрата з'єднання");
                    return "Запит до БД виконано";
                });
                Console.WriteLine($"Результат: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB final: {ex.GetType().Name}: {ex.Message}");
            }
        }
        Console.WriteLine();
    }

    private static async Task MessageQueueScenario()
    {
        Console.WriteLine("=== 3. Черга повідомлень: Timeout ===");
        var pipeline = new ResiliencePipelineBuilder()
            .AddTimeout(new TimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromMilliseconds(180),
                OnTimeout = args =>
                {
                    Console.WriteLine(
                        $"Операція перевищила timeout {args.Timeout.TotalMilliseconds:F0} мс.");
                    return default;
                }
            })
            .Build();

        try
        {
            var result = await pipeline.ExecuteAsync(async cancellationToken =>
            {
                _queueAttempts++;
                Console.WriteLine($"Queue attempt {_queueAttempts}: відправка...");
                await Task.Delay(_queueAttempts == 1 ? 350 : 50, cancellationToken);
                return "Повідомлення поставлено в чергу";
            });
            Console.WriteLine($"Результат: {result}");
        }
        catch (TimeoutRejectedException ex)
        {
            Console.WriteLine($"Очікувана відмова timeout: {ex.Message}");
        }
    }
}
