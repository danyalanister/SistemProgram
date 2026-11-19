using System;
using System.Linq;
using System.Threading.Tasks;

namespace SystemProgrammingTasks
{
    class Program
    {
        static void Main()
        {
            while (true)
            {
                Console.WriteLine("1 - Задание 1 (Task: три способа запуска)");
                Console.WriteLine("2 - Задание 2 (Простые числа от 0 до 1000)");
                Console.WriteLine("3 - Задание 3 (Простые числа в диапазоне + счетчик)");
                Console.WriteLine("4 - Задание 4 (Массив Task: Мин, Макс, Среднее, Сумма)");
                Console.WriteLine("5 - Задание 5 (Continuation tasks: дубли, сортировка, поиск)");
                Console.WriteLine("0 - Выход");
                Console.Write("Выбор: ");

                string choice = Console.ReadLine();

                if (choice == "0") break;
                if (choice == "1") Task1();
                if (choice == "2") Task2();
                if (choice == "3") Task3();
                if (choice == "4") Task4();
                if (choice == "5") Task5();

                Console.WriteLine();
            }
        }

        static void Task1()
        {
            Task t1 = new Task(() => Console.WriteLine("Start: " + DateTime.Now));
            t1.Start();
            t1.Wait();

            Task t2 = Task.Factory.StartNew(() => Console.WriteLine("Factory.StartNew: " + DateTime.Now));
            t2.Wait();

            Task t3 = Task.Run(() => Console.WriteLine("Task.Run: " + DateTime.Now));
            t3.Wait();
        }

        static bool IsPrime(int n)
        {
            if (n < 2) return false;
            for (int i = 2; i * i <= n; i++)
            {
                if (n % i == 0) return false;
            }
            return true;
        }

        static void Task2()
        {
            Task t = Task.Run(() =>
            {
                for (int i = 0; i <= 1000; i++)
                {
                    if (IsPrime(i))
                    {
                        Console.Write(i + " ");
                    }
                }
                Console.WriteLine();
            });
            t.Wait();
        }

        static void Task3()
        {
            Console.Write("Начало диапазона: ");
            int start = int.Parse(Console.ReadLine());
            Console.Write("Конец диапазона: ");
            int end = int.Parse(Console.ReadLine());

            Task<int> t = Task.Run(() =>
            {
                int count = 0;
                for (int i = start; i <= end; i++)
                {
                    if (IsPrime(i))
                    {
                        Console.Write(i + " ");
                        count++;
                    }
                }
                Console.WriteLine();
                return count;
            });

            t.Wait();
            Console.WriteLine("Количество простых чисел: " + t.Result);
        }

        static void Task4()
        {
            int[] arr = { 15, 4, 82, 19, 0, 42, 7, 63, 11, 28 };
            Console.WriteLine("Массив: " + string.Join(", ", arr));

            int min = 0;
            int max = 0;
            double avg = 0;
            int sum = 0;

            Task[] tasks = new Task[4];
            tasks[0] = Task.Run(() => min = arr.Min());
            tasks[1] = Task.Run(() => max = arr.Max());
            tasks[2] = Task.Run(() => avg = arr.Average());
            tasks[3] = Task.Run(() => sum = arr.Sum());

            Task.WaitAll(tasks);

            Console.WriteLine("Минимум: " + min);
            Console.WriteLine("Максимум: " + max);
            Console.WriteLine("Среднее: " + avg);
            Console.WriteLine("Сумма: " + sum);
        }

        static void Task5()
        {
            int[] arr = { 30, 10, 20, 10, 50, 40, 30, 20 };
            int target = 40;
            Console.WriteLine("Исходный массив: " + string.Join(", ", arr));

            var tDistinct = Task.Run(() =>
            {
                int[] unique = arr.Distinct().ToArray();
                Console.WriteLine("Без дублей: " + string.Join(", ", unique));
                return unique;
            });

            var tSort = tDistinct.ContinueWith(prev =>
            {
                int[] sorted = prev.Result;
                Array.Sort(sorted);
                Console.WriteLine("Отсортированный: " + string.Join(", ", sorted));
                return sorted;
            });

            var tSearch = tSort.ContinueWith(prev =>
            {
                int[] sorted = prev.Result;
                int index = Array.BinarySearch(sorted, target);
                if (index >= 0)
                {
                    Console.WriteLine("Число " + target + " найдено на индексе: " + index);
                }
                else
                {
                    Console.WriteLine("Число " + target + " не найдено");
                }
            });

            tSearch.Wait();
        }
    }
}