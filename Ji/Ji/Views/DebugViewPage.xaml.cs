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
    public partial class DebugViewPage : ContentPage
    {
//        public ObservableCollection<Log_DataSting> Items { get; set; }
        public ObservableCollection<string> Items { get; set; }

        public DebugViewPage()
        {
            try
            {
                InitializeComponent();

            
                
               // Items.Add("fgfgfgf");
/*

                MyListView.ItemsSource = Items;*/
                BindingContext =  new LodDataString_ViewModel();// new SetupAppParam();
            }
            catch (Exception err)
            {
            }
        }

       
    }
}
