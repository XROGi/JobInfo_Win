using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ji.Models;
using Ji.Services;
using Ji.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;

namespace Ji.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactPage : ContentPage
    {
        private ContactViewModel contactViewModel;

        public ContactPage(Models.UserChat u)
        {
       
        }

        protected override void OnDisappearing()
        {
            if (App.ddd != null)
                if (App.ddd.SR != null)
                    App.ddd.SR.OnReciveUserParamUpdate -= SR_OnReciveUserParamUpdate;
    //        BindingContext = null;
    //        contactViewModel.Close();
    //        contactViewModel.Item = null;
    //        contactViewModel = null;

        }
        public ContactPage(ContactViewModel _contactViewModel)
        {
            try
            {
                InitializeComponent();
                this.contactViewModel = _contactViewModel;

                BindingContext = contactViewModel.Item;

                if (App.ddd != null)
                    if (App.ddd.SR != null)
                        App.ddd.SR.OnReciveUserParamUpdate += SR_OnReciveUserParamUpdate;

            }
            catch (Exception err)
            {

            }

        }

        private async void Button_Clicked(object sender, EventArgs e)
        {

            try
            {
                //    Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();

                int[] users = new int[] { contactViewModel.Item.UserId };

                int ParentChatId = 0;//SgnId	Name7   Общий чат 8   Приватный чат9   Доска обьявлений10  "Стол заказов"15  Избранное 30  Корпоративный чат32  Папка43  Блокнот
                int TypeChatId = 8;//SgnId	Name7   Общий чат 8   Приватный чат9   Доска обьявлений10  "Стол заказов"15  Избранное 30  Корпоративный чат32  Папка43  Блокнот

               

                if (contactViewModel.Item.PersonalChatId!=null && contactViewModel.Item.PersonalChatId.HasValue)
                {
                    Navigation.PopAsync(true);
                    Navigation.PushAsync(new ChatsListViewPage(contactViewModel.Item.PersonalChatId.Value));
                }
                else
                {
                   
                    App.ddd.OnReciveChatAppend += Ddd_OnReciveChatAppend;
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        DependencyService.Get<IXROGiToast>().ShortAlert("Идёт создание чата...");
                      
                    })
                    ;
                    await App.ddd.SR.Request_Chat_Create(ParentChatId, TypeChatId, contactViewModel.Item.FIO, "Приватный чат для общения 2х человек", users);
                }

                //        
                /*
                
                


                if (e.Item != null)
                {
                    try
                    {
                        GroupChat g = e.Item as GroupChat;
                        //                Navigation.PushAsync(new MessageListViewPage(g));
                        Navigation.PushAsync(new TestListViewPage(g));
                    }
                    catch (Exception err)
                    {

                    }
                }

    */

                //         g.Url = App.ddd.connectInterface.Server_SOAP;
                //JI_WS.Obj[] list = g.Chat_GetList(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, 0);
                //     int res = g.Chat_CreateAndSubscribe(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, -1, 8, Name, "Приватный чат для общения 2х человек", users);
                //    return res;

            }
            catch (Exception err)
            {

            }
          

           
         
        }

        private async void Ddd_OnReciveChatAppend(int chatId, Obj objtemp)
        {
            try
            {
                App.ddd.OnReciveChatAppend -= Ddd_OnReciveChatAppend;
                //       Navigation.PopAsync();

                contactViewModel.Item.PersonalChatId = chatId;
                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>  
  //              MainThread.BeginInvokeOnMainThread(() => 
                {
                    try
                    {
                        //Navigation.PopAsync(true);

                        NavigationPage np = null;
                        if (App.Current.MainPage is NavigationPage)
                            np = (App.Current.MainPage as NavigationPage);

                        if ((App.Current.MainPage as MasterDetailPage).Detail is NavigationPage)
                            np = ((App.Current.MainPage as MasterDetailPage).Detail as NavigationPage);
                        //np.PopAsync(true);
                        NavigationPage npRoot = np.CurrentPage.Parent as NavigationPage;
                        await this.Navigation.PopAsync(true);
                        npRoot.PushAsync(new NavigationPage(new ChatsListViewPage(objtemp.ObjId)));
                        /*
                        if (App.Current.MainPage is NavigationPage)
                            (App.Current.MainPage as NavigationPage)//.PushAsync(new Page2());
                         .PushAsync(new ChatsListViewPage(objtemp.ObjId));
                        if ((App.Current.MainPage as MasterDetailPage).Detail is NavigationPage)
                            ((App.Current.MainPage as MasterDetailPage).Detail as NavigationPage).Navigation//.PushAsync(new page());
                        .PushAsync(new ChatsListViewPage(objtemp.ObjId));
                        */
                        //if (ddd != null)
                        //{
                        //    ddd.Log(errIn);
                        //}
                        //else
                        //{
                        //    if (setup != null)
                        //    {
                        //        if (setup.b_ShowExceptionText)
                        //            DependencyService.Get<IXROGiToast>().ShortAlert(errIn.Message.ToString());
                        //    }
                        //    else
                        //        DependencyService.Get<IXROGiToast>().ShortAlert(errIn.Message.ToString());
                        //}

                    }
                    catch (Exception err)
                    {

                    }
                });


                /*
                var tp = Parent as NavigationPage;
                if ((tp.RootPage as MainTabbedPage) != null)
                {
                    MainTabbedPage mp = tp.RootPage as MainTabbedPage;
                  //  mp.CurrentPage = mp.Children[0];
                    ItemsPage ip = mp.CurrentPage as ItemsPage;
                    if (ip != null)
                    {
                        Navigation.PopAsync();
                        ip.SelectChatAndOpen(chatId);
                    }
                    //int chatid = App.ddd.Chat_CreateAndSubscribe(contactViewModel.Item);
                    //if (chatid > 0)
                    //{
                    //    //  await Navigation.PushAsync(new MessageListViewPage(new MessageViewModel_Obj(u)));
                    //    //    GroupChat g = ev.Parameter as GroupChat;
                    //    //78 ok       Navigation.PushAsync(new MessageListViewPage(chatid));

                    //}



                }*/
            }
            catch (Exception err)
            { 
            }
        }


      

        private async void Button_Clicked_1(object sender, EventArgs e)
        {//https://issue.life/questions/53613490
            try
            {
                string Phone = contactViewModel.Item.GetMobilePhone;
                //      string ffff = (sender as Button).Text;
                if (Phone != "")
                    Xamarin.Forms.Device.OpenUri(new Uri("tel:"+ Phone));
                //PhoneDialer.Open(Phone);
                else
                {
                    await DisplayAlert("Вызов", "Извините, не могу найти подходящий для сотовой связи номер телефона.", "OK");
                }
            }
            catch (ArgumentNullException anEx)
            {
                // Number was null or white space
            }
            catch (FeatureNotSupportedException ex)
            {
                // Phone Dialer is not supported on this device.
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }

        private void Button_Clicked_2(object sender, EventArgs e)
        {

        }

        private void Button_Clicked_3(object sender, EventArgs e)
        {

        }

        private void Button_Clicked_Favorit(object sender, EventArgs e)
        {
  //          contactViewModel.Item.bFavorite = !contactViewModel.Item.bFavorite;
            App.ddd.SR.Request_User_Param_Set(contactViewModel.Item.UserId, "favorite", "change");
            
        }
        

        private void SR_OnReciveUserParamUpdate(int userid, int useridFavorite, string param, string value)
        {
            if (param == "favorite")
            {
                if (useridFavorite == contactViewModel.Item.UserId)
                {
                    if (value == "change")
                    {
                        contactViewModel.Item.bFavorite = !contactViewModel.Item.bFavorite;
                    }
                    if (value == "set")
                    {
                        contactViewModel.Item.bFavorite = true;
                    }
                    if (value == "clear")
                    {
                        contactViewModel.Item.bFavorite = false;
                    }
                }
            }
            
        }

        private void Button_Clicked_NotFavorit(object sender, EventArgs e)
        {
   //         contactViewModel.Item.bFavorite = !contactViewModel.Item.bFavorite;
            App.ddd.SR.Request_User_Param_Set(contactViewModel.Item.UserId, "favorite", "change");
        }

       
    }
}