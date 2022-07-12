using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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

namespace OpenStoreInstaller
{
    public partial class MainWindow : Window
    {
        readonly string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\OpenStore\\";
        int progress = 0;

        public MainWindow()
        {
            InitializeComponent();
            InstanceAlreadyRunning();
        }

        void Install()
        {
            Directory.CreateDirectory(path);
            if (File.Exists(path + "\\OpenStore.exe"))
            {
                File.Delete(path + "\\OpenStore.exe");
            }
            string URL = "https://github.com/SagMeinenNamen/OpenStore/raw/main/OpenStore.exe";
            try
            {
                try
                {
                    WebClient wc = new WebClient();
                    wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                    wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                    wc.DownloadFileAsync(new Uri(URL), path + "\\OpenStore.exe");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage > progress)
            {
                Dispatcher.BeginInvoke(new Action(() => DownloadProgress.Value = e.ProgressPercentage));
                progress = e.ProgressPercentage;
            }
        }

        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            try 
            {
                Process.Start(path + "\\OpenStore.exe");
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void InstanceAlreadyRunning()
        {
            Process[] processlist = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
            foreach (var p in processlist)
            {
                if (p.Id != Process.GetCurrentProcess().Id)
                {
                    MessageBox.Show("Installation is already in progress.");
                    this.Close();
                }
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Install();
        }
    }
}
