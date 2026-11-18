namespace IndependentWork7;

internal static class Program
{
    private static void Main()
    {
        var cache = new Cache<string, string>(2);

        cache.Add("first", "Перший елемент");
        cache.Add("second", "Другий елемент");
        Console.WriteLine(cache.Get("first"));

        cache.Add("third", "Третій елемент");
        Console.WriteLine($"Ключ 'first' є в кеші: {cache.ContainsKey("first")}");
        Console.WriteLine($"Ключ 'second' є в кеші: {cache.ContainsKey("second")}");
        Console.WriteLine($"Кількість елементів: {cache.Count}");
    }
}
