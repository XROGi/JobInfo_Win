using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using ChatLibrary;
using Firebase.Iid;
using Ji.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Firebase;
using Ji.Droid;
using Ji.Views;
using Ji.Droid.JI_WS;
using Xamarin.Essentials;
using System.Collections.Concurrent;
using Android.Util;
using System.Text.Json;
using Newtonsoft.Json;
using Ji.ClassSR;

[assembly: UsesFeature("android.hardware.camera", Required = false)]
[assembly: UsesFeature("android.hardware.camera.autofocus", Required = false)]

namespace Ji.Droid
{

    //Label = "Ji",
    [Activity(Icon = "@mipmap/icon", LaunchMode = LaunchMode.Multiple, Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        App app;
     //   static ServiceReference1.ChatLevelServiceClient c;

        public static FirebaseApp appFire;

        /*        bool b_InitMainExceptionLog = true;
                bool b_InitFireBase = true;
                bool b_InitDataStore = true;
                bool b_InitMainApp = true;
                bool b_StartMainPage = true;
                bool b_StartTimers = true;
        */
        bool b_InitCacheImage = true;
        bool b_InitInstance = true;
        bool b_InitApp = true;
        bool b_InitMainExceptionLog = true;
        bool b_InitFireBase = true;
        bool b_InitDataStore = true;
        bool b_InitMainApp = true;
        bool b_StartMainPage = true;
        bool b_StartTimers = true;

        protected override void OnPause()
        {
            base.OnPause();
            ActiveActivitiesTracker.activityStopped();
        }
        protected override void OnStart()
        {
            base.OnStart();
            ActiveActivitiesTracker.activityStarted();
        }
        static ClientWebSocket client;
        protected override void OnPostResume()
        {
            base.OnPostResume();
        }
        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnStop()
        {
            base.OnStop();
            
        }

        protected override void OnDestroy()
        {
            UnsubscribeMethods();
            if (App.ddd != null)
            {
                App.ddd.Close_OnDestroy();
                App.ddd = null;
            }
            base.OnDestroy();
        }

       

        //@Override public void onStart() { super.onStart(); ActiveActivitiesTracker.activityStarted(); }
        //@Override public void onStop() { super.onStop(); ActiveActivitiesTracker.activityStopped(); }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            global::Xamarin.Forms.Forms.SetFlags("Shell_Experimental", "Visual_Experimental", "CollectionView_Experimental"); 
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            // Bundle bundle = this.geti.getArguments();
            //     setRetainInstance(true);
            TabLayoutResource = Resource.Layout.Tabbar;

            ToolbarResource = Resource.Layout.Toolbar;

            try
            {
                base.OnCreate(savedInstanceState);
                if (b_InitInstance)
                {
                    Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
                    Xamarin.Forms.Forms.Init(this, savedInstanceState);
                }
                //Bundle bundle1 = new Bundle(); ;
                //      base.OnSaveInstanceState(bundle1);
                //    savedInstanceState = bundle1;
                //        this.Window.SetFlags(WindowManagerFlags.Secure, WindowManagerFlags.Secure);
            }
            catch (Exception err)
            {

            }
            try
            {
                //         if (savedInstanceState != null)
                if (b_InitCacheImage)
                    Xamarin.Essentials.Platform.Init(this, savedInstanceState);


                //if (savedInstanceState != null)
                //    Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);
            }
            catch (Exception err)
            {

            }
            try
            {
                //      global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
                if (b_InitCacheImage)
                {
                    FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);
                    //Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity.iInit(this, bundle);
                    //     Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity = this;
                    //     LoadApplication(new App());

                    App.deviceIn = new AndroidApplicationInfo();
                }
            }
            catch (Exception err)
            {

            }


            try
            {
                if (b_InitMainExceptionLog)
                {
                    AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironmentOnUnhandledException;
                    AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
                    TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
                }
                app = new App();
                if (App.ddd == null)
                {
                  
                }
                if (b_InitApp)
                {
                    app.Init();//.ConfigureAwait(false);
                    LoadApplication(app);
                    Thread.Sleep(1000);
                }
                //string dss = Android.App.Application.Context.ToString();
                //appFire = FirebaseApp.InitializeApp(Android.App.Application.Context);
         //       appFire = FirebaseApp.InitializeApp(this);


                if (b_InitFireBase)
                    FirebaseApp.InitializeApp(this);
                //boolean isMain = isMainProcess(this);
                /*FirebaseFirestoreSettings settings = new FirebaseFirestoreSettings.Builder().setPersistenceEnabled(isMain).build();
                FirebaseFirestore.getInstance().setFirestoreSettings(settings);
                */


                //FirebaseApp.InitializeApp(this);
            }
            catch (Exception err)
            {
                //"  at Xamarin.Forms.Platform.Android.FormsAppCompatActivity.InternalSetPage (Xamarin.Forms.Page page)…"
            }

            if (b_InitDataStore)
            {
                try
                {
                    if (App.ddd != null)
                    {
                        App.ddd.Fill();
                    }
                    else
                    {
                    }
                }
                catch (Exception err)
                {
                }
            }


            //c = new ServiceReference1.ChatLevelServiceClient();

            //try
            //{

            //    //                string json = @"[{"ObjId":444407,"MsgId":444728,"Xml":"\u003Croot\u003E\u003Ctext ver =\u00221.0\u0022\u003E\u041A\u0430\u0437\u043C\u0438\u043D\u0441\u043A\u0438\u0439 \u0410\u043B\u0435\u043A\u0441\u0435\u0439 \u0410\u043B\u0435\u043A\u0441\u0430\u043D\u0434\u0440\u043E\u0432\u0438\u0447\u003C/ text\u003E\u003C/ root\u003E","SgTypeId":8,"ChatType":"\u041F\u0440\u0438\u0432\u0430\u0442\u043D\u044B\u0439 \u0447\u0430\u0442","SgClassId":6,"PeriodId":3908579,"Dtb":"2019 - 10 - 25T10: 34:24.577","Dte":null,"Dtc":"2019 - 10 - 25T10: 34:24.577","UidSubdiv":null,"UserId":14722,"ObjStatusId":3470391,"SgMsgStatusId":21,"ChatName":"\u041A\u0430\u0437\u043C\u0438\u043D\u0441\u043A\u0438\u0439 \u0410\u043B\u0435\u043A\u0441\u0435\u0439 \u0410\u043B\u0435\u043A\u0441\u0430\u043D\u0434\u0440\u043E\u0432\u0438\u0447"},{"ObjId":158,"MsgId":197,"Xml":"\u003Ctext\u003E\u0422\u0430\u0439\u043C\u0435\u0440\u003C/ text\u003E","SgTypeId":7,"ChatType":"\u041E\u0431\u0449\u0438\u0439 \u0447\u0430\u0442","SgClassId":6,"PeriodId":544,"Dtb":"2019 - 01 - 15T08: 32:41.68","Dte":null,"Dtc":"2019 - 01 - 15T08: 32:41.68","UidSubdiv":null,"UserId":14722,"ObjStatusId":10136,"SgMsgStatusId":21,"ChatName":"\u0422\u0430\u0439\u043C\u0435\u0440"},{"ObjId":469592,"MsgId":469913,"Xml":"\u003Croot\u003E\u003Ctext ver =\u00221.0\u0022\u003E\u043F\u0440\u043E\u0431\u0430\u003C/ text\u003E\u003C/ root\u003E","SgTypeId":7,"ChatType":"\u041E\u0431\u0449\u0438\u0439 \u0447\u0430\u0442","SgClassId":6,"PeriodId":3989330,"Dtb":"2019 - 11 - 08T17: 48:19.943","Dte":null,"Dtc":"2019 - 11 - 08T17: 48:19.943","UidSubdiv":null,"UserId":14722,"ObjStatusId":3525686,"SgMsgStatusId":21,"ChatName":"\u043F\u0440\u043E\u0431\u0430"},{"ObjId":33276,"MsgId":214227,"Xml":"\u003Ctext\u003E\u0447\u0430\u0442\u003C/ text\u003E","SgTypeId":7,"ChatType":"\u041E\u0431\u0449\u0438\u0439 \u0447\u0430\u0442","SgClassId":6,"PeriodId":434847,"Dtb":"2019 - 06 - 18T08: 54:27.55","Dte":null,"Dtc":"2019 - 06 - 18T08: 54:27.563","UidSubdiv":null,"UserId":14722,"ObjStatusId":232096,"SgMsgStatusId":21,"ChatName":"\u0447\u0430\u0442"},{"ObjId":446317,"MsgId":446638,"Xml":"\u003Croot\u003E\u003Ctext ver =\u00221.0\u0022/\u003E\u003C/ root\u003E","SgTypeId":7,"ChatType":"\u041E\u0431\u0449\u0438\u0439 \u0447\u0430\u0442","SgClassId":6,"PeriodId":3912429,"Dtb":"2019 - 10 - 25T20: 37:41.377","Dte":null,"Dtc":"2019 - 10 - 25T20: 37:41.377","UidSubdiv":null,"UserId":14722,"ObjStatusId":3472306,"SgMsgStatusId":21,"ChatName":""},{"ObjId":424257,"MsgId":424578,"Xml":"\u003Croot\u003E\u003Ctext ver =\u00221.0\u0022\u003E\u043F\u0440\u043E\u0432\u0435\u0440\u043A\u0430 \u0441\u043E\u0437\u0434\u0430\u043D\u0438\u044F\u003C/ text\u003E\u003C/ root\u003E","SgTypeId":7,"ChatType":"\u041E\u0431\u0449\u0438\u0439 \u0447\u0430\u0442","SgClassId":6,"PeriodId":3851482,"Dtb":"2019 - 10 - 14T19: 04:38.47","Dte":null,"Dtc":"2019 - 10 - 14T19: 04:38.47","UidSubdiv":null,"UserId":14722,"ObjStatusId":3433551,"SgMsgStatusId":21,"ChatName":"\u043F\u0440\u043E\u0432\u0435\u0440\u043A\u0430 \u0441\u043E\u0437\u0434\u0430\u043D\u0438\u044F"},{"ObjId":469628,"MsgId":469949,"Xml":"\u003Croot\u003E\u003Ctext ver =\u00221.0\u0022\u003E\u043F\u0440\u043E\u0431\u0430 6\u003C/ text\u003E\u003C/ root\u003E","SgTypeId":7,"ChatType":"\u041E\u0431\u0449\u0438\u0439 \u0447\u0430\u0442","SgClassId":6,"PeriodId":3989404,"Dtb":"2019 - 11 - 08T17: 59:38.35","Dte":null,"Dtc":"2019 - 11 - 08T17: 59:38.35","UidSubdiv":null,"UserId":14722,"ObjStatusId":3525723,"SgMsgStatusId":21,"ChatName":"\u043F\u0440\u043E\u0431\u0430 6"},{"ObjId":446401,"MsgId":446722,"Xml":"\u003Croot\u003E\u003Ctext ver =\u00221.0\u0022\u003E\u0410\u0434\u043C\u0438\u043D\u044B.Svodinfo\u003C/ text\u003E\u003C/ root\u003E","SgTypeId":7,"ChatType":"\u041E\u0431\u0449\u0438\u0439 \u0447\u0430\u0442","SgClassId":6,"PeriodId":3912614,"Dtb":"2019 - 10 - 26T16: 29:47.66","Dte":null,"Dtc":"2019 - 10 - 26T16: 29:47.66","UidSubdiv":null,"UserId":14722,"ObjStatusId":3472402,"SgMsgStatusId":21,"ChatName":"\u0410\u0434\u043C\u0438\u043D\u044B.Svodinfo"},{"ObjId":467464,"MsgId":467785,"Xml":"\u003Croot\u003E\u003Ctext ver =\u00221.0\u0022\u003E\u0410\u0431\u0437\u0435\u043B\u0438\u043B\u043E\u0432\u0430 \u0420\u0435\u0433\u0438\u043D\u0430 \u0412\u0430\u043B\u0435\u0440\u044C\u0435\u0432\u043D\u0430\u003C/ text\u003E\u003C/ root\u003E","SgTypeId":8,"ChatType":"\u041F\u0440\u0438\u0432\u0430\u0442\u043D\u044B\u0439 \u0447\u0430\u0442","SgClassId":6,"PeriodId":3979470,"Dtb":"2019 - 11 - 07T19: 17:25.23","Dte":null,"Dtc":"2019 - 11 - 07T19: 17:25.23","UidSubdiv":null,"UserId":14722,"ObjStatusId":3518385,"SgMsgStatusId":21,"ChatName":"\u0410\u0431\u0437\u0435\u043B\u0438\u043B\u043E\u0432\u0430 \u0420\u0435\u0433\u0438\u043D\u0430 \u0412\u0430\u043B\u0435\u0440\u044C\u0435\u0432\u043D\u0430"},{"ObjId":469513,"MsgId":469834,"Xml":"\u003Croot\u003E\u003Ctext ver =\u00221.0\u0022\u003E\u0410\u0431\u0438\u043B\u0435\u043D\u0446\u0435\u0432\u0430 \u0412\u0438\u043A\u0442\u043E\u0440\u0438\u044F \u0410\u043D\u0434\u0440\u0435\u0435\u0432\u043D\u0430\u003C/ text\u003E\u003C/ root\u003E","SgTypeId":8,"ChatType":"\u041F\u0440\u0438\u0432\u0430\u0442\u043D\u044B\u0439 \u0447\u0430\u0442","SgClassId":6,"PeriodId":3989145,"Dtb":"2019 - 11 - 08T17: 23:54.01","Dte":null,"Dtc":"2019 - 11 - 08T17: 23:54.01","UidSubdiv":null,"UserId":14722,"ObjStatusId":3525583,"SgMsgStatusId":21,"ChatName":"\u0410\u0431\u0438\u043B\u0435\u043D\u0446\u0435\u0432\u0430 \u0412\u0438\u043A\u0442\u043E\u0440\u0438\u044F \u0410\u043D\u0434\u0440\u0435\u0435\u0432\u043D\u0430"},{"ObjId":5897,"MsgId":16597,"Xml":"\u003Ctext\u003E\u0413\u0421\u0410 \u0438 \u041F\u041A\u003C/ text\u003E","SgTypeId":30,"ChatType":"\u041A\u043E\u0440\u043F\u043E\u0440\u0430\u0442\u0438\u0432\u043D\u044B\u0439 \u0447\u0430\u0442","SgClassId":6,"PeriodId":93636,"Dtb":"2019 - 02 - 19T15: 07:26.63","Dte":null,"Dtc":"2019 - 02 - 19T15: 07:26.63","UidSubdiv":"7d1ebabc - 00fa - 11e8 - 9398 - 000c29bfd672","UserId":14722,"ObjStatusId":8704,"SgMsgStatusId":21,"ChatName":"\u0413\u0421\u0410 \u0438 \u041F\u041A"},{"ObjId":469669,"MsgId":469990,"Xml":"\u003Croot\u003E\u003Ctext ver =\u00221.0\u0022\u003E\u0442\u0435\u0441\u0442 7\u003C/ text\u003E\u003C/ root\u003E","SgTypeId":7,"ChatType":"\u041E\u0431\u0449\u0438\u0439 \u0447\u0430\u0442","SgClassId":6,"PeriodId":3989488,"Dtb":"2019 - 11 - 08T18: 12:35.57","Dte":null,"Dtc":"2019 - 11 - 08T18: 12:35.57","UidSubdiv":null,"UserId":14722,"ObjStatusId":3525765,"SgMsgStatusId":21,"ChatName":"\u0442\u0435\u0441\u0442 7"},{"ObjId":469583,"MsgId":469904,"Xml":"\u003Croot\u003E\u003Ctext ver =\u00221.0\u0022\u003E\u0410\u0431\u0440\u0430\u043C\u043A\u0438\u043D \u042F\u043D \u0412\u0438\u0442\u0430\u043B\u044C\u0435\u0432\u0438\u0447\u003C/ text\u003E\u003C/ root\u003E","SgTypeId":8,"ChatType":"\u041F\u0440\u0438\u0432\u0430\u0442\u043D\u044B\u0439 \u0447\u0430\u0442","SgClassId":6,"PeriodId":3989305,"Dtb":"2019 - 11 - 08T17: 46:18.58","Dte":null,"Dtc":"2019 - 11 - 08T17: 46:18.58","UidSubdiv":null,"UserId":14722,"ObjStatusId":3525671,"SgMsgStatusId":21,"ChatName":"\u0410\u0431\u0440\u0430\u043C\u043A\u0438\u043D \u042F\u043D \u0412\u0438\u0442\u0430\u043B\u044C\u0435\u0432\u0438\u0447"},{"ObjId":424273,"MsgId":424594,"Xml":"\u003Croot\u003E\u003Ctext ver =\u00221.0\u0022\u003E\u043F\u0440\u043E\u0432\u0435\u0440\u043A\u0430\u003C/ text\u003E\u003C/ root\u003E","SgTypeId":7,"ChatType":"\u041E\u0431\u0449\u0438\u0439 \u0447\u0430\u0442","SgClassId":6,"PeriodId":3851516,"Dtb":"2019 - 10 - 14T19: 09:14.877","Dte":null,"Dtc":"2019 - 10 - 14T19: 09:14.877","UidSubdiv":null,"UserId":14722,"ObjStatusId":3433568,"SgMsgStatusId":21,"ChatName":"\u043F\u0440\u043E\u0432\u0435\u0440\u043A\u0430"}]";


            //    c.Open();

            //    //c.OpenAsync();
            //    _ = Test();
            //}
            //catch (Exception err)
            //{
            //}
            
            if (b_InitMainApp)
             InitMainApp_v2();

            if (b_StartMainPage)
            {
                if (App.ddd.connectInterface?.isSetup() == false)// == null || String.IsNullOrEmpty(ddd.connectInterface.Server_Name)
                {
                    app.MainPage = new ConnectServerPage();
                }
                else

                {
                    app.MainPage = new SvodInfMasterDetailPage();
                }
            }


            if (b_StartTimers)
            {
                try
                {
                    //b_StatisticChanged_NeedUpdate = true;
                    App.ddd.SQL_LoadUsersList(0);
                    //InitMainApp();
                    //11     InitTimer_SendShown(1);
                    //     InitTimer_SendMessage(1);
                    InitTimer_Reconnect(7);
                    InitTimer_Ping(30);
                    //11    InitTimer_RequestStatistic(1);

                }
                catch (Exception err)
                {
                    App.Log(err);
                }
            }

        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;

            // log won't be available, because dalvik is destroying the process
            //Log.Debug (logTag, "MyHandler caught : " + e.Message);
            // instead, your err handling code shoudl be run:
            Console.WriteLine("========= MyHandler caught : " + e.Message);
            //TraceWriter.WriteTrace(args.Exception);
         //   args.Handled = true;
        }

