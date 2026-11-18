using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace SystemProgrammingTasks
{
    class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        static void Main()
        {
            while (true)
            {
                Console.WriteLine("1 - Задание 1 (Hello World)");
                Console.WriteLine("2 - Задание 2 (Угадай число)");
                Console.WriteLine("3 - Задание 3 (Закрыть Блокнот)");
                Console.WriteLine("4 - Задание 4 (Время в Блокноте)");
                Console.WriteLine("0 - Выход");
                Console.Write("Выбери задание: ");

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
            MessageBox(IntPtr.Zero, "Hello, World!", "Задание 1", 0);
        }

        static void Task2()
        {
            bool play = true;
            while (play)
            {
                int min = 0;
                int max = 100;
                bool guessed = false;

                MessageBox(IntPtr.Zero, "Загадай число от 0 до 100", "Правила", 0);

                while (min <= max)
                {
                    int mid = (min + max) / 2;
                    int result = MessageBox(IntPtr.Zero, "Твое число " + mid + "?", "Компьютер угадывает", 4);

                    if (result == 6)
                    {
                        MessageBox(IntPtr.Zero, "Ура, я угадал!", "Победа", 0);
                        guessed = true;
                        break;
                    }
                    else
                    {
                        int greater = MessageBox(IntPtr.Zero, "Твое число больше " + mid + "?", "Вопрос", 4);
                        if (greater == 6)
                        {
                            min = mid + 1;
                        }
                        else
                        {
                            max = mid - 1;
                        }
                    }
                }

                if (!guessed)
                {
                    MessageBox(IntPtr.Zero, "Где-то ошибка, число не найдено", "Ошибка", 0);
                }

                int replay = MessageBox(IntPtr.Zero, "Сыграем еще раз?", "Конец игры", 4);
                if (replay != 6)
                {
                    play = false;
                }
            }
        }

        static void Task3()
        {
            IntPtr hwnd = FindWindow("Notepad", null);
            if (hwnd != IntPtr.Zero)
            {
                SendMessage(hwnd, 0x0010, IntPtr.Zero, IntPtr.Zero);
                Console.WriteLine("Команда на закрытие Блокнота отправлена.");
            }
            else
            {
                Console.WriteLine("Окно Блокнота не найдено. Открой Блокнот и попробуй снова.");
            }
        }

        static void Task4()
        {
            IntPtr hwnd = FindWindow("Notepad", null);
            if (hwnd != IntPtr.Zero)
            {
                Console.WriteLine("Время обновляется каждую секунду. Нажми любую клавишу в консоли для остановки.");
                while (!Console.KeyAvailable)
                {
                    string time = DateTime.Now.ToString("HH:mm:ss");
                    SendMessage(hwnd, 0x000C, IntPtr.Zero, time);
                    Thread.Sleep(1000);
                }
                Console.ReadKey(true);
                Console.WriteLine("Обновление остановлено.");
            }
            else
            {
                Console.WriteLine("Окно Блокнота не найдено. Открой Блокнот и попробуй снова.");
            }
        }
    }
}