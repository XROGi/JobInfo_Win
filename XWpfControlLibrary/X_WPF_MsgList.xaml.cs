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
    /// Логика взаимодействия для X_WPF_MsgList.xaml
    /// </summary>
    public partial class X_WPF_MsgList : UserControl
    {
        public ListBox Msglist;
        public X_WPF_MsgList()
        {
            InitializeComponent();
            Msglist =  msglist;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
