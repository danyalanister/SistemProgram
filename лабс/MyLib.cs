using System;

namespace MyLib
{
    public class InfoMessage
    {
        public static void ShowMessage()
        {
            Console.WriteLine("=== Информационное сообщение ===");
            Console.WriteLine("Привет! Я метод из динамически подключаемой библиотеки (DLL).");
            Console.WriteLine($"Текущее время: {DateTime.Now}");
        }
    }
    public class YearChecker
    {
        public static bool IsLeapYear(int year)
        {
            return (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);
        }
    }

    public class MathOps
    {
        public static double MaxOfThree(double a, double b, double c)
        {
            double m = a;
            if (b > m) m = b;
            if (c > m) m = c;
            return m;
        }

        public static double MinOfThree(double a, double b, double c)
        {
            double m = a;
            if (b < m) m = b;
            if (c < m) m = c;
            return m;
        }

        public static double SumOfThree(double a, double b, double c)
        {
            return a + b + c;
        }
    }

    public class NumberUtils
    {
        public static long Factorial(int n)
        {
            long f = 1;
            for (int i = 2; i <= n; i++) f *= i;
            return f;
        }

        public static bool IsPrime(int n)
        {
            if (n < 2) return false;
            for (int i = 2; i * i <= n; i++)
                if (n % i == 0) return false;
            return true;
        }

        public static bool IsEven(int n) { return n % 2 == 0; }
        public static bool IsOdd(int n) { return n % 2 != 0; }
    }

    public class Fraction
    {
        public int Numerator { get; set; }    
        public int Denominator { get; set; }   

        public Fraction(int numerator, int denominator)
        {
            if (denominator == 0)
                throw new ArgumentException("Знаменатель не может быть равен нулю!");
            Numerator = numerator;
            Denominator = denominator;
        }

        // --- статические методы арифметики ---
        public static Fraction Add(Fraction a, Fraction b)
        {
            return new Fraction(
                a.Numerator * b.Denominator + b.Numerator * a.Denominator,
                a.Denominator * b.Denominator);
        }

        public static Fraction Subtract(Fraction a, Fraction b)
        {
            return new Fraction(
                a.Numerator * b.Denominator - b.Numerator * a.Denominator,
                a.Denominator * b.Denominator);
        }

        public static Fraction Multiply(Fraction a, Fraction b)
        {
            return new Fraction(a.Numerator * b.Numerator, a.Denominator * b.Denominator);
        }

        public static Fraction Divide(Fraction a, Fraction b)
        {
            if (b.Numerator == 0)
                throw new ArgumentException("Деление на дробь, равную нулю!");
            return new Fraction(a.Numerator * b.Denominator, a.Denominator * b.Numerator);
        }

        // --- метод экземпляра: сокращение дроби ---
        public void Reduce()
        {
            int g = Gcd(Math.Abs(Numerator), Math.Abs(Denominator));
            Numerator /= g;
            Denominator /= g;
            if (Denominator < 0)   
            {
                Numerator = -Numerator;
                Denominator = -Denominator;
            }
        }

        static int Gcd(int a, int b)         
        {
            while (b != 0)
            {
                int t = b;
                b = a % b;
                a = t;
            }
            return a == 0 ? 1 : a;
        }

        public override string ToString()
        {
            return $"{Numerator}/{Denominator}";
        }
    }
}