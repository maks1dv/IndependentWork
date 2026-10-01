using System;

namespace IndependentWork3;

public static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("=== Самостійна робота №3 ===");
        Console.WriteLine("Аналіз інкапсуляції у проєкті .NET eShop\n");

        try
        {
            // Створення покупця
            var buyer = new Buyer("usr-101", "Максим");
            Console.WriteLine($"Покупець створений: {buyer.Name} (GUID: {buyer.IdentityGuid})");

            // Створення замовлення та додавання товарів
            var order = new Order();
            order.AddOrderItem(1, "Ноутбук", 35000m, 1);
            order.AddOrderItem(1, "Ноутбук", 35000m, 1); // Збільшує кількість існуючого товару
            order.AddOrderItem(2, "Мишка", 800m, 2);

            Console.WriteLine($"\nЗамовлення від {order.OrderDate:g}:");
            foreach (var item in order.OrderItems)
            {
                Console.WriteLine($" - {item.ProductName}: {item.Units} шт. х {item.UnitPrice:C}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка валідації: {ex.Message}");
        }
    }
}