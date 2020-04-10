using Ji.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ji.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdateServerPage : ContentPage
    {
        TestHtmlElementClass A;
        public String HTML = "Локальная переменнвая";
        public UpdateServerPage()
        {
            InitializeComponent();
            //https://xamarinhelp.com/hyperlink-in-xamarin-forms-label/

            //label1.Text = "Локальный текст <a htef='https://194.192.100.194/jinstall.html'>test</a>";
            A = new TestHtmlElementClass() {  HTML= "Для получения последней версии приложения нажмите <a href=\"https://194.192.100.194/jinstall.html\">здесь</a>" };
            //    this.BindingContext = A;
            label1.BindingContext = A;
            //    label1.SetBinding(Label.FormattedTextProperty, "HTML");
         



        }
           
    }
  


}