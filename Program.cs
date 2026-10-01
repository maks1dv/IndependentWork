using System;

namespace IndependentWork1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== ДЕМОНСТРАЦІЯ РОБОТИ КЛАСІВ ===\n");

            // 1. Робота з класом Rectangle
            Rectangle rect = new Rectangle(5.0, 4.0);
            Console.WriteLine($"[1] Об'єкт Rectangle ({rect.Width}x{rect.Height}):");
            Console.WriteLine($"    Площа: {rect.CalculateArea()}");
            Console.WriteLine($"    Периметр: {rect.CalculatePerimeter()}");
            Console.WriteLine($"    Чи є квадратом: {(rect.IsSquare ? "Так" : "Ні")}\n");

            // 2. Робота з класом Recipe
            Recipe recipe = new Recipe("Український Борщ", 90, 6);
            Console.Write("[2] Об'єкт Recipe: ");
            recipe.PrintRecipeInfo();
            Console.WriteLine($"    Швидке приготування (до 30 хв): {(recipe.IsQuickToPrepare() ? "Так" : "Ні")}\n");

            // 3. Робота з класом Employee
            Employee employee = new Employee("Олена Ковальчук", 28000.0, 6);
            double bonus = employee.CalculateAnnualBonus(0.15); 
            Console.WriteLine($"[3] Об'єкт Employee: {employee.FullName}");
            Console.WriteLine($"    Місячний оклад: {employee.MonthlySalary} грн | Стаж: {employee.ExperienceYears} років");
            Console.WriteLine($"    Розрахована річна премія (із бонусом за стаж): {bonus:F2} грн\n");
        }
    }
}