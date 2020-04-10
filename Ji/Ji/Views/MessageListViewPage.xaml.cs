//using Android.Webkit;
using Ji.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.IO;
using System.Threading.Tasks;
using Ji.Services;
using Ji.ViewModels;

namespace Ji.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessageListViewPage : ContentPage
    {
        MessageListViewModel model;
        
        // GroupChat chat;
        //   ChatStatisticUser stat;

        int rec = 0;
   //     public ObservableCollection<ObjMsg> Items { get; set; }
        //        public ObservableCollection<string> Items { get; set; }
        Command SelectedItemMsg { get; set; }


        private Command<object> _GoDetailCommand;
        public Command<object> GoDetailCommand
        {
            get
            {
                return _GoDetailCommand = _GoDetailCommand ?? new Command<object>((o) 
                    => 
                {
           //         _navi.NavigateAsync("DetailPage", new NavigationParameters { { "key", o.ToString() } });
                });
            }
        }


 //       SortedList<int, int> Pages = null;
 //       ChatPagesController pc;
        public MessageListViewPage(GroupChat g)
        {
            try
            {
                InitializeComponent();
                //       MyListView.ItemTemplate = (DataTemplate)Resources["Forum"];
                //                MyListView.ItemTemplate = (DataTemplate)Resources["Chat"];
               
                SelectedItemMsg = new Command(async () => await ExecuteLoadItemsCommand()
                );
                BindingContext = model = new MessageListViewModel(g);
                Init(g); 
               
            }
            catch (Exception err)
            {
                App.Log(err);
            }
        }
    

        async private Task ExecuteLoadItemsCommand()
        {
            
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
            ShowChat(model.chat.ObjId);
            MyEditTexBox.Keyboard = Keyboard.Chat;
            App.ddd.WS.OnWSReciveNewMessage += WS_OnWSReciveNewMessage;

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

       



        protected override void OnDisappearing()
        {
            try
            {
                App.ddd.WS.OnWSReciveNewMessage -= WS_OnWSReciveNewMessage;
            }catch (Exception err)
            {

            }
            base.OnDisappearing();
            
        }

        int _inChatId;
        public MessageListViewPage(int  chatid)
        {
            try
            {
                InitializeComponent();
                //       MyListView.ItemTemplate = (DataTemplate)Resources["Forum"];
                //                MyListView.ItemTemplate = (DataTemplate)Resources["Chat"];
                _inChatId = chatid;
            //    App.ddd.OnReciveChatList += Ddd_OnReciveChatList; ; ;
                App.ddd.SR.Request_Chat_List(chatid);
                

                var ttt  = Chat_RequestChat(chatid);
                Init(ttt);
//                ShowChat(chatid);

            }
            catch (Exception err)
            {

            }
        }

     

        private GroupChat Chat_RequestChat(int chatid)
        {
            return App.ddd.Chat_GetChat(chatid);
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
                if (stat!=null)
                    model.chat.Statistic = stat;
                model.pc.SetMaxPageNum(model.chat.Statistic.PageLastObj);
                // Items = new ObservableCollection<string>(); 
        //        model.ItemsMsgs = null;
        //        model.ItemsMsgs = new ObservableCollection<ObjMsg>();

                model.pc.Chat_GetPage(model.chat.Statistic.PageLastShownObj);
                model.nPage = model.chat.Statistic.PageLastShownObj;
                /*20191021
                            ObjMsg[] msgs;
                            if (pc.isPageLoad(stat.PageLastShownObj))
                            {
                                msgs = App.ddd.Message_GetPage(chatid, stat.PageLastShownObj); ;
                            }

                          //  = App.ddd.Message_GetPage(chatid, stat.PageLastShownObj); ;


                            if (msgs != null)
                            {
                                nPage = stat.PageLastShownObj;
                                if (msgs.Count() > 0) Pages.Add(nPage, nPage);
                                //int tt = 0;
                                foreach (var t in msgs)
                                {
                                    // tt++;
                                    // if (tt >= 8) break;
                                    try
                                    {

                                        Items.Add(t);
                                        if (t.ObjId == stat.LastShownObjId)
                                            lastShown = t;
                                    }
                                    catch (Exception err)
                                    {


                                    }
                                }

                                // for (int i = 0; i <= 10; i++)
                                {

                                    //   Android.Webkit.WebView w = new Android.Webkit.WebView()  ;
                                    //   w.Source =   new HtmlWebViewSource() { Html = "<html><body>Item <b>" + i.ToString() + "</b><BR/>ttttt</body></html>" };
                                    //double wq =  w.Width;
                                    //double hq = w.Height;
                                    // WebViewClient dddd = new WebViewClient();
                                    //// object yyy = FindByName("webView1");
                                    // Android.Webkit.WebView webView =  FindByName("webView1")  as Android.Webkit.WebView;
                                    // webView.SetWebViewClient(dddd);

                                    //webView.baseur("http://developer.android.com/reference/packages.html");


                                    //               Items.Add("Item " + i.ToString());
                                }

                            }
                */
   //55             MyListView.ItemsSource = model.ItemsMsgs;


                ObjMsg lastShown = model.ItemsMsgs.Where(s => s.ObjId == model.chat.Statistic.LastShownObjId).FirstOrDefault();

                if (lastShown != null)
                {


                    Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                    {
                        try
                        {
                            await Task.Delay(300);
                            MyListView.ScrollTo(lastShown, ScrollToPosition.Center, false);
                        }
                        catch (Exception err)
                        {

                        }
                    }
                            );
                }

                //     this.stackLayout_ChatMessages.Children.Add(new Button() { Text = "12334343" }); ;
                //       this.MyListView.Children.Add(new Button() { Text = "987654351" }); ;
                //    MyListView.SelectedItem = "Item 35";

            }
            catch (Exception err)
            {

            }
        }

        private void WS_OnWSReciveNewMessage(X_WS sender, string Msg, int ChatId, int NewMsgId)
        {

            try
            {
                // просто комманда с
                if (ChatId != model.chat.ObjId) return;

                ChatStatisticUser stat = App.ddd.Chat_GetMyStatistic(ChatId);
                if (stat != null)
                    model.chat.Statistic = stat;
                model.pc.Chat_GetPage(model.chat.Statistic.PageLastObj);
                //pc.SetMaxPageNum(stat.PageLastObj);
                //   foreach (var t in Items.Reverse())
                try
                {

                    ObjMsg lastShown = null;
                    //    t.Data = rec++.ToString();
                    lastShown = model.ItemsMsgs.Where(s => s.ObjId == NewMsgId).Last();//.SingleOrDefault();
                    if (lastShown != null)
                    {
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                            {
                                try
                                {
                                    await Task.Delay(300);
                                    ObservableCollection<ObjMsg> os = MyListView.ItemsSource as ObservableCollection<ObjMsg>;
                                    ObjMsg last = os.Last();
                                    MyListView.ScrollTo(last, ScrollToPosition.End, false); //Android.Util.AndroidRuntimeException: Only the original thread that created a view hierarchy can touch its views.
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
            catch (Exception err)
            {

            }
            //            UpdateLasMessage(ChatId,NewMsgId);
        }

        private void UpdateLasMessage(int chatId, int newMsgId)
        {try
            {

                /*Пришло новое собщение
                 1. Если это моё сообщение, то перевести курсор на него. При этом, если текущая позиция соединяется с позицией последней закачки, то обновить концовку и показать
                        Если позиция далеко, (более 10 страниц) то  качаем последнюю.. Фактически начинаем закачку с начала. (show_page)
                 2.
                 */

                ChatStatisticUser stat = App.ddd.Chat_GetMyStatistic(chatId);
                // Items = new ObservableCollection<string>();
                //   Items = new ObservableCollection<ObjMsg>();
                App.ddd.DB_MessageChatPage_Clear(model.chat.ObjId, stat.PageLastObj);
                ObjMsg[] msgs = App.ddd.Message_GetPage(model.chat.ObjId, stat.PageLastObj);

                //
                ObjMsg temp = null;
                {

                    if (msgs != null && msgs.Count() > 0)
                    {
                        //if (Pages.ContainsKey(nPage) == false)
                        //    if (msgs.Count() > 0)
                        //        Pages.Add(nPage, stat.PageLastObj);

                        //if (msgs.Count() == 0)
                        //{

                        //}
                        foreach (var t in msgs)
                        {
                            //    t.Data = rec++.ToString();
                            if (model.ItemsMsgs.Any(s => s.ObjId == t.ObjId) == false)
                            {
                                model.SetMinMaxList(t.ObjId);
                                model.ItemsMsgs.Add(t);
                                temp = t;
                            }
                        }
                    }
                    if (temp != null)
                    {
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {
                            try
                            {
                                MyListView.ScrollTo(temp, ScrollToPosition.MakeVisible, true);
                            }
                            catch (Exception err)
                            {

                            }
                        }
                           );

                    }
                }

            }
            catch (Exception err)
            {

            }
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            
            if (e.Item == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }


        async void GetMessage(int ChatId)
        {
                // App.ddd.getm
        }

        private void OnSelectedMessage(object sender, SelectedItemChangedEventArgs e)
        {

        }
        int tttt = 0;
        bool b_ShownCompleat;
        private void OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            try
            {
                b_ShownCompleat = false;
                if (e.Item != null)
                {
                    ObjMsg set = e.Item as ObjMsg;
                    if (set != null)
                    {
                        if (set.ShownState==12 || (model.chat.Statistic != null && model.chat.Statistic.LastShownObjId < set.ObjId))
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
                                        App.ddd.Message_Shown(model.chat.ObjId, set.ObjId);
                                        set.ShownState = 14;
                                        b_ShownCompleat = true;
                                    }
                                }catch (Exception err)
                                {

                                }
                            });
                        }

                        if (e.ItemIndex == model.ItemsMsgs.Count - 1)
                        {

                          /*  ChatStatisticUser stat = App.ddd.Chat_GetMyStatistic(model.chat.ObjId);
                            if (stat != null)
                            {
                                model.chat.Statistic = stat;
                            }
                            else
                            {
                                DependencyService.Get<IXROGiToast>().ShortAlert("Нет связи.");
                                return;
                            }
                           */ model.pc.SetMaxPageNum(model.chat.Statistic.PageLastObj); // обновим последний объект
                          //  if (stat.PageLastObj >= set.PageNum)
                            {
                                //     if (stat.PageLastObj > set.PageNum) 
                                ObjMsg temp = model.ItemsMsgs[model.ItemsMsgs.Count-1];
                              
                                model.pc.Chat_GetPage(set.PageNum + 1);

                                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                                {
                                    try
                                    {
                                        await Task.Delay(300); //Sometimes code runs too fast and a delay is needed, you may test whether only a specific platform needs the delay
                                        MyListView.ScrollTo(temp, ScrollToPosition.Start, false); //Android.Util.AndroidRuntimeException: Only the original thread that created a view hierarchy can touch its views.
                                    }
                                    catch (Exception err)
                                    {

                                    }
                                }
                               );
                                /*   if (stat.PageLastObj == set.PageNum)
                                       model.pc.Chat_GetPage(set.PageNum ); //перечитать последнее
                                      */
                                //            MyListView.ScrollTo(e.Item, ScrollToPosition.End, false);
                            }
                            tttt++;
                  /*          var t = new ObjMsg() { ObjId = 7879, PageNum = 777, Data = "<text>TEst ADdd:\r\n" + tttt.ToString() + "</text>" };
                            model.ItemsMsgs.Add(t);
                  */          
                         //   model.ItemsMsgs.Add(new ObjMsg() { ObjId = 777777, Data = "rtrtrtr" , }); ;
                            /*
                            //nPage = set.PageNum + 1;
                            if (Pages.ContainsKey(nPage+1) == false)
                            {
                                ObjMsg[] msgs = App.ddd.Message_GetPage(chat.ObjId, nPage+1);
                                if (msgs != null && msgs.Count() > 0)
                                {
                                    nPage++;
                                    Pages.Add(nPage, nPage);
                                    
                                    foreach (var t in msgs)
                                    {
                                        //    t.Data = rec++.ToString();
                                        Items.Add(t);
                                    }
                                }
                            }*/
                        }


                        // Качаем предыдущий
                        if (e.ItemIndex == 0)
                        {
                            ObjMsg temp = model.ItemsMsgs[0];

                            if ((temp.PageNum - 1) > 0)
                            {
                                model.pc.Chat_GetPage(temp.PageNum - 1);



                             Xamarin.Forms.Device.BeginInvokeOnMainThread(async  () =>
                                {
                                    try
                                    {
                                        await Task.Delay(300); //Sometimes code runs too fast and a delay is needed, you may test whether only a specific platform needs the delay
                                        MyListView.ScrollTo(temp, ScrollToPosition.Start, false); //Android.Util.AndroidRuntimeException: Only the original thread that created a view hierarchy can touch its views.
                                    }
                                    catch (Exception err)
                                    {

                                    }
                                }
                            );

                            }
                            else
                            {
                                // самое начало чата. Грузить нечего
                            }
                            

                        }
                        if (model.chat.Statistic==null)
                        { //невероятно

                            /// вероято !! связи нет
                            ChatStatisticUser stat = App.ddd.Chat_GetMyStatistic(model.chat.ObjId);
                            model.chat.Statistic = stat;
                            if (model.chat.Statistic==null)
                            {

                            }
                        }
                        if (model.chat.Statistic != null && model.chat.Statistic.LastShownObjId < set.ObjId )
                        {
                            if (set.ShownState == 14)
                            {// Незафиксированов в статистике chat.Statistic Странно

                                App.ddd.Message_Shown(model.chat.ObjId, set.ObjId);
                                //14	Сообщение просмотрено
                            //    set.ShownState = 14;//просмотрено
                                                    //chat.Statistic.LastShownObjId = set.ObjId;
                            }
                            if (set.ShownState == 12)
                            {

                                App.ddd.Message_Shown(model.chat.ObjId, set.ObjId);
                                //14	Сообщение просмотрено
                                set.ShownState = 14;//просмотрено
                                                    //chat.Statistic.LastShownObjId = set.ObjId;
                            }
                        }
                        if (set.ShownState == 12)//получено
                        {
                            if (set.ObjId != 0)
                            {
                                App.ddd.Message_Shown(model.chat.ObjId, set.ObjId);
                                set.ShownState = 14;//просмотрено
                            }
                            else

                            {

                            }
                        }
                  
                    }
                }

            }catch (Exception err)
            {
                var t = new ObjMsg() { ObjId=-1,  PageNum=-1, Data = "<text>Возникла ошибка показа списка. Текст ошибки для анализа:\r\n" + err.Message.ToString() + "</text>" };
                model.ItemsMsgs.Add(t);
           //     MyListView.ScrollTo(t, ScrollToPosition.Start, false);
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

        //public async Task ScrollMessagesToEndAsync() //Adding Async to method name, also try to return Task and await this method in the calling code as well
        //{
        //    await Task.Delay(300); //Sometimes code runs too fast and a delay is needed, you may test whether only a specific platform needs the delay

        //    StackLayout messagesContent = (messagesScrollView.Content as StackLayout);
        //    var frame = messagesContent.Children.LastOrDefault();

        //    if (frame != null)
        //    {
        //        Device.BeginInvokeOnMainThread(async () => await messagesScrollView.ScrollToAsync(frame, ScrollToPosition.MakeVisible, true)); //You could try passing in false to disable animation and see if that helps
        //    }
        //}
        //protected ConversationViewModel Model
        //{
        //    get { return (ConversationViewModel)BindingContext; }
        //}
        private bool _autoScroll = true;
        private void onItemDisAppearing(object sender, ItemVisibilityEventArgs e)
        {
            try
            {
                //            if (e.Item == Model.Messages.Last())
                if (e.Item == model.ItemsMsgs.Last())
                    _autoScroll = false;
            }
            catch (Exception err)
            {

            }
        }

        private void ButtonGreen_Clicked(object sender, EventArgs e)
        {
            MyEditTexBox.Text = "";
            MyEditText.IsVisible = true;
            MyEditTexBox.Focus();
            
        }
       

        private void MyEditText_Completed(object sender, EventArgs e)
        {
          //  MyEditText.IsVisible = false;
        }

    
        private void ButtonSend_Clicked(object sender, EventArgs e)
        {
            try
            {
                //      MyEditText.IsVisible = false;
                string TextToSend = MyEditTexBox.Text.Trim();
                bool res = App.ddd.Message_Send(model.chat.ObjId, "<text>" + TextToSend + "</text>");//	&#128522;
                if (res)
                {
                    MyEditTexBox.Text = "";
                    //     Items.Add(new ObjMsg() { ObjId = -1, Data = "<root><text>" + "Идёт отправка.." + "</text></root>", userid = 0 });
                    model.b_send_Begin = true;
                }
            }catch (Exception err)
            {
                DependencyService.Get<IXROGiToast>().ShortAlert("Ошибка отправки " + err.Message.ToString()) ;
            }
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
      // https://github.com/jamesmontemagno/MediaPlugin
         //   await CrossMedia.Current.Initialize();
            //var photo = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });


            //if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            //{
            //    DisplayAlert("No Camera", ":( No camera available.", "OK");
            //    return;
            //}

            //var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            //{
            //    Directory = "Sample",
            //    Name = "test.jpg"
            //});

            //if (file == null)
            //    return;

            //await DisplayAlert("File Location", file.Path, "OK");

            //image.Source = ImageSource.FromStream(() =>
            //{
            //    var stream = file.GetStream();
            //    return stream;
            //});
        }

        private void Refresh(object sender, EventArgs e)
        {
            try
            {
                App.ddd.DB_MessageChatPage_Clear(model.chat.ObjId);
                //         Pages.Clear();
                ShowChat(model.chat.ObjId);
            }catch (Exception err)
            {

            }
        }

        private void OnItemAppearing2(object sender, ItemVisibilityEventArgs e)
        {

        }

        private void OnItemAppearing3(object sender, ItemVisibilityEventArgs e)
        {

        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                //AddUserInChat
                Navigation.PushAsync(new UserListTabbedPage(model.chat));
            }
            catch (Exception err)
            { }
        }

        private void MyListView_Scrolled(object sender, ScrolledEventArgs e)
        {
            //Debug.WriteLine("HorizontalDelta: " + e.HorizontalDelta);
            //Debug.WriteLine("VerticalDelta: " + e.VerticalDelta);
            //Debug.WriteLine("HorizontalOffset: " + e.HorizontalOffset);
            //Debug.WriteLine("VerticalOffset: " + e.VerticalOffset);
            //Debug.WriteLine("FirstVisibleItemIndex: " + e.FirstVisibleItemIndex);
            //Debug.WriteLine("CenterItemIndex: " + e.CenterItemIndex);
            //Debug.WriteLine("LastVisibleItemIndex: " + e.LastVisibleItemIndex);
        }

        private void MyMsgTapped(object sender, EventArgs e)
        {

        }

        private void onFrameCitizenReporterTapped(object sender, EventArgs e)
        {

        }

        private void onStackCitizenReporterTapped2(object sender, EventArgs e)
        {

        }

        private void onFrameCitizenReporterTappedListItems(object sender, EventArgs e)
        {

        }


        private void OnTapGestureRecognizerTapped(object sender, EventArgs e)
        {

        }
    }
 
}
