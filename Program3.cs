using System;
using System.Diagnostics;
using System.IO;

namespace SystemProgrammingTasks
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] == "child3")
                {
                    Console.WriteLine(args[1] + " " + args[2] + " " + args[3]);
                    int a = int.Parse(args[1]);
                    int b = int.Parse(args[2]);
                    int res = 0;

                    if (args[3] == "+") res = a + b;
                    if (args[3] == "-") res = a - b;
                    if (args[3] == "*") res = a * b;
                    if (args[3] == "/") res = a / b;

                    Console.WriteLine(res);
                    return;
                }

                if (args[0] == "child4")
                {
                    string path = args[1];
                    string word = args[2];

                    if (File.Exists(path))
                    {
                        string text = File.ReadAllText(path);
                        int count = 0;
                        int i = 0;

                        while ((i = text.IndexOf(word, i)) != -1)
                        {
                            i += word.Length;
                            count++;
                        }

                        Console.WriteLine(count);
                    }
                    return;
                }
            }

            while (true)
            {
                Console.WriteLine("1 - Задание 1");
                Console.WriteLine("2 - Задание 2");
                Console.WriteLine("3 - Задание 3");
                Console.WriteLine("4 - Задание 4");
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
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/c exit 77";
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();

            Console.WriteLine(p.ExitCode);
        }

        static void Task2()
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/c timeout /t 10";
            p.Start();

            Console.WriteLine("1 - Ожидать завершения");
            Console.WriteLine("2 - Принудительно завершить");
            string c = Console.ReadLine();

            if (c == "1")
            {
                p.WaitForExit();
                Console.WriteLine(p.ExitCode);
            }
            else if (c == "2")
            {
                try
                {
                    p.Kill();
                    p.WaitForExit();
                    Console.WriteLine("Процесс принудительно завершен");
                }
                catch
                {
                    Console.WriteLine("Ошибка завершения");
                }
            }
        }

        static void Task3()
        {
            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            Process p = new Process();
            p.StartInfo.FileName = exePath;
            p.StartInfo.Arguments = "child3 7 3 +";
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();
        }

        static void Task4()
        {
            string tempFile = "test.txt";
            File.WriteAllText(tempFile, "bicycle car bicycle bike bicycle");

            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            Process p = new Process();
            p.StartInfo.FileName = exePath;
            p.StartInfo.Arguments = "child4 " + tempFile + " bicycle";
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();
        }
    }
}