using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;
using Point = System.Drawing.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace OpenStore
{
    public partial class MainWindow : Window
    {
        string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\OpenStore\\";
        string[] data;

        public MainWindow()
        {
            InitializeComponent();
            Directory.CreateDirectory(folderPath + "Icons");
            Directory.CreateDirectory(folderPath + "Programs");

            WebClient wc = new WebClient();
            wc.DownloadFile("https://raw.githubusercontent.com/SagMeinenNamen/OpenStore/main/Programs/Data", folderPath + "data");
            data = File.ReadAllLines(folderPath + "data");
            for (int i = 0; i < data.Length; i++)
            {
                if (i % 2 == 0)
                {
                    Grid parentGrid = new Grid();
                    parentGrid.VerticalAlignment = VerticalAlignment.Top;
                    parentGrid.RowDefinitions.Add(new RowDefinition());
                    parentGrid.RowDefinitions.Add(new RowDefinition());
                    for (int k = i; k < i + 2; k++)
                    {
                        if (k < data.Length)
                        {
                            Grid grid = new Grid();
                            for (int j = 0; j < 4; j++)
                            {
                                grid.RowDefinitions.Add(new RowDefinition());
                            }

                            string[] seperatedData = data[k].Split('|');
                            string programName = seperatedData[0];

                            TextBlock text = new TextBlock();
                            text.TextAlignment = TextAlignment.Center;
                            text.VerticalAlignment = VerticalAlignment.Center;
                            text.Foreground = Brushes.White;
                            text.FontSize = 25;
                            text.FontWeight = FontWeights.Bold;
                            text.Text = programName;


                            wc.DownloadFile(seperatedData[1], folderPath + "Icons\\" + programName);
                            Image image = new Image();
                            image.Source = Imaging.CreateBitmapSourceFromHBitmap(new Bitmap(folderPath + "Icons\\" + programName).GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            Button button = new Button();
                            button.Uid = k.ToString();
                            button.Width = 100;
                            button.Content = "Install";
                            button.FontSize = 18;
                            button.Foreground = Brushes.Black;
                            button.Background = Brushes.White;
                            button.FontWeight = FontWeights.Bold;
                            button.BorderThickness = new Thickness(0, 0, 0, 0);
                            button.HorizontalAlignment = HorizontalAlignment.Center;
                            button.VerticalAlignment = VerticalAlignment.Bottom;
                            button.Click += Button_Click;
                            grid.Children.Add(text);
                            grid.Children.Add(image);
                            grid.Children.Add(button);
                            Rectangle rect1 = new Rectangle() { Height = 20, Width = 300 };
                            Rectangle rect2 = new Rectangle() { Height = 50, Width = 300 };
                            grid.Children.Add(rect1);
                            grid.Children.Add(rect2);
                            Grid.SetRow(text, 0);
                            Grid.SetRow(rect1, 1);
                            Grid.SetRow(image, 2);
                            Grid.SetRow(rect2, 3);
                            Grid.SetRow(button, 4);
                            if(parentGrid.Children.Count > 0)
                            {
                                grid.Margin = new Thickness(0, 50, 0, 0);
                            }
                            parentGrid.Children.Add(grid);
                            Grid.SetRow(grid, k - i);
                        }
                    }
                    Programs.Items.Add(parentGrid);
                }
            }   
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)e.Source;
            Programs.IsEnabled = false;
            button.Content = "...";
            UpdateLayout();
            string[] seperatedData = data[Convert.ToInt32(button.Uid)].Split('|');
            WebClient wc = new WebClient();
            wc.DownloadFileCompleted += delegate
            {
                Wc_DownloadFileCompleted(button);
            };
            wc.DownloadFileAsync(new Uri(seperatedData[2]), folderPath + "Programs\\" + seperatedData[0] + ".exe");
        }


        private void Wc_DownloadFileCompleted(Button button)
        {
            Programs.IsEnabled = true;
            button.Content = "Install";
            string[] seperatedData = data[Convert.ToInt32(button.Uid)].Split('|');
            Process.Start(folderPath + "Programs\\" + seperatedData[0] + ".exe");
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            if (e.Delta < 0)
            {
                scrollViewer.LineRight();
                scrollViewer.LineRight();
            }
            else
            {
                scrollViewer.LineLeft();
                scrollViewer.LineLeft();
            }
            e.Handled = true;
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/SagMeinenNamen/");
        }
    }
}