        private async Task<string> Test()
        {
            try
            {
             //   string a = c.About();
                return "";
            }
            catch (Exception err)
            {
            }
            return "??";
        }

        private void AndroidEnvironmentOnUnhandledException(object sender, RaiseThrowableEventArgs e)
        {
           
            //IsGroupingEnabled="false" CachingStrategy="RecycleElement"
            e.Handled = true;
        }

        private void InitTimer_Ping(int TimeSec)
        {
            try
            {
                Device.StartTimer(TimeSpan.FromSeconds(TimeSec),    () =>
                {
                    try
                    {
                        return true; 
                        if (App.ddd != null)
                            if (App.ddd.connectInterface != null)
                                if (App.ddd.connectInterface.isSetup() == true)
                                {
                                    if (App.ddd.isOpen())
                                    {
                                        // Do something
                                        _ = App.ddd.SendMessageAsync("ping");

                                    }
                                    else
                                    {
                                        App.ddd.Open(); 
                                        _ = App.ddd.SendMessageAsync("ping");
                                    }

                                }

                       // return true; // True = Repeat again, False = Stop the timer
                    }
                    catch (Exception err)
                    {

                    }
                    return true;
                });
            }
            catch (Exception err)
            {

            }
        }

        object lockAutoconnect = new object();
        bool b_lockAutoconnect = false;

        private void InitTimer_Reconnect(int TimeSec)
        {
            try
            {
                Device.StartTimer(TimeSpan.FromSeconds(TimeSec), () =>
                {
                    Task.Factory.StartNew(async () =>
                    {
                        if (b_lockAutoconnect == true)
                            return false;
                        b_lockAutoconnect = true;
                        //  lock (lockAutoconnect)
                        try
                        {

                            try
                            {
                                if (App.ddd != null)
                                    if (App.ddd.connectInterface != null)
                                        if (App.ddd.connectInterface.isSetup() == true)
                                        {
                                            if (App.ddd.isOpen() == false)
                                            {
                                                App.ddd.Open();
                                            }
                                        }
                                //Device.BeginInvokeOnMainThread(() =>
                                //{
                                //}
                                return true; // True = Repeat again, False = Stop the timer
                            }
                            catch (Exception err)
                            {

                            }

                        }
                        finally
                        {
                            b_lockAutoconnect = false;
                        }
                        return false;
                    });


                    return false;
                });
            }
            catch (Exception err)
            {

            }
        }

        private void InitTimer_SendShown(int TimeSec)
        {
            try
            {
                Device.StartTimer(TimeSpan.FromSeconds(TimeSec), () =>
                {
                    try
                    {
                        SendShownMessagesChatNow();

                        return true; // True = Repeat again, False = Stop the timer
                    }
                    catch (Exception err)
                    {

                    }
                    return false;
                });
            }
            catch (Exception err)
            {

            }
        }

