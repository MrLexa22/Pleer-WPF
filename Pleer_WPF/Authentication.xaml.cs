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
    /// Логика взаимодействия для Authentication.xaml
    /// </summary>
    public partial class Authentication : Window
    {
        public Authentication()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(login.Text == "" || pass.Password == "")
            {
                MessageBox.Show("Введите логин/пароль!");
                return;
            }
            else if(!(Directory.Exists(Directory.GetCurrentDirectory() + @"\Users\" + login.Text)))
            {
                MessageBox.Show("Логин не найден!");
                return;
            }
            string pasw = "";
            if (File.Exists((Directory.GetCurrentDirectory() + @"\Users\" + login.Text + @"\password.pass")))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(Directory.GetCurrentDirectory() + @"\Users\" + login.Text + @"\password.pass", FileMode.Open)))
                {
                    while (reader.PeekChar() > -1)
                    {
                        pasw = reader.ReadString();
                    }
                }
                if (pass.Password != pasw)
                {
                    MessageBox.Show("Неккоректный пароль!");
                    return;
                }
                MessageBox.Show("Вы успешно авторизовались");
                this.DialogResult = true;
                PROVersion pro = new PROVersion(login.Text);
                pro.Show();
                this.Close();
            }
            else
                MessageBox.Show("Данный аккаунт изменён/удалён");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
