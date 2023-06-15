using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using org.mariuszgromada.math.mxparser;
using Monte_Karlo.Properties;


namespace Monte_Karlo
{
    class MonteKarlo
    {
        private static readonly string path = "logs.txt";
        private static Function Fun = new Function("f(x) = " + Settings.Default["pFun"].ToString());
        private static double Down = Convert.ToDouble(Settings.Default["pDown"].ToString());
        private static double Up = Convert.ToDouble(Settings.Default["pUp"].ToString());
        private static int Points = Convert.ToInt32(Settings.Default["pPoints"].ToString());
        private static double[] IntegralMK = new double[1];


        /// <summary>
        /// Получает от пользователя значение типа Function.
        /// </summary>
        /// <param name="message">Сообщение для пользователя.</param>
        /// <returns>Значение типа Function.</returns>
        private static Function InputFunction(string message)
        {
            Console.WriteLine(message);

            string result;
            result = Console.ReadLine().ToLower().Replace(" ", "");
            Function f = new Function("f(x) = " + result);
            Func:
            while (f.checkSyntax() == false)
            {
                Console.WriteLine("Некорректный ввод, повторите попытку");
                result = Console.ReadLine().ToLower().Replace(" ", "");
                f = new Function("f(x) = " + result);
            }
            if(double.IsNaN(f.calculate(1)))
                {
                    Console.WriteLine("Невозможно рассчитать функцию, повторите попытку");
                    goto Func;
                }
            return f;
        }


        /// <summary>
        /// Получает от пользователя значение типа double.
        /// </summary>
        /// <param name="message">Сообщение для пользователя.</param>
        /// <param name="condition">Условие, которому должно соответствовать значение.</param>
        /// <returns>Значение типа double.</returns>
        private static double InputDouble(string message, Predicate<double> condition = null)
        {
            Console.WriteLine(message);

            double result;
            while (!double.TryParse(Console.ReadLine(), out result) || (condition != null && !condition(result)))
                Console.WriteLine("Некорректный ввод, повторите попытку");
            Funcd:
            if (double.IsNaN(Fun.calculate(result)))
            {
                Console.WriteLine("Невозможно рассчитать функцию с данным пределом, повторите попытку");
                goto Funcd;
            }
            return result;
        }


        /// <summary>
        /// Получает от пользователя значение типа int.
        /// </summary>
        /// <param name="message">Сообщение для пользователя.</param>
        /// <param name="condition">Условие, которому должно соответствовать значение.</param>
        /// <returns>Значение типа integer.</returns>
        private static int InputInteger(string message, Predicate<int> condition = null)
        {
            Console.WriteLine(message);

            int result;
            while (!int.TryParse(Console.ReadLine(), out result) || (condition != null && !condition(result)))
                Console.WriteLine("Некорректный ввод, повторите попытку");

            return result;
        }

        private static int mark = 3;

        /// <summary>
        /// Вывод информационного сообщения.
        /// </summary>
        /// <param name="message">Сообщение для пользователя.</param>
        /// /// <param name="message">Отступ в строку.</param>
        private static void Attention(string message, bool str)
        {
            //mark = (mark != 3) ? mark++ : 1;
            if (mark != 3)
            {
                mark++;
            }
            else
            {
                mark = 1;
            }
            Console.WriteLine(message + new String('!', mark));
            if (str)
            {
                Console.WriteLine();
            }
        }