        object lockedInitTimer_RequestStatistic = new object();
        private void InitTimer_RequestStatistic(int TimeSec)
        {
            try
            {
                Device.StartTimer(TimeSpan.FromSeconds(TimeSec), () =>
                {
                    try
                    {
                        if (b_StatisticChanged_NeedUpdate)
                        {
                            lock (lockedInitTimer_RequestStatistic)
                            {
                                try
                                {
                                    //         App.Current.MainPage
                                    //     App.ddd.StatisticChat = OnChat_Statistic();

                                }
                                finally
                                {
                                    b_StatisticChanged_NeedUpdate = false;
                                }
                            }
                        }

                        return true; // True = Repeat again, False = Stop the timer
                    }
                    catch (Exception err)
                    {

                    }
                    return false;
                });
            }
            catch (Exception err)
            {

            }
        }
        private void InitTimer_SendMessage(int TimeSec)
        {
            try
            {
                Device.StartTimer(TimeSpan.FromSeconds(TimeSec), () =>
                {
                    try
                    {
                        SendNewMessages();

                        return true; // True = Repeat again, False = Stop the timer
                    }
                    catch (Exception err)
                    {

                    }
                    return false;
                });
            }
            catch (Exception err)
            {

            }
        }

        ConcurrentBag<SendMessage> MessageSendList;

        public SendMessage Lockal_MessageSendList_Add(int chatId, string messageText)
        {
            try
            {
                if (MessageSendList == null) MessageSendList = new ConcurrentBag<SendMessage>();
                SendMessage sm = new SendMessage();
                sm.ChatId = chatId;
                sm.MessageText = messageText;
                MessageSendList.Add(sm);
                return sm;
            } catch (Exception err)
            {

            }
            return null;
        }
        private void SendNewMessages()
        {
            try
            {
                if (MessageSendList == null) return;
                // Do something
                if (!MessageSendList.IsEmpty)
                {
                    if (App.ddd.connectInterface.isSetup())
                    {
                        //   await AsyncHelper.RedirectToThreadPool();
                        SendMessage t;
                        if (MessageSendList.TryTake(out t))
                        {
                            Droid.JI_WS.GetUserInfo g = GetUserInfo();
                            g.Url = App.ddd.connectInterface.Server_SOAP;
                            g.Message_AddAsync(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, t.ChatId, t.MessageText);
                        }
                    }
                }


            }
            catch (Exception err)
            {

            }
        }

        private void SendShownMessagesChatNow()
        {
            try
            {
                // Do something

                if (!ShownObjId.IsEmpty)
                {
                    int[] list;
                    lock (lockAdd)
                    {
                        list = ShownObjId.Distinct().ToArray();
                        ShownObjId.Clear();
                    }
                    //      int max = list.Max();
                    /*int item;
                    if (ShownObjId.TryTake(out item))
                    {
                        Console.WriteLine(item);
                    //    itemsInBag++;
                    }
                    */
                    int _ChatId_ForShownList = ChatId_ForShownList;
                    if (App.ddd.connectInterface.isSetup() && list.Count() > 0)
                    {

                        try
                        {



                            if (App.ddd != null)
                                if (App.ddd.connectInterface != null)
                                    if (App.ddd.connectInterface.isSetup() == true)
                                    {
                                        if (App.ddd.isOpen())
                                        {
                                            // Do something
                                            //  if (App.ddd.WS != null)
                                            {
                                                string S = String.Join(",", list);
                                                App.ddd.SendMessageAsync("SHOWN[" + ChatId_ForShownList.ToString() + "]=" + S);
                                            }

                                        }
                                    }





                        }
                        catch (Exception err)
                        {

                        }
                        /*

                        Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                        {
                            Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();
                            g.Url = App.ddd.connectInterface.Server_SOAP;
//                            g.Message_ShownList(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, _ChatId_ForShownList, list, 0);

                        });
                        */

                        //    g.Message_ShownListAsync(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, ChatId_ForShownList, list, 0);
                    }

                }

            }
            catch (Exception err)
            {

            }
        }


        /*https://www.cyberforum.ru/csharp-beginners/thread695654.html
           public Class3( )
                : base( )
            {
                base.Call(); // для проверки
                var methodInfo = typeof( Class2 ).GetMethod("method1", BindingFlags.NonPublic | BindingFlags.Instance );
                var eventInfo = typeof( Class2 ).GetEvent( "Args" );
 
                eventInfo.RemoveEventHandler( this, Delegate.CreateDelegate( eventInfo.EventHandlerType, this, methodInfo ) );
                base.Call(); // для проверки
            }
        */

        private void InitMainApp_v2()
        {

            Ji.X_SignalR sr = new Ji.X_SignalR();
            App.ddd.SR = sr;

            

            //App.ddd.deviceIn = new AndroidApplicationInfo();
            //     App.ddd.Fill();

            sr.OnWSConnected += SR_OnWSConnected;
            sr.OnWSReConnected += SR_OnWSReConnected;//+
            sr.OnWSDisConnected += SR_OnWSDisConnected; ;//+
            sr.OnWSReciveToken += SR_OnWSReciveToken;//+
            sr.OnWSRecivePong += SR_OnWSRecivePong;
            sr.OnWSReciveTokenClosed += SR_OnWSReciveTokenClosed;
            sr.OnWSRecive += SR_OnWSRecive;
            sr.OnWSReciveChatEvent += SR_OnWSReciveChatEvent;
            sr.OnWSReciveStatisicUpdate += Sr_OnWSReciveStatisicUpdate;

            sr.OnReciveChatList += Sr_OnReciveChatList;
            sr.OnReciveChatAppend += Sr_OnReciveChatAppend; ;
            sr.OnReciveMessagePageList += Sr_ReciveMessagePageList;
            sr.OnReciveMessage += Sr_OnReciveMessage; ;
            
            sr.OnReciveMessageAppend += Sr_OnReciveMessageAppend;
            sr.OnReciveMessageStatus += Sr_OnReciveMessageStatus;
            sr.OnReciveMessageAllStatus += Sr_OnReciveMessageAllStatus;
            sr.OnReciveUserList += Sr_OnReciveUserList;
            sr.OnResultReciveUserParams += Sr_OnResultReciveUserParams;
            sr.OnReciveChatUserLeave += Sr_OnReciveChatUserLeave;
            sr.OnReciveMessageUpdate += Sr_OnReciveMessageUpdate;
            sr.OnLogConnecttionAdd += SR_OnLogConnecttionAdd;

            //      SR_OnWSReciveNewChatStatistic;

            /*
            ws.OnWSConnected += Ws_OnWSConnected;
            ws.OnWSDisConnected += Ws_OnWSDisConnected; ;
            ws.OnWSReciveToken += Ws_OnWSReciveToken;
            ws.OnWSRecivePong += Ws_OnWSRecivePong;
            ws.OnWSReciveTokenClosed += Ws_OnWSReciveTokenClosed;
            ws.OnWSRecive += Ws_OnWSRecive;
            ws.OnWSReciveChatEvent += Ws_OnWSReciveChatEvent;
            ws.OnWSReciveNewChatStatistic += OnWSReciveNewChatStatistic;
            */

            App.ddd.OnRegister_FB += OnRegister_FB;

            App.ddd.OnGetChatList += OnGetChatList;
            App.ddd.OnChatGetOne += OnChatGetOne;
            App.ddd.OnChat_Leave += OnChatLeave;
            App.ddd.OnChat_CreateAndSubscribe += OnChat_CreateAndSubscribe;
            App.ddd.OnChat_GetMyStatistic += OnChat_GetMyStatistic;
            App.ddd.OnChat_GetStatistic += OnChat_Statistic;
            App.ddd.OnChat_SubscribeUser += OnChat_SubscribeUser; ;
            App.ddd.OnChat_UnSubscribeUser += OnChat_UnSubscribeUser; ;
            App.ddd.OnSetupParam_Load += OnSetupParam_Load;

            //        App.ddd.OnNewPageRecive += OnNewPageRecive;

            App.ddd.OnGetUserList += OnGetUserList;

            App.ddd.OnMessage_GetPage += OnMessage_GetPage;
            App.ddd.OnMessage_Send += OnMessage_Send;
            App.ddd.OnMessageFile_Send += OnMessageFile_Send;
            App.ddd.OnMessage_Shown += OnMessage_Shown;

            App.ddd.OnParking_Step1 += OnParking_Step1;
            App.ddd.OnParking_Step2 += OnParking_Step2;
            App.ddd.OnChat_Create_Public += OnChat_Create_Public;

        //    App.ddd.OnReciveChatList += Ddd_OnReciveChatList;
            



            /*
            if (ws.Url != "")
                ws.Open();
 */

        }
        private void UnsubscribeMethods()
        {
            if (App.ddd != null && App.ddd.SR != null)
            {
                Ji.X_SignalR sr = App.ddd.SR;
                {

                    sr.OnWSReConnected  -= SR_OnWSReConnected;//+
                    sr.OnWSDisConnected -= SR_OnWSDisConnected; ;//+
                    sr.OnWSReciveToken  -= SR_OnWSReciveToken;//+
                    sr.OnWSRecivePong   -= SR_OnWSRecivePong;
                    sr.OnWSReciveTokenClosed -= SR_OnWSReciveTokenClosed;
                    sr.OnWSRecive -= SR_OnWSRecive;
                    sr.OnWSReciveChatEvent -= SR_OnWSReciveChatEvent;
                    sr.OnWSReciveStatisicUpdate -= Sr_OnWSReciveStatisicUpdate;

                    sr.OnReciveChatList -= Sr_OnReciveChatList;
                    sr.OnReciveChatAppend -= Sr_OnReciveChatAppend; ;
                    sr.OnReciveMessagePageList -= Sr_ReciveMessagePageList;
                    sr.OnReciveMessage -= Sr_OnReciveMessage; ;

                    sr.OnReciveMessageAppend -= Sr_OnReciveMessageAppend;
                    sr.OnReciveMessageStatus -= Sr_OnReciveMessageStatus;
                    sr.OnReciveMessageAllStatus -= Sr_OnReciveMessageAllStatus;
                    sr.OnReciveUserList -= Sr_OnReciveUserList;
                    sr.OnResultReciveUserParams -= Sr_OnResultReciveUserParams;
                    sr.OnReciveChatUserLeave -= Sr_OnReciveChatUserLeave;
                    sr.OnReciveMessageUpdate -= Sr_OnReciveMessageUpdate;
                    sr.OnLogConnecttionAdd -= SR_OnLogConnecttionAdd;
                    App.ddd.SR.Disconnect();
                    App.ddd.SR = null;
                    sr = null;

                }
            }
        }


        private void SR_OnLogConnecttionAdd(ConnectionLog log)
        {
            App.ddd.LogConnecttionAdd(log);
        }

