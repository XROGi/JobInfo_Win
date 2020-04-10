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
using JobInfo;
using JobInfo.XROGi_Class;


namespace JI_WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window

    {

        private static ChatDataStore data;
        public MainWindow()
        {
            InitializeComponent();
            if (data == null)
            {
                data = new ChatDataStore();
                data.OnUser_Update += Data_OnUser_Update;
                string Prifix = "XML/";
                data.connections.Add(new JobInfo.XROGi_Class.ConnectInterface {Server_Name = "localhost:53847/"  , Server_SOAP = Prifix + "GetUserInfo.asmx", Server_WS = Prifix + "ChatHandler.ashx", Id = 0 });
                data.connections.Add(new JobInfo.XROGi_Class.ConnectInterface {Server_Name = "jobinfo/"          , Server_SOAP = Prifix + "GetUserInfo.asmx", Server_WS = Prifix + "ChatHandler.ashx", Id = 1 });
                data.connections.Add(new JobInfo.XROGi_Class.ConnectInterface {Server_Name = "ghp-sql/"          , Server_SOAP = Prifix + "GetUserInfo.asmx", Server_WS = Prifix + "ChatHandler.ashx", Id = 2 });
                data.connections.Add(new JobInfo.XROGi_Class.ConnectInterface {Server_Name = "svod-int.ru:777/"  , Server_SOAP = Prifix + "GetUserInfo.asmx", Server_WS = Prifix + "ChatHandler.ashx", Id = 3 });
                data.Selectet_ConnectInterfaceId = 1;
            }
            data.ConnectBegin();
            
        }

        private void Data_OnUser_Update(ChatDataStore sender)
        {
            ChatControl user = new ChatControl();
            user.SetUser("FIO " + DateTime.Now.ToString());
            ChatUserList.Items.Clear();
            ChatUserList.ItemsSource = sender.users ;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           
            /*    TextBox textBox = (TextBox)sender;

            ///      MessageBox.Show(textBox.Text);
            ChatTextBlock d = new ChatTextBlock();
            d.Text = "";
            */
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value as string))
            {
                FlowDocument fd = new FlowDocument();

                string[] text = ((string)value).Split(' ');

                Paragraph p = new Paragraph();
                StringBuilder sb = new StringBuilder();

                //add text and pictures, etc. and return now InlineCollection instead of FlowDocument

                return p.Inlines;
            }
            else
            {
                return new FlowDocument();
            }
        }
    }
}