        /// <summary>
        /// Возвращает интеграл.
        /// </summary>
        /// <param name="f">Введённая функция.</param>
        /// <param name="a">Нижний предел.</param>
        /// <param name="b">Верхний предел.</param>
        /// <param name="n">Количество точек.</param>
        /// <returns>Рассчитанный интеграл.</returns>
        private static double Integral(Function f, double a, double b, int n)
        {
            double max, min, z, integral;

            if (f.calculate(a) >= f.calculate(b))
            {
                max = f.calculate(a);            //максимальное значение функции
                min = f.calculate(b);            //минимальное значение ф-ции 
            }
            else
            {
                max = f.calculate(b);
                min = f.calculate(a);
            }
            for (z = a; z <= b; z += (b - a) / 500.0)
            {
                if (f.calculate(z) > max) max = f.calculate(z);
                if (f.calculate(z) < min) min = f.calculate(z);
            }
            double[] x = new double[n];
            double[] y = new double[n];
            Random rand = new Random();
            int count = 0;
            using (var progressBar = new ConsoleProgressBar())
            {
                for (int i = 0; i < n; i++)
                {
                    x[i] = a + (b - a) * rand.Next(-99, 99) * 0.001;               //случайные точки X
                    y[i] = min + (max - min) * rand.Next(-99, 99) * 0.001;         //случайные точки Y
                    if (y[i] < f.calculate(x[i]))
                    {
                        count++;
                    }
                    progressBar.SetProgress((i / (float)n) * 100);
                }
            }
            integral = ((count * (b - a) * max) / n);               //нахождение интеграла
            return integral;
        }



        /// <summary>
        /// Ввод всех данных.
        /// </summary>
        private static void InputAll()
        {
            Fun = InputFunction("Введите функцию");
            Down = InputDouble("Введите значение нижнего предела");
            Up = InputDouble("Введите значение верхнего предела");
            Points = InputInteger("Введите количество точек", I => I > 0);
            Settings.Default["pFun"] = Fun.getFunctionExpressionString();
            Settings.Default["pDown"] = Down;
            Settings.Default["pUp"] = Up;
            Settings.Default["pPoints"] = Points;
            Settings.Default.Save();
            Console.Clear();
            Console.WriteLine("Значения сохранены");
            Console.WriteLine();
        }



        /// <summary>
        /// Выборочный ввод данных.
        /// </summary>
        private static void Input()
        {
            ConsoleKeyInfo menui;
            do
            {
            Menui:
                Console.WriteLine($"f(x) ={Fun.getFunctionExpressionString()}");
                Console.WriteLine($"Нижний предел = {Down}");
                Console.WriteLine($"Верхний предел = {Up}");
                Console.WriteLine($"Количество точек = {Points}");
                Console.WriteLine();
                Console.WriteLine("Выберите, что хотите изменить");
                Console.WriteLine();
                Console.WriteLine("1 - Функцию");
                Console.WriteLine("2 - Значение нижнего предела");
                Console.WriteLine("3 - Введите значение верхнего предела");
                Console.WriteLine("4 - Количество точек");
                Console.WriteLine("0 - Назад");
                Console.WriteLine();
                Console.WriteLine("Введите нужный номер");
                menui = Console.ReadKey();
                Console.Clear();
                switch (menui.Key)
                {
                    case ConsoleKey.D1:
                        Console.WriteLine($"f(x) ={Fun.getFunctionExpressionString()}");
                        Console.WriteLine();
                        Fun = InputFunction("Введите новую функцию");
                        Settings.Default["pFun"] = Fun.getFunctionExpressionString();
                        Settings.Default.Save();
                        break;
                    case ConsoleKey.D2:
                        Console.WriteLine($"Нижний предел = {Down}");
                        Console.WriteLine();
                        Down = InputDouble("Введите новое значение нижнего предела");
                        Settings.Default["pDown"] = Down;
                        Settings.Default.Save();
                        break;
                    case ConsoleKey.D3:
                        Console.WriteLine($"Верхний предел = {Up}");
                        Console.WriteLine();
                        Up = InputDouble("Введите новое значение верхнего предела");
                        Settings.Default["pUp"] = Up;
                        Settings.Default.Save();
                        break;
                    case ConsoleKey.D4:
                        Console.WriteLine($"Количество точек = {Points}");
                        Console.WriteLine();
                        Points = InputInteger("Введите новое количество точек", I => I > 0);
                        Settings.Default["pPoints"] = Points;
                        Settings.Default.Save();
                        break;
                    case ConsoleKey.D0:
                        return;
                    case ConsoleKey.Escape:
                        Environment.Exit(0);
                        break;
                    default:
                        Attention("Такого пункта нет", true);
                        goto Menui;
                }
                Console.Clear();
                Console.WriteLine("Значение изменено");
                Console.WriteLine();
            } while (menui.Key != ConsoleKey.D0);
        }



