using LibVLCSharp.Shared;
using LibVLCSharp.WPF;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Worms2
{
    public partial class MainWindow : Window
    {
        private const string UniqueEventName = "Worms 2 Launcher by Carlmundo";
        public static string fileExtension = ".mp4";
        string msgFileNotFound = "File not found: ";
        int forceClose = 0;
        int videosPlayed = 0;
        int mediaLoaded = 0;
        string fileIntro = "Intro" + fileExtension;

        public static Assembly currentAssembly = Assembly.GetEntryAssembly();
        public static string currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;

        LibVLC _libVLC;
        MediaPlayer _mediaPlayer;
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
                InitializeComponent();        
                if (!File.Exists(fileIntro))
                {
                    MessageBox.Show(msgFileNotFound + fileIntro);
                    Close();
                }
                else
                {
                    videoView.Loaded += VideoView_Loaded;
                    this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
                }
            }
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                OnMediaEnded(this,null);
            }
        }

        private void VideoView_Loaded(object sender, RoutedEventArgs e)
        {
            Core.Initialize();
            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);
            videoView.MediaPlayer = _mediaPlayer;
            _mediaPlayer.Play(new Media(_libVLC, new Uri("file://" + currentDirectory + "\\" + fileIntro)));
            _mediaPlayer.EndReached += OnMediaEnded;
            mediaLoaded = 1;
        }

        private void OnMediaEnded(object sender, EventArgs e)
        {
            videosPlayed++;
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;

            if (videosPlayed == 1)
            {
                string[] videoList = { "ARMAG", "BANDIT", "BASEBALL", "GRENADE1", "PINGPONG", "TV", "VIDCAM" };
                Random random = new Random();
                var randomVideoIndex = random.Next(0, videoList.Length);
                var randomVideoFile = videoList[randomVideoIndex] + fileExtension;
                if (!File.Exists(randomVideoFile))
                {
                    MessageBox.Show("File not found: " + randomVideoFile);
                    Close();
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(_ => _mediaPlayer.Play(new Media(_libVLC, new Uri("file://" + currentDirectory + "\\" + videoList[randomVideoIndex] + fileExtension))));
                }
            }
            else
            {
                Close();
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

        private void Main_Closing(object sender, CancelEventArgs e)
        {
            if (mediaLoaded == 1)
            {
                _mediaPlayer.Stop();
                _mediaPlayer.Dispose();
                _libVLC.Dispose();
            }
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
