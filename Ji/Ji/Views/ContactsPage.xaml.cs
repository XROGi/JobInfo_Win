using Ji.Models;
using Ji.ViewModels;
using System;
using System.Collections;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Reflection;

namespace Ji.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactsPage : ContentPage
    {
        //public ObservableCollection<UserChat> Items { get; set; }
        //public Command LoadItemsCommand { get; set; }

        ContactsViewModel viewModel;
        string Filter;
        SearchBar searchBar;
        public ContactsPage(string _Filter)
        {
            InitializeComponent();
            Filter = _Filter;
            BindingContext = viewModel = new ContactsViewModel(Filter);

            filterTextBox.IsVisible = false; 
    
            //try
            //{
            //    XROGi.Source = new UriImageSource { CachingEnabled = false, Uri = 
            //        new Uri("https://dotnet.microsoft.com/static/images/xamarin/ios-apps.png?v=oKJVTNo5vj0-vQ1WpDeV4GZMoZ2FyGmU7KHELwwDKNk")  
            //    };
            //    //"http://194.190.100.194/xml/img/img1.png"
            //}
            //catch (Exception err)
            //{

            //}
            //     FFImageLoading.ImageService.Instance.
            //.Forms.CachedImage. .CachedImageEvents
        }

        private void SR_OnReciveUserParamUpdate(int userid, int useridFavorite, string param, string value)
        {
            try
            {
                LoadUsers();
            }
            catch (Exception err)
            {
            }
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                if (e.Item == null)
                    return;

                //     await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

                //UserChat u = e.Item as UserChat;
                //await Navigation.PushAsync(new ContactPage(new ContactViewModel(u)));

                UserChat u = e.Item as UserChat;
                ContactViewModel ddsd = new ContactViewModel(u);
                ContactPage cp = new ContactPage(ddsd);
                Navigation.PushAsync(cp);


                //Deselect Item
                ((ListView)sender).SelectedItem = null;
            }
            catch (Exception err)
            {

            }
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {

            /**/
            //   await Navigation.PopModalAsync();
            //async void Cancel_Clicked(object sender, EventArgs e)
            //{
            //    
            //}
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }
        protected override void OnDisappearing()
        {
          
            return;
            App.ddd.OnStatusConectChanged -= Ddd_OnStatusConectChanged;
            if (App.ddd != null)
            {
                App.ddd.OnStatusConectChanged -= Ddd_OnStatusConectChanged;
                if (App.ddd.SR != null)
                    App.ddd.SR.OnReciveUserParamUpdate -= SR_OnReciveUserParamUpdate;
            }

            base.OnDisappearing();

        }
        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                if (App.ddd.SR.IsConnected)
                {
                    if (viewModel.Items.Count == 0)
                        viewModel.LoadItemsCommand.Execute(null);
                }
                else
                {
                    if (b_FirstLoad == true)
                    {
                        App.ddd.OnStatusConectChanged += Ddd_OnStatusConectChanged;
                        App.ddd.SR.OnReciveUserParamUpdate += SR_OnReciveUserParamUpdate; 

                        b_FirstLoad = false;
                    }
                          
                }
            }
            catch (Exception err)
            {
            }
        
        }

        bool b_FirstLoad = true;
        private void Ddd_OnStatusConectChanged(Droid.StatusConected statusConected)
        {
            try
            {
                if (viewModel.Items.Count == 0)
                    viewModel.LoadItemsCommand.Execute(null);

                if (b_FirstLoad)
                {
                    if (App.ddd != null)
                        if (App.ddd.SR != null)
                        {
                            App.ddd.SR.OnReciveUserParamUpdate += SR_OnReciveUserParamUpdate;
                            b_FirstLoad = false;
                        }

                }
            }
            catch (Exception err)
            {
            }
        }

        private void SR_OnWSConnected(X_SignalR sender)
        {
           
        }

        private void OnTapped(object sender, EventArgs e)
        {
            TappedEventArgs ev = e as TappedEventArgs;
            if (ev != null)
            {

                UserChat u = ev.Parameter as UserChat;
                //App.ddd.Chat_OpenOrCreate();
            }
        }

        private void OnMore(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            if (mi != null)
            {
                try
                {
                    UserChat u = mi.CommandParameter as UserChat;
                    ContactViewModel ddsd = new ContactViewModel(u);
                    ContactPage cp = new ContactPage(ddsd);
                    Navigation.PushAsync(cp);
                }
                catch (Exception err)
                {
                }

            }
        }

        private void OnDelete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            DisplayAlert("Delete Context Action", mi.CommandParameter + " delete context action", "OK");

        }

        private void OnOpenChat(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            if (mi != null)
            {
                UserChat u = mi.CommandParameter as UserChat;


                int chatid = App.ddd.Chat_CreateAndSubscribe(u);
                if (chatid > 0)
                {
                    //  await Navigation.PushAsync(new MessageListViewPage(new MessageViewModel_Obj(u)));
                    //    GroupChat g = ev.Parameter as GroupChat;
                    Navigation.PushAsync(new MessageListViewPage(chatid));

                }
            }
            ////Deselect Item
            //((ListView)sender).SelectedItem = null;
        }

        private void OnFilterTextChanged(object sender, TextChangedEventArgs e)
        {
             searchBar = (sender as SearchBar);
            //if (viewModel.DataSource != null)
            {
                //  this.MyListView.BindingContext.Filter = FilterContacts;
                this.MyListView.ItemsSource = filterText(); ;

            }


        }

        private IEnumerable filterText()
        {
            return viewModel.Items.Where(s => s.FIO.ToLower().Contains(searchBar.Text.ToLower()));//Count 
        }

        private void SearchIcon_Clicked(object sender, EventArgs e)
        {
            filterTextBox.IsVisible = !filterTextBox.IsVisible ;
        }

        private void CachedImage_Success(object sender, FFImageLoading.Forms.CachedImageEvents.SuccessEventArgs e)
        {
            string d = FFImageLoading.ImageService.Instance.ToString();
        //public ImageSource Avatar
        //{
        //    get
        //    {
        //        return ImageSource.FromStream(() =>
        //        {
        //            return new MemoryStream(this.User.Avatar);
        //        });
        //    }
        }

        internal void LoadUsers()
        {
            viewModel.Refresh();
        }
    }
}