        /// <summary>
        /// Ввод данных из файла.
        /// </summary>
        private static void Open()
        {
            string ReadFile = null;
        OpenF:
            Console.WriteLine("Укажите полный путь до файла или только название файла, если он находится в одной папке с программой");
            Console.WriteLine("Значения должны быть разделены точкой с запятой и записаны в одну строку в следующем порядке:");
            Console.WriteLine("'Функция'; 'Нижний предел'; 'Верхний предел'; 'Количество точек'");
            Console.WriteLine("Для возврата нажмите Enter");
            string FName = Console.ReadLine();
            Console.Clear();
            if (FName == "")
            {
                return;
            }
            try
            {
                ReadFile = File.ReadAllText(FName);

            }
            catch
            {
                Attention("Такого файла не существует", true);
                goto OpenF;
            }
            try
            {
                string[] tmp = ReadFile.ToLower().Replace(" ", "").Split(';');
                if (tmp.Length != 4)
                {
                    throw new Exception();
                }
                Fun = new Function("f(x) = " + tmp[0]);
                Fun.calculate(1);
                Down = Convert.ToDouble(tmp[1]);
                Up = Convert.ToDouble(tmp[2]);
                Points = Convert.ToInt32(tmp[3]);
                Settings.Default["pFun"] = Fun.getFunctionExpressionString();
                Settings.Default["pDown"] = Down;
                Settings.Default["pUp"] = Up;
                Settings.Default["pPoints"] = Points;
                Settings.Default.Save();
                Console.WriteLine("Значения сохранены");
                Console.WriteLine();
            }
            catch
            {
                Attention("Из файла невозможно получить значения", true);
                goto OpenF;
            }

        }



        /// <summary>
        /// Удаление данных.
        /// </summary>
        private static void DelIn()
        {
            if ((Fun != null) & (Points != 0) & (Up != 0) & (Down != 0))
            {
            ParDel:
                Console.WriteLine($"f(x) ={Fun.getFunctionExpressionString()}");
                Console.WriteLine($"Нижний предел = {Down}");
                Console.WriteLine($"Верхний предел = {Up}");
                Console.WriteLine($"Количество точек = {Points}");
                Console.WriteLine();
                Console.WriteLine("Вы точно хотите удалить значения?");
                Console.WriteLine("Выберите (Y/N)");
                ConsoleKeyInfo pardel;
                pardel = Console.ReadKey();
                Console.Clear();
                switch (pardel.Key)
                {
                    case ConsoleKey.Y:
                        Fun = null;
                        Down = 0;
                        Up = 0;
                        Points = 0;
                        Settings.Default["pFun"] = null;
                        Settings.Default["pDown"] = Down;
                        Settings.Default["pUp"] = Up;
                        Settings.Default["pPoints"] = Points;
                        Settings.Default.Save();
                        Console.WriteLine("Значения удалены");
                        Console.WriteLine();
                        break;
                    case ConsoleKey.N:
                        break;
                    default:
                        Attention("Неверное значение", false);
                        goto ParDel;
                }
            }
            else
            {
                Attention("Значения не введены", true);
            }
        }


