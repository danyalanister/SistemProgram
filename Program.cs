using System;
using System.Diagnostics;
using System.Linq;

namespace SystemProgrammingTasks
{
    class Program
    {
        static void Main()
        {
            while (true)
            {
                Console.WriteLine("1 - Задание 1 (Мониторинг процесса)");
                Console.WriteLine("2 - Задание 2 (Дочерний процесс cmd)");
                Console.WriteLine("3 - Задание 3 (Завершение notepad)");
                Console.WriteLine("4 - Задание 4 (Отчет AppDomain)");
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
            Process current = Process.GetCurrentProcess();
            Console.WriteLine("Имя процесса: " + current.ProcessName);
            Console.WriteLine("ID: " + current.Id);
            Console.WriteLine("Время запуска: " + current.StartTime);
            Console.WriteLine("Память (МБ): " + (current.WorkingSet64 / 1024 / 1024));
            Console.WriteLine("Количество потоков: " + current.Threads.Count);
            Console.WriteLine();
            Console.WriteLine("Топ-5 процессов по памяти:");

            Process[] all = Process.GetProcesses();
            var top5 = all.OrderByDescending(p => p.WorkingSet64).Take(5);

            foreach (var p in top5)
            {
                Console.WriteLine(p.ProcessName + " - " + (p.WorkingSet64 / 1024 / 1024) + " МБ");
            }
        }

        static void Task2()
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "cmd.exe";
            psi.Arguments = "/c ping 8.8.8.8 -n 4";
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.CreateNoWindow = true;

            Process p = Process.Start(psi);
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            Console.WriteLine(output);
        }

        static void Task3()
        {
            Process p = Process.Start("notepad.exe");
            Console.WriteLine("Процесс запущен, ожидаем 3 секунды...");

            if (!p.WaitForExit(3000))
            {
                Console.WriteLine("Процесс не завершился. Принудительное завершение...");
                p.Kill();
                p.WaitForExit();
                Console.WriteLine("Процесс убит, ресурсы ОС освобождены.");
            }
            else
            {
                Console.WriteLine("Процесс завершился сам.");
            }
        }

        static void Task4()
        {
            Console.WriteLine("Отчет по AppDomain (Вариант Б)");
            Console.WriteLine("1. AppDomain использовались для загрузки и выполнения кода в изолированной среде с возможностью его полной выгрузки, например, в ASP.NET или для плагинов.");
            Console.WriteLine("2. AppDomain - это логическая изолированная среда внутри одного процесса, в то время как процесс - это отдельная сущность ОС со своим адресным пространством.");
            Console.WriteLine("3. В современном .NET для изоляции кода на смену пришел AssemblyLoadContext.");
        }
    }
}