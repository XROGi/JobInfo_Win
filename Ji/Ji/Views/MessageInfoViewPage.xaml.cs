using Ji.Models;
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
    public partial class MessageInfoViewPage : ContentPage
    {
        public ObjMsg obj { get; set; }
        public MessageInfoViewPage()
        {
            InitializeComponent();
        } 
        public MessageInfoViewPage(ObjMsg _chat)
        {
            InitializeComponent();
            obj = _chat;

            BindingContext = this;
            id.Text = obj.ObjId.ToString();
        }
    }
}