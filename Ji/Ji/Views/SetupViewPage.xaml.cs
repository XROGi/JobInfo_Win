using Ji.Models;
using Ji.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ji.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetupViewPage : ContentPage
    {
        SetupAppParam viewModel;
     //   public ObservableCollection<string> Items { get; set; }
      public string Text2 { get; set; }
        public SetupViewPage()
        {
            InitializeComponent();

            BindingContext = viewModel = App.setup;// new SetupAppParam();
//            BindingContext = viewModel = new SetupViewModel();

        }
        /*
        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }*/
    }
}
