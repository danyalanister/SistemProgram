using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
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
                Console.WriteLine("1 - Задание 1 (Потоки с приоритетами)");
                Console.WriteLine("2 - Задание 2-3 (Многопоточное копирование файла с паузой)");
                Console.WriteLine("3 - Задание 4-5 (Многопоточное копирование папки с паузой)");
                Console.WriteLine("4 - Задание 6 (Асинхронный факториал)");
                Console.WriteLine("5 - Задание 7 (Асинхронная степень)");
                Console.WriteLine("6 - Задание 8 (Асинхронный анализ текста)");
                Console.WriteLine("0 - Выход");
                Console.Write("Выбор: ");

                string choice = Console.ReadLine();

                if (choice == "0") break;
                if (choice == "1") Task1();
                if (choice == "2") Task2_3();
                if (choice == "3") Task4_5();
                if (choice == "4") Task6();
                if (choice == "5") Task7();
                if (choice == "6") Task8();

                Console.WriteLine();
            }
        }

        static void Task1()
        {
            Form f = new Form { Text = "Задание 1", Width = 600, Height = 400 };
            ListBox lbNum = new ListBox { Left = 10, Top = 50, Width = 170, Height = 250 };
            ListBox lbLet = new ListBox { Left = 200, Top = 50, Width = 170, Height = 250 };
            ListBox lbSym = new ListBox { Left = 390, Top = 50, Width = 170, Height = 250 };

            ComboBox cbNum = new ComboBox { Left = 10, Top = 10, Width = 170 };
            ComboBox cbLet = new ComboBox { Left = 200, Top = 10, Width = 170 };
            ComboBox cbSym = new ComboBox { Left = 390, Top = 10, Width = 170 };

            string[] priorities = { "Lowest", "Normal", "Highest" };
            cbNum.Items.AddRange(priorities); cbNum.SelectedIndex = 1;
            cbLet.Items.AddRange(priorities); cbLet.SelectedIndex = 1;
            cbSym.Items.AddRange(priorities); cbSym.SelectedIndex = 1;

            Button btnStart = new Button { Left = 10, Top = 320, Width = 550, Text = "Старт" };

            bool isRunning = false;

            btnStart.Click += (s, e) =>
            {
                if (isRunning) return;
                isRunning = true;
                lbNum.Items.Clear(); lbLet.Items.Clear(); lbSym.Items.Clear();

                Thread tNum = new Thread(() => Generate(lbNum, () => new Random().Next(0, 10).ToString()));
                Thread tLet = new Thread(() => Generate(lbLet, () => ((char)new Random().Next(65, 91)).ToString()));
                Thread tSym = new Thread(() => Generate(lbSym, () => ((char)new Random().Next(33, 48)).ToString()));

                tNum.Priority = GetPriority(cbNum.Text);
                tLet.Priority = GetPriority(cbLet.Text);
                tSym.Priority = GetPriority(cbSym.Text);

                tNum.Start(); tLet.Start(); tSym.Start();
            };

            f.Controls.AddRange(new Control[] { lbNum, lbLet, lbSym, cbNum, cbLet, cbSym, btnStart });
            f.ShowDialog();
        }

        static ThreadPriority GetPriority(string p)
        {
            if (p == "Lowest") return ThreadPriority.Lowest;
            if (p == "Highest") return ThreadPriority.Highest;
            return ThreadPriority.Normal;
        }

        static void Generate(ListBox lb, Func<string> generator)
        {
            for (int i = 0; i < 100; i++)
            {
                string val = generator();
                lb.Invoke(new Action(() => lb.Items.Add(val)));
                Thread.Sleep(50);
            }
        }

        static void Task2_3()
        {
            Form f = new Form { Text = "Копирование файла", Width = 400, Height = 250 };
            TextBox txtSrc = new TextBox { Left = 10, Top = 10, Width = 280, Text = "source.txt" };
            TextBox txtDst = new TextBox { Left = 10, Top = 40, Width = 280, Text = "dest.txt" };
            NumericUpDown numTh = new NumericUpDown { Left = 10, Top = 70, Width = 50, Minimum = 1, Maximum = Environment.ProcessorCount, Value = 2 };
            ProgressBar pb = new ProgressBar { Left = 10, Top = 100, Width = 360, Maximum = 100 };

            Button btnStart = new Button { Left = 10, Top = 140, Width = 80, Text = "Старт" };
            Button btnPause = new Button { Left = 100, Top = 140, Width = 80, Text = "Пауза" };
            Button btnStop = new Button { Left = 190, Top = 140, Width = 80, Text = "Стоп" };

            ManualResetEvent pauseEvent = new ManualResetEvent(true);
            bool stopFlag = false;

            btnStart.Click += (s, e) =>
            {
                if (!File.Exists(txtSrc.Text))
                {
                    File.WriteAllBytes(txtSrc.Text, new byte[1024 * 1024 * 10]);
                }

                stopFlag = false;
                pauseEvent.Set();
                pb.Value = 0;

                int threadsCount = (int)numTh.Value;
                long fileLen = new FileInfo(txtSrc.Text).Length;
                long chunkSize = fileLen / threadsCount;
                int completed = 0;

                using (FileStream fs = new FileStream(txtDst.Text, FileMode.Create, FileAccess.Write, FileShare.Write))
                {
                    fs.SetLength(fileLen);
                }

                for (int i = 0; i < threadsCount; i++)
                {
                    int index = i;
                    new Thread(() =>
                    {
                        long startPos = index * chunkSize;
                        long endPos = (index == threadsCount - 1) ? fileLen : startPos + chunkSize;
                        byte[] buffer = new byte[4096];

                        using (FileStream fsIn = new FileStream(txtSrc.Text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (FileStream fsOut = new FileStream(txtDst.Text, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                        {
                            fsIn.Position = startPos;
                            fsOut.Position = startPos;
                            long current = startPos;

                            while (current < endPos && !stopFlag)
                            {
                                pauseEvent.WaitOne();
                                int toRead = (int)Math.Min(buffer.Length, endPos - current);
                                int read = fsIn.Read(buffer, 0, toRead);
                                if (read == 0) break;
                                fsOut.Write(buffer, 0, read);
                                current += read;
                            }
                        }
                        Interlocked.Increment(ref completed);
                        f.Invoke(new Action(() => pb.Value = (completed * 100) / threadsCount));
                    }).Start();
                }
            };

            btnPause.Click += (s, e) =>
            {
                if (btnPause.Text == "Пауза") { pauseEvent.Reset(); btnPause.Text = "Продолжить"; }
                else { pauseEvent.Set(); btnPause.Text = "Пауза"; }
            };

            btnStop.Click += (s, e) => stopFlag = true;

            f.Controls.AddRange(new Control[] { txtSrc, txtDst, numTh, pb, btnStart, btnPause, btnStop });
            f.ShowDialog();
        }

        static void Task4_5()
        {
            Form f = new Form { Text = "Копирование папки", Width = 400, Height = 250 };
            TextBox txtSrc = new TextBox { Left = 10, Top = 10, Width = 280, Text = "SrcFolder" };
            TextBox txtDst = new TextBox { Left = 10, Top = 40, Width = 280, Text = "DstFolder" };
            NumericUpDown numTh = new NumericUpDown { Left = 10, Top = 70, Width = 50, Minimum = 1, Value = 2 };
            ProgressBar pb = new ProgressBar { Left = 10, Top = 100, Width = 360, Maximum = 100 };

            Button btnStart = new Button { Left = 10, Top = 140, Width = 80, Text = "Старт" };
            Button btnPause = new Button { Left = 100, Top = 140, Width = 80, Text = "Пауза" };
            Button btnStop = new Button { Left = 190, Top = 140, Width = 80, Text = "Стоп" };

            ManualResetEvent pauseEvent = new ManualResetEvent(true);
            bool stopFlag = false;

            btnStart.Click += (s, e) =>
            {
                if (!Directory.Exists(txtSrc.Text)) Directory.CreateDirectory(txtSrc.Text);
                if (!Directory.Exists(txtDst.Text)) Directory.CreateDirectory(txtDst.Text);

                stopFlag = false;
                pauseEvent.Set();
                pb.Value = 0;

                string[] files = Directory.GetFiles(txtSrc.Text);
                if (files.Length == 0) return;

                int threadsCount = (int)numTh.Value;
                int completed = 0;

                for (int i = 0; i < threadsCount; i++)
                {
                    int threadIndex = i;
                    new Thread(() =>
                    {
                        for (int j = threadIndex; j < files.Length; j += threadsCount)
                        {
                            if (stopFlag) break;
                            pauseEvent.WaitOne();

                            string destFile = Path.Combine(txtDst.Text, Path.GetFileName(files[j]));
                            File.Copy(files[j], destFile, true);

                            int c = Interlocked.Increment(ref completed);
                            f.Invoke(new Action(() => pb.Value = (c * 100) / files.Length));
                        }
                    }).Start();
                }
            };

            btnPause.Click += (s, e) =>
            {
                if (btnPause.Text == "Пауза") { pauseEvent.Reset(); btnPause.Text = "Продолжить"; }
                else { pauseEvent.Set(); btnPause.Text = "Пауза"; }
            };
            btnStop.Click += (s, e) => stopFlag = true;

            f.Controls.AddRange(new Control[] { txtSrc, txtDst, numTh, pb, btnStart, btnPause, btnStop });
            f.ShowDialog();
        }

        static void Task6()
        {
            Form f = new Form { Text = "Факториал", Width = 300, Height = 150 };
            TextBox txtIn = new TextBox { Left = 10, Top = 10, Width = 100 };
            Button btn = new Button { Left = 120, Top = 10, Width = 100, Text = "Посчитать" };
            Label lbl = new Label { Left = 10, Top = 50, Width = 250 };

            btn.Click += async (s, e) =>
            {
                if (long.TryParse(txtIn.Text, out long n))
                {
                    lbl.Text = "Считаю...";
                    long res = await Task.Run(() =>
                    {
                        long r = 1;
                        for (long i = 1; i <= n; i++) r *= i;
                        return r;
                    });
                    lbl.Text = "Результат: " + res;
                }
            };

            f.Controls.AddRange(new Control[] { txtIn, btn, lbl });
            f.ShowDialog();
        }

        static void Task7()
        {
            Form f = new Form { Text = "Степень", Width = 300, Height = 150 };
            TextBox txtNum = new TextBox { Left = 10, Top = 10, Width = 50, Text = "2" };
            TextBox txtPow = new TextBox { Left = 70, Top = 10, Width = 50, Text = "10" };
            Button btn = new Button { Left = 130, Top = 10, Width = 100, Text = "Посчитать" };
            Label lbl = new Label { Left = 10, Top = 50, Width = 250 };

            btn.Click += async (s, e) =>
            {
                if (double.TryParse(txtNum.Text, out double num) && double.TryParse(txtPow.Text, out double pow))
                {
                    lbl.Text = "Считаю...";
                    double res = await Task.Run(() => Math.Pow(num, pow));
                    lbl.Text = "Результат: " + res;
                }
            };

            f.Controls.AddRange(new Control[] { txtNum, txtPow, btn, lbl });
            f.ShowDialog();
        }

        static void Task8()
        {
            Form f = new Form { Text = "Анализ текста", Width = 400, Height = 250 };
            TextBox txtIn = new TextBox { Left = 10, Top = 10, Width = 360, Height = 100, Multiline = true };
            Button btn = new Button { Left = 10, Top = 120, Width = 100, Text = "Анализ" };
            Label lbl = new Label { Left = 10, Top = 150, Width = 360, Height = 50 };

            btn.Click += async (s, e) =>
            {
                string text = txtIn.Text;
                lbl.Text = "Анализирую...";

                var res = await Task.Run(() =>
                {
                    string v = "аеёиоуыэюяaeiouy";
                    int vowels = text.Count(c => v.Contains(char.ToLower(c)));
                    int letters = text.Count(char.IsLetter);
                    int symbols = text.Length;
                    return new { V = vowels, C = letters - vowels, S = symbols };
                });

                lbl.Text = $"Гласных: {res.V}\nСогласных: {res.C}\nВсего символов: {res.S}";
            };

            f.Controls.AddRange(new Control[] { txtIn, btn, lbl });
            f.ShowDialog();
        }
    }
}
