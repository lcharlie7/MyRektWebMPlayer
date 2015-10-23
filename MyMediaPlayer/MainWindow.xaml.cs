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
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace MyMediaPlayer
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string title = "MyRektWebMPlayer";
        private bool userIsDraggingSlider = false;
        private bool mediaPlayerIsPlaying = false;
        private List<PlaylistItem> items = new List<PlaylistItem>();
        private string currentTitle;
        private bool mediaPlayerIsPaused = false;

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
                getPlaylistInfos(ofd.FileName);
            }
        }

        // Works for video files
        private void getPlaylistInfos(string filename)
        {
            var file = ShellObject.FromParsingName(filename);
            currentTitle = file.Properties.GetProperty(SystemProperties.System.Title).ValueAsObject.ToString();
            string runtime = TimeSpan.FromTicks(Convert.ToInt64(file.Properties.GetProperty(SystemProperties.System.Media.Duration).ValueAsObject.ToString())).ToString(@"hh\:mm\:ss");
            items.Add(new PlaylistItem() { Name = currentTitle, RunTime = runtime });
            playList.ItemsSource = items;
        }

        private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (mediaElement1 != null) && (mediaElement1.Source != null);
        }

        private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mediaElement1.Visibility = Visibility.Visible;
            Title = System.IO.Path.GetFileName(currentTitle) + " - " + title;
            mediaElement1.Play();
            mediaPlayerIsPlaying = true;
            playButton.Visibility = Visibility.Hidden;
            pauseButton.Visibility = Visibility.Visible;
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
            pauseButton.Visibility = Visibility.Hidden;
            playButton.Visibility = Visibility.Visible;
            Title = title;
        }

        private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaPlayerIsPlaying;
        }

        private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mediaElement1.Pause();
            pauseButton.Visibility = Visibility.Hidden;
            playButton.Visibility = Visibility.Visible;
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

        private void progSli_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
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

        private void playlist_Hide(object sender, RoutedEventArgs e)
        {
            playlistPanel.Width = 0;
            playlistSplitter.HorizontalAlignment = HorizontalAlignment.Right;
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            
        }
    }
}
