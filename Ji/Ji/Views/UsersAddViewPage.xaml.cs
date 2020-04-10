using Ji.Models;
using Ji.ViewModels;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ji.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UsersAddViewPage : ContentPage
    {
        UserChat[] selected_UserChat;
        SearchBar searchBar;
        ContactsViewModel viewModel;
        public UsersAddViewPage(Models.GroupChat _newGroup)
        {
            InitializeComponent();
            selected_UserChat = null;
            var cont = new ContactsViewModel("All");
            viewModel = cont;
            BindingContext = viewModel;
            viewModel.Refresh();
        }
        public UserChat[] Selected_UserChat
        {
            get { return selected_UserChat; }
            set { selected_UserChat = value; }
        }
        private IEnumerable filterText()
        {
            return viewModel.Items.Where(s => s.FIO.ToLower().Contains(searchBar.Text.ToLower()));//Count 
        }
        public void OnFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            searchBar = (sender as SearchBar);
            //if (viewModel.DataSource != null)
            {
                //  this.MyListView.BindingContext.Filter = FilterContacts;
                this.AllUsersList.ItemsSource = filterText(); ;

            }
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        private void Switch_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {

        }
        public delegate void OnUsersSelectedDelegate(UserChat[] selected_UserChat);
        public event OnUsersSelectedDelegate OnUsersSelected /* = delegate { }*/;

     

        private void ReturnUsersSelected(UserChat[] selected_UserChat)
        {
            if (OnUsersSelected != null)
            {
                OnUsersSelected(selected_UserChat);

            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            selected_UserChat = viewModel.GetSelected();
            ReturnUsersSelected(selected_UserChat);
             //BindingContext = selected_UserChat.ToList();
            await Navigation.PopAsync();
        }
    }
}