        private void Sr_OnReciveMessage(int ChatId, string json)
        {

            try
            {
                b_StatisticChanged_NeedUpdate = true;
                ObjMsg[] list = System.Text.Json.JsonSerializer.Deserialize<ObjMsg[]>(json);
                if (list != null && list.Length > 0)
                {
                    App.ddd.ReciveMessage(list); // https://docs.microsoft.com/ru-ru/dotnet/standard/serialization/system-text-json-how-to
                }
            }
            catch (Exception err)
            {
            }
        }

        private void Sr_OnReciveMessageUpdate(int ObjId, string json)
        {
            try
            {
                Models.Obj objtemp = System.Text.Json.JsonSerializer.Deserialize<Models.Obj>(json);

                if (objtemp != null)

                {

                    App.ddd.ReciveMessageUpdate(ObjId, objtemp); // https://docs.microsoft.com/ru-ru/dotnet/standard/serialization/system-text-json-how-to
                }
            }
            catch (Exception err)
            {
            }
        }

        private void Sr_OnReciveChatUserLeave(int ChatId, int userid)
        {
            App.ddd.ReciveChatUserLeave(ChatId, userid);
        }

        private void Sr_OnResultReciveUserParams(int UserId, string jsonResult)
        {
         //   if (App.ddd.UserId == UserId)
            {
                try
                {
                    App.ddd.SelfParams = System.Text.Json.JsonSerializer.Deserialize<ViewGetUserParameters[]>(jsonResult);

                    if (App.ddd.SelfParams.Where(s => s.SgValueType == 57).Any())//.StartsWith("Отображение меню Парковка")).Any()
                    {
                        App.ddd.b_IsPPSEnabled = true;
                        /*MainPage mp = app.MainPage as MainPage;
                        if (mp!=null)
                            mp.RemobePPSPage();
                            */
                    }
                }
                catch (Exception err)
                {
                }
            }
        }

        private void Sr_OnReciveChatAppend(int ChatId, string json)
        { 
            try
            {
                Models.Obj   objtemp = System.Text.Json.JsonSerializer.Deserialize<Models.Obj>(json);

                if (objtemp != null)

                {
                    
                    App.ddd.ReciveChatAppend(ChatId, objtemp ); // https://docs.microsoft.com/ru-ru/dotnet/standard/serialization/system-text-json-how-to
                }
            }
            catch (Exception err)
            {
            }
        }

        private void Sr_OnReciveUserList(int maxVers, int nPage, string json)
        {
            try
            {
                Ji.ClassSR.User [] objtemp = System.Text.Json.JsonSerializer.Deserialize<Ji.ClassSR.User []>(json);
                
                if (objtemp != null)

                {
                    UserChat[] uc = objtemp.Select(s => new UserChat() { UserId = s.UserId, FIO = s.UserName, Params = s.Params
                    , Skill = s.positions.FirstOrDefault() != null ? s.positions.FirstOrDefault().Position : ""
                    , OU = s.positions.FirstOrDefault() != null ? s.positions.FirstOrDefault().Subdiv : ""
                    , bFavorite = s.isFavorite
                    , PersonalChatId = s.PersonalChatId
                    , UserPositions = s.positions.ToList()
                    , OUMini = s.positions.FirstOrDefault() != null ? s.positions.FirstOrDefault().MainAbbriv : ""
                    }).ToArray();
          //        App.ddd.UsersList = uc.ToList();
                    App.ddd.ReciveUserList(maxVers, nPage, uc); // https://docs.microsoft.com/ru-ru/dotnet/standard/serialization/system-text-json-how-to
                }
            }
            catch (Exception err)
            {
            }
        }

        public class tObjRes
        {
            public int objid { get; set; }
            public int st { get; set; }
        };


        private void Sr_OnReciveMessageAllStatus(int ChatId, int ObjId, string json)
        {
            try
            {
                b_StatisticChanged_NeedUpdate = true;
                {
                    ClassSR.ObjStatus[] status = System.Text.Json.JsonSerializer.Deserialize<ClassSR.ObjStatus[]>(json);
                    App.ddd.ReciveMessageAllStatus(ChatId, ObjId, status); // https://docs.microsoft.com/ru-ru/dotnet/standard/serialization/system-text-json-how-to

                }

            }
            catch (Exception err)
            {
            }
        }

        private void Sr_OnReciveMessageStatus(int ChatId, string json)
        {
            try
            {
                b_StatisticChanged_NeedUpdate = true;
                //string json = "{\"objid\":" + msgs[0].ToString() + ",st=14}";
                //    List<Models.Obj> chatlist = System.Text.Json.JsonSerializer.Deserialize<List<Models.Obj>  > (json);

              
                 tObjRes objtemp = System.Text.Json.JsonSerializer.Deserialize<tObjRes>(json);

                if (objtemp != null)
                {

                    App.ddd.ReciveMessageStatus(objtemp.objid, objtemp.st); // https://docs.microsoft.com/ru-ru/dotnet/standard/serialization/system-text-json-how-to
                }
 
            }
            catch (Exception err)
            {
            }
        }
       
        private void Sr_OnReciveMessageAppend(int ChatId, string json)
        {
            try
            {
                b_StatisticChanged_NeedUpdate = true;
                //    List<Models.Obj> chatlist = System.Text.Json.JsonSerializer.Deserialize<List<Models.Obj>  > (json);
                ObjMsg [] chatlist = System.Text.Json.JsonSerializer.Deserialize<ObjMsg []>(json);
              
                if (chatlist != null && chatlist.Length>=1)
                {
                    App.ddd.ReciveMessageAppend(chatlist[0]); // https://docs.microsoft.com/ru-ru/dotnet/standard/serialization/system-text-json-how-to
                }
            }
            catch (Exception err)
            {
            }
        }

        private void Ddd_OnReciveChatList(Models.Obj[] chatlist)
        {
            
        }

        private void Sr_OnReciveChatList(int ChatId, string json)
        {
            try
            {
                b_StatisticChanged_NeedUpdate = true;
             //    List<Models.Obj> chatlist = System.Text.Json.JsonSerializer.Deserialize<List<Models.Obj>  > (json);
               Models.Obj[] chatlist = System.Text.Json.JsonSerializer.Deserialize<Models.Obj[]>(json);
                //ResultObject  res = System.Text.Json.JsonSerializer.Deserialize<ResultObject>(json
                    
                //    );
                //ResultObject res2 = Newtonsoft.Json.JsonConvert. DeserializeObject<ResultObject>(json
                //    , new JsonSerializerSettings
                //    {
                //        MissingMemberHandling = MissingMemberHandling.Ignore
                //    }
                //   );

                if (chatlist != null)
                {
                    App.ddd.ReciveChatList (chatlist); // https://docs.microsoft.com/ru-ru/dotnet/standard/serialization/system-text-json-how-to
                }
            }
            catch (Exception err)
            {
            }
        }

        private void Sr_OnWSReciveStatisicUpdate(int ChatId, string json)
        {
          
            try
            {
                if (json == null)
                    return;
                b_StatisticChanged_NeedUpdate = true;
                ViewUserStatistic v = System.Text.Json.JsonSerializer.Deserialize<ViewUserStatistic>(json);
                ChatStatisticUser stat = new ChatStatisticUser(v);
           /*     {
                    ChatId = v.ChatId,
                    UserId = v.UserId,
                    StartShownObjId = v.StartShownObjId,
                    LastObjId = v.LastObjId,
                    CountNew = v.CountNew,
                    LastShownObjId = v.LastShownObjId,
                    PageLastObj = v.LastObjPage.Value,
                    PageLastShownObj = v.LastShownPage.Value,
                    LastObjIdDateCreate = v.LastObjDate
                };*/
                    //System.Text.Json.JsonSerializer.Deserialize<ChatStatisticUser>(json);

                if (stat != null)
                {
                    App.ddd.ReciveStatisticUpdate(stat); // https://docs.microsoft.com/ru-ru/dotnet/standard/serialization/system-text-json-how-to
                }
            }
            catch (Exception err)
            {
            }
        }

        private void Sr_ReciveMessagePageList(int ChatId, string json)
        {

            try
            {
                b_StatisticChanged_NeedUpdate = true;
                if (json == "--")
                {
                    App.ddd.ReciveMessagePageList(null); // https://docs.microsoft.com/ru-ru/dotnet/standard/serialization/system-text-json-how-to
                    return;
                }
                ObjMsg[] list  = System.Text.Json.JsonSerializer.Deserialize< ObjMsg[]> (json);
                if (list != null && list.Length>0)
                {
                    App.ddd.ReciveMessagePageList(list); // https://docs.microsoft.com/ru-ru/dotnet/standard/serialization/system-text-json-how-to
                }
            }
            catch (Exception err)
            {
            }
        }

        //private void SR_OnWSReciveNewChatStatistic(X_SignalR sender, int ChatId)
        //{
        //    b_StatisticChanged_NeedUpdate = true;
        //}

        private void SR_OnWSReciveChatEvent(X_SignalR sender, string Msg)
        {
            
        }

        private void SR_OnWSRecive(X_SignalR sender, WS_EventType type, string Msg)
        {
            throw new NotImplementedException();
        }

        private void SR_OnWSReciveTokenClosed(X_SignalR sender)
        {
           
        }

        private void SR_OnWSRecivePong(X_SignalR sender)
        {
             
        }

