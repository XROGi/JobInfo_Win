using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ji.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisconnectedServerInfoPage : ContentPage
    {
        public DisconnectedServerInfoPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                App.ddd.WS.Open();
            }catch (Exception err)
            {

            }
        }
    }
}