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
using System.Windows.Shapes;

namespace Pleer_WPF
{
    /// <summary>
    /// Логика взаимодействия для VideoMusic.xaml
    /// </summary>
    public partial class VideoMusic : Window
    {
        public VideoMusic(string file)
        {
            InitializeComponent();
            mediaElement.Source = new Uri(file);
            isPlaying = true;
            mediaElement.Play();
        }

        private void VolumeSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.mediaElement.Volume = this.volumeSlider.Value;
        }

        private void PlayPause_OnClick(object sender, RoutedEventArgs e)
        {
            this.ChangeIsPlaying(!this.isPlaying);
        }

        private bool isPlaying = false;

        private void ChangeIsPlaying(bool isPlaying)
        {
            this.isPlaying = isPlaying;

            if (this.isPlaying)
            {
                isPlaying = !isPlaying;
                this.mediaElement.Play();
                playPause.Content = "Pause";
            }
            else
            {
                isPlaying = !isPlaying;
                playPause.Content = "Play";
                this.mediaElement.Pause();
            }
        }

        private void Stop_OnClick(object sender, RoutedEventArgs e)
        {
            this.ChangeIsPlaying(false);
            this.mediaElement.Stop();
        }

        private void MediaElement_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            
        }

        private void MediaElement_OnMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            mediaElement.Stop();
        }
    }
}
