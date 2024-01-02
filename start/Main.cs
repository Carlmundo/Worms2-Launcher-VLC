using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace start
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            this.Hide();
            string videoPlayer = "VideoPlayer\\intro.exe";
            if (File.Exists(videoPlayer))
            {
                Process.Start(videoPlayer);
            }
            else
            {
                var appLaunch = "frontend.exe";
                if (File.Exists(appLaunch))
                {
                    Process.Start(appLaunch);
                }
                else
                {
                    MessageBox.Show("File not found: " + appLaunch);
                }
            }
            Close();
        }
    }
}
