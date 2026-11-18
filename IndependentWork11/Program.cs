using System.Diagnostics;
using System.Text;

namespace IndependentWork11;

public sealed record Sale(int Id, int ProductId, int Quantity, DateTime SaleDate, int CustomerId);
public sealed record Product(int Id, string Name, string Category, decimal UnitPrice);
public sealed record Customer(int Id, string Name, string City);

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        var random = new Random(1708);
        var products = Enumerable.Range(1, 20).Select(id => new Product(
            id,
            $"Товар {id}",
            $"Категорія {(id - 1) % 5 + 1}",
            100m + id * 37m)).ToList();
        var customers = Enumerable.Range(1, 80).Select(id => new Customer(
            id,
            $"Клієнт {id}",
            (id % 5) switch
            {
                0 => "Київ",
                1 => "Львів",
                2 => "Одеса",
                3 => "Дніпро",
                _ => "Харків"
            })).ToList();
        var sales = Enumerable.Range(1, 100).Select(id => new Sale(
            id,
            random.Next(1, 21),
            random.Next(1, 21),
            new DateTime(2024 + random.Next(3), random.Next(1, 13), random.Next(1, 28)),
            random.Next(1, 81))).ToList();

        Console.WriteLine("Дані: 20 продуктів, 80 клієнтів, 100 продажів.");
        TopProductsByCategory(sales, products);
        RevenueByCity(sales, products, customers);
        SalesByMonth(sales, products);
        CustomerCategoryTotals(sales, products, customers);
    }

    private static void TopProductsByCategory(
        IReadOnlyCollection<Sale> sales,
        IReadOnlyCollection<Product> products)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = sales
            .GroupBy(sale => sale.ProductId)
            .Select(group => new
            {
                ProductId = group.Key,
                Quantity = group.Sum(sale => sale.Quantity)
            })
            .Join(products,
                quantity => quantity.ProductId,
                product => product.Id,
                (quantity, product) => new
                {
                    product.Name,
                    product.Category,
                    quantity.Quantity
                })
            .GroupBy(item => item.Category)
            .Select(category => new
            {
                Category = category.Key,
                Top = category
                    .OrderByDescending(item => item.Quantity)
                    .Take(3)
                    .ToList()
            })
            .OrderBy(item => item.Category)
            .ToList();
        stopwatch.Stop();

        Console.WriteLine("\n1. Найпопулярніші продукти за категоріями");
        foreach (var category in result)
        {
            Console.WriteLine($"{category.Category}:");
            foreach (var product in category.Top)
                Console.WriteLine($"  {product.Name}: {product.Quantity}");
        }
        Console.WriteLine($"Час: {stopwatch.Elapsed.TotalMilliseconds:F3} мс");
    }

    private static void RevenueByCity(
        IReadOnlyCollection<Sale> sales,
        IReadOnlyCollection<Product> products,
        IReadOnlyCollection<Customer> customers)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = sales
            .Join(products, sale => sale.ProductId, product => product.Id,
                (sale, product) => new { sale, product })
            .Join(customers, pair => pair.sale.CustomerId, customer => customer.Id,
                (pair, customer) => new
                {
                    customer.City,
                    Revenue = pair.sale.Quantity * pair.product.UnitPrice
                })
            .GroupBy(item => item.City)
            .Select(group => new
            {
                City = group.Key,
                Revenue = group.Sum(item => item.Revenue)
            })
            .OrderByDescending(item => item.Revenue)
            .ToList();
        stopwatch.Stop();

        Console.WriteLine("\n2. Дохід за містами");
        foreach (var item in result)
            Console.WriteLine($"{item.City}: {item.Revenue:N2} грн");
        Console.WriteLine($"Час: {stopwatch.Elapsed.TotalMilliseconds:F3} мс");
    }

    private static void SalesByMonth(
        IReadOnlyCollection<Sale> sales,
        IReadOnlyCollection<Product> products)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = sales
            .Join(products, sale => sale.ProductId, product => product.Id,
                (sale, product) => new
                {
                    sale.SaleDate,
                    sale.Quantity,
                    Revenue = sale.Quantity * product.UnitPrice
                })
            .GroupBy(item => new { item.SaleDate.Year, item.SaleDate.Month })
            .Select(group => new
            {
                group.Key.Year,
                group.Key.Month,
                Units = group.Sum(item => item.Quantity),
                Revenue = group.Sum(item => item.Revenue)
            })
            .OrderBy(item => item.Year)
            .ThenBy(item => item.Month)
            .ToList();
        stopwatch.Stop();

        Console.WriteLine("\n3. Продажі за місяцями");
        foreach (var item in result)
            Console.WriteLine($"{item.Year}-{item.Month:D2}: {item.Units} од., {item.Revenue:N2} грн");
        Console.WriteLine($"Час: {stopwatch.Elapsed.TotalMilliseconds:F3} мс");
    }

    private static void CustomerCategoryTotals(
        IReadOnlyCollection<Sale> sales,
        IReadOnlyCollection<Product> products,
        IReadOnlyCollection<Customer> customers)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = sales
            .Join(customers, sale => sale.CustomerId, customer => customer.Id,
                (sale, customer) => new { sale, customer })
            .Join(products, pair => pair.sale.ProductId, product => product.Id,
                (pair, product) => new
                {
                    pair.customer.City,
                    product.Category,
                    Value = pair.sale.Quantity * product.UnitPrice
                })
            .Where(item => item.Value >= 100m)
            .GroupBy(item => new { item.City, item.Category })
            .Select(group => new
            {
                group.Key.City,
                group.Key.Category,
                Total = group.Sum(item => item.Value)
            })
            .Where(item => item.Total > 1_000m)
            .OrderByDescending(item => item.Total)
            .Take(10)
            .ToList();
        stopwatch.Stop();

        Console.WriteLine("\n4. Топ місто/категорія");
        foreach (var item in result)
            Console.WriteLine($"{item.City}, {item.Category}: {item.Total:N2} грн");
        Console.WriteLine($"Час: {stopwatch.Elapsed.TotalMilliseconds:F3} мс");
    }
}
