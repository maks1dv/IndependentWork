namespace IndependentWork1
{
    public class Rectangle
    {
        // Приватні поля
        private double _width;
        private double _height;

        // Властивість для ширини з перевіркою коректності
        public double Width
        {
            get { return _width; }
            set { _width = value > 0 ? value : 1.0; }
        }

        // Властивість для висоти з перевіркою коректності
        public double Height
        {
            get { return _height; }
            set { _height = value > 0 ? value : 1.0; }
        }

        // Властивість "тільки для читання" (Read-only)
        public bool IsSquare
        {
            get { return _width == _height; }
        }

        // Конструктор
        public Rectangle(double width, double height)
        {
            Width = width;
            Height = height;
        }

        // Метод з обчисленням площі
        public double CalculateArea()
        {
            return _width * _height;
        }

        // Метод з обчисленням периметра
        public double CalculatePerimeter()
        {
            return 2 * (_width + _height);
        }
    }
}