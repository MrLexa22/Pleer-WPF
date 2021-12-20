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
    public partial class MainWindow : Window
    {
        public ObservableCollection<PlayList> play_list = new ObservableCollection<PlayList>();
        public ObservableCollection<Music_PlayList> music = new ObservableCollection<Music_PlayList>();
        public ObservableCollection<Music_PlayList> now_playing = new ObservableCollection<Music_PlayList>();
        public string ActivePlayList = "";
        System.Timers.Timer timer;
       
        delegate void TickHandler();
        public MainWindow()
        {
            InitializeComponent();
            //mediaElement.Source = new Uri(@"D:\gachi.mp3");
            this.ChangeIsPlaying(false);

            string[] allfolders = Directory.GetDirectories(Directory.GetCurrentDirectory()+ @"\Unuser\");
            foreach (string folder in allfolders)
            {
                play_list.Add(new PlayList() { Name = new DirectoryInfo(folder).Name, Path = folder });
            }
            playlist.ItemsSource = play_list;
            Random n = new Random();
            int t = n.Next(1, 5);
            Adds.Source = LoadImage(@"pack://application:,,,/Adds/" + t.ToString() + ".jpeg", true);

            timer = new System.Timers.Timer(30000);
            TickHandler tick = test;
            timer.Elapsed += (object sender, ElapsedEventArgs args)
                => Dispatcher.BeginInvoke(tick);
            timer.Start();
        }

        public void test()
        {
                    Random n = new Random();
                    int t = n.Next(1, 5);
                    Adds.Source = LoadImage(@"pack://application:,,,/Adds/" + t.ToString() + ".jpeg", true);
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
            else if (now_playing.Count != 0 && music.Count > 0)
            {
                string[] allfiles = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Unuser\" + now_playing[0].PL);
                int i = 0; bool ch = false; bool jh = false; int j = allfiles.Length-1;
                foreach (string filename in allfiles)
                {
                    if (j==i && new DirectoryInfo(filename).Name == now_playing[0].Name1)
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
                    else if(j!=i && new DirectoryInfo(filename).Name == now_playing[0].Name1 && ch == false)
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
            AddPlaylist pl = new AddPlaylist();
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

            File.Copy(file, Directory.GetCurrentDirectory() + @"\Unuser\"+ tetu.Text+@"\"+ new DirectoryInfo(file).Name, true);

            music.Clear();
            string[] allfiles = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Unuser\" + tetu.Text);
            foreach (string filename in allfiles)
            {
                music.Add(new Music_PlayList() { Name1 = new DirectoryInfo(filename).Name, Path1 = filename,  PL= tetu.Text });
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
                string[] allfiles = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Unuser\" + tetu.Text);
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
            if(playlist.SelectedItem != null)
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
            if(Treki.SelectedItem != null)
            {
                Music_PlayList a = (Music_PlayList)Treki.SelectedItem;
                if(a.Name1 == now_playing[0].Name1)
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
                string[] allfiles = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Unuser\" + tetu.Text);
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
                string[] allfiles = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Unuser\" + now_playing[0].PL);
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
                string[] allfiles = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Unuser\" + now_playing[0].PL);
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
                            if (h == i-1)
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
                    now_playing.Add(new Music_PlayList() { Name1 = new DirectoryInfo(music[music.Count-1].Path1).Name, Path1 = music[music.Count - 1].Path1, PL = pl });
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
            Registration reg = new Registration();
            reg.Owner = this;
            reg.ShowDialog();
        }

        private void vxod_Click(object sender, RoutedEventArgs e)
        {
            Authentication th = new Authentication();
            th.Owner = this;
            var t = th.ShowDialog();
            if (t == true)
            {
                mediaElement.Stop();
                this.Close();
            }
        }
    }

    public class PlayList : INotifyPropertyChanged
    {
        private string name { get; set; }
        private string path { get; set; }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.NotifyPropertyChanged("Name");
                }
            }
        }

        public string Path
        {
            get
            {
                return this.path;
            }
            set
            {
                if (this.path != value)
                {
                    this.path = value;
                    this.NotifyPropertyChanged("Path");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }

    public class Music_PlayList : INotifyPropertyChanged
    {
        private string name1 { get; set; }
        private string path1 { get; set; }
        private string pl { get; set; }

        public string Name1
        {
            get
            {
                return this.name1;
            }
            set
            {
                if (this.name1 != value)
                {
                    this.name1 = value;
                    this.NotifyPropertyChanged("Name1");
                }
            }
        }

        public string Path1
        {
            get
            {
                return this.path1;
            }
            set
            {
                if (this.path1 != value)
                {
                    this.path1 = value;
                    this.NotifyPropertyChanged("Path1");
                }
            }
        }

        public string PL
        {
            get
            {
                return this.pl;
            }
            set
            {
                if (this.pl != value)
                {
                    this.pl = value;
                    this.NotifyPropertyChanged("PL");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
