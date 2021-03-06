﻿using Ji.ClassSR;
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
    public partial class DebugConnectLogPage : ContentPage
    {

        DebugConnectLodViewModel model;

        public DebugConnectLogPage()
        {
            InitializeComponent();
            model = new DebugConnectLodViewModel();
            BindingContext = model ;
            //foreach (var t in App.ddd.Log_ConnecionSR)
            //{
            //    Items.Add(t);
            //};
            //{
            //    "Item 1",
            //    "Item 2",
            //    "Item 3",
            //    "Item 4",
            //    "Item 5"
            //};

            //MyListView.ItemsSource = Items;
        }

       

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
