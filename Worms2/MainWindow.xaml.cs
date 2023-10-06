﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Worms2
{
    public partial class MainWindow : Window
    {
        private const string UniqueEventName = "Worms 2 Launcher by Carlmundo";
        string msgFileNotFound = "File not found: ";
        int forceClose = 0;

        public MainWindow()
        {
            SingleInstance();
            Process[] Process1 = Process.GetProcessesByName("frontend");
            Process[] Process2 = Process.GetProcessesByName("worms2");
            if (Process1.Length > 0 || Process2.Length > 0)
            {
                MessageBox.Show("Worms 2 is already running.");
                forceClose = 1;
                Close();
            }
            else
            {
                string fileIntro = "Intro.wmv";
                if (!File.Exists(fileIntro))
                {
                    MessageBox.Show(msgFileNotFound + fileIntro);
                    Close();
                }

                InitializeComponent();
                VideoPlayer.MediaEnded += OnMediaEnded;
                VideoPlayer.Play();
                this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
            }
        }
        private void SingleInstance()
        {
            try
            {
                EventWaitHandle.OpenExisting(UniqueEventName);
                forceClose = 1;
                Close();
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                new EventWaitHandle(false, EventResetMode.AutoReset, UniqueEventName);
            }
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                VideoPlayer.Position = TimeSpan.MaxValue;
            }
        }
        private void OnMediaEnded(object sender, RoutedEventArgs e)
        {
            var currentVideo = VideoPlayer.Source.ToString();
            if (currentVideo == "Intro.wmv")
            {
                string[] videoList = { "ARMAG", "BANDIT", "BASEBALL", "GRENADE1", "PINGPONG", "TV", "VIDCAM" };
                Random random = new Random();
                var randomVideoIndex = random.Next(0, videoList.Length);
                var randomVideoFile = videoList[randomVideoIndex] + ".wmv";
                if (!File.Exists(randomVideoFile))
                {
                    MessageBox.Show("File not found: "+ randomVideoFile);
                    Close();
                }
                VideoPlayer.Source = new Uri(videoList[randomVideoIndex]+".wmv", UriKind.Relative);
            }
            else
            {
                Close();
            }
            
        }
        private void Main_Closing(object sender, CancelEventArgs e)
        {
            if (forceClose != 1)
            {
                var appLaunch = "frontend.exe";
                if (File.Exists(appLaunch))
                {
                    var processInfo = new ProcessStartInfo("frontend.exe");
                    Process.Start(processInfo);
                }
                else
                {
                    MessageBox.Show(msgFileNotFound + appLaunch);
                }
            }
                
        }
    }
}