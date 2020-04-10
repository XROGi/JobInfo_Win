 
using Ji.Models;
using Ji.Services;
using Ji.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Ji.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UsersSelectPage :  ContentPage
    {
        ContactsViewModel viewModel;
        SearchBar searchBar;
        List<int> ListUsersAdd;
        //String Name;
        //String Info;
        private bool b_firstinit = true;
        private GroupChat newGroup;

        public UsersSelectPage(String _Name, String _Info)
        {
            InitializeComponent();
            ListUsersAdd = new List<int>();
            Title = "Добавить людей в группу";
            CreateChat.IsVisible = true;
        }

        bool _b_OnlySelcted;


        public UsersSelectPage(GroupChat _newGroup , bool b_OnlySelcted)
        {
            newGroup = _newGroup; 
        
            Init();
            _b_OnlySelcted = b_OnlySelcted;
       }



        public UsersSelectPage(GroupChat _newGroup)
        {
            newGroup = _newGroup;
            Init();
            CreateChat.IsVisible = true;
        }
        protected override void OnDisappearing()
        {
          
            base.OnDisappearing();

        }

        public void Close()
        {
            try
            {
                if (viewModel != null)

                    viewModel.Close();
            }
            catch (Exception err)
            { }
        }
        private void Init()
        {
            _b_OnlySelcted = false;
            InitializeComponent();
            ListUsersAdd = new List<int>();
            if (newGroup != null)
            {
                int[] users = newGroup.GetUsers();
                if (users!=null)
                ListUsersAdd.AddRange(users);
            }
            Title = "Добавить людей в группу";
            b_firstinit = true;
            CreateChat.IsVisible = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (b_firstinit == true)
            {
                var cont = new ContactsViewModel(ListUsersAdd.ToArray(), _b_OnlySelcted);

                cont.OnLoadUsersCompleat += Cont_OnLoadUsersCompleat;
       //         cont.Items.Where(s => s.Selected == true).ForEach(s => s.Selected = false);
                viewModel = cont;
                BindingContext = viewModel;
                viewModel.Refresh();

            }
            else
            {
                viewModel.Refresh();
            }
        }

        private void Cont_OnLoadUsersCompleat()
        {
     //       AllUsersList.IsVisible = true;
            //viewModel.Items.Where(s => s.Selected == true)
            //    .ForEach(s => s.Selected = false);

            //if (ListUsersAdd.Count > 0)
            //{
            //    //UserChat
            //    //UserChat [] uc =
            //    //                viewModel.Items.Select(s=>s.).All(s => { s; return s; });//.ForEach(s => s.Selected= true);
           
            //    //Select(s => { s.Selected = false; return s; });

            //    viewModel.Items.Where(s => ListUsersAdd.Contains(s.UserId))
            //        .ForEach(s => s.Selected = true);
            //        //.Select(s => { s.Selected = true; return s; });//.ForEach(s => s.Selected= true);

            //}
            //OnPropertyChanged();
        //    AllUsersList.IsVisible = true;
            
            b_firstinit = false;
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // CountAdd.Text = "Добавть всех(" + "0" +")";
        }

        private void MenuItem_Clicked(object sender, EventArgs e)
        {

        }

        private void Add_SelectedUsers(object sender, EventArgs e)
        {
            //AllUsersList.SelectedItem.
        }

        private void OnFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            searchBar = (sender as SearchBar);
            //if (viewModel.DataSource != null)
            {
                //  this.MyListView.BindingContext.Filter = FilterContacts;
                this.AllUsersList.ItemsSource = filterText(); ;

            }

        }
        private IEnumerable filterText()
        {
            return viewModel.Items.Where(s => s.FIO.ToLower().Contains(searchBar.Text.ToLower()));//Count 
        }

        private void AddItem_Clicked(object sender, EventArgs e)
        {

        }

        private void Switch_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            if (b_firstinit) {
                return; 
            }

            ViewCell cell = (sender as Switch).Parent.Parent.Parent as ViewCell;
            UserChat u = ((UserChat)cell.BindingContext);
            int Id = u.UserId;

            if (e.Value == true)
            {
             //   if (u.Selected != true)
                    AddUser(Id);
            }
            else
            {
              //  if (u.Selected == true)
                    DeleteUser(Id);
            }
            ToolCountSelected.Text = ListUsersAdd.Count.ToString();
        }

        private void DeleteUser(int id)
        {
            if (ListUsersAdd.Where(s => s == id).Any())
            {
                App.ddd.Chat_UnSubscribeUser(newGroup, id);
                ListUsersAdd.Remove(id);
                newGroup.UserList = ListUsersAdd.ToArray();// чтоб не перезапрашивать с сервера
            }
        }

        private void AddUser(int id)
        {

            if (ListUsersAdd.Where(s => s == id).Any() == false)
            {

                App.ddd.Chat_SubscribeUser(newGroup, id);
                ListUsersAdd.Add(id);
                newGroup.UserList = ListUsersAdd.ToArray();// чтоб не перезапрашивать с сервера
            }

        }

        [Obsolete]
        private  async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                try
                {

                    CreateChat.IsEnabled = false;
                    AddUser(App.ddd.SR.UserId);

                    newGroup.Users_SetRange(ListUsersAdd.ToArray());

                    if (newGroup.GetUsers()?.Length > 1)
                    {

                        //         Navigation.NavigationStack.cl.NavigationStack.Count
                        //int chatid = 
                            App.ddd.Chat_Create_Public(-1, newGroup.Text, newGroup.Description, newGroup.GetUsers());

                        CreateChat.Text = "Создано!!!";

                        DependencyService.Get<IXROGiToast>().ShortAlert("Создан новый чат");
                        //var ttttt = (((App.Current.MainPage as SvodInfMasterDetailPage).Detail as NavigationPage).Parent as MainTabbedPage).Pages[0];
                        //var tp = (Parent as NavigationPage).RootPage as NewItemPage;
                        //var mn = (tp.Parent as NavigationPage).RootPage as MainTabbedPage;
                        //            if ((tp.RootPage.Parent.Parent.Parent as SvodInfMasterDetailPage) != null)
                        {
                            //MainTabbedPage mp = tp.RootPage as MainTabbedPage;
                            //mp.CurrentPage = mp.Children[0];
                            //ItemsPage ip = mp.CurrentPage as ItemsPage;
                            //if (ip != null)
                            {
                                Navigation.PopToRootAsync();
                                /*  for (var counter = Navigation.NavigationStack.Count; counter > 0; counter--)
                                   {
                                       if (Xamarin.Forms.Device.OS == TargetPlatform.Android)
                                       {
                                           Navigation.RemovePage(Navigation.NavigationStack[counter-1]);
                                       }
                                       else
                                       {
                                           Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                                       }
                                   }


                                  Navigation.PopAsync();*/
                                //                            Navigation.RemovePage(this);

                                //         ip.SelectChatAndOpen(chatid);
                            }

                        }



                    }
                    else
                    {
                        DependencyService.Get<IXROGiToast>().ShortAlert("Необходимо добавить хотя бы одного человека в чат");
                    }
                }
                catch (Exception err)
                {

                }
            }
            finally
            {
                newGroup.Users_SetRange(null);
                CreateChat.IsEnabled = true;
            }

        }
    }
}