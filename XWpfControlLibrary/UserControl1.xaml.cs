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
using WpfEmoticons.Controls;

namespace XWpfControlLibrary
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {

       public Button b;
        public TextBoxControl tb;
        public UserControl1()
        {
            InitializeComponent();
            b = b1;

            tb =   tb1;
            tb1.GetPlainText();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b    = sender as Button;
            int Number  = Convert.ToInt32(b.Tag);
            var img = "images/emoticons/" + b.Tag .ToString()+ ".png";
            RichTextBox textBox = tb1.txtTextbox as RichTextBox;
            var reposition = textBox.CaretPosition;

            var ttt =
            new InlineUIContainer
               (new Image()
               {
                   Source = new BitmapImage(new Uri(img, UriKind.RelativeOrAbsolute))
                   ,
                   Width = EmoticonsHelper.SizeImotic
               }
               , textBox.CaretPosition);
            textBox.CaretPosition =  ttt.ContentEnd;
/*
            new InlineUIContainer
               (new Image()
               {
                   Source = new BitmapImage(new Uri(img, UriKind.RelativeOrAbsolute))
                   ,
                   Width = EmoticonsHelper.SizeImotic
               }
               , textBox.CaretPosition);
            */
            /*
            BitmapImage bimg = new BitmapImage();
            bimg.BeginInit();
            bimg.UriSource = new Uri(imageFile, UriKind.Relative);
            bimg.EndInit();
            image.Source = bimg;
            image.Width = SizeImotic;
            */

        }
    }
}
