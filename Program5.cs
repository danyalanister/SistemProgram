using System;
using System.Threading;

namespace SystemProgrammingTasks
{
    class Program
    {
        delegate double HeavyMathDelegate(int n);
        delegate void TrajectoryDelegate();

        static int ticketsLeft = 100;
        static int fuelLevel = 0;

        static void Main()
        {
            while (true)
            {
                Console.WriteLine("1 - Задание 1 (Диспетчер задач)");
                Console.WriteLine("2 - Задание 2 (Асинхронный калькулятор)");
                Console.WriteLine("3 - Задание 3 (Касса кинотеатра)");
                Console.WriteLine("4 - Задание 4 (Космический запуск)");
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
            Thread bgThread = new Thread(() =>
            {
                while (true)
                {
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss"));
                    Thread.Sleep(1000);
                }
            });
            bgThread.IsBackground = true;
            bgThread.Start();

            for (int i = 1; i <= 3; i++)
            {
                ThreadPool.QueueUserWorkItem(Task1Method, i);
            }

            Thread.Sleep(5000);
        }

        static void Task1Method(object state)
        {
            int taskId = (int)state;
            Console.WriteLine("Задача №" + taskId + " выполняется в потоке " + Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(2000);
        }

        static void Task2()
        {
            HeavyMathDelegate del = CalculateHeavyMath;
            del.BeginInvoke(25, CalculationFinished, del);

            Console.WriteLine("Расчет запущен. Главный поток свободен.");
            Thread.Sleep(4000);
        }

        static double CalculateHeavyMath(int n)
        {
            Thread.Sleep(3000);
            return Math.Sqrt(n);
        }

        static void CalculationFinished(IAsyncResult ar)
        {
            HeavyMathDelegate del = (HeavyMathDelegate)ar.AsyncState;
            double result = del.EndInvoke(ar);
            Console.WriteLine("Результат: " + result);
        }

        static void Task3()
        {
            ticketsLeft = 100;
            Thread[] threads = new Thread[5];

            for (int i = 0; i < 5; i++)
            {
                int cashierId = i + 1;
                threads[i] = new Thread(() => CashierWork(cashierId));
                threads[i].Start();
            }

            for (int i = 0; i < 5; i++)
            {
                threads[i].Join();
            }
        }

        static void CashierWork(int cashierId)
        {
            while (true)
            {
                int current = Interlocked.Decrement(ref ticketsLeft);
                if (current >= 0)
                {
                    Console.WriteLine("Кассир " + cashierId + " продал билет. Осталось: " + current);
                    Thread.Sleep(10);
                }
                else
                {
                    break;
                }
            }
        }

        static void Task4()
        {
            fuelLevel = 0;

            Thread monitorThread = new Thread(() =>
            {
                while (true)
                {
                    Console.WriteLine("Телеметрия в норме...");
                    Thread.Sleep(1000);
                }
            });
            monitorThread.IsBackground = true;
            monitorThread.Start();

            TrajectoryDelegate del = CalculateTrajectory;
            del.BeginInvoke(TrajectoryFinished, del);

            ThreadPool.QueueUserWorkItem(PumpFuel);

            Thread.Sleep(5000);

            int finalFuel = Interlocked.CompareExchange(ref fuelLevel, 0, 0);
            Console.WriteLine("Итоговый уровень топлива: " + finalFuel);
        }

        static void CalculateTrajectory()
        {
            Thread.Sleep(4000);
        }

        static void TrajectoryFinished(IAsyncResult ar)
        {
            TrajectoryDelegate del = (TrajectoryDelegate)ar.AsyncState;
            del.EndInvoke(ar);
            Console.WriteLine("Траектория рассчитана! К запуску готов");
        }

        static void PumpFuel(object state)
        {
            for (int i = 0; i < 10; i++)
            {
                Interlocked.Add(ref fuelLevel, 10);
                Thread.Sleep(500);
            }
        }
    }
}