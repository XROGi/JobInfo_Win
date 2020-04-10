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

namespace XWpfControlLibrary
{
    /// <summary>
    /// Логика взаимодействия для X_WPF_Msg.xaml
    /// </summary>
    public partial class X_WPF_Msg : UserControl
    {
        public X_WPF_Msg()
        {
            InitializeComponent();
        }
        public X_WPF_Msg(String _FiO)
        {
            InitializeComponent();
            FIO.Content = _FiO;

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            FIO.Content = DateTime.Now.ToString();
            tb.Content = "Проба :) !!5!";
    //        tb.Content="";
                  ;
        }
    }
}
