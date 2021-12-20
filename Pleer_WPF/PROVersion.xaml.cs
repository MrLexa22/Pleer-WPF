using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel; //!
using System.Collections.ObjectModel;
using System.IO;
using CefSharp.Wpf;
using System.Timers;
using System.Threading;

namespace Pleer_WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class PROVersion : Window
    {
        public ObservableCollection<PlayList> play_list = new ObservableCollection<PlayList>();
        public ObservableCollection<Music_PlayList> music = new ObservableCollection<Music_PlayList>();
        public ObservableCollection<Music_PlayList> now_playing = new ObservableCollection<Music_PlayList>();
        public string ActivePlayList = "";
        System.Timers.Timer timer;
        public string login = "";

        delegate void TickHandler();
        public PROVersion(string log)
        {
            if(File.Exists(Directory.GetCurrentDirectory() + @"\Users\" + log + @"\ico1.jpg"))
            {
                try
                {
                    File.Delete(Directory.GetCurrentDirectory() + @"\Users\" + log + @"\ico.jpg");
                    File.Move(Directory.GetCurrentDirectory() + @"\Users\" + log + @"\ico1.jpg", Directory.GetCurrentDirectory() + @"\Users\" + log + @"\ico.jpg");
                }
                catch { MessageBox.Show("Некорректное изменение картинки"); }
            }
            if(File.Exists(Directory.GetCurrentDirectory() + @"\Users\" + log + @"\Delete.del"))
            {
                try
                {
                    string te = "";
                    using (BinaryReader reader = new BinaryReader(File.Open(Directory.GetCurrentDirectory() + @"\Users\" + log + @"\Delete.del", FileMode.Open)))
                    {
                        while (reader.PeekChar() > -1)
                        {
                            te = reader.ReadString();
                        }
                    }
                    Directory.Delete(Directory.GetCurrentDirectory() + @"\Users\" + te, true);
                    File.Delete(Directory.GetCurrentDirectory() + @"\Users\" + log + @"\Delete.del");
                }
                catch { }
            }
            InitializeComponent();
            this.ChangeIsPlaying(false);
            login = log;
            string[] allfolders = Directory.GetDirectories(Directory.GetCurrentDirectory() + @"\Users\"+login+@"\");
            foreach (string folder in allfolders)
            {
                play_list.Add(new PlayList() { Name = new DirectoryInfo(folder).Name, Path = folder });
            }
            playlist.ItemsSource = play_list;

            Uri fileUri = new Uri(Directory.GetCurrentDirectory() + @"\Users\" + login + @"\ico.jpg");
            avatat.Source = new BitmapImage(fileUri);
            nik.Text = login;

            if (File.Exists(Directory.GetCurrentDirectory() + @"\Users\" + login + @"\act_theme.th"))
            {
                string fg = "";
                using (BinaryReader reader = new BinaryReader(File.Open(Directory.GetCurrentDirectory() + @"\Users\" + login + @"\act_theme.th", FileMode.Open)))
                {
                    while (reader.PeekChar() > -1)
                    {
                        fg = reader.ReadString();
                    }
                }
                if (fg == "Светлая тема")
                    Theme.SelectedIndex = 0;
                else if (fg == "Тёмная тема")
                    Theme.SelectedIndex = 1;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.LoadedBehavior = MediaState.Play;
        }

        public BitmapImage LoadImage(string text, bool decode)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(text, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            return bitmap;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            mediaElement.LoadedBehavior = MediaState.Stop;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            mediaElement.LoadedBehavior = MediaState.Pause;
        }

        private bool isPlaying = false;

        private void PlayPause_OnClick(object sender, RoutedEventArgs e)
        {
            this.ChangeIsPlaying(!this.isPlaying);
        }

        private void SetMediaSource(string file)
        {
            this.mediaElement.Source = new Uri(file);
            mediaElement.Play();
            ChangeIsPlaying(true);
        }

        private void Stop_OnClick(object sender, RoutedEventArgs e)
        {
            this.ChangeIsPlaying(false);
            this.mediaElement.Stop();
        }

        private void MediaElement_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            this.OnMediaEnded();
        }

        private void MediaElement_OnMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            this.OnMediaEnded();
        }

        private void OnMediaEnded()
        {
            if (ret_treck.IsChecked == true)
            {
                this.mediaElement.Stop();
                this.mediaElement.Play();
            }
            else if (now_playing.Count != 0)
            {
                string[] allfiles = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Users\" + login + @"\" + now_playing[0].PL);
                int i = 0; bool ch = false; bool jh = false; int j = allfiles.Length - 1;
                foreach (string filename in allfiles)
                {
                    if (j == i && new DirectoryInfo(filename).Name == now_playing[0].Name1)
                    {
                        ch = true;
                        break;
                    }
                    else if (jh == true && ch == false)
                    {
                        string pl = now_playing[0].PL;
                        now_playing.Clear();
                        now_playing.Add(new Music_PlayList() { Name1 = new DirectoryInfo(filename).Name, Path1 = filename, PL = pl });
                        tek_playlist.Text = now_playing[0].PL;
                        tek_trek.Text = now_playing[0].Name1;
                        this.SetMediaSource(filename);
                        break;
                    }
                    else if (j != i && new DirectoryInfo(filename).Name == now_playing[0].Name1 && ch == false)
                    {
                        jh = true;
                    }
                    i++;

                }
                if (ch == true)
                {
                    if (ret_playlist.IsChecked == true)
                    {
                        string pl = now_playing[0].PL;
                        now_playing.Clear();
                        now_playing.Add(new Music_PlayList() { Name1 = new DirectoryInfo(music[0].Path1).Name, Path1 = music[0].Path1, PL = pl });
                        tek_playlist.Text = now_playing[0].PL;
                        tek_trek.Text = now_playing[0].Name1;
                        this.SetMediaSource(music[0].Path1);
                    }
                    else
                        this.mediaElement.Stop();
                }
            }

        }

        private void ChangeIsPlaying(bool isPlaying)
        {
            this.isPlaying = isPlaying;

            if (this.isPlaying)
            {
                this.playPause.ChangeToPlayingState();
                this.mediaElement.Play();
            }
            else
            {
                this.playPause.ChangeToPauseState();
                this.mediaElement.Pause();
            }
        }

        private void VolumeSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.mediaElement.Volume = this.volumeSlider.Value;
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            string file = this.OpenFile();
            if (file == null)
            {
                return;
            }

            this.SetMediaSource(file);
        }

        private string OpenFile()
        {
            string filter = "Audio(*.ogg, *.mp3, *.wav)|*.ogg;*.mp3;*.wav;";
            var openDialog = new OpenFileDialog { Multiselect = false, CheckFileExists = true, CheckPathExists = true, Title = "Select video file", AddExtension = true, Filter = filter };
            if (openDialog.ShowDialog(this) == true)
            {
                return openDialog.FileName;
            }
            return null;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            AddPlayListPRO pl = new AddPlayListPRO(login);
            pl.Owner = this;
            pl.ShowDialog();
            playlist.ItemsSource = play_list;
        }


        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (tetu.Text == "")
            {
                MessageBox.Show("Выберите плейлист");
                return;
            }

            string file = this.OpenFile();
            if (file == null)
            {
                return;
            }

            File.Copy(file, Directory.GetCurrentDirectory() + @"\Users\" + login + @"\" + tetu.Text + @"\" + new DirectoryInfo(file).Name, true);

            music.Clear();
            string[] allfiles = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Users\" + login + @"\" + tetu.Text);
            foreach (string filename in allfiles)
            {
                music.Add(new Music_PlayList() { Name1 = new DirectoryInfo(filename).Name, Path1 = filename, PL = tetu.Text });
            }
            Treki.ItemsSource = music;
        }

        private void playlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (playlist.SelectedItem != null)
            {
                PlayList a = (PlayList)playlist.SelectedItem;
                tetu.Text = a.Name;

                music.Clear();
                string[] allfiles = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Users\" + login + @"\" + tetu.Text);
                foreach (string filename in allfiles)
                {
                    music.Add(new Music_PlayList() { Name1 = new DirectoryInfo(filename).Name, Path1 = filename, PL = tetu.Text });
                }
                Treki.ItemsSource = music;
            }
        }

        private void Treki_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Music_PlayList a = (Music_PlayList)Treki.SelectedItem;
            now_playing.Clear();
            now_playing.Add(a);
            tek_playlist.Text = now_playing[0].PL;
            tek_trek.Text = now_playing[0].Name1;
            this.SetMediaSource(a.Path1);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (playlist.SelectedItem != null)
            {
                PlayList a = (PlayList)playlist.SelectedItem;
                if (a.Name == tetu.Text)
                {
                    tetu.Text = "";
                    Treki.ItemsSource = "";
                }
                if (now_playing[0].PL == new DirectoryInfo(a.Path).Name)
                {
                    mediaElement.Stop();
                    mediaElement.Close();
                }
                Directory.Delete(a.Path, true);
                play_list.Remove(a);

                playlist.ItemsSource = play_list;
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (Treki.SelectedItem != null)
            {
                Music_PlayList a = (Music_PlayList)Treki.SelectedItem;
                if (a.Name1 == now_playing[0].Name1)
                {
                    mediaElement.Stop();
                    mediaElement.Close();
                }
                File.Delete(a.Path1);
                music.Remove(a);
                Treki.ItemsSource = music;
            }
        }

        private void Random_Click(object sender, RoutedEventArgs e)
        {
            if (playlist.Items.Count > 0)
            {
                Random rand1 = new Random();

                int lol = rand1.Next(0, playlist.Items.Count);

                PlayList a = (PlayList)playlist.Items[lol];
                tetu.Text = a.Name;

                music.Clear();
                string[] allfiles = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Users\" + login + @"\" + tetu.Text);
                foreach (string filename in allfiles)
                {
                    music.Add(new Music_PlayList() { Name1 = new DirectoryInfo(filename).Name, Path1 = filename, PL = tetu.Text });
                }
                Treki.ItemsSource = music;
                if (Treki.Items.Count > 0)
                {
                    int k = music.Count();
                    Random rand = new Random();
                    int j = rand.Next(0, k);
                    now_playing.Clear();
                    now_playing.Add(music[j]);
                    tek_playlist.Text = now_playing[0].PL;
                    tek_trek.Text = now_playing[0].Name1;
                    this.SetMediaSource(music[j].Path1);
                }
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (tek_playlist.Text != "")
            {
                string[] allfiles = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Users\" + login + @"\" + now_playing[0].PL);
                int i = 0; bool ch = false; bool jh = false; int j = allfiles.Length - 1;
                foreach (string filename in allfiles)
                {
                    if (j == i && new DirectoryInfo(filename).Name == now_playing[0].Name1)
                    {
                        ch = true;
                        break;
                    }
                    else if (jh == true && ch == false)
                    {
                        string pl = now_playing[0].PL;
                        now_playing.Clear();
                        now_playing.Add(new Music_PlayList() { Name1 = new DirectoryInfo(filename).Name, Path1 = filename, PL = pl });
                        tek_playlist.Text = now_playing[0].PL;
                        tek_trek.Text = now_playing[0].Name1;
                        this.SetMediaSource(filename);
                        break;
                    }
                    else if (j != i && new DirectoryInfo(filename).Name == now_playing[0].Name1 && ch == false)
                    {
                        jh = true;
                    }
                    i++;

                }
                if (ch == true)
                {
                    string pl = now_playing[0].PL;
                    now_playing.Clear();
                    now_playing.Add(new Music_PlayList() { Name1 = new DirectoryInfo(music[0].Path1).Name, Path1 = music[0].Path1, PL = pl });
                    tek_playlist.Text = now_playing[0].PL;
                    tek_trek.Text = now_playing[0].Name1;
                    this.SetMediaSource(music[0].Path1);
                }
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            if (tek_playlist.Text != "")
            {
                string[] allfiles = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Users\" + login + @"\" + now_playing[0].PL);
                int i = 0; bool ch = false; bool jh = false; int j = allfiles.Length - 1;
                foreach (string filename in allfiles)
                {
                    if (i == 0 && new DirectoryInfo(filename).Name == now_playing[0].Name1)
                    {
                        ch = true;
                        break;
                    }

                    else if (new DirectoryInfo(filename).Name == now_playing[0].Name1 && ch == false)
                    {
                        int h = 0;
                        foreach (string filename1 in allfiles)
                        {
                            if (h == i - 1)
                            {
                                string pl = now_playing[0].PL;
                                now_playing.Clear();
                                now_playing.Add(new Music_PlayList() { Name1 = new DirectoryInfo(filename1).Name, Path1 = filename1, PL = pl });
                                tek_playlist.Text = now_playing[0].PL;
                                tek_trek.Text = now_playing[0].Name1;
                                this.SetMediaSource(filename1);
                                break;
                            }
                            h++;
                        }
                    }
                    i++;

                }
                if (ch == true)
                {
                    string pl = now_playing[0].PL;
                    now_playing.Clear();
                    now_playing.Add(new Music_PlayList() { Name1 = new DirectoryInfo(music[music.Count - 1].Path1).Name, Path1 = music[music.Count - 1].Path1, PL = pl });
                    tek_playlist.Text = now_playing[0].PL;
                    tek_trek.Text = now_playing[0].Name1;
                    this.SetMediaSource(music[music.Count - 1].Path1);
                }
            }
        }

        private void search_playlist_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (play_list.Count != 0)
            {
                playlist.ItemsSource = "";
                if (string.IsNullOrWhiteSpace(search_playlist.Text))
                {
                    playlist.ItemsSource = play_list;
                }
                else
                {
                    var tt = play_list.Where(s => s.Name.StartsWith(search_playlist.Text)).ToArray();
                    playlist.ItemsSource = tt;
                }
            }
        }

        private void search_trec_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (music.Count != 0)
            {
                Treki.ItemsSource = "";
                if (string.IsNullOrWhiteSpace(search_trec.Text))
                {
                    Treki.ItemsSource = music;
                }
                else
                {
                    var tt = music.Where(s => s.Name1.StartsWith(search_trec.Text)).ToArray();
                    Treki.ItemsSource = tt;
                }
            }
        }

        private void regist_Click(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("Light.xaml", UriKind.Relative);
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
            MainWindow ma = new MainWindow();
            ma.Show();
            mediaElement.Stop();
            this.Close();
        }

        private void vxod_Click(object sender, RoutedEventArgs e)
        {
            Authentication th = new Authentication();
            th.Owner = this;
            var t = th.ShowDialog();
            if (t == true)
                this.Close();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            SettingsUser set = new SettingsUser(login);
            set.Owner = this;
            var t = set.ShowDialog();
            if(t==true)
            {
                mediaElement.Stop();
                var uri = new Uri("Light.xaml", UriKind.Relative);
                ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
                Application.Current.Resources.MergedDictionaries.Add(resourceDict);
                MainWindow ma = new MainWindow();
                ma.Show();
                this.Close();
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem)Theme.SelectedItem;
            if (typeItem.Content != null)
            {
                string style = typeItem.Content.ToString();
                if (style == "Светлая тема")
                {
                    var uri = new Uri("Light.xaml", UriKind.Relative);
                    ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
                    Application.Current.Resources.MergedDictionaries.Add(resourceDict);
                    if (File.Exists(Directory.GetCurrentDirectory() + @"\Users\" + login + @"\act_theme.th"))
                        File.Delete(Directory.GetCurrentDirectory() + @"\Users\" + login + @"\act_theme.th");
                    using (BinaryWriter writer = new BinaryWriter(File.Open(Directory.GetCurrentDirectory() + @"\Users\" + login + @"\act_theme.th", FileMode.OpenOrCreate)))
                    {
                        writer.Write("Светлая тема");
                    }
                }
                else if (style == "Тёмная тема")
                {
                    var uri = new Uri("Dark.xaml", UriKind.Relative);
                    ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
                    Application.Current.Resources.MergedDictionaries.Add(resourceDict);

                    if (File.Exists(Directory.GetCurrentDirectory() + @"\Users\" + login + @"\act_theme.th"))
                        File.Delete(Directory.GetCurrentDirectory() + @"\Users\" + login + @"\act_theme.th");
                    using (BinaryWriter writer = new BinaryWriter(File.Open(Directory.GetCurrentDirectory() + @"\Users\" + login + @"\act_theme.th", FileMode.OpenOrCreate)))
                    {
                        writer.Write("Тёмная тема");
                    }
                }
            }
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            string filter = "Video(*.mp4, *.avi, *.mpeg)|*.mp4; *.avi; *.mpeg";
            string hj = "";
            var openDialog = new OpenFileDialog { Multiselect = false, CheckFileExists = true, CheckPathExists = true, Title = "Select video file", AddExtension = true, Filter = filter };
            if (openDialog.ShowDialog(this) == true)
            {
                hj = openDialog.FileName;
            }
            if (hj != "")
            {
                VideoMusic vid = new VideoMusic(hj);
                mediaElement.Pause();
                vid.ShowDialog();
            }
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            double t = playTimeline.CurrentTimeSpan.TotalSeconds;
            TimeSpan position = new TimeSpan(0, 0, (int)t-10);
            mediaElement.Position = position;
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            double t = playTimeline.CurrentTimeSpan.TotalSeconds;
            TimeSpan position = new TimeSpan(0, 0, (int)t + 10);
            mediaElement.Position = position;
        }
    }
}
