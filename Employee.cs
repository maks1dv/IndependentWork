namespace IndependentWork1
{
    public class Employee
    {
        // Приватні поля
        private string _fullName;
        private double _monthlySalary;
        private int _experienceYears;

        // Властивості
        public string FullName
        {
            get { return _fullName; }
            set { _fullName = value; }
        }

        public double MonthlySalary
        {
            get { return _monthlySalary; }
        }

        public int ExperienceYears
        {
            get { return _experienceYears; }
        }

        // Конструктор
        public Employee(string fullName, double monthlySalary, int experienceYears)
        {
            _fullName = fullName;
            _monthlySalary = monthlySalary;
            _experienceYears = experienceYears;
        }

        // Метод обчислення річної премії з урахуванням стажу
        public double CalculateAnnualBonus(double bonusPercentage)
        {
            double baseBonus = _monthlySalary * bonusPercentage;
            
            // Якщо стаж більше або дорівнює 5 рокам — нараховується додатковий коефіцієнт
            if (_experienceYears >= 5)
            {
                baseBonus *= 1.2;
            }
            
            return baseBonus;
        }
    }
}