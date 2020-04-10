//using System;
using Xamarin.Forms;
using Ji.Views;
using Ji.Droid;
using System;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Ji.Models;
using Ji.Services;
using Xamarin.Essentials;
using Ji.ViewModels;
using System.Threading.Tasks;

namespace Ji
{
    public partial class App : Xamarin.Forms.Application
    {
        public static ChatDataStore ddd;
        public static IMyApplicationInfo deviceIn;
        public static SetupAppParam setup;

        public static MenuViewModel MainMenu { get; internal set; }

        public async Task Init()
        {

            setup = new SetupAppParam();
            setup.LoadParams();//***.ConfigureAwait(false);
            //        DependencyService.Register<MockDataStore>();

            ddd = new ChatDataStore();
            if (ddd == null)
            {

            }
            /*     ConnectInterface connectInterfac = new ConnectInterface();
             if (Application.Current.Properties.ContainsKey("TokenReqId"))
                   connectInterfac.TokenReqId = Application.Current.Properties["TokenReqId"].ToString();
               if (Application.Current.Properties.ContainsKey("Server_SOAP"))
                   connectInterfac.Server_Name = Application.Current.Properties["Server_SOAP"].ToString();
               ddd.connectInterface = connectInterfac;
               */
            //    if (ddd.connectInterface==null ||String.IsNullOrEmpty(ddd.connectInterface.Server_Name))
            //    {
            //        MainPage = new
            //            ConnectServerPage();
            //    }
            //    else

            //    {
            //           MainPage = new MainPage();
            //        //   MainPage = new TestTabbedPage();
            //    /*  Xamarin.Forms.TabbedPage tb = 
            //        new Xamarin.Forms.TabbedPage
            //      {
            //            Children = {
            //                new MainPage() {  Title = "Чаты"}, //IconImageSource = "chat48.png" ,
            //                new ContactsPage() {  Title = "Люди"} //IconImageSource = "user48.png",
            //            }
            //      //      , BackgroundColor = Color.White,
            //           // , SelectedTabColor = Color.White
            //           // ,UnselectedTabColor = new Color(0, 0xac, 0x6b)

            //    };
            //       tb.On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            //     //   tb.Children.Add (     new MainPage() { IconImageSource = "chat48.png", Title = "Чаты" });
            //        MainPage = tb; 
            //        */

            //     //   MainPage = new MainTabbedPage();
            //        //android:TabbedPage.ToolbarPlacement = "Bottom";
            //        //BackgroundColor = Color.Green
            //        //MainPage.BackgroundColor = Color.Red;// new Color(0,0xac,0x6b);//"#00AC6B"
            //        //   TestWebViewMainPage();
            //        //ParkingPassPage();
            //        //    ContactsPage();
            //}

        }

        public  App()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception err)
            {

            }
        }

        public static void Log(Exception errIn)
        {
            try
            {
                //if (setup.b_ShowExceptionText == false) 
                //    return;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (ddd != null)
                    {
                        ddd.Log(errIn);
                    }
                    else
                    {
                        if (setup != null)
                        {
                            if (setup.b_ShowExceptionText)
                                DependencyService.Get<IXROGiToast>().ShortAlert(errIn.Message.ToString());
                        }
                        else
                            DependencyService.Get<IXROGiToast>().ShortAlert(errIn.Message.ToString());
                    }

                    
                });

                
            }
            catch (Exception err)
            {
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
        
    }
}
