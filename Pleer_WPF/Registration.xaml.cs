using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Pleer_WPF
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public string image = Directory.GetCurrentDirectory() + @"\standart.png";
        public Registration()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fb = new OpenFileDialog();
            fb.FilterIndex = 1;
            fb.Filter = "jpg|*.jpg| bmp|*.bmp | png|*.png";
            if (fb.ShowDialog() == true)
            {
                Uri fileUri = new Uri(fb.FileName);
                avatar.Source = new BitmapImage(fileUri);
                image = fb.FileName;
                return;
            }
            return;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (login.Text.Trim().Length < 2)
            {
                MessageBox.Show("Длина логина должна быть как минимум 2 символа!");
                return;
            }
            else if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Users\" + login.Text.Trim()))
            {
                MessageBox.Show("Введённый логин уже существует");
                return;
            }
            try
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Users\" + login.Text.Trim());
                Directory.Delete(Directory.GetCurrentDirectory() + @"\Users\" + login.Text.Trim(),true);
            }
            catch
            {
                MessageBox.Show("Логин содержит недопустимые символы! Повторите попытку!");
                return;
            }
            if (pass.Password.Trim().Length < pass.Password.Length)
            {
                MessageBox.Show("Пробелы в начале и конце пароля недопустимы!");
                return;
            }
            else if (pass.Password.Trim().Length < 8)
            {
                MessageBox.Show("Длина паролья должна быть не менее 8 символов");
                return;
            }
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Users\" + login.Text.Trim());
            File.Copy(image, Directory.GetCurrentDirectory() + @"\Users\" + login.Text.Trim() + @"\ico.jpg");
            using (BinaryWriter writer = new BinaryWriter(File.Open(Directory.GetCurrentDirectory() + @"\Users\" + login.Text.Trim() + @"\password.pass", FileMode.OpenOrCreate)))
            {
                writer.Write(pass.Password);
            }
            MessageBox.Show("Аккаунт успешно создан: \nЛогин: " + login.Text + "\nПароль: " + pass.Password);
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
