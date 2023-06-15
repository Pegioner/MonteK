using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.IO;
using org.mariuszgromada.math.mxparser;

namespace MonteK
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        private static Function Fun;
        private static double Down ;
        private static double Up;
        private static int Points;
        private static double IntegralMK;
        private static bool check = false;

        public MainPage()
        {
            InitializeComponent();

            

            ToolbarItem tb1 = new ToolbarItem
            {
                Text = "Сохранить",
                Order = ToolbarItemOrder.Primary,
                Priority = 0,
                IconImageSource = "save_inv.png"
            };

            tb1.Clicked += async (s, e) =>
            {

                if (check == false)
                {
                    await DisplayAlert("Ошибка", "Файл не может быть сохранён, так как значения ещё не введены!", "OK");
                }
                else
                {
                    string save = $"Функция: f(x) = {Fun.getFunctionExpressionString()} \nНижний предел: {Down} \nВерхний предел: {Up} \nКоличество точек: {Points} \nИнтеграл: {IntegralMK:F3}";
                    var backingFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "saveMK.txt");
                    File.Delete(backingFile);
                    using (var writer = File.CreateText(backingFile))
                    {
                        await writer.WriteLineAsync(save);
                    }
                    await DisplayAlert("Данные", "Данные сохранены!", "OK");
                }
            };


            ToolbarItem tb2 = new ToolbarItem
            {
                Text = "Открыть",
                Order = ToolbarItemOrder.Secondary,
                Priority = 1,
            };

            tb2.Clicked += async (s, e) =>
            {
                var backingFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "saveMK.txt");
                if (backingFile == null || !File.Exists(backingFile))
                {
                    await DisplayAlert("Ошибка", "Файл с сохранёнными значениями не найден!", "OK");
                }
                else
                {
                    string save = null;
                    using (var reader = new StreamReader(backingFile, true))
                    {
                        string line;
                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            save += "\n" + line + "\n";
                        }
                    }
                    await DisplayAlert("Данные", save, "OK");
                }
            };


            ToolbarItem tb3 = new ToolbarItem
            {
                Text = "Удалить",
                Order = ToolbarItemOrder.Secondary,
                Priority = 2,
            };

            tb3.Clicked += async (s, e) =>
            {
                var backingFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "saveMK.txt");
                if (backingFile == null || !File.Exists(backingFile))
                {
                    await DisplayAlert("Ошибка", "Файл с сохранёнными значениями не найден!", "OK");
                }
                else
                {
                    bool result = await DisplayAlert("Удаление", "Вы точно хотите удалить данные из файла?", "Да", "Нет");
                    if (result)
                    {
                        File.Delete(backingFile);
                        await DisplayAlert("Данные", "Данные удалены!", "OK");
                    }
                }
            };

            ToolbarItems.Add(tb1);
            ToolbarItems.Add(tb2);
            ToolbarItems.Add(tb3);
        }


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



            for (int i = 0; i < n; i++)
            {
                x[i] = a + (b - a) * rand.NextDouble();               //случайные точки X
                y[i] = min + (max - min) * rand.NextDouble();         //случайные точки Y
                if (y[i] < f.calculate(x[i]))
                {
                    count++;
                }
            }
            integral = ((count * (b - a) * max) / n);               //нахождение интеграла
            return integral;
        }


        private async void Button_Click(object sender, EventArgs e)
        {
            Function f;
            int n;
            double a, b;
            string result;
            result = Convert.ToString(EntryF.Text);
            f = new Function("f(x) = " + result);

            while (f.checkSyntax() == false)
            {
                await DisplayAlert("Ошибка", "Неправильно введена функция!", "OK");
                return;
            }


            while (!double.TryParse(EntryD.Text, out a))
            {
                await DisplayAlert("Ошибка", "Неправильно введено значение нижнего предела!", "OK");
                return;
            }


            while (!double.TryParse(EntryU.Text, out b))
            {
                await DisplayAlert("Ошибка", "Неправильно введено значение верхнего предела!", "OK");
                return;
            }


            while (!int.TryParse(EntryN.Text, out n) | n < 1)
            {
                await DisplayAlert("Ошибка", "Неправильно введено количество точек!", "OK");
                return;
            }

            Fun = f;
            Down = a;
            Up = b;
            Points = n;
            IntegralMK = Integral(Fun, Down, Up, Points);
            check = true;

            Label1.Text = "";
            Label1.Text = "Интеграл = ";
            Label1.Text += String.Format("{0:N3}", IntegralMK);

            
        }
    }
}
