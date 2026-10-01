namespace IndependentWork1
{
    public class Recipe
    {
        // Приватні поля
        private string _title;
        private int _cookingTimeMinutes;
        private int _servings;

        // Властивість тільки для читання
        public string Title
        {
            get { return _title; }
        }

        // Властивість з читанням і записом
        public int Servings
        {
            get { return _servings; }
            set { if (value > 0) _servings = value; }
        }

        // Конструктор
        public Recipe(string title, int cookingTimeMinutes, int servings)
        {
            _title = title;
            _cookingTimeMinutes = cookingTimeMinutes;
            _servings = servings;
        }

        // Метод з перевіркою стану (чи є рецепт швидким)
        public bool IsQuickToPrepare()
        {
            return _cookingTimeMinutes <= 30;
        }

        // Метод виведення інформації про рецепт
        public void PrintRecipeInfo()
        {
            string speedTag = IsQuickToPrepare() ? "Швидкий рецепт" : "Потребує часу";
            System.Console.WriteLine($"Рецепт: \"{_title}\" | Порцій: {_servings} | Час: {_cookingTimeMinutes} хв. [{speedTag}]");
        }
    }
}