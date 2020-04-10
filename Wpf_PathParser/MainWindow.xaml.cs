using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Wpf_PathParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] paths;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GetPath_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.GetEnvironmentVariable("Path");
            //PathTextBox.Text = string.Join("\r\n", path.Split(";"));
            paths = path.Split(";");
            PathList.ItemsSource = paths;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string path = (sender as Button).DataContext.ToString();
            Process.Start("explorer.exe", path); 
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string path = (sender as Button).DataContext.ToString();
            paths = paths.Where(s1 => s1 != path).ToArray();
            PathList.ItemsSource = paths;
        }

        private void SavePath_Click(object sender, RoutedEventArgs e)
        {
            Environment.SetEnvironmentVariable("Path", String.Join(";", paths));
        }
    }
}
