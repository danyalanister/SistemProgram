using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace SystemProgrammingTasks
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            while (true)
            {
                Console.WriteLine("1 - Задание 1 (0 до 50)");
                Console.WriteLine("2 - Задание 2 (Свой диапазон)");
                Console.WriteLine("3 - Задание 3 (Диапазон и количество потоков)");
                Console.WriteLine("4 - Задание 4 (Макс, Мин, Среднее в 3 потоках)");
                Console.WriteLine("5 - Задание 5 (+ Запись в файл)");
                Console.WriteLine("6 - Задание 6 (Оконный интерфейс для 3 задания)");
                Console.WriteLine("7 - Задание 7 (Оконный интерфейс для 5 задания)");
                Console.WriteLine("0 - Выход");
                Console.Write("Выбор: ");

                string choice = Console.ReadLine();

                if (choice == "0") break;
                if (choice == "1") Task1();
                if (choice == "2") Task2();
                if (choice == "3") Task3();
                if (choice == "4") Task4();
                if (choice == "5") Task5();
                if (choice == "6") Task6();
                if (choice == "7") Task7();

                Console.WriteLine();
            }
        }

        static void Task1()
        {
            Thread t = new Thread(() =>
            {
                for (int i = 0; i <= 50; i++)
                {
                    Console.WriteLine(i);
                }
            });
            t.Start();
            t.Join();
        }

        static void Task2()
        {
            Console.Write("Начало диапазона: ");
            int start = int.Parse(Console.ReadLine());
            Console.Write("Конец диапазона: ");
            int end = int.Parse(Console.ReadLine());

            Thread t = new Thread(() =>
            {
                for (int i = start; i <= end; i++)
                {
                    Console.WriteLine(i);
                }
            });
            t.Start();
            t.Join();
        }

        static void Task3()
        {
            Console.Write("Начало диапазона: ");
            int start = int.Parse(Console.ReadLine());
            Console.Write("Конец диапазона: ");
            int end = int.Parse(Console.ReadLine());
            Console.Write("Количество потоков: ");
            int count = int.Parse(Console.ReadLine());

            Thread[] threads = new Thread[count];

            for (int i = 0; i < count; i++)
            {
                threads[i] = new Thread(() =>
                {
                    for (int j = start; j <= end; j++)
                    {
                        Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId}: {j}");
                    }
                });
                threads[i].Start();
            }

            for (int i = 0; i < count; i++)
            {
                threads[i].Join();
            }
        }

        static void Task4()
        {
            int[] arr = new int[10000];
            Random rnd = new Random();
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = rnd.Next(1, 100000);
            }

            int max = 0;
            int min = 0;
            double avg = 0;

            Thread tMax = new Thread(() => max = arr.Max());
            Thread tMin = new Thread(() => min = arr.Min());
            Thread tAvg = new Thread(() => avg = arr.Average());

            tMax.Start();
            tMin.Start();
            tAvg.Start();

            tMax.Join();
            tMin.Join();
            tAvg.Join();

            Console.WriteLine($"Макс: {max}, Мин: {min}, Среднее: {avg}");
        }

        static void Task5()
        {
            int[] arr = new int[10000];
            Random rnd = new Random();
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = rnd.Next(1, 100000);
            }

            int max = 0;
            int min = 0;
            double avg = 0;

            Thread tMax = new Thread(() => max = arr.Max());
            Thread tMin = new Thread(() => min = arr.Min());
            Thread tAvg = new Thread(() => avg = arr.Average());

            tMax.Start();
            tMin.Start();
            tAvg.Start();

            tMax.Join();
            tMin.Join();
            tAvg.Join();

            Thread tFile = new Thread(() =>
            {
                using (StreamWriter sw = new StreamWriter("task5_results.txt"))
                {
                    sw.WriteLine("Массив:");
                    sw.WriteLine(string.Join(" ", arr));
                    sw.WriteLine($"Макс: {max}, Мин: {min}, Среднее: {avg}");
                }
            });

            tFile.Start();
            tFile.Join();

            Console.WriteLine($"Макс: {max}, Мин: {min}, Среднее: {avg}");
            Console.WriteLine("Данные успешно сохранены в task5_results.txt");
        }

        static void Task6()
        {
            Form form = new Form { Text = "Задание 6", Width = 400, Height = 350 };

            TextBox txtStart = new TextBox { Left = 10, Top = 10, Width = 80, Text = "0" };
            TextBox txtEnd = new TextBox { Left = 100, Top = 10, Width = 80, Text = "50" };
            TextBox txtThreads = new TextBox { Left = 190, Top = 10, Width = 80, Text = "2" };
            Button btnRun = new Button { Left = 280, Top = 10, Width = 80, Text = "Запуск" };
            ListBox lstOut = new ListBox { Left = 10, Top = 40, Width = 350, Height = 250 };

            btnRun.Click += (s, e) =>
            {
                lstOut.Items.Clear();
                int start = int.Parse(txtStart.Text);
                int end = int.Parse(txtEnd.Text);
                int count = int.Parse(txtThreads.Text);

                Thread[] threads = new Thread[count];

                for (int i = 0; i < count; i++)
                {
                    threads[i] = new Thread(() =>
                    {
                        for (int j = start; j <= end; j++)
                        {
                            string msg = $"Поток {Thread.CurrentThread.ManagedThreadId}: {j}";
                            form.Invoke(new Action(() => lstOut.Items.Add(msg)));
                        }
                    });
                    threads[i].Start();
                }
            };

            form.Controls.Add(txtStart);
            form.Controls.Add(txtEnd);
            form.Controls.Add(txtThreads);
            form.Controls.Add(btnRun);
            form.Controls.Add(lstOut);

            form.ShowDialog();
        }

        static void Task7()
        {
            Form form = new Form { Text = "Задание 7", Width = 300, Height = 200 };
            Button btnRun = new Button { Left = 40, Top = 20, Width = 200, Text = "Сгенерировать и посчитать" };
            Label lblRes = new Label { Left = 40, Top = 60, Width = 200, Height = 80 };

            btnRun.Click += (s, e) =>
            {
                int[] arr = new int[10000];
                Random rnd = new Random();
                for (int i = 0; i < arr.Length; i++) arr[i] = rnd.Next(1, 100000);

                int max = 0, min = 0;
                double avg = 0;

                Thread tMax = new Thread(() => max = arr.Max());
                Thread tMin = new Thread(() => min = arr.Min());
                Thread tAvg = new Thread(() => avg = arr.Average());

                tMax.Start(); tMin.Start(); tAvg.Start();
                tMax.Join(); tMin.Join(); tAvg.Join();

                lblRes.Text = $"Макс: {max}\nМин: {min}\nСреднее: {avg}\nСохранено в task7_results.txt";

                Thread tFile = new Thread(() =>
                {
                    using (StreamWriter sw = new StreamWriter("task7_results.txt"))
                    {
                        sw.WriteLine("Массив:");
                        sw.WriteLine(string.Join(" ", arr));
                        sw.WriteLine($"Макс: {max}, Мин: {min}, Среднее: {avg}");
                    }
                });
                tFile.Start();
            };

            form.Controls.Add(btnRun);
            form.Controls.Add(lblRes);

            form.ShowDialog();
        }
    }
}