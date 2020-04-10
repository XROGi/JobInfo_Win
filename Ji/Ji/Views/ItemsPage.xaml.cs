using System;
using System.ComponentModel;

using Xamarin.Forms;

using Ji.Models;
using Ji.ViewModels;
using Ji.Droid;
using System.Linq;
using System.Threading.Tasks;
using Ji.Services;

namespace Ji.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;

        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new ItemsViewModel();
            SetConnectIcon(App.ddd.statusConect);
            App.ddd.OnStatusConectChanged += Ddd_OnStatusConectChanged;
            App.ddd.OnStatusConectChanged += viewModel.OnStatusChanged;
            App.ddd.OnChat_ListRefresh += Ddd_OnChat_ListRefresh;
            App.ddd.OnStatistic_Changed += Ddd_OnStatistic_Changed;
            App.ddd.OnReciveChatUserLeave += Ddd_OnReciveChatUserLeave; ;
            
        }

        private void Ddd_OnReciveChatUserLeave(int ChatId, int userid)
        {
            if (userid == App.ddd.UserId)
            {
                viewModel.LeaveChat(ChatId);
            }
        }

        private void Ddd_OnStatistic_Changed(DateTime oldLastVersion)
        {
            
        }

        private void Ddd_OnChat_ListRefresh(int ChatId)
        {

            try
            {
                viewModel.LoadItemsCommand.Execute(null);
            }
            catch (Exception err)
            {

                App.Log(err);
            }
        }

        private void Chat_OnStatistic_Changed(DateTime oldLastVersion)
        {
            
        }

        
        private void WS_OnWSReciveNewMessage(X_WS sender, string Msg, int ChatId, int NewMsgId)
        {
            foreach (GroupChat t in viewModel.ChatItems.Where(s=>s.ObjId== ChatId))
            {
                GroupChat set = t as GroupChat;
                if (set != null && t.ObjId== ChatId)
                {
                    LoadStatistic(set);
                }
            }
        }

        private void Ddd_OnStatusConectChanged(Droid.StatusConected statusConected)
        {
            SetConnectIcon(statusConected);
        }

        private void SetConnectIcon(StatusConected statusConected)
        {
            try
            {
                if (statusConected == StatusConected.Open)
                {
      //              //StatusConnectIcon.IconImageSource = "logo.png";
      //              //StatusConnectIcon.BindingContext = "";
      //              Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
      //              {
      //         //8         StatusConnectIcon.IconImageSource = null;
      //         //8         StatusConnectIcon.IconImageSource = ImageSource.FromFile("check48.png"); ;
      //                  if (App.ddd.WS != null)
      //                      App.ddd.WS.OnWSReciveNewMessage += WS_OnWSReciveNewMessage;
      //              });
      ////8              DependencyService.Get<IXROGiToast>().ShortAlert("Соединение восстановлено");
                }
                else
                {
             //       Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
             //       {
             //  //8         StatusConnectIcon.IconImageSource = null;
             //  //8         StatusConnectIcon.IconImageSource = ImageSource.FromFile("uncheck48.png"); ;
             //           if (App.ddd.WS != null)
             //               App.ddd.WS.OnWSReciveNewMessage -= WS_OnWSReciveNewMessage;
             //       });
             ////8       DependencyService.Get<IXROGiToast>().ShortAlert("Соединение прервалось");
             //       //StatusConnectIcon.IconImageSource = "xamarin_logo.png";
                }
            }catch (Exception err)
            {

            }
        }

        internal void SelectChatAndOpen(int chatid)
        {
            foreach (GroupChat set in viewModel.ChatItems.Where(s => s.ObjId == chatid))
            {
                if (OpenMessagesInChat(set))
                {
                    return;
                }

            }
            //если чат только созда и его нет в перечне - обновить и так-же открыть
            viewModel.LoadItemsCommand.Execute(null);
            foreach (GroupChat set in viewModel.ChatItems.Where(s => s.ObjId == chatid))
            {
                if (OpenMessagesInChat(set))
                {
                    return;
                }

            }
        }

        private bool  OpenMessagesInChat(GroupChat set)
        {
            //        GroupChat set = t as GroupChat;
            if (set != null //&& set.ObjId == chatid
                )
            {


                //ItemsListView.SelectedItem = set;
                
                {
                    try
                    {

                        Navigation.PushAsync(new MessageListViewPage(set));
                        return true;
                    }
                    catch (Exception err)
                    {

                    }
                }


                // LoadStatistic(set);
            }
            return false;
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Item;
            if (item == null)
                return;

            await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
//            await Navigation.PushAsync(new ContactPage(new ContactViewModel(u)));

            await Navigation.PushAsync((new NewItemPage()));//PushModalAsync
            //NavigationPage.
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            /*
            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);*/
        }

        private void ClearItem_Clicked(object sender, EventArgs e)
        {

        }

        private void OnTapped(object sender, EventArgs e)
        {   //https://stackoverflow.com/questions/30934314/how-to-add-click-event-in-stack-layout-or-frame
            //sender = StackLayout
            //var item = .SelectedItem as MasterPageItem;
            //    if (item != null)

            /*Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
            masterPage.listView.SelectedItem = null;
            IsPresented = false;
            */
            //GroupChat g = e as GroupChat;

            TappedEventArgs ev = e as TappedEventArgs;
            if (ev != null)
            {
                try
                {
                    GroupChat g = ev.Parameter as GroupChat;
                    Navigation.PushAsync(new MessageListViewPage(g));
                }catch (Exception err)
                {

                }
            }

            
            


        }

        private void OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            
            
            
            
            try
            {
                if (e.Item != null)
                {
                    GroupChat chat = e.Item as GroupChat;
                    if (chat != null)
                    {
            //20200325            viewModel.Request_Chat_Statistic(chat); по идее запрашивать теперь не надо.

                        ////не грузим статистику если недавно грузили
                        //if (chat.Statistic!=null)
                        //{
                        //    if ((DateTime.Now- chat.Statistic.dt_Statistic).TotalSeconds<=60)
                        //    {
                        //        return;
                        //    }
                        //}
                        //                        await System.Threading.Tasks.Task.Run(async () => { await LoadStatistic(chat); });
                        // LoadStatistic(chat);

                    }
                }
                 
            }
            catch (Exception err)
            {
                //var t = new ObjMsg() { Data = "<text>Возникла ошибка показа списка. Текст ошибки для анализа:\r\n" + err.Message.ToString() + "</text>" };

                //ItemsListView.ScrollTo(t, ScrollToPosition.Start, false);
            }
            
         



            /*
            try
            {
                var p = e;// as Project.Model.MenuItem;
                if ((p.Item as ObjMsg).Data.ToString() == "Item 44")
                {
                    MyListView.SelectedItem = e.Item;
                }
            }catch (Exception err)
            {

            }*/
        }

        private void LoadStatistic(GroupChat chat)
        {
            try
                 
            {
             //   App.ddd.StatisticChat
                var res = App.ddd.Chat_GetMyStatistic(chat.ObjId);
                //view.OnRefreshObjectExplorerClicked += async (s, e) => await RefreshObjectExplorerAsync();
                if (res != null && res.CountNew > 0)
                {
                    chat.CountNewMessage = res.CountNew.ToString();
                }
                else
                    chat.CountNewMessage = "";

                chat.Statistic = res;

                //async ,???????
           //     App.ddd.Request_GetMyStatistic(chat);


                return;
            }
            catch (Exception err)
            {

            }
        }

        private void OnChatOpen(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            if (mi != null)
            {
                GroupChat g = mi.CommandParameter as GroupChat;

                if (g != null)
                {
                    try
                    {
                        Navigation.PushAsync(new MessageListViewPage(g));
                    }
                    catch (Exception err)
                    {

                    }
                }
            }
        }

        private async void OnChatLeave(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            if (mi != null)
            {
                GroupChat g = mi.CommandParameter as GroupChat;

                if (g != null)
                {
                    try
                    {
                        var result = await DisplayAlert("Покинуть чат", "Вы уверены что хотите покинуть чат?", "Принять", "Отмена");  // since we are using async, we should specify the DisplayAlert as awaiting.
                        if (result == true) // if it's equal to Ok
                        {
                            App.ddd.Chat_Leave(g);
                            viewModel.LoadItemsCommand.Execute(null);

                            //Navigation.PushAsync(new MessageListViewPage(g));
                        }
                        else // if it's equal to Cancel
                        {
                            return; // just return to the page and do nothing.
                        }

                        //if ( await DisplayAlert("Покинуть чат", "Вы уверены что хотите покинуть чат?", "Принять", "Отмена") == true)
                        //{
                        
                        //}
                    }
                    catch (Exception err)
                    {

                    }
                }
            }
        }

        private void ItemsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
 
            
            if (e.Item != null)
            {
                try
                {
                    GroupChat g = e.Item as GroupChat;
       //                Navigation.PushAsync(new MessageListViewPage(g));
                 Navigation.PushAsync(new ChatsListViewPage(g));
                }
                catch (Exception err)
                {

                }
            }
        }

        object lockAutoconnect = new object();
        private void StatusConnectIcon_Clicked(object sender, EventArgs e)
        {
            lock (lockAutoconnect)
            {
              
                try
                {
                    if (App.ddd != null)
                        if (App.ddd.connectInterface != null)
                            if (App.ddd.connectInterface.isSetup() == true)
                            {
                                if (App.ddd.isOpen()==false)
                                {
                                    if (App.ddd.isConnectSetup())
                                    {
                                        App.ddd.Open();
                                        if (App.ddd.isOpen() == false)
                                        {
                                            DependencyService.Get<IXROGiToast>().ShortAlert("Соединение не установлено");
                                        }
                                    }
                                    else
                                    {
                                        DependencyService.Get<IXROGiToast>().ShortAlert("Соединение не инициализировано");
                                    }
                                }
                                else
                                {
                                    DependencyService.Get<IXROGiToast>().ShortAlert("Подключен к серверу");
                                }



                            }
                    else
                            {
                                DependencyService.Get<IXROGiToast>().ShortAlert("Соединение не настроено");
                            }

            //        return true; // True = Repeat again, False = Stop the timer
                }
                catch (Exception err)
                {

                }
            }
         //   return false;
        }

        private void OnChatInfo(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            if (mi != null)
            {
                GroupChat g = mi.CommandParameter as GroupChat;

                if (g != null)
                {
                    try
                    {
                        Navigation.PushAsync(new ChatSetupViewPage(g));
                    }
                    catch (Exception err)
                    {

                    }
                }
            }
        }

        private void OnFilterTextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}