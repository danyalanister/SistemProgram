using System;
using System.Collections.Generic;
using System.Threading;

namespace SystemProgrammingTasks
{
    class Program
    {
        static void Main()
        {
            while (true)
            {
                Console.WriteLine("1 - Задание 1 (Mutex: один экземпляр)");
                Console.WriteLine("2 - Задание 2 (Mutex: защита счетчика)");
                Console.WriteLine("3 - Задание 3 (Semaphore: пул потоков)");
                Console.WriteLine("4 - Задание 4 (Semaphore: производитель-потребитель)");
                Console.WriteLine("5 - Задание 5 (ManualResetEvent)");
                Console.WriteLine("6 - Задание 6 (AutoResetEvent)");
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

                Console.WriteLine();
            }
        }

        static void Task1()
        {
            try
            {
                Mutex.OpenExisting("MY_MUTEX");
                Console.WriteLine("Приложение уже запущено!");
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                using (Mutex m = new Mutex(true, "MY_MUTEX"))
                {
                    Console.WriteLine("Приложение работает. Нажмите любую клавишу для выхода...");
                    Console.ReadKey(true);
                    m.ReleaseMutex();
                }
            }
        }

        class MutexCounter
        {
            public int count = 0;
            public Mutex m = new Mutex();
        }

        static void Task2()
        {
            MutexCounter mc = new MutexCounter();
            Thread[] threads = new Thread[5];

            for (int i = 0; i < 5; i++)
            {
                threads[i] = new Thread(() =>
                {
                    for (int j = 0; j < 1000000; j++)
                    {
                        mc.m.WaitOne();
                        mc.count++;
                        mc.m.ReleaseMutex();
                    }
                });
                threads[i].Start();
            }

            foreach (var t in threads) t.Join();
            Console.WriteLine("Итоговое значение: " + mc.count);
        }

        static void Task3()
        {
            Semaphore sem = new Semaphore(3, 3);
            ManualResetEvent allDone = new ManualResetEvent(false);
            int finished = 0;

            for (int i = 0; i < 6; i++)
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    while (!sem.WaitOne(500))
                    {
                        Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId} - таймаут, ждет...");
                    }

                    try
                    {
                        Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId} работает.");
                        Thread.Sleep(2000);
                    }
                    finally
                    {
                        Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId} освободил семафор.");
                        sem.Release();
                        if (Interlocked.Increment(ref finished) == 6) allDone.Set();
                    }
                });
            }

            allDone.WaitOne();
        }

        static void Task4()
        {
            Semaphore empty = new Semaphore(5, 5);
            Semaphore full = new Semaphore(0, 5);
            Queue<int> q = new Queue<int>();
            object locker = new object();
            bool isRunning = true;

            Thread prod = new Thread(() =>
            {
                for (int i = 1; i <= 10; i++)
                {
                    empty.WaitOne();
                    lock (locker)
                    {
                        q.Enqueue(i);
                        Console.WriteLine("Производитель добавил: " + i);
                    }
                    full.Release();
                    Thread.Sleep(300);
                }
                isRunning = false;
            });

            ThreadStart consWork = () =>
            {
                while (isRunning || q.Count > 0)
                {
                    if (full.WaitOne(100))
                    {
                        int item;
                        lock (locker)
                        {
                            item = q.Dequeue();
                        }
                        empty.Release();
                        Console.WriteLine($"Потребитель {Thread.CurrentThread.ManagedThreadId} забрал: {item}");
                        Thread.Sleep(800);
                    }
                }
            };

            Thread cons1 = new Thread(consWork);
            Thread cons2 = new Thread(consWork);

            prod.Start(); cons1.Start(); cons2.Start();
            prod.Join(); cons1.Join(); cons2.Join();
        }

        static void Task5()
        {
            ManualResetEvent startEvent = new ManualResetEvent(false);
            Thread[] threads = new Thread[10];

            for (int i = 0; i < 10; i++)
            {
                int id = i + 1;
                threads[i] = new Thread(() =>
                {
                    startEvent.WaitOne();
                    Console.WriteLine($"Поток {id} стартовал");
                });
                threads[i].Start();
            }

            Console.WriteLine("Ожидание 2 секунды...");
            Thread.Sleep(2000);
            startEvent.Set();

            foreach (var t in threads) t.Join();
        }

        static void Task6()
        {
            AutoResetEvent event1 = new AutoResetEvent(true);
            AutoResetEvent event2 = new AutoResetEvent(false);

            Thread t1 = new Thread(() =>
            {
                for (int i = 1; i <= 9; i += 2)
                {
                    event1.WaitOne();
                    Console.WriteLine("Поток 1: " + i);
                    event2.Set();
                }
            });

            Thread t2 = new Thread(() =>
            {
                for (int i = 2; i <= 10; i += 2)
                {
                    event2.WaitOne();
                    Console.WriteLine("Поток 2: " + i);
                    event1.Set();
                }
            });

            t1.Start(); t2.Start();
            t1.Join(); t2.Join();
        }
    }
}