using System;
using System.Diagnostics;
using System.Threading;

namespace SystemProgrammingTasks
{
    class Program
    {
        static int counter = 0;
        static object locker = new object();

        static void Main()
        {
            while (true)
            {
                Console.WriteLine("1 - Задание 1 (Базовые потоки)");
                Console.WriteLine("2 - Задание 2 (Фоновые потоки и Таймер)");
                Console.WriteLine("3 - Задание 3 (Синхронизация lock)");
                Console.WriteLine("4 - Задание 4 (Игра Успел-не успел)");
                Console.WriteLine("0 - Выход");
                Console.Write("Выбор: ");
                string choice = Console.ReadLine();

                if (choice == "0") break;
                if (choice == "1") Task1();
                if (choice == "2") Task2();
                if (choice == "3") Task3();
                if (choice == "4") Task4();

                Console.WriteLine();
            }
        }

        static void Task1()
        {
            Thread t1 = new Thread(Task1Method);
            Thread t2 = new Thread(Task1Method);

            t1.Start("Поток А");
            t2.Start("Поток Б");

            t1.Join();
            t2.Join();

            Console.WriteLine("Все потоки завершили работу!");
        }

        static void Task1Method(object obj)
        {
            string name = (string)obj;
            for (int i = 1; i <= 5; i++)
            {
                Console.WriteLine(name + " Шаг: " + i);
                Thread.Sleep(500);
            }
        }

        static void Task2()
        {
            Timer t = new Timer(TimerMethod, null, 1000, 2000);

            Thread bgThread = new Thread(() =>
            {
                while (true)
                {
                    Console.WriteLine("Фоновый поток работает...");
                    Thread.Sleep(300);
                }
            });
            bgThread.IsBackground = true;
            bgThread.Start();

            Console.WriteLine("Нажмите Enter для выхода...");
            Console.ReadLine();
            t.Dispose();
        }

        static void TimerMethod(object state)
        {
            Console.WriteLine(DateTime.Now);
        }

        static void Task3()
        {
            counter = 0;
            Thread[] threads = new Thread[5];

            for (int i = 0; i < 5; i++)
            {
                threads[i] = new Thread(IncrementCounter);
                threads[i].Start();
            }

            for (int i = 0; i < 5; i++)
            {
                threads[i].Join();
            }

            Console.WriteLine("Итоговое значение: " + counter);
        }

        static void IncrementCounter()
        {
            for (int i = 0; i < 100000; i++)
            {
                lock (locker)
                {
                    counter++;
                }
            }
        }

        static void Task4()
        {
            Console.WriteLine("Приготовьтесь...");
            Random rnd = new Random();
            int delay = rnd.Next(2000, 5000);

            Thread.Sleep(delay);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("жми!");
            Console.ResetColor();

            Stopwatch sw = Stopwatch.StartNew();
            bool isPressed = false;

            Thread timeoutThread = new Thread(() =>
            {
                Thread.Sleep(3000);
                if (!isPressed)
                {
                    Console.WriteLine("Вы не успели! Слишком долго.");
                    Environment.Exit(0);
                }
            });
            timeoutThread.IsBackground = true;
            timeoutThread.Start();

            Console.ReadKey(true);
            isPressed = true;
            sw.Stop();

            Console.WriteLine();
            Console.WriteLine("Ваше время реакции: " + sw.ElapsedMilliseconds + " мс");
        }
    }
}