        private async void SR_OnWSReciveToken(X_SignalR sender, string tokenSeance)
        {
            //App.ddd.connectInterface.TokenReqId = tokenSeance;
            App.ddd.connectInterface.TokenSeanceId = tokenSeance;
            try
            {
                //_ = sender.Request_Chat_Statisic(7160);
                //_ = sender.Request_Chat_List(0);
           
                await App.ddd.SR.Request_UserParams(App.ddd.UserId);//App.ddd.UserId




                appFire = FirebaseApp.InitializeApp(Android.App.Application.Context);
                //          var tttt = FirebaseInstanceId.Instance.GetInstanceId();
                if (String.IsNullOrEmpty(FirebaseInstanceId.Instance?.Token) == false)
                {
                    
                    App.ddd.Register_FB(FirebaseInstanceId.Instance?.Token?.ToString());
                }
                App.ddd.statusConect = StatusConected.Open;
                return;
                //   ds = new ChatDataStore();
                if (App.ddd.connectInterface != null && App.ddd.connectInterface.isSetup())
                {
                    //       Droid.JI_WS_connect.connect c = new JI_WS_connect.connect();
                    try
                    {
                        Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();
                        g.Url = App.ddd.connectInterface.Server_SOAP;
                        string result = g.About();
                        App.ddd.SOAP_AboutVersion = result;
                    }
                    catch (Exception err)
                    {

                    }
                    #region MyRegion

                    //try
                    //{
                    //    //     ClientLaunchAsync();

                    //    /* client = new ClientWebSocket();

                    //     //client.ConnectAsync(new Uri("ws://10.0.2.2/xml/ChatHandler.ashx"), cts);
                    //     //ConnectToServerAsync();
                    //     client.ConnectAsync(new Uri(App.ddd.connectInterface.Server_WS ), cts);//"ws://10.0.2.2/xml/ChatHandler.ashx"

                    //     //  Thread.Sleep(1000);
                    //     client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("ping")), WebSocketMessageType.Text, true, cts);
                    //     //SendMessageAsync("ping");
                    //     */
                    //}
                    //catch (Exception err)
                    //{

                    //} 
                    #endregion
                }

         //   App.ddd.
           //     User = OnUser_GetSelf();
                App.ddd.statusConect = StatusConected.Open;
                try
                {
                    if (appFire == null)
                    {
                        //Android.App.Application.Context
                        appFire = FirebaseApp.InitializeApp(Android.App.Application.Context);
                        //          var tttt = FirebaseInstanceId.Instance.GetInstanceId();
                        if (String.IsNullOrEmpty(FirebaseInstanceId.Instance?.Token) == false)
                        {
                            App.ddd.Register_FB(FirebaseInstanceId.Instance?.Token?.ToString());
                        }
                        else
                        {
                        }
                    }
                }
                catch (Exception err)
                {
                }
                try
                {
                    // if (String.IsNullOrEmpty(FirebaseInstanceId.Instance?.Token) == false)
                    {
                        string[] SelfUserParams = GetUserParams();
                    }
                }
                catch (Exception err)
                {
                }
                /*
                try
                {
                    if (String.IsNullOrEmpty(FirebaseInstanceId.Instance?.Token) == false)
                    {
                        App.ddd.Register_FB(FirebaseInstanceId.Instance?.Token?.ToString());
                    }
                }
                catch (Exception err)
                {
                }
                try
                {
                    if (String.IsNullOrEmpty(FirebaseInstanceId.Instance?.Token) == false)
                    {
                        App.ddd.Register_FB(FirebaseInstanceId.Instance?.Token?.ToString());
                    }
                }
                catch (Exception err)
                {
                }*/
            }
            catch (Exception err)
            {

            }
        }

        private void SR_OnWSDisConnected(X_SignalR sender, Exception err)
        {
            try
            {
             //   if (App.ddd.statusConectWS != StatusConected.Close)
                    App.ddd.statusConect = StatusConected.Close;
            }
            catch (Exception errThis)
            {

            }
        }

        private async void  SR_OnWSConnected(X_SignalR sender)
        {
            try
            {

                if (String.IsNullOrEmpty(App.ddd.connectInterface.TokenSeanceId))
                {
                    string cmd = App.ddd.RequestTokenSeanceString();
                    await App.ddd.SR.RequestTokenNow(cmd);
                }
                //    App.ddd.RequestTokenSeance();
            //    App.ddd.statusConect = StatusConected.Open;

                if (TemplateMainPage != null)
                {
                    app.MainPage = TemplateMainPage;
                    TemplateMainPage = null;
                }
            }
            catch (Exception err)
            {

            }

        }
        private async void SR_OnWSReConnected(X_SignalR sender)
        {
            try
            {

                //if (String.IsNullOrEmpty(App.ddd.connectInterface.TokenSeanceId))
                {
                    string cmd = App.ddd.RequestTokenSeanceString();
                    await App.ddd.SR.RequestTokenNow(cmd);
                }
               
            }
            catch (Exception err)
            {

            }

        }

 //       private void InitMainApp()
 //       {
 //           return;
 //           Ji.X_WS ws = new Ji.X_WS();
 //           App.ddd.WS = ws;
 //           //App.ddd.deviceIn = new AndroidApplicationInfo();
 //           //     App.ddd.Fill();

 //           ws.OnWSConnected += Ws_OnWSConnected;
 //           ws.OnWSDisConnected += Ws_OnWSDisConnected; ;
 //           ws.OnWSReciveToken += Ws_OnWSReciveToken;
 //           ws.OnWSRecivePong += Ws_OnWSRecivePong;
 //           ws.OnWSReciveTokenClosed += Ws_OnWSReciveTokenClosed;
 //           ws.OnWSRecive += Ws_OnWSRecive;
 //           ws.OnWSReciveChatEvent += Ws_OnWSReciveChatEvent;
 //           ws.OnWSReciveNewChatStatistic += OnWSReciveNewChatStatistic;


            

 //           App.ddd.OnRegister_FB += OnRegister_FB;

 //           App.ddd.OnGetChatList += OnGetChatList;
 //           App.ddd.OnChatGetOne += OnChatGetOne;
 //           App.ddd.OnChat_Leave += OnChatLeave;
 //           App.ddd.OnChat_CreateAndSubscribe += OnChat_CreateAndSubscribe;
 //           App.ddd.OnChat_GetMyStatistic += OnChat_GetMyStatistic;
 //           App.ddd.OnChat_GetStatistic += OnChat_Statistic;
 //           App.ddd.OnChat_SubscribeUser += OnChat_SubscribeUser; ;
 //           App.ddd.OnChat_UnSubscribeUser += OnChat_UnSubscribeUser; ;
 //           App.ddd.OnSetupParam_Load += OnSetupParam_Load;

 //           //        App.ddd.OnNewPageRecive += OnNewPageRecive;

 //           App.ddd.OnGetUserList += OnGetUserList;

 //           App.ddd.OnMessage_GetPage += OnMessage_GetPage;
 //           App.ddd.OnMessage_Send += OnMessage_Send;
 //           App.ddd.OnMessageFile_Send += OnMessageFile_Send;
 //           App.ddd.OnMessage_Shown += OnMessage_Shown;

 //           App.ddd.OnParking_Step1 += OnParking_Step1;
 //           App.ddd.OnParking_Step2 += OnParking_Step2;
 //           App.ddd.OnChat_Create_Public += OnChat_Create_Public;

          


 //           /*
 //           if (ws.Url != "")
 //               ws.Open();
 //*/

 //       }

      

        bool b_StatisticChanged_NeedUpdate =false;
        private void OnWSReciveNewChatStatistic(X_WS sender, int ChatId)
        {
            b_StatisticChanged_NeedUpdate = true;
        }

        private string[] OnSetupParam_Load()
        {
            try
            {

                Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();
                g.Url = App.ddd.connectInterface.Server_SOAP;
                //                g.Setup_Params(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, "");
                return g.Setup_Params("");

            }
            catch (Exception err)
            {

            }
            return null;
        }

        private void OnChat_UnSubscribeUser(int ChatId, int WhoDeleteUserId, int id)
        {
            try
            {
                if (ChatId > 0)
                {
                    Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();
                    g.Url = App.ddd.connectInterface.Server_SOAP;
                    g.Chat_UnSubscribeUser(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, ChatId, id, 21);
                }
            }
            catch (Exception err)
            {

            }
            return;

        }

        private void OnChat_SubscribeUser(int ChatId, int WhoDeleteUserId, int id)
        {
            try
            {
                if (ChatId > 0)
                {
                    Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();
                    g.Url = App.ddd.connectInterface.Server_SOAP;
                    g.Chat_SubscribeUser(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, ChatId, id, 21);
                }
            }
            catch (Exception err)
            {

            }
            return;
        }

        private void Ws_OnWSReciveChatEvent(X_WS sender, string Msg)
        {

        }

        private async Task<bool> OnRegister_FB(string token)
        {
            try
            {

                if (String.IsNullOrEmpty(token))
                    return false;

                await SecureStorage.SetAsync("TokenFB", token);
                await SecureStorage.SetAsync("TokenFB_Send", "false");


                await App.ddd.SR.Register_Param("FBToken", token.ToString());
                /*
                Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();

                g.Url = App.ddd.connectInterface.Server_SOAP;
                g.Register_FB(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, token);
               */
                await SecureStorage.SetAsync("TokenFB_Send", "true");
                
                return true;
            }
            catch (Exception err)
            {

            }
            return false;
        }

        private void OnStatistic_Changed(DateTime oldLastVersion)
        {
         //   throw new NotImplementedException();
        }

        ConcurrentBag<int> ShownObjId = new ConcurrentBag<int>();
        int _ChatId_ForShownList;
        private int ChatId_ForShownList
        {
            get { return _ChatId_ForShownList; }
            set
            {
                if (_ChatId_ForShownList != 0 && _ChatId_ForShownList != value)
                    SendShownMessagesChatNow();
                _ChatId_ForShownList = value;
            }
        }

        object lockAdd= new object();
        
        private void OnMessage_Shown(int ChatId,int ObjId, int UserId)
        {
            try
            {
                lock (lockAdd)
                {

                    ChatId_ForShownList = ChatId;
                    ShownObjId.Add(ObjId);
                    /*if (ShownObjId.Count < 10)
                        return;*/
                  
                   
                    //return res.Select(s => new Models.GroupChat()
                    //{
                    //    Text = s.xml,
                    //    ObjId = (int)s.ObjId,
                    //    TypeId = s.sgTypeId,
                    //    period = new Period() { dtb = s.period.dtb, dte = s.period.dte, dtc = s.period.dtc, dtd = s.period.dtd }
                    //  ,
                    //    CountNewMessage = ""
                    //}).FirstOrDefault();
                }
            }
            catch (Exception err)
            {

            }
            return;
        }

        GetUserInfo MainGetUserInfo = null;
        private GetUserInfo GetUserInfo()
        {
            if (MainGetUserInfo==null)
                MainGetUserInfo = new JI_WS.GetUserInfo();
            return MainGetUserInfo;
        }

        private GroupChat OnChatGetOne(int ChatId)
        {
            try
            {


                Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();

                g.Url = App.ddd.connectInterface.Server_SOAP;
                //JI_WS.Obj[] list = g.Chat_GetList(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, 0);
                var res = g.Chat_Get(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, ChatId);
                return res.Select(s => new Models.GroupChat()
                {
                    Text = s.xml,
                    ObjId = (int)s.ObjId,
                    TypeId = (MsgObjType)s.sgTypeId,
                    period = new Period() { dtb = s.period.dtb, dte = s.period.dte, dtc = s.period.dtc, dtd = s.period.dtd }
                  ,
                    CountNewMessage = ""
                }).FirstOrDefault();
          
            }
            catch (Exception err)
            {

            }
            return null;
        }



        private string [] GetUserParams()
        {
            try
            {
                Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();

                g.Url = App.ddd.connectInterface.Server_SOAP;
                //JI_WS.Obj[] list = g.Chat_GetList(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, 0);
                var res = g.User_GetParameters(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, App.ddd.UserId);
                if (res.Where(s=>s.sgValueType==57).Any())//.StartsWith("Отображение меню Парковка")).Any()
                {
                    App.ddd.b_IsPPSEnabled = true;
                    /*MainPage mp = app.MainPage as MainPage;
                    if (mp!=null)
                        mp.RemobePPSPage();
                        */
                }

            }
            catch (Exception err)
            {

            }
            return null;
        }


