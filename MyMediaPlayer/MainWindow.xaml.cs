﻿using Microsoft.Win32;
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
        private string mediaName;
        private bool isPlaying = false;
        private bool userIsDraggingSlider = false;

        public MainWindow()
        {
            InitializeComponent();

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

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.AddExtension = true;
            ofd.DefaultExt = "*.*";
            ofd.Filter = "Media(*.*)|*.*";
            ofd.ShowDialog();
            if (ofd.FileName != "")
            {
                mediaElement1.MediaOpened += new RoutedEventHandler(mediaElement1_MediaOpened);
                mediaElement1.Source = new Uri(ofd.FileName);
                mediaName = mediaElement1.Source.ToString();
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            mediaElement1.Play();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            mediaElement1.Stop();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            mediaElement1.Pause();

        }

        void mediaElement1_MediaOpened(object sender, RoutedEventArgs e)
        {

            Title = System.IO.Path.GetFileName(mediaElement1.Source.LocalPath) + " - MyRektWebMPlayer";
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
    }
}
