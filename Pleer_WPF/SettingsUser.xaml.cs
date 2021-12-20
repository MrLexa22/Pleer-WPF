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
    public partial class SettingsUser : Window
    {
        public string log = "";
        public string image = Directory.GetCurrentDirectory() + @"\standart.png";
        public SettingsUser(string logi)
        {
            InitializeComponent();
            log = logi;
            Uri fileUri = new Uri(Directory.GetCurrentDirectory() + @"\Users\" + logi + @"\ico.jpg");
            avatar.Source = new BitmapImage(fileUri);
            login.Text = logi;
            using (BinaryReader reader = new BinaryReader(File.Open(Directory.GetCurrentDirectory() + @"\Users\" + logi + @"\password.pass", FileMode.Open)))
            {
                while (reader.PeekChar() > -1)
                {
                    pass.Text = reader.ReadString();
                }
            }
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
            else if (Directory.Exists(Directory.GetCurrentDirectory() + @"\Users\" + login.Text.Trim()) && login.Text != log)
            {
                MessageBox.Show("Введённый логин уже существует");
                return;
            }
            if (log != login.Text)
            {
                try
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Users\" + login.Text.Trim());
                    Directory.Delete(Directory.GetCurrentDirectory() + @"\Users\" + login.Text.Trim(), true);
                }
                catch
                {
                    MessageBox.Show("Логин содержит недопустимые символы! Повторите попытку!");
                    return;
                }
            }
            else if (pass.Text.Trim().Length < pass.Text.Length)
            {
                MessageBox.Show("Пробелы в начале и конце пароля недопустимы!");
                return;
            }
            else if (pass.Text.Trim().Length < 8)
            {
                MessageBox.Show("Длина паролья должна быть не менее 8 символов");
                return;
            }
            if (log != login.Text)
            {
                DirectoryCopy(Directory.GetCurrentDirectory() + @"\Users\" + log, Directory.GetCurrentDirectory() + @"\Users\" + login.Text.Trim(),true);
                using (BinaryWriter writer = new BinaryWriter(File.Open(Directory.GetCurrentDirectory() + @"\Users\" + login.Text.Trim() + @"\DELETE.del", FileMode.OpenOrCreate)))
                {
                    writer.Write(log);
                }
                File.Delete(Directory.GetCurrentDirectory() + @"\Users\" + login.Text.Trim() + @"\ico.jpg");
                File.Delete(Directory.GetCurrentDirectory() + @"\Users\" + login.Text.Trim() + @"\password.pass");
                File.Copy(image, Directory.GetCurrentDirectory() + @"\Users\" + login.Text.Trim() + @"\ico.jpg");
                using (BinaryWriter writer = new BinaryWriter(File.Open(Directory.GetCurrentDirectory() + @"\Users\" + login.Text.Trim() + @"\password.pass", FileMode.OpenOrCreate)))
                {
                    writer.Write(pass.Text);
                }
                MessageBox.Show("Данные аккаунта обновлены! Необходима повторная авторизация!");
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                File.Delete(Directory.GetCurrentDirectory() + @"\Users\" + login.Text.Trim() + @"\password.pass");
                File.Copy(image, Directory.GetCurrentDirectory() + @"\Users\" + login.Text.Trim() + @"\ico1.jpg");
                using (BinaryWriter writer = new BinaryWriter(File.Open(Directory.GetCurrentDirectory() + @"\Users\" + login.Text.Trim() + @"\password.pass", FileMode.OpenOrCreate)))
                {
                    writer.Write(pass.Text);
                }
                MessageBox.Show("Данные аккаунта обновлены! Необходима повторная авторизация!");
                this.DialogResult = true;
                this.Close();
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = System.IO.Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = System.IO.Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