        private async Task OnChatLeave(int ChatId)
        {
            try
            {

                await App.ddd.SR.Request_Chat_Leave(ChatId);
                //Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();

                //g.Url = App.ddd.connectInterface.Server_SOAP;
                ////JI_WS.Obj[] list = g.Chat_GetList(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, 0);
                //var res = g.Chat_Leave(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, ChatId);
                //return true;

            }
            catch (Exception err)
            {

            }
          //  return false;
        }

        private UserChat OnUser_GetSelf()
        {
            try
            {

                Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();

                g.Url = App.ddd.connectInterface.Server_SOAP;
                //JI_WS.Obj[] list = g.Chat_GetList(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, 0);
                var s = g.User_GetSelf(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter);
                if (s != null)
                {
                    Models.UserChat u = new Models.UserChat()
                    {
                        FIO = s.UserName,
                        UserId = s.UserId,
                        Phones = s.UserLogin
                    };

                    try
                    {
                        u.Skill = s.positions != null ? s.positions.Where(s => s.Period.dte == null).FirstOrDefault()?.Position : "Должность не задана";
                    }
                    catch (Exception err) { u.Skill = "Должность не задана"; }
                    try
                    {
                      u.OU =   (s.positions != null 
                            && s.positions.Where(s => s.Period.dte == null).FirstOrDefault()?.Subdiv!=null)
                            ? s.positions.Where(s => s.Period.dte == null).FirstOrDefault()?.Subdiv : "Управление не задано";
                    }
                    catch (Exception err) { u.Skill = "Управление не задано"; }

                    return u;
                }

                

            }
            catch (Exception err)
            {

            }
            return null;
        }

        private int OnChat_CreateAndSubscribe(int UserId, string Name)
        {
            try
            {
            //    Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();

                int[] users = new int[] { UserId };

                int ParentChatId = 0;//SgnId	Name7   Общий чат 8   Приватный чат9   Доска обьявлений10  "Стол заказов"15  Избранное 30  Корпоративный чат32  Папка43  Блокнот
                int TypeChatId = 8;//SgnId	Name7   Общий чат 8   Приватный чат9   Доска обьявлений10  "Стол заказов"15  Избранное 30  Корпоративный чат32  Папка43  Блокнот
                  App.ddd.SR.Request_Chat_Create(ParentChatId, TypeChatId, Name, "Приватный чат для общения 2х человек", users);
                //         g.Url = App.ddd.connectInterface.Server_SOAP;
                //JI_WS.Obj[] list = g.Chat_GetList(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, 0);
                //     int res = g.Chat_CreateAndSubscribe(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, -1, 8, Name, "Приватный чат для общения 2х человек", users);
                //    return res;
                return 0;
            }
            catch (Exception err)
            {

            }
            return -1;
        }

        /// <summary>
        /// Создаёт и подписывает в чат
        /// </summary>
        /// <param name="ParentChatId">Родитель в дереве чатов</param>
        /// <param name="Name"></param>
        /// <param name="Comment"></param>
        /// <param name="usersList"></param>
        /// <returns></returns>
        private async Task OnChat_Create_Public(int ParentChatId, string Name,string Comment,int [] usersList)
        {
            try
            {
                //int ParentChatId = 0;//SgnId	Name7   Общий чат 8   Приватный чат9   Доска обьявлений10  "Стол заказов"15  Избранное 30  Корпоративный чат32  Папка43  Блокнот
                int TypeChatId = 7;//SgnId	Name7   Общий чат 8   Приватный чат9   Доска обьявлений10  "Стол заказов"15  Избранное 30  Корпоративный чат32  Папка43  Блокнот

                await App.ddd.SR.Request_Chat_Create(ParentChatId, TypeChatId, Name, "Публичный чат", usersList);
                //App.ddd.SR.Request_Chat_Create(
                /*
                Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();
                g.Url = App.ddd.connectInterface.Server_SOAP;
                int res = g.Chat_CreateAndSubscribe(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, ParentChatId , 7, Name, Comment, usersList);
                return res;
               */
            }
            catch (Exception err)
            {

            }
            //return -1;
        }
        private bool OnMessageFile_Send(int ChatId, string MessageText , byte [] file)
        {
            try
            {
/*
                if (App.ddd.SR != null)
                {
                    App.ddd.SR.Request_Message_SendFile(ChatId, file, MessageText);
                }
                */
                Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();


                g.Url = App.ddd.connectInterface.Server_SOAP;
                //JI_WS.Obj[] list = g.Chat_GetList(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, 0);
                g.Message_Image_Add(1255,App.ddd.UserId, ChatId,file, "jpg", "Изображение" );
                
                return true;
            }
            catch (Exception err)
            {

            }
            return false;
        }
        private bool OnMessage_Send(int ChatId, string MessageText)
        {
            try
            {
                Lockal_MessageSendList_Add(ChatId, MessageText);
                /*
                Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();
                g.Url = App.ddd.connectInterface.Server_SOAP;
                //JI_WS.Obj[] list = g.Chat_GetList(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, 0);
                g.Message_AddAsync(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, ChatId, MessageText);
                */
                return true;
            }
            catch (Exception err)
            {

            }
            return false;
        }


        private ObjMsg[] OnMessage_GetPage(int ChatId, int nPage, bool bReload = false)
        {
            bool b_Find = false;
            if (bReload == false && App.ddd.DB_Page_isExists(ChatId, nPage))
            {
                b_Find = true;
                ObjMsg[] msgs = App.ddd.DB_MessageGetPage(ChatId, nPage);
                //ObjMsg[] msgs = stat.ListMessages.Select(s => new ObjMsg()
                //{
                //    ObjId = (int)s.ObjId,
                //    Data = s.xml,
                //    userCreater = s.userCreater,
                //    PageNum = s.PageNum,
                //    MessageNum = s.MessageNum,
                //    userid = s.userid,
                //    Type = s.Type,
                //    sgTypeId = s.sgTypeId,
                //    sgClassId = s.sgClassId,
                //    DateCreate = s.period.dtc.Value,
                //}).ToArray();

                return msgs;
            }

            Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();

            if (false)
            try
            {
                g.Url = App.ddd.connectInterface.Server_SOAP;
                /*
                for (int i = 0; i <= 200; i++)
                {
                    DateTime db = DateTime.Now;
                    //JI_WS.Obj[] list = g.Chat_GetList(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, 0);
                    var msgs1 = g.Message_GetPage(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, ChatId, nPage);
                    //                return list.Select(s => new Models.ObjMsg() { Text = s.xml, ObjId = (int)s.ObjId, TypeId = s.sgTypeId, period = new Period() { dtb = s.period.dtb, dte = s.period.dte, dtc = s.period.dtc, dtd = s.period.dtd } }).ToList();//.Cast<GroupChat>();
                    //   return list.Select(s => new Models.ObjMsg() { Data=s.xml , ObjId = (int)s.ObjId }).ToList();//.Cast<GroupChat>();
                    DateTime de = DateTime.Now;
                    Console.WriteLine("Message_GetPage " + nPage.ToString() + "=" + ((de - db).TotalMilliseconds.ToString()));
                    //    Thread.Sleep(500);
                }

                */


                //JI_WS.Obj[] list = g.Chat_GetList(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, 0);
                g.Message_GetPageCompleted += G_Message_GetPageCompleted;
              
                g.Message_GetPageAsync(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, ChatId, nPage);
                //                return list.Select(s => new Models.ObjMsg() { Text = s.xml, ObjId = (int)s.ObjId, TypeId = s.sgTypeId, period = new Period() { dtb = s.period.dtb, dte = s.period.dte, dtc = s.period.dtc, dtd = s.period.dtd } }).ToList();//.Cast<GroupChat>();
                //   return list.Select(s => new Models.ObjMsg() { Data=s.xml , ObjId = (int)s.ObjId }).ToList();//.Cast<GroupChat>();


                return null;
                /*
                return new ChatStatisticUser()
                {
                    ChatId = stat.ChatId,
                    ChatUserInfoId = stat.ChatUserInfoId,
                    CountNew = stat.CountNew,
                    CountShownEndObjId = stat.CountShownEndObjId,
                    LastObjId = stat.LastObjId,
                    LastShownObjId = stat.LastShownObjId,
                    StartShownObjId = stat.StartShownObjId,
                    UserId = stat.UserId
                };*/
            }
            catch (Exception err)
            {//System.Net.WebExceptionStatus

            }
            return null;
        }

        private void G_Message_GetPageCompleted(object sender, Message_GetPageCompletedEventArgs e)
        {
            try
            {
                JI_WS.asyncReturn_Messages stat = e.Result;

                if (e.Result.ErrorCount == 0)
                {
                    ObjMsg[] msgs = stat.ListMessages.Select(s => new ObjMsg()
                    {
                        ObjId = (int)s.ObjId,
                        Data = s.xml,
                        userCreater = s.userCreater,
                        PageNum = s.PageNum,
                        MessageNum = s.MessageNum,
                        userid = s.userid,
                        Type = s.Type,
                        sgTypeId = s.sgTypeId,
                        sgClassId = s.sgClassId,
                        DateCreate = s.period.dtc.Value,
                        ShownState = s.ShownState
                    }).ToArray();


                    OnNewPageRecive(msgs);
                }
            }
            catch (Exception err)
            {

            }
      /*      if (stat != null && stat.ErrorCount > 0)
            {

            }
            else
                    if (stat != null && stat.ListMessages.Length > 0)
            {
                try
                {
                    ObjMsg[] msgs = stat.ListMessages.Select(s => new ObjMsg()
                    {
                        ObjId = (int)s.ObjId,
                        Data = s.xml,
                        userCreater = s.userCreater,
                        PageNum = s.PageNum,
                        MessageNum = s.MessageNum,
                        userid = s.userid,
                        Type = s.Type,
                        sgTypeId = s.sgTypeId,
                        sgClassId = s.sgClassId,
                        DateCreate = s.period.dtc.Value,
                        ShownState = s.ShownState
                    }).ToArray();
                    return msgs;
                }
                catch (Exception err)

                {
                    return null;
                }
                //b_Find
                //     App.ddd.DB_MessageSavePage(ChatId,nPage,  msgs);

            }
            else
            {
                //  "Соединение разовано";
            }*/
        }

        private void OnNewPageRecive(ObjMsg[] msgs)
        {
            App.ddd.NewPageRecive(msgs);
        }