        /// <summary>
        /// Работа с данными.
        /// </summary>
        private static void Parametrs()
        {
            ConsoleKeyInfo menup;
            do
            {
            Menup:
                Console.WriteLine("Работа со значениями");
                Console.WriteLine();
                if (Fun != null)
                {
                    Console.WriteLine($"f(x) ={Fun.getFunctionExpressionString()}");
                }
                else
                {
                    Console.WriteLine($"f(x) = 0");
                }
                Console.WriteLine($"Нижний предел = {Down}");
                Console.WriteLine($"Верхний предел = {Up}");
                Console.WriteLine($"Количество точек = {Points}");
                Console.WriteLine();
                Console.WriteLine("1 - Ввести все значения");
                Console.WriteLine("2 - Исправить введённые значения");
                Console.WriteLine("3 - Загрузить значения из файла");
                Console.WriteLine("4 - Удалить введённые значения");
                Console.WriteLine("0 - Назад");
                Console.WriteLine("ESC - Выход");
                Console.WriteLine();
                Console.WriteLine("Введите нужный номер");
                menup = Console.ReadKey();
                Console.Clear();
                switch (menup.Key)
                {
                    case ConsoleKey.D1:
                        InputAll();
                        break;
                    case ConsoleKey.D2:
                        if (Fun != null)
                        {
                            Input();
                        }
                        else
                        {
                            Attention("Значения ещё не введены", true);
                        }
                        break;
                    case ConsoleKey.D3:
                        Open();
                        break;
                    case ConsoleKey.D4:
                        DelIn();
                        break;
                    case ConsoleKey.D0:
                        return;
                    case ConsoleKey.Escape:
                        Environment.Exit(0);
                        break;
                    default:
                        Attention("Такого пункта нет", true);
                        goto Menup;
                }
            } while (menup.Key != ConsoleKey.D0);

        }


        /// <summary>
        /// Рассчитывает и выводит интеграл.
        /// </summary>
        private static void Result()
        {
            Array.Resize(ref IntegralMK, IntegralMK.Length + 1);
            IntegralMK[IntegralMK.Length-1] = Integral(Fun, Down, Up, Points);
            Console.Clear();
            /*Console.WriteLine($"Интеграл = {IntegralMK[0]:F3}");
            Console.WriteLine();
            Console.WriteLine("1 - Сохранить значения и продолжить.");
            Console.WriteLine("Чтобы продолжить, без сохранения нажмите любую клавишу.");
            if (Console.ReadKey(true).Key == ConsoleKey.D1)
            {
                Console.Clear();
                Save();
            }*/
        }



        /// <summary>
        /// Рассчитывает и выводит интеграл.
        /// </summary>
        private static void ResultLot()
        {
            int n = InputInteger("Введите количество расчётов", I => I > 0);
            Console.Clear();
            for (int i = 0; i<n; i++)
            {
                Array.Resize(ref IntegralMK, IntegralMK.Length + 1);
                IntegralMK[IntegralMK.Length - 1] = Integral(Fun, Down, Up, Points);
                Console.Clear();
                IntegralMK[0] = 0;
                for (int j = i; j >= 0; j--)
                {
                    Console.WriteLine($"{i - j + 1}: {IntegralMK[IntegralMK.Length - j-1]:F3}");
                    IntegralMK[0] += IntegralMK[IntegralMK.Length - j - 1];
                }
            }
            IntegralMK[0] /= n;
            Console.WriteLine();
            Console.WriteLine($"Среднее значение: {IntegralMK[0]:F3}");
            Console.WriteLine();
            Console.WriteLine("Для продолжения нажмите любую кнопку");
            Console.ReadKey();
            Console.Clear();
        }


