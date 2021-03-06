﻿using Ji.Models;
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
    public partial class ChatSetupViewPage : ContentPage
    {
        // public ObservableCollection<string> Items { get; set; }

        GroupChat Chat { get; set; }
        //public ChatSetupViewPage(ObjMsg _chat)
        //{
        //    InitializeComponent();
        //    Chat = _chat;
        //    BindingContext = this;
        //    //      Chat.userCreaterObjId
        //    /*Items = new ObservableCollection<string>
        //    {
        //        "Item 1",
        //        "Item 2",
        //        "Item 3",
        //        "Item 4",
        //        "Item 5"
        //    };

        //    MyListView.ItemsSource = Items;
        //    */
        //}
        public ChatSetupViewPage(GroupChat _chat)
        {
            InitializeComponent();
            Chat = _chat;
            
            BindingContext = this;
            id.Text = Chat.ObjId.ToString();
            //      Chat.userCreaterObjId
            /*Items = new ObservableCollection<string>
            {
                "Item 1",
                "Item 2",
                "Item 3",
                "Item 4",
                "Item 5"
            };

            MyListView.ItemsSource = Items;
            */
        }
        

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

       //     await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
