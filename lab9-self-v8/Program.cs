using System.Text;

namespace Lab9SelfVN;

public sealed class Product
{
    public required string Name { get; init; }
    public decimal Price { get; init; }
}

public sealed class Student
{
    public required string Name { get; init; }
    public int Score { get; init; }
}

public static class CollectionUtils
{
    public static IEnumerable<TResult> Map<TSource, TResult>(
        IEnumerable<TSource> collection,
        Func<TSource, TResult> mapper)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(mapper);

        foreach (var item in collection)
            yield return mapper(item);
    }

    public static T MaxBy<T, TKey>(
        IEnumerable<T> collection,
        Func<T, TKey> keySelector)
        where TKey : IComparable<TKey>
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(keySelector);

        using var enumerator = collection.GetEnumerator();
        if (!enumerator.MoveNext())
            throw new InvalidOperationException("Колекція не містить елементів.");

        var maxItem = enumerator.Current;
        var maxKey = keySelector(maxItem);

        while (enumerator.MoveNext())
        {
            var item = enumerator.Current;
            var key = keySelector(item);
            if (key.CompareTo(maxKey) > 0)
            {
                maxItem = item;
                maxKey = key;
            }
        }

        return maxItem;
    }

    public static IEnumerable<T> Filter<T>(
        IEnumerable<T> collection,
        Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(predicate);

        foreach (var item in collection)
            if (predicate(item))
                yield return item;
    }

    public static void ForEach<T>(
        IEnumerable<T> collection,
        Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var item in collection)
            action(item);
    }
}

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var numbers = new List<int> { 4, 7, 2, 9, 12, 5 };
        var words = new List<string> { "LINQ", "delegate", "lambda", "generic" };
        var products = new List<Product>
        {
            new() { Name = "Laptop", Price = 25_000 },
            new() { Name = "Mouse", Price = 500 },
            new() { Name = "Keyboard", Price = 1_500 },
            new() { Name = "Monitor", Price = 8_000 }
        };
        var students = new List<Student>
        {
            new() { Name = "Олена", Score = 91 },
            new() { Name = "Ігор", Score = 76 },
            new() { Name = "Марія", Score = 84 }
        };

        Console.WriteLine("=== Колекція чисел: Map і Filter ===");
        var squared = CollectionUtils.Map(numbers, number => number * number);
        Console.WriteLine("Квадрати: " + string.Join(", ", squared));
        var evenNumbers = CollectionUtils.Filter(numbers, number => number % 2 == 0);
        Console.WriteLine("Парні:");
        CollectionUtils.ForEach(evenNumbers,
            number => Console.WriteLine($" - {number}"));

        Console.WriteLine("\n=== Колекція рядків: Map ===");
        var lengths = CollectionUtils.Map(words, word => $"{word}:{word.Length}");
        Console.WriteLine(string.Join(", ", lengths));

        Console.WriteLine("\n=== Колекція продуктів: MaxBy і Filter ===");
        var mostExpensive = CollectionUtils.MaxBy(products, product => product.Price);
        Console.WriteLine($"Найдорожчий: {mostExpensive.Name} — {mostExpensive.Price} грн");
        var expensive = CollectionUtils.Filter(products, product => product.Price > 1_000);
        CollectionUtils.ForEach(expensive,
            product => Console.WriteLine($" - {product.Name}: {product.Price} грн"));

        Console.WriteLine("\n=== Колекція студентів: інший тип і лямбда ===");
        var topStudent = CollectionUtils.MaxBy(students, student => student.Score);
        var grades = CollectionUtils.Map(students,
            student => $"{student.Name} ({student.Score})");
        Console.WriteLine("Студенти: " + string.Join(", ", grades));
        Console.WriteLine($"Найкращий бал: {topStudent.Name} — {topStudent.Score}");
    }
}
