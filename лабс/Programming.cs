using System;
using MyLib;          // подключаем пространство имён библиотеки

class Program1
{
    static void Main()
    {
        InfoMessage.ShowMessage();   // статический метод вызывается от имени класса
        Console.ReadKey();
    }
}

class Program2
{
    static void Main()
    {
        Console.Write("Введите год: ");
        int year = int.Parse(Console.ReadLine());

        if (YearChecker.IsLeapYear(year))
            Console.WriteLine($"{year} год — високосный.");
        else
            Console.WriteLine($"{year} год — не високосный.");

        Console.ReadKey();
    }
}

class Program3
{
    static void Main()
    {
        Console.Write("Число 1: "); double a = double.Parse(Console.ReadLine());
        Console.Write("Число 2: "); double b = double.Parse(Console.ReadLine());
        Console.Write("Число 3: "); double c = double.Parse(Console.ReadLine());

        Console.WriteLine($"Максимум: {MathOps.MaxOfThree(a, b, c)}");
        Console.WriteLine($"Минимум:  {MathOps.MinOfThree(a, b, c)}");
        Console.WriteLine($"Сумма:    {MathOps.SumOfThree(a, b, c)}");

        Console.ReadKey();
    }
}

class Program4
{
    static void Main()
    {
        Console.Write("Введите число: ");
        int n = int.Parse(Console.ReadLine());

        Console.WriteLine($"{n}! = {NumberUtils.Factorial(n)}");
        Console.WriteLine($"Простое:     {NumberUtils.IsPrime(n)}");
        Console.WriteLine($"Чётное:      {NumberUtils.IsEven(n)}");
        Console.WriteLine($"Нечётное:    {NumberUtils.IsOdd(n)}");

        Console.ReadKey();
    }
}

class Program5
{
    static void Main()
    {
        Fraction f1 = new Fraction(1, 2);
        Fraction f2 = new Fraction(1, 3);

        Console.WriteLine($"Дробь 1: {f1}");
        Console.WriteLine($"Дробь 2: {f2}");

        Fraction sum = Fraction.Add(f1, f2); sum.Reduce();
        Console.WriteLine($"Сумма:      {sum}");    

        Fraction diff = Fraction.Subtract(f1, f2); diff.Reduce();
        Console.WriteLine($"Разность:   {diff}");  

        Fraction mul = Fraction.Multiply(f1, f2); mul.Reduce();
        Console.WriteLine($"Произведение: {mul}");  

        Fraction div = Fraction.Divide(f1, f2); div.Reduce();
        Console.WriteLine($"Частное:    {div}");   

        Console.ReadKey();
    }
}