        private ObjMsg[] OnMessage_GetPage_Old(int ChatId, int nPage, bool bReload = false)
        {
            bool b_Find = false;
            if (bReload == false && App.ddd.DB_Page_isExists(ChatId, nPage))
            {
                b_Find = true;
                ObjMsg[] msgs  = App.ddd.DB_MessageGetPage(ChatId, nPage);
                //ObjMsg[] msgs = stat.ListMessages.Select(s => new ObjMsg()
                //{
                //    ObjId = (int)s.ObjId,
                //    Data = s.xml,
                //    userCreater = s.userCreater,
                //    PageNum = s.PageNum,
                //    MessageNum = s.MessageNum,
                //    userid = s.userid,
                //    Type = s.Type,
                //    sgTypeId = s.sgTypeId,
                //    sgClassId = s.sgClassId,
                //    DateCreate = s.period.dtc.Value,
                //}).ToArray();

                return msgs;
            }

            Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();

            try
            {
                g.Url = App.ddd.connectInterface.Server_SOAP;
                /*
                for (int i = 0; i <= 200; i++)
                {
                    DateTime db = DateTime.Now;
                    //JI_WS.Obj[] list = g.Chat_GetList(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, 0);
                    var msgs1 = g.Message_GetPage(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, ChatId, nPage);
                    //                return list.Select(s => new Models.ObjMsg() { Text = s.xml, ObjId = (int)s.ObjId, TypeId = s.sgTypeId, period = new Period() { dtb = s.period.dtb, dte = s.period.dte, dtc = s.period.dtc, dtd = s.period.dtd } }).ToList();//.Cast<GroupChat>();
                    //   return list.Select(s => new Models.ObjMsg() { Data=s.xml , ObjId = (int)s.ObjId }).ToList();//.Cast<GroupChat>();
                    DateTime de = DateTime.Now;
                    Console.WriteLine("Message_GetPage " + nPage.ToString() + "=" + ((de - db).TotalMilliseconds.ToString()));
                    //    Thread.Sleep(500);
                }

                */


                //JI_WS.Obj[] list = g.Chat_GetList(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, 0);
                JI_WS.asyncReturn_Messages stat = g.Message_GetPage(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, ChatId, nPage);
                //                return list.Select(s => new Models.ObjMsg() { Text = s.xml, ObjId = (int)s.ObjId, TypeId = s.sgTypeId, period = new Period() { dtb = s.period.dtb, dte = s.period.dte, dtc = s.period.dtc, dtd = s.period.dtd } }).ToList();//.Cast<GroupChat>();
                //   return list.Select(s => new Models.ObjMsg() { Data=s.xml , ObjId = (int)s.ObjId }).ToList();//.Cast<GroupChat>();
                if (stat != null && stat.ErrorCount > 0)
                {

                }
                else
                    if (stat != null && stat.ListMessages.Length > 0)
                    {
                        try
                        {
                            ObjMsg[] msgs = stat.ListMessages.Select(s => new ObjMsg()
                            {
                                ObjId = (int)s.ObjId,
                                Data = s.xml,
                                userCreater = s.userCreater,
                                PageNum = s.PageNum,
                                MessageNum = s.MessageNum,
                                userid = s.userid,
                                Type = s.Type,
                                sgTypeId = s.sgTypeId,
                                sgClassId = s.sgClassId,
                                DateCreate = s.period.dtc.Value,
                                ShownState = s.ShownState
                            }).ToArray();
                            return msgs;
                        }
                        catch (Exception err)

                        {
                            return null;
                        }
                        //b_Find
                        //     App.ddd.DB_MessageSavePage(ChatId,nPage,  msgs);

                    }
                    else
                    {
                        //  "Соединение разовано";
                    }
                 
                 
                /*
                return new ChatStatisticUser()
                {
                    ChatId = stat.ChatId,
                    ChatUserInfoId = stat.ChatUserInfoId,
                    CountNew = stat.CountNew,
                    CountShownEndObjId = stat.CountShownEndObjId,
                    LastObjId = stat.LastObjId,
                    LastShownObjId = stat.LastShownObjId,
                    StartShownObjId = stat.StartShownObjId,
                    UserId = stat.UserId
                };*/
            }
            catch (Exception err)
             {//System.Net.WebExceptionStatus

            }
            return null;
        }




        private void Request_GetMyStatistic(int chat_objId, JI_WS.Chat_GetMyStatisticCompletedEventHandler handlerMy)
        {
            Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();

            try
            {
                g.Url = App.ddd.connectInterface.Server_SOAP;
                //JI_WS.Obj[] list = g.Chat_GetList(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, 0);


                g.Chat_GetMyStatisticCompleted += handlerMy ;
                g.Chat_GetMyStatisticAsync(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, chat_objId);
                /*
                JI_WS.UserChatInfo stat = g.Chat_GetMyStatistic(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, chat_objId);
                //                return list.Select(s => new Models.ObjMsg() { Text = s.xml, ObjId = (int)s.ObjId, TypeId = s.sgTypeId, period = new Period() { dtb = s.period.dtb, dte = s.period.dte, dtc = s.period.dtc, dtd = s.period.dtd } }).ToList();//.Cast<GroupChat>();
                //   return list.Select(s => new Models.ObjMsg() { Data=s.xml , ObjId = (int)s.ObjId }).ToList();//.Cast<GroupChat>();
                return new ChatStatisticUser()
                {
                    ChatId = stat.ChatId,
                    ChatUserInfoId = stat.ChatUserInfoId,
                    CountNew = stat.CountNew,
                    CountShownEndObjId = stat.CountShownEndObjId,
                    LastObjId = stat.LastObjId,
                    LastShownObjId = stat.LastShownObjId,
                    StartShownObjId = stat.StartShownObjId,
                    UserId = stat.UserId
                    ,
                    PageLastObj = stat.PageLastObj
                    ,
                    PageLastShownObj = stat.PageLastShownObj
                    ,
                    dt_Statistic = DateTime.Now
                };*/
            }
            catch (Exception err)
            {

            }
           // return null;
        }

