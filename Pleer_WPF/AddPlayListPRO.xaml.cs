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
    /// Логика взаимодействия для AddPlaylist.xaml
    /// </summary>
    public partial class AddPlayListPRO : Window
    {
        public string log = "";
        public AddPlayListPRO(string login)
        {
            log = login;
            InitializeComponent();
        }

        private void Name_TB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(Name_TB.Text))
            {
                OkB.IsEnabled = true;
            }
            else
            {
                OkB.IsEnabled = false;
            }
        }

        private void CancB_Click(object sender, RoutedEventArgs e)
        {
            PROVersion main = this.Owner as PROVersion;
            main.Opacity = 1;
            this.Close();
        }

        private void OkB_Click(object sender, RoutedEventArgs e)
        {
            PROVersion main = this.Owner as PROVersion;
            string file = Directory.GetCurrentDirectory();
            file += @"\Users\"+log+@"\" + Name_TB.Text;
            try
            {
                if (Directory.Exists(file))
                {
                    MessageBox.Show("Такой плейлист уже существует!");
                    return;
                }
                Directory.CreateDirectory(file);
                main.play_list.Add(new PlayList() { Name = Name_TB.Text, Path = file });
                this.Close();
            }
            catch
            {
                MessageBox.Show("Создание не удалось, введите другое имя!");
            }
        }
    }
}