        /// <summary>
        /// Работа с расчётами.
        /// </summary>
        private static void Calculate()
        {
            ConsoleKeyInfo menuc;
            do
            {
            Menuc:
                Console.WriteLine("Работа с расчётом");
                Console.WriteLine();
                if (IntegralMK.Length != 1)
                {
                    IntegralMK[0] = 0;
                    for (int i = 1; i < IntegralMK.Length; i++)
                    {
                        Console.WriteLine($"{i} расчёт: {IntegralMK[i]:F3}");
                        IntegralMK[0] += IntegralMK[i];
                    }
                    IntegralMK[0] /= IntegralMK.Length - 1;
                    Console.WriteLine();
                    Console.WriteLine($"Среднее значение: {IntegralMK[0]:F3}");
                    Console.WriteLine();
                }
                Console.WriteLine("1 - Выполнить расчёт");
                Console.WriteLine("2 - Выполнить несколько расчётов сразу");
                Console.WriteLine("3 - Удалить полученные значения");
                Console.WriteLine("0 - Назад");
                Console.WriteLine("ESC - Выход");
                Console.WriteLine();
                Console.WriteLine("Введите нужный номер");
                menuc = Console.ReadKey();
                Console.Clear();
                switch (menuc.Key)
                {
                    case ConsoleKey.D1:
                        if ((Fun != null) & (Points != 0))
                        {
                            Result();
                        }
                        else
                        {
                            Attention("Невозможно выполнить расчёт", true);
                        }
                        break;
                    case ConsoleKey.D2:
                        if ((Fun != null) & (Points != 0))
                        {
                            ResultLot();
                        }
                        else
                        {
                            Attention("Невозможно выполнить расчёт", true);
                        }
                        break;
                    case ConsoleKey.D3:
                        if (IntegralMK.Length > 1)
                        {
                        ResDel:
                            for (int i = 1; i < IntegralMK.Length; i++)
                            {
                                Console.WriteLine($"{i} расчёт: {IntegralMK[i]:F3}");
                            }
                            Console.WriteLine();
                            Console.WriteLine("Вы точно хотите удалить значения?");
                            Console.WriteLine("Выберите (Y/N)");
                            ConsoleKeyInfo resdel;
                            resdel = Console.ReadKey();
                            Console.Clear();
                            switch (resdel.Key)
                            {
                                case ConsoleKey.Y:
                                    Array.Resize(ref IntegralMK, 1);
                                    Console.WriteLine("Значения удалены");
                                    Console.WriteLine();
                                    break;
                                case ConsoleKey.N:
                                    break;
                                default:
                                    Attention("Неверное значение", false);
                                    goto ResDel;
                            }
                        }
                        else 
                        {
                            Attention("Значений нет", true);
                        }
                    
                        goto Menuc;
                    case ConsoleKey.D0:
                        SaveLog();
                        return;
                    case ConsoleKey.Escape:
                        SaveLog();
                        Environment.Exit(0);
                        break;
                    default:
                        Attention("Такого пункта нет", true);
                        goto Menuc;
                }
            } while (menuc.Key != ConsoleKey.D0);

        }



        /// <summary>
        /// Работа с логами.
        /// </summary>
        private static void SaveLog()
        {
            if ((Fun != null) | (IntegralMK.Length > 1))
            {
                string save = $"f(x) ={Fun.getFunctionExpressionString()} | {Down} | {Up} | {Points} || {IntegralMK[0]:F3} |";
                for (int i = 1; i < IntegralMK.Length; i++)
                {
                    save += $"| {IntegralMK[i]:F3} ";
                }
                File.AppendAllText(path, save + Environment.NewLine);
                Console.WriteLine();
            }
        }


        /// <summary>
        /// Работа с сохранением.
        /// </summary>
        private static void Save()
        {
            Console.WriteLine("Введите название и формат файла, в котором хотите сохранить значения (например: save.txt)");
            Console.WriteLine("Для возврата нажмите Enter");
            string FileName = Console.ReadLine();
            Console.Clear();
            if (FileName == "")
            {
                return;
            }
            string save;
            if (IntegralMK.Length == 2)
            {
                save = $"Функция: f(x) = {Fun.getFunctionExpressionString()} \nНижний предел: {Down} \nВерхний предел: {Up} \nКоличество точек: {Points} \nИнтеграл: {IntegralMK[1]:F3}";
            }
            else
            {
                save = $"Функция: f(x) = {Fun.getFunctionExpressionString()} \nНижний предел: {Down} \nВерхний предел: {Up} \nКоличество точек: {Points} \nСреднее значение интеграла: {IntegralMK[0]:F3} \nЗначения интеграла: ";
                for (int i = 1; i < IntegralMK.Length; i++)
                {
                    save += $"\n{i} значение: {IntegralMK[i]:F3} ";
                }
            }

            File.AppendAllText(FileName, save);
            Console.WriteLine($"Значения сохранены по пути: {Environment.CurrentDirectory}\\{FileName}");
            Console.WriteLine();
            Console.WriteLine("Чтобы показать файл в папке, нажмите 1");
            Console.WriteLine("Чтобы открыть файл, нажмите 2");
            Console.WriteLine("Для проделжения нажмите любую другую клавишу");
            ConsoleKeyInfo savekey;
            savekey = Console.ReadKey();
            Console.Clear();
            if (savekey.Key == ConsoleKey.D1)
            {
                if (File.Exists(FileName))
                {
                    Process.Start(new ProcessStartInfo("explorer.exe", " /select, " + FileName));
                }
            }
            else if (savekey.Key == ConsoleKey.D2)
            {
                File.Open(FileName, FileMode.OpenOrCreate).Close();
            }
        }



