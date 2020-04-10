using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
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
    public partial class MessagePopupMenuView : PopupPage
    {
        public MessagePopupMenuView()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync(true);
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item.ToString() == "Ответить")
            {
                PopupNavigation.Instance.PopAsync(true);

            }
            if (e.Item.ToString() == "Информация")
            {
                //    App.Current.MainPage.Navigation.PushAsync(new MessageTabbedPage());
         //       PopupNavigation.Instance.PushAsync(new MessageTabbedPage());
                PopupNavigation.Instance.PopAsync(true);
            }

        }
    }
}