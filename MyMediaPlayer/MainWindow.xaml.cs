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
using System.IO;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;

namespace MyMediaPlayer
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string title = "MyRektWebMPlayer";
        private string mediaName;
        private bool userIsDraggingSlider = false;
        private bool mediaPlayerIsPlaying = false;

        public MainWindow()
        {
            InitializeComponent();

            volume_Handler();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if ((mediaElement1.Source != null) && (mediaElement1.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                progSli.Minimum = 0;
                progSli.Maximum = mediaElement1.NaturalDuration.TimeSpan.TotalSeconds;
                progSli.Value = mediaElement1.Position.TotalSeconds;
            }
        }

        private void volume_Handler()
        {
            soundVolume.Minimum = 0;
            soundVolume.Maximum = 1;
            soundVolume.Value = 0.5;
            mediaElement1.Volume = 0.5;
        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.AddExtension = true;
            ofd.DefaultExt = "*.*";
            ofd.Filter = "All Files (*.*)|*.*";
            if (ofd.ShowDialog() == true)
            {
                mediaElement1.Source = new Uri(ofd.FileName);
                mediaName = mediaElement1.Source.ToString();
            }
        }

        private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (mediaElement1 != null) && (mediaElement1.Source != null);
        }

        private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mediaElement1.Visibility = Visibility.Visible;
            mediaPlayerIsPlaying = true;
            Title = System.IO.Path.GetFileName(mediaElement1.Source.LocalPath) + title;
            mediaElement1.Play();
        }

        private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaPlayerIsPlaying;
        }

        private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mediaElement1.Stop();
            mediaPlayerIsPlaying = false;
            mediaElement1.Visibility = Visibility.Hidden;
            Title = title;
        }

        private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaPlayerIsPlaying;
        }

        private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mediaElement1.Pause();
        }

        private void progSli_DragStarted(object sender, DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        private void progSli_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            labelProgress.Text = TimeSpan.FromSeconds(progSli.Value).ToString(@"hh\:mm\:ss");
        }

        private void progSli_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            mediaElement1.Position = TimeSpan.FromSeconds(progSli.Value);
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (mediaElement1.Volume < 1 && e.Delta > 0)
            {
                mediaElement1.Volume += 0.1;
                soundVolume.Value += 0.1;
            }
            if (mediaElement1.Volume > 0 && e.Delta < 0)
            {
                mediaElement1.Volume += -0.1;
                soundVolume.Value += -0.1;
            }
        }

        private void soundVolume_DragStarted(object sender, RoutedEventArgs e)
        {
            
        }

        private void soundVolume_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            mediaElement1.Volume = soundVolume.Value;
        }

        private void soundVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            soundValue.Text = String.Format("{0:P0}", soundVolume.Value);
        }
    }
}
