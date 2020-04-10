using Ji.Models;
using Ji.Services;
using Ji.ViewModels;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ji.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatsListViewPage : ContentPage
    {
        //   public ObservableCollection<ObjMsg> Items { get; set; }

        MessageListViewModel model;
        static ObjMsg lastShown = null;

        protected override void OnDisappearing()
        {
            try
            {
                if (model!=null)
                    model.Close();
                base.OnDisappearing();
            }
            catch (Exception err)
            { 
            }
        }
        public ChatsListViewPage(int _ChatId)
        {
            InitializeComponent();
            RequestChatById(_ChatId);
        }

     
        public ChatsListViewPage(Models.GroupChat g)
        {
            InitializeComponent();
            InitFromGroupChat(g);
        }

        private void InitFromGroupChat(GroupChat g)
        {
            //   Items =  new ObservableCollection<ObjMsg>();
            BindingContext = model = new MessageListViewModel(g);
            Init(g);
            //   MyListView.ItemsSource = Items;


            Title = g.Text;

            ((MessageListViewModel)this.BindingContext).OnArticoloScannerizzato = ((obj) =>
            { //We tell the action to scroll to the passed in object here
                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        await Task.Delay(700);
                        //                    MyListView.ScrollTo(obj, ScrollToPosition.End, true);
                        if (obj != null)
                        {
                            MyListView.ScrollTo(obj, ScrollToPosition.MakeVisible, false);
                        }
                        else
                        {
                        }

                    }
                    catch (Exception err)
                    {
                    }
                });

            });
            /*
            ((MessageListViewModel)this.BindingContext).OnScrollToObjId = ((objid) =>
            { //We tell the action to scroll to the passed in object here
                try
                {
                    //                    MyListView.ScrollTo(obj, ScrollToPosition.End, true);
                    MyListView.ScrollTo(objid, ScrollToPosition.End, false);
                }
                catch (Exception err)
                {
                }

            });
            */
            ((MessageListViewModel)this.BindingContext).OnBeginRefresh = ((obj) =>
            { //We tell the action to scroll to the passed in object here

                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        MyListView.BeginRefresh();
                    }
                    catch (Exception err)
                    {
                    }
                });



            });
            ((MessageListViewModel)this.BindingContext).OnEndRefresh = ((obj) =>
            { //We tell the action to scroll to the passed in object here

                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        MyListView.EndRefresh();
                    }
                    catch (Exception err)
                    {
                    }
                });
            });
        }

        int _inChatId;
        private void RequestChatById(int chatId)
        {
            GroupChat g = null; _inChatId = chatId;

            App.ddd.OnReciveChatList += Ddd_OnReciveChatList;
            App.ddd?.SR?.Request_Chat_List(_inChatId);
      
            if (g != null)
            {
                InitFromGroupChat(g);
            }  
          

        }

        private void Ddd_OnReciveChatList(Obj[] chatlist)
        {
            foreach (Obj chat in chatlist)
            {
                if (chat.ObjId == _inChatId)
                {
                    App.ddd.OnReciveChatList -= Ddd_OnReciveChatList;
                    GroupChat  g = new GroupChat(chat);
                    //isBusyLoadPage = false;
                    
                    InitFromGroupChat(g);
                    break;
                }
            }
        }

        private void Init(GroupChat g)
        {
            model.chat = g;

            //     model.pc = new ChatPagesController(model.chat.ObjId);


            /*Pages = new System.Collections.Generic.SortedList<int, int>();


            App.ddd.Chat_Select(chat.ObjId);//
            ChatPages[] pages = App.ddd.DB_MessageGetPages(chat.ObjId);
            foreach (ChatPages page in pages)
            {
                Pages.Add(page.PageNumber, page.PageNumber);
            }
            */
         //   ShowChat(model.chat.ObjId);

         //   App.ddd.WS.OnWSReciveNewMessage += WS_OnWSReciveNewMessage;


            buttonFoto.Clicked += async (sender, args) =>
            {
                try
                {

                    await CrossMedia.Current.Initialize();

                    if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                    {
                        await DisplayAlert("No Camera", ":( No camera available.", "OK");
                        return;
                    }
                    var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        //Directory = "Test",
                        //  SaveToAlbum = true,
                        CompressionQuality = 75,
                        CustomPhotoSize = 50,
                        PhotoSize = PhotoSize.MaxWidthHeight,
                        MaxWidthHeight = 2000,
                        DefaultCamera = CameraDevice.Rear
                    });



                    if (file == null)
                        return;
                    Stream stream = file.GetStream();
                    byte[] foto = new byte[stream.Length];
                    stream.Read(foto, 0, (int)stream.Length);

                    App.ddd.Message_Send(model.chat.ObjId, "", foto);
                    await model.Request_Message_Send("<text>Изображение добавлено из мобильной версии.</text>").ConfigureAwait(false);

                    //await DisplayAlert("File Location", file.Path, "OK");

                    //Image image = new Image();
                    //image.Source = ImageSource.FromStream(() =>
                    //{
                    //    Stream stream = file.GetStream();
                    //    return stream;
                    //});
                }
                catch (Exception err)
                {

                }
            };

        }

        //private void WS_OnWSReciveNewMessage(X_WS sender, string Msg, int ChatId, int NewMsgId)
        //{
        //    try
        //    {
        //        // просто комманда с
        //        if (ChatId != model.chat.ObjId) return;

        //        ChatStatisticUser stat = App.ddd.Chat_GetMyStatistic(ChatId);
        //        if (stat != null)
        //            model.chat.Statistic = stat;
        //        model.pc.Chat_GetPage(model.chat.Statistic.PageLastObj);
        //        //pc.SetMaxPageNum(stat.PageLastObj);
        //        //   foreach (var t in Items.Reverse())
        //        int stepError = 0;
        //        try
        //        {

        //            stepError = 1;
        //            //    t.Data = rec++.ToString();
        //            lastShown = model.ItemsMsgs[model.ItemsMsgs.Count - 1];//.Where(s => s.ObjId == NewMsgId).Last();//.SingleOrDefault();
        //            stepError = 2;
        //            if (lastShown.userid == App.ddd.UserId)
        //            {
        //                stepError = 3;
        //                if (lastShown != null)
        //                {
        //                    stepError = 4;
        //                    Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
        //                    {
        //                        try
        //                        {
        //                          //  await Task.Delay(700);
        //                            //        ObservableCollection<ObjMsg> os = MyListView.ItemsSource as ObservableCollection<ObjMsg>;
        //                            //        ObjMsg last = os.Last();
        //                            var lastShown = model.ItemsMsgs[model.ItemsMsgs.Count - 1];//.Where(s => s.ObjId == NewMsgId).Last();//.SingleOrDefault();
        //                            MyListView.ScrollTo(lastShown, ScrollToPosition.MakeVisible, true); //Android.Util.AndroidRuntimeException: Only the original thread that created a view hierarchy can touch its views.
        //                    }
        //                        catch (Exception err)
        //                        {

        //                        }

        //                    }


        //                        );
        //                    stepError = 5;
        //                }
        //                stepError = 6;
        //            }
        //            stepError = 7;
        //        }
        //        catch (Exception err)
        //        {

        //        }
        //    }
        //    catch (Exception err)
        //    {

        //    }
        //    //            UpdateLasMessage(ChatId,NewMsgId);
        //}

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

      //      await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;

            string value = await DisplayActionSheet("Сообщение", "Отмена", null, "О сообщении", "Создать задачу", "Переслать..");
            if (value == "О сообщении")
            {
                //var mi = ((MenuItem)sender);
                //if (mi != null)
                {
                    ObjMsg g = e.Item as ObjMsg;

                    if (g != null)
                    {
                        try
                        {
                            Navigation.PushAsync(new MessageInfoViewPage(g));
                        }
                        catch (Exception err)
                        {

                        }
                    }
                }
                //DisplayAlert(value, "Извините, функция заканчивает тестирование","OK");
            }
            if (value == "Создать задачу")
            {
                await Navigation.PushAsync(new JobCreatePage(e.Item as ObjMsg));   
         //       DisplayAlert(value, "Извините, функция заканчивает тестирование", "OK");
            }
            if (value == "Переслать..")
            {
                DisplayAlert(value, "Извините, функция заканчивает тестирование", "OK");
            }
        }

        private void ShowChat(int chatid)
        {
            try
            {

                //   chat = g;
                if (model.chat == null)
                {
                    throw new Exception("Не выбран чат");
                }
                Title = model.chat.Text;
                ChatStatisticUser stat = App.ddd.Chat_GetMyStatistic(chatid);
                if (stat != null)
                    model.chat.Statistic = stat;
                model.pc.SetMaxPageNum(model.chat.Statistic.PageLastObj);
                // Items = new ObservableCollection<string>(); 
                //        model.ItemsMsgs = null;
                //        model.ItemsMsgs = new ObservableCollection<ObjMsg>();

                model.pc.Chat_GetPage(model.chat.Statistic.PageLastShownObj);
                model.nPage = model.chat.Statistic.PageLastShownObj;


                ObjMsg lastShown = model.ItemsMsgs.Where(s => s.ObjId == model.chat.Statistic.LastShownObjId).FirstOrDefault();

                if (lastShown != null)
                {


                    Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                    {
                        try
                        {
                            await Task.Delay(300);
                            //           MyListView.ScrollTo(lastShown, ScrollToPosition.Center, false);
                        }
                        catch (Exception err)
                        {

                        }
                    }
                            );
                }



            }
            catch (Exception err)
            {

            }
        }

        ObjMsg FirstShownObj=null;

        int _lastItemAppearedIdx ;
        enum Direction { Down,Up };
        Direction ListDirection = Direction.Down;
        private async void OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            try
            {
                if (model.b_addMode == true)
                    return;
                try
                {

                    int currentIdx = model.ItemsMsgs.IndexOf((ObjMsg)e.Item);
                    if (currentIdx > _lastItemAppearedIdx)
                        ListDirection = Direction.Up;
                    else
                        ListDirection = Direction.Down;

                    //if (ListDirection == Direction.Up)
                    //{
                    //    if (model.b_NeedChangeCursor == false)
                    //    {
                    //        model.bVisibleCountNewMessage = false;
                    //    }

                    //}
                    //else
                    //    model.bVisibleCountNewMessage = true;

                    model.bVisibleCountNewMessage = false;

                    //int ListViewItemID = MyListView.ItemsSource..IndexOf(e.Item);
                    if (e.Item is ObjMsg msgs)
                    {
                        if (msgs.ShownState != 14)
                        {
                            await model.Request_Message_Shown(msgs);
                            BtnMoveToLastShown.Text = model.chat.Statistic.CountNew.ToString();
                        }

                        //b_addMode = true;
                        if (e.ItemIndex == 0)
                        {
                            FirstShownObj = msgs;
                            model.Page_Start = msgs.PageNum;
                            if (msgs.PageNum > 0)
                            {

                                int requestPage = msgs.PageNum - 1;
                                await model.Request_Message_Page(requestPage).ConfigureAwait(false);
                                //App.ddd.Message_GetPage(model.chat.ObjId, requestPage);
                            }
                        }
                        if (e.ItemIndex == model.ItemsMsgs.Count - 1)
                        {
                            model.Page_Stop = msgs.PageNum;
                            if (msgs.PageNum >= 0)
                            {
                                int requestPage = msgs.PageNum + 1;
                                await model.Request_Message_Page(requestPage).ConfigureAwait(false);
                                //App.ddd.Message_GetPage(model.chat.ObjId, requestPage);
                            }
                        }

                        if (false && e.ItemIndex == 0)
                        {
                            //     MyListView.ItemsSource = null;
                            //if (model.ItemsMsgs.Count > 0)
                            //{
                            //    if (model.ItemsMsgs[1] is ObjMsg msgs2)
                            //    {
                            //    //    MyListView.ScrollTo(msgs2, ScrollToPosition.Start, false); //Android.Util.AndroidRuntimeException: Only the original thread that created a view hierarchy can touch its views.
                            //    }
                            //}



                            if (msgs.PageNum > 0)
                            {
                                MyListView.BeginRefresh();
                                model.pc.Chat_GetPage(msgs.PageNum - 1);
                                model.nPage = msgs.PageNum - 1;
                                MyListView.EndRefresh();
                                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                                         {
                                             try
                                             {
                                                 //       await Task.Delay(300); //Sometimes code runs too fast and a delay is needed, you may test whether only a specific platform needs the delay
                                                 MyListView.ScrollTo(msgs, ScrollToPosition.Start, false); //Android.Util.AndroidRuntimeException: Only the original thread that created a view hierarchy can touch its views.
                                             }
                                             catch (Exception err)
                                             {

                                             }
                                         }
                                );

                            }
                            //model.pc.Chat_GetPage(model.nPage - 1);
                            //model.nPage--;
                            // 
                        }
                        if (false && e.ItemIndex == model.ItemsMsgs.Count - 1)
                        {
                            model.pc.Chat_GetPage(msgs.PageNum + 1);
                            model.nPage = msgs.PageNum + 1;


                            // Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                            // {
                            //     try
                            //     {
                            //         await Task.Delay(300); //Sometimes code runs too fast and a delay is needed, you may test whether only a specific platform needs the delay
                            //         MyListView.ScrollTo(temp, ScrollToPosition.Start, false); //Android.Util.AndroidRuntimeException: Only the original thread that created a view hierarchy can touch its views.
                            //     }
                            //     catch (Exception err)
                            //     {

                            //     }
                            // }
                            //);
                        }
                        if (false)
                            try
                            {
                                if (msgs.ShownState == 12
                                    //|| (model.chat.Statistic != null && model.chat.Statistic.LastShownObjId < set.ObjId)
                                    )
                                {

                                    //}
                                    //else
                                    //{
                                    Task.Run(() =>
                                   {
                                       try
                                       {
                                           //       if (Items.Where(s=>s.ObjId== set.ObjId && s.ShownState==40))
                                           //        if (set.ShownState != 14)
                                           {
                                               App.ddd.Message_Shown(model.chat.ObjId, msgs.ObjId);
                                               msgs.ShownState = 14;
                                               //  b_ShownCompleat = true;
                                           }
                                       }
                                       catch (Exception err)
                                       {

                                       }
                                   });
                                    /* */
                                }

                            }
                            catch (Exception err)
                            {

                            }

                    }
                }
                finally
                {
                    _lastItemAppearedIdx = model.ItemsMsgs.IndexOf((ObjMsg)e.Item);
                    //  b_addMode = false;
                }
            }
            catch (Exception err)
            {

            }
        }

        private void MyEditText_Completed(object sender, EventArgs e)
        {

        }

        private void MyEditTexBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InvalidateMeasure();
            if (String.IsNullOrEmpty(MyEditTexBox.Text.Trim()))
            {
                buttonSend.IsVisible = false;
                buttonFoto.IsVisible = true;
            }
            else
            {
                buttonSend.IsVisible = true;
                buttonFoto.IsVisible = false;
            }
            /*

            if (MyEditTexBox == null)
            {
                return;
            }

            var method = typeof(View).GetMethod("InvalidateMeasure", BindingFlags.Instance | BindingFlags.NonPublic);

            method.Invoke(MyEditTexBox, null); 
             */
        }

        private void ButtonFoto_Clicked(object sender, EventArgs e)
        {

        }

        private  async void ButtonSend_Clicked(object sender, EventArgs e)
        {
            try
            {
                //      MyEditText.IsVisible = false;
                string TextToSend = MyEditTexBox.Text.Trim();
                await model.Request_Message_Send(  "<text>" + TextToSend + "</text>");//	&#128522;
                //bool res = 
                if (true)
                {
                    MyEditTexBox.Text = "";
                    //     Items.Add(new ObjMsg() { ObjId = -1, Data = "<root><text>" + "Идёт отправка.." + "</text></root>", userid = 0 });
                    model.b_send_Begin = true;
                }
            }
            catch (Exception err)
            {
                DependencyService.Get<IXROGiToast>().ShortAlert("Ошибка отправки " + err.Message.ToString());
            }
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                Navigation.PushAsync(new UserListTabbedPage(model.chat));
            }
            catch (Exception)
            {

                 
            }
        }

        private void Refresh(object sender, EventArgs e)
        {
            try
            {
            //    App.ddd.DB_MessageChatPage_Clear(model.chat.ObjId);
                //         Pages.Clear();
              //  ShowChat(model.chat.ObjId);
            }
            catch (Exception err)
            {

            }
        }

        private void MenuItem_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            if (mi != null)
            {
                ObjMsg g = mi.CommandParameter as ObjMsg;

                if (g != null)
                {
                    try
                    {
                        Navigation.PushAsync(new MessageInfoPage(g));
                    }
                    catch (Exception err)
                    {

                    }
                }
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            
            DependencyService.Get<IXROGiToast>().ShortAlert("Событие не записано.");
        }

        private void BtnMoveToLastShown_Clicked(object sender, EventArgs e)
        {

        }
    }
}