        /// <summary>
        /// Работа с логами.
        /// </summary>
        private static void Logs()
        {
            ConsoleKeyInfo menul;
            Menul:
                Console.WriteLine("Работа с логами");
                Console.WriteLine();
                Console.WriteLine("1 - Сохранить значения в файл");
                Console.WriteLine("2 - Открыть файл с логами");
                Console.WriteLine("3 - Удалить файл с логами");
                Console.WriteLine("0 - Назад");
                Console.WriteLine("ESC - Выход");
                Console.WriteLine();
                Console.WriteLine("Введите нужный номер");
                menul = Console.ReadKey();
                Console.Clear();
                switch (menul.Key)
                {
                    case ConsoleKey.D1:
                        if ((Fun == null) | (IntegralMK.Length < 2))
                        {
                            Attention("Невозможно сохранить, так как отсутствуют некоторые значеня", true);
                        }
                        else
                        {
                            Save();
                        }
                    goto Menul;
                    case ConsoleKey.D2:
                        File.Open(path, FileMode.OpenOrCreate).Close();
                        Process.Start(path);
                    goto Menul;
                    case ConsoleKey.D3:
                    IfDel:
                        Console.WriteLine("Вы точно хотите удалить логи?");
                        Console.WriteLine("Выберите (Y/N)");
                        ConsoleKeyInfo ifdel;
                        ifdel = Console.ReadKey();
                        Console.Clear();
                        switch (ifdel.Key)
                        {
                            case ConsoleKey.Y:
                                File.Delete(path);
                                Console.WriteLine("Файл с логами удалён");
                                Console.WriteLine();
                                break;
                            case ConsoleKey.N:
                                break;
                            default:
                                Attention("Неверное значение", false);
                                goto IfDel;
                        }
                    goto Menul;
                    case ConsoleKey.D0:
                        return;
                    case ConsoleKey.Escape:
                        Environment.Exit(0);
                        break;
                    default:
                        Attention("Такого пункта нет", true);
                        goto Menul;
                }
        }


        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        static extern int LoadKeyboardLayout(string pwszKLID, uint Flags);    

        public static void Main(string[] args)
        {
            string lang = "00000409";
            int ret = LoadKeyboardLayout(lang, 1);
            PostMessage(GetForegroundWindow(), 0x50, 1, ret);

            ConsoleKeyInfo menu;
            do
            {
            //Menu:
                Console.WriteLine("Меню");
                Console.WriteLine();
                Console.WriteLine("1 - Ввод значений");
                Console.WriteLine("2 - Расчёт");
                Console.WriteLine("3 - Сохранение");
                Console.WriteLine("ESC - Выход");
                Console.WriteLine();
                Console.WriteLine("Введите нужный пункт меню");
                menu = Console.ReadKey();
                Console.Clear();
                switch (menu.Key)
                {
                    case ConsoleKey.D1:
                        Parametrs();
                        break;
                    case ConsoleKey.D2:
                        Calculate();
                        break;
                    case ConsoleKey.D3:
                        Logs();
                        break;
                    case ConsoleKey.Escape:
                        Environment.Exit(0);
                        break;
                    default:
                        Attention("Такого пункта в меню нет", true);
                        break;
                }
            } while (menu.Key != ConsoleKey.Escape);

        }
    }
}
