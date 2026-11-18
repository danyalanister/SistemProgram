using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace SystemProgrammingTasks
{
    class Program
    {
        static ManualResetEvent syncEvent = new ManualResetEvent(false);
        static int[] numbers = new int[1000];

        static object lockTask2 = new object();
        static object lockTask3 = new object();

        static void Main()
        {
            while (true)
            {
                Console.WriteLine("1 - Задание 1 (События)");
                Console.WriteLine("2 - Задание 2 (Критические секции - файл)");
                Console.WriteLine("3 - Задание 3 (Критические секции - массив)");
                Console.WriteLine("0 - Выход");
                Console.Write("Выбор: ");

                string choice = Console.ReadLine();

                if (choice == "0") break;
                if (choice == "1") Task1();
                if (choice == "2") Task2();
                if (choice == "3") Task3();

                Console.WriteLine();
            }
        }

        static void Task1()
        {
            syncEvent.Reset();

            Thread tGen = new Thread(() =>
            {
                Random rnd = new Random();
                for (int i = 0; i < 1000; i++)
                {
                    numbers[i] = rnd.Next(0, 5001);
                }
                syncEvent.Set();
            });

            Thread tMax = new Thread(() =>
            {
                syncEvent.WaitOne();
                Console.WriteLine("Максимум: " + numbers.Max());
            });

            Thread tMin = new Thread(() =>
            {
                syncEvent.WaitOne();
                Console.WriteLine("Минимум: " + numbers.Min());
            });

            Thread tAvg = new Thread(() =>
            {
                syncEvent.WaitOne();
                Console.WriteLine("Среднее: " + numbers.Average());
            });

            tMax.Start();
            tMin.Start();
            tAvg.Start();
            tGen.Start();

            tGen.Join();
            tMax.Join();
            tMin.Join();
            tAvg.Join();
        }

        static void Task2()
        {
            string filePath = "task2_text.txt";
            File.WriteAllText(filePath, "Привет! Это проверка работы программы. Все работает отлично! Ура!");

            Thread t1 = new Thread((obj) =>
            {
                lock (lockTask2)
                {
                    string path = (string)obj;
                    string text = File.ReadAllText(path);
                    int count = text.Split(new char[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries).Length;
                    Console.WriteLine("Количество предложений: " + count);
                }
            });

            Thread t2 = new Thread((obj) =>
            {
                lock (lockTask2)
                {
                    string path = (string)obj;
                    string text = File.ReadAllText(path);
                    text = text.Replace('!', '#');
                    File.WriteAllText(path, text);
                    Console.WriteLine("Файл изменен (заменены ! на #)");
                }
            });

            t1.Start(filePath);
            Thread.Sleep(50);
            t2.Start(filePath);

            t1.Join();
            t2.Join();
        }

        static void Task3()
        {
            int[] data = { 45, 12, 89, 3, 27, 60 };
            int searchNumber = 27;

            Thread t1 = new Thread((obj) =>
            {
                lock (lockTask3)
                {
                    int[] arr = (int[])obj;
                    Array.Sort(arr);
                    Console.WriteLine("Массив отсортирован: " + string.Join(", ", arr));
                }
            });

            Thread t2 = new Thread((obj) =>
            {
                lock (lockTask3)
                {
                    object[] parameters = (object[])obj;
                    int[] arr = (int[])parameters[0];
                    int target = (int)parameters[1];

                    bool exists = Array.IndexOf(arr, target) >= 0;
                    if (exists)
                    {
                        Console.WriteLine("Число " + target + " найдено в массиве.");
                    }
                    else
                    {
                        Console.WriteLine("Число " + target + " не найдено в массиве.");
                    }
                }
            });

            t1.Start(data);
            Thread.Sleep(50);
            t2.Start(new object[] { data, searchNumber });

            t1.Join();
            t2.Join();
        }
    }
}