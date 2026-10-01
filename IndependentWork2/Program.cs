using System;

namespace IndependentWork2;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("Створення товарів\n");

        // 1. Товар створений через основний конструктор
        Product product1 = new Product(101, "Laptop", 35000.00m, "Electronics", 15);
        Console.WriteLine($"Товар 1 (основний конструктор): {product1}");

        // 2. Товар створений через скорочений конструктор
        Product product2 = new Product(102, "Mouse", 800.00m);
        Console.WriteLine($"Товар 2 (скорочений конструктор): {product2}");

        // 3. Товар створений через конструктор копіювання
        Product product3 = new Product(product1);
        Console.WriteLine($"Товар 3 (конструктор копіювання): {product3}");
    }
}