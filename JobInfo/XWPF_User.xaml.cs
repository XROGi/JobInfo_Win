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

namespace JobInfo
{
    /// <summary>
    /// Логика взаимодействия для XWPF_User.xaml
    /// </summary>
    public partial class XWPF_User : UserControl
    {
        public List<Test_Class> Customers { get; set; }
        public XWPF_User()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
