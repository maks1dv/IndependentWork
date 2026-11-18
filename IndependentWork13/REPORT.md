# Звіт до самостійної роботи No14

## 1. Зовнішній API

**Проблема:** API може тимчасово повернути HTTP-помилку.

**Стратегія:** exponential retry з jitter і максимум трьома повторними спробами. Результат успішний після другої невдалої першої хвилі.

## 2. База даних

**Проблема:** з'єднання може перерватися короткочасно.

**Стратегія:** retry плюс circuit breaker. Retry не дозволяє короткому збою одразу завершити операцію; circuit breaker не дає повторювати кілька послідовних невдалих звернень без паузи.

## 3. Черга повідомлень

**Проблема:** перевантажена черга може затримати підтвердження над довго.

**Стратегія:** timeout у 180 мс. Повільна відправка завершується `TimeoutRejectedException`, замість необмеженого очікування.

Усі політики виводять спроби і причини реакції у консоль.

## Висновки

Polly centralizes retry, breaker and timeout behavior and makes resilience policy configurable. Benefits include bounded waits, protection of dependencies, logging and a reusable pipeline. Limits include duplication risk for non-idempotent operations, request amplification during retries and the need to tune thresholds for the real traffic profile.
