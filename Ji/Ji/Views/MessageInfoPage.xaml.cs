using Ji.Models;
using Ji.ViewModels;
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
    public partial class MessageInfoPage : ContentPage
    {
        MessageInfoViewModel model ;
        public MessageInfoPage(ObjMsg _msg)
        {
            InitializeComponent();
            BindingContext = model = new MessageInfoViewModel(_msg);
        }
    }
}