        private ChatStatisticUser [] OnChat_Statistic()
        {
            Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();

            try
            {
                g.Url = App.ddd.connectInterface.Server_SOAP;
                long countr = App.ddd.connectInterface.TokenSeance_Counter;
                var ret = g.Chat_GetStatistic(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), countr);
                if (ret.ErrorCount==0)
                {
                    if (ret.ListChatStatistic != null && ret.ListChatStatistic.Length>0)
                    {
                        return ret.ListChatStatistic.Select(s =>
                           new ChatStatisticUser()
                           {
                               ChatId = s.ChatId,
                               ChatUserInfoId = s.ChatUserInfoId,
                               CountNew = s.CountNew,
                               CountShownEndObjId = s.CountShownEndObjId,
                               LastObjId = s.LastObjId,
                               LastShownObjId = s.LastShownObjId,
                               StartShownObjId = s.StartShownObjId,
                               UserId = s.UserId,
                               PageLastObj = s.PageLastObj,
                               PageLastShownObj = s.PageLastShownObj,
                               LastObjIdDateCreate = s.LastObjIdDateCreate,
                               dt_Statistic = ret.dte
                           }
                            ).ToArray();
                    }
                    else
                    {

                    }
                }
              
            }
            catch (Exception err)
            {
                App.Log(err);
            }
            return null;
        }
        private ChatStatisticUser OnChat_GetMyStatistic(int chat_objId)
        {
            Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();

            try
            {
                g.Url = App.ddd.connectInterface.Server_SOAP;
                JI_WS.UserChatInfo stat = g.Chat_GetMyStatistic(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, chat_objId);
                return new ChatStatisticUser()
                {
                    ChatId = stat.ChatId,
                    ChatUserInfoId = stat.ChatUserInfoId,
                    CountNew = stat.CountNew,
                    CountShownEndObjId = stat.CountShownEndObjId,
                    LastObjId = stat.LastObjId,
                    LastShownObjId = stat.LastShownObjId,
                    StartShownObjId = stat.StartShownObjId,
                    UserId = stat.UserId
                    , PageLastObj = stat.PageLastObj
                    , PageLastShownObj = stat.PageLastShownObj
                    ,dt_Statistic = DateTime.Now
                };
            }
            catch (Exception err)
            {

            }
            return null;
        }
     


        //private void G_Chat_GetMyStatisticCompleted(object sender, JI_WS.Chat_GetMyStatisticCompletedEventArgs e)
        //{
        // if (e.Error   ==null && e.Result !=null)
        //    {
        //        JI_WS.UserChatInfo stat = e.Result;
        //        ChatStatisticUser chatstat = new ChatStatisticUser()
        //        {
        //            ChatId = stat.ChatId,
        //            ChatUserInfoId = stat.ChatUserInfoId,
        //            CountNew = stat.CountNew,
        //            CountShownEndObjId = stat.CountShownEndObjId,
        //            LastObjId = stat.LastObjId,
        //            LastShownObjId = stat.LastShownObjId,
        //            StartShownObjId = stat.StartShownObjId,
        //            UserId = stat.UserId
        //            ,
        //            PageLastObj = stat.PageLastObj
        //            ,
        //            PageLastShownObj = stat.PageLastShownObj
        //            ,
        //            dt_Statistic = DateTime.Now
        //        };

        //    }
        //}

        private ParkResult OnParking_Step2(string PassNumber, string Out_Who, string Out_CarType, string Out_CarNumber, string Out_Reason)
        {
            Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();

            try
            {
                g.Url = App.ddd.connectInterface.Server_SOAP;
                var list = g.Parking_CancelTiket(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, PassNumber, Out_Who, Out_CarType, Out_CarNumber, Out_Reason);
                if (list != null)
                {
                    return new ParkResult()
                    {
                        DateIn = list.DateIn,
                        DateOut = list.DateOut,
                        Out_CarNumber = list.Out_CarNumber,
                        Out_CarType = list.Out_CarType,
                        Out_Reason = list.Out_Reason,
                        Out_Who = list.Out_Who,
                        Out_Who_UserId = list.Out_Who_UserId,
                        ParkingNumber = list.ParkingNumber,
                        Place_In = list.Place_In
                          ,
                        Status = list.Status
                    };
                }
                return null;
            }
            catch (Exception err)
            {

            }
            return null;
        }

        private ParkResult OnParking_Step1(string PARKNUM)
        {
            Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();
            if (App.ddd.connectInterface == null || App.ddd.connectInterface.Server_SOAP == null)
            {
                throw new Exception("Нет идентификатора доступа. Выберите Меню->Сервер->Сканировать QR код");
            }
            try
            {
                g.Url = App.ddd.connectInterface.Server_SOAP;
                var list = g.Parking_Test1(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, PARKNUM);
                if (list != null)
                {
                    return new ParkResult()
                    {
                        DateIn = list.DateIn,
                        DateOut = list.DateOut,
                        Out_CarNumber = list.Out_CarNumber,
                        Out_CarType = list.Out_CarType,
                        Out_Reason = list.Out_Reason,
                        Out_Who = list.Out_Who,
                        Out_Who_UserId = list.Out_Who_UserId,
                        ParkingNumber = list.ParkingNumber,
                        Place_In = list.Place_In
                          ,
                        Status = list.Status
                    };
                }
                return null;
            }
            catch (Exception err)
            {

            }
            return null;
        }

        private System.Collections.Generic.List<Models.UserChat> OnGetUserList(long MaxVersion)
        {


            try
            {
                //   return new List<Models.UserChat> () { new Models.UserChat() { FIO = "dd", UserId = 1}, new Models.UserChat() { FIO = "assddsd", UserId = 2222} };
                if (App.ddd.UsersList == null || App.ddd.UsersList.Count==0)
                    App.ddd.SQL_LoadUsersList(MaxVersion);

                if (App.ddd.UsersList == null || App.ddd.UsersList.Count == 0)
                {
                    Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();
                    //    return null;
                    g.Url = App.ddd.connectInterface.Server_SOAP;
                    if (App.ddd.connectInterface.TokenSeanceId != null)
                    {
                        JI_WS.User[] list = g.User_GetListAll(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter);
                        if (list != null)
                        {
                            List<Models.UserChat> list2 = list.Select(s => new Models.UserChat()
                            {
                                FIO = s.UserName,
                                UserId = s.UserId,
                                Skill = s.positions.FirstOrDefault()?.Position,
                                OU = s.positions.FirstOrDefault()?.Subdiv,
                                Phones = s.UserLogin
                               ,
                                Params = s.Params
                            }).ToList();

                            App.ddd.UsersList = list2;//.OrderBy(s => s.FIO).ToList();
                        }
                    }
                }
                if (App.ddd.UsersList != null)
                {
                    MaxVersion = App.ddd.UsersList.Max(s => s.Version).Ticks;
                }
                return App.ddd.UsersList;//.Select(s => new Models.UserChat() { FIO = s.UserName, UserId = s.UserId }).ToList();
            }
            catch (Exception err)
            {

            }
            return null;
        }

        private System.Collections.Generic.List<Models.GroupChat> OnGetChatList(X_WS sender)
        {
            Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();

            try
            {
                g.Url = App.ddd.connectInterface.Server_SOAP;
                JI_WS.Obj[] list = g.Chat_GetList(Convert.ToInt64(App.ddd.connectInterface.TokenSeanceId), App.ddd.connectInterface.TokenSeance_Counter, 0);
                return list.Select(s => new Models.GroupChat()
                {
                    Text = s.xml,
                    ObjId = (int)s.ObjId,
                    TypeId = (MsgObjType)s.sgTypeId,
                    period = new Period() { dtb = s.period.dtb, dte = s.period.dte, dtc = s.period.dtc, dtd = s.period.dtd }
                   , CountNewMessage = ""
                   ,Description = "Описание не задано"
                   , UserList= s.UsersInChat
                }).ToList();//.Cast<GroupChat>();
            }
            catch (Exception err)
            {

            }
            return null;
        }

        Page TemplateMainPage;
        //private void Ws_OnWSDisConnected(X_WS sender, Exception err)
        //{
        //    try
        //    {
        //        if (App.ddd.statusConect != StatusConected.Close)
        //            App.ddd.statusConect = StatusConected.Close;
        //        //if (TemplateMainPage == null)
        //        //{
        //        //    try
        //        //    {
        //        //        TemplateMainPage = App.Current.MainPage;
        //        //    }
        //        //    catch (Exception err7)
        //        //    {

        //        //    }
        //        //    DisconnectedServerInfoPage pp = new DisconnectedServerInfoPage();
        //        //    app.MainPage = pp;
        //        //    //app.MainPage.Navigation.PopModalAsync(pp);
        //        //}
        //    }
        //    catch (Exception errThis)
        //    {

        //    }
        //}

        private void Ws_OnWSRecive(X_WS sender, WS_EventType type, String Msg)
        {
            try
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    App.ddd.Log_IncomeMsgAsync(Msg);
                }

                if (Msg.ToLower().StartsWith("chat"))
                {
                    App.ddd.Chat_ListRefresh(-1);
                    //   viewModel.LoadItemsCommand.Execute(null);
                }
            }
            catch (Exception err)
            {
                App.Log(err);
            }

        }

        private void Ws_OnWSReciveTokenClosed(X_WS sender)
        {

        }

        private void Ws_OnWSRecivePong(X_WS sender)
        {

        }

        [Obsolete]
        private void Ws_OnWSReciveToken(X_WS sender, string tokenSeance)
        {
            //App.ddd.connectInterface.TokenReqId = tokenSeance;
            App.ddd.connectInterface.TokenSeanceId = tokenSeance;
            try
            {


                //   ds = new ChatDataStore();
                if (App.ddd.connectInterface != null && App.ddd.connectInterface.isSetup())
                {
                    //       Droid.JI_WS_connect.connect c = new JI_WS_connect.connect();
                    try
                    {
                        Droid.JI_WS.GetUserInfo g = new JI_WS.GetUserInfo();
                        g.Url = App.ddd.connectInterface.Server_SOAP;
                        string result = g.About();
                        App.ddd.SOAP_AboutVersion = result;
                    }
                    catch (Exception err)
                    {

                    }
                    #region MyRegion

                    //try
                    //{
                    //    //     ClientLaunchAsync();

                    //    /* client = new ClientWebSocket();

                    //     //client.ConnectAsync(new Uri("ws://10.0.2.2/xml/ChatHandler.ashx"), cts);
                    //     //ConnectToServerAsync();
                    //     client.ConnectAsync(new Uri(App.ddd.connectInterface.Server_WS ), cts);//"ws://10.0.2.2/xml/ChatHandler.ashx"

                    //     //  Thread.Sleep(1000);
                    //     client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("ping")), WebSocketMessageType.Text, true, cts);
                    //     //SendMessageAsync("ping");
                    //     */
                    //}
                    //catch (Exception err)
                    //{

                    //} 
                    #endregion
                }

             //   App.ddd.SelfUser = OnUser_GetSelf();
                App.ddd.statusConect = StatusConected.Open;
                try
                {
                    if (appFire == null)
                    {
                        //Android.App.Application.Context
                        appFire = FirebaseApp.InitializeApp(Android.App.Application.Context);
                        //          var tttt = FirebaseInstanceId.Instance.GetInstanceId();
                        if (String.IsNullOrEmpty(FirebaseInstanceId.Instance?.Token) == false)
                        {
                            App.ddd.Register_FB(FirebaseInstanceId.Instance?.Token?.ToString());
                        }
                        else
                        {
                        }
                    }
                }
                catch (Exception err)
                {
                }
                try
                {
                   // if (String.IsNullOrEmpty(FirebaseInstanceId.Instance?.Token) == false)
                    {
                        string[] SelfUserParams =  GetUserParams();
                    }
                }
                catch (Exception err)
                {
                }
                /*
                try
                {
                    if (String.IsNullOrEmpty(FirebaseInstanceId.Instance?.Token) == false)
                    {
                        App.ddd.Register_FB(FirebaseInstanceId.Instance?.Token?.ToString());
                    }
                }
                catch (Exception err)
                {
                }
                try
                {
                    if (String.IsNullOrEmpty(FirebaseInstanceId.Instance?.Token) == false)
                    {
                        App.ddd.Register_FB(FirebaseInstanceId.Instance?.Token?.ToString());
                    }
                }
                catch (Exception err)
                {
                }*/
            }
            catch (Exception err)
            {

            }
        }

        private void Ws_OnWSConnected(X_WS sender)
        {
            try
            {

                App.ddd.RequestTokenSeance();
            

                if (TemplateMainPage != null)
                {
                    app.MainPage = TemplateMainPage;
                    TemplateMainPage = null;
                }
            }catch (Exception err)
            {

            }
            
        }

        //     var client = new ClientWebSocket();
        static CancellationToken cts;
        static CancellationToken cts1;
        private static async void ClientLaunchAsync()
        {
            //     ClientWebSocket client = null;
            client = new ClientWebSocket();
            await client.ConnectAsync(new Uri(App.ddd.connectInterface.Server_WS), CancellationToken.None);

            WebSocketReceiveResult result;
            var message = new ArraySegment<byte>(new byte[4096]);
            result = await client.ReceiveAsync(message, cts);
            var messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
            string receivedMessage = Encoding.UTF8.GetString(messageBytes);

            SendMessageAsync("ping");

            result = await client.ReceiveAsync(message, cts1);
            var messageBytes1 = message.Skip(message.Offset).Take(result.Count).ToArray();
            string receivedMessage1 = Encoding.UTF8.GetString(messageBytes);

            await Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await ReadMessage(client);
                }
            }, cts, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            // Do something with WebSocket

            SendMessageAsync("ping");
            /*var arraySegment = new ArraySegment<byte>(Encoding.Default.GetBytes("ping"));
            await client.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
            */
        }


        async static void SendMessageAsync(string message)
        {
            //if (!CanSendMessage(message))                return;

            var byteMessage = Encoding.UTF8.GetBytes(message);
            var segmnet = new ArraySegment<byte>(byteMessage);

            await client.SendAsync(segmnet, WebSocketMessageType.Text, true, cts);
        }
        async static Task ReadMessage(ClientWebSocket client)
        {
            WebSocketReceiveResult result;
            var message = new ArraySegment<byte>(new byte[4096]);
            do
            {
                result = await client.ReceiveAsync(message, cts);
                if (result.MessageType != WebSocketMessageType.Text)
                    break;
                var messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
                string receivedMessage = Encoding.UTF8.GetString(messageBytes);
                Console.WriteLine("Received: {0}", receivedMessage);
            }
            while (!result.EndOfMessage);
        }
        async void ConnectToServerAsync()
        {
            CancellationToken cts;

            await client.ConnectAsync(new Uri("ws://10.0.2.2/xml/ChatHandler.ashx"), cts);
            //          UpdateClientState();
            SendMessageAsync("ping");
            /*              await Task.Factory.StartNew(async () =>
                       {
                           while (true)
                           {
                               await ReadMessage();
                           }
                       }, cts, TaskCreationOptions.LongRunning, TaskScheduler.Default);*/
        }

        /*
        async void SendMessageAsync(string message)
        {
            if (!CanSendMessage(message))
                return;

            var byteMessage = Encoding.UTF8.GetBytes(message);
            var segmnet = new ArraySegment(byteMessage);

            await client.SendAsync(segmnet, WebSocketMessageType.Text, true, cts.Token);
        }
        */
        /*     async void SendMessageAsync(string message)
           {
               try
               {
                   //      if (!CanSendMessage(message))
                   //          return;
                   CancellationToken cts;
                   var byteMessage = Encoding.UTF8.GetBytes(message);
                   var segmnet = new ArraySegment<byte>(byteMessage);

              //     await client.SendAsync(segmnet, WebSocketMessageType.Binary, true, cts);

                    client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("ping")), WebSocketMessageType.Text, true, cts);
                   Thread.Sleep(5000);
               }catch (Exception err)
               {

               }
           }
         async Task ReadMessage()
           {
               CancellationToken cts;
               WebSocketReceiveResult result;
               var message = new ArraySegment<byte>(new byte[4096]);
               do
               {
                   result = await client.ReceiveAsync(message, cts);
                   if (result.MessageType != WebSocketMessageType.Text)
                       break;
               //    var messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
               //    string receivedMessage = Encoding.UTF8.GetString(messageBytes);
               //    Console.WriteLine("Received: {0}", receivedMessage);
               }
               while (!result.EndOfMessage);
           }
           */
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}