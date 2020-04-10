using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ji.Droid;
using Ji.Models;
using Ji.Services;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using Xamarin.Forms.Xaml;

namespace Ji.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainTabbedPage : Xamarin.Forms.TabbedPage
    {
        Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();

        public MainTabbedPage()
        {
            InitializeComponent();
            //https://docs.microsoft.com/ru-ru/xamarin/xamarin-forms/platform/windows/tabbedpage-icons
            //On<Windows>().SetHeaderIconsEnabled(true);
            //On<Windows>().SetHeaderIconsSize(new Size(48, 48));

            //var navigationPage = new NavigationPage(new NewItemPage());
            //navigationPage.IconImageSource = "schedule.png";
            //navigationPage.Title = "Schedule";
            SelectedTabColor = Color.White;
            
//            UnselectedTabColor
            BarTextColor = Color.FromHex( "#66FFFFFF");  
            Children.Clear();
            if (App.setup.isProgrammMode())
            {
                Children.Add(new DebugConnectLogPage() { IconImageSource = "ButtonContacts512x512.png", BackgroundImageSource = "Background.png" });  
                Children.Add(new TestSRData() { IconImageSource = "ButtonContacts512x512.png", BackgroundImageSource = "Background.png" });  
            }
            Children.Add(new MainContactsTabbedPage() { IconImageSource = "ButtonContacts512x512.png", BackgroundImageSource = "Background.png"
                /*                                            , BarTextColor = Color.White ,
                HeightRequest=12
                                                            , BackgroundColor = Color.FromHex("e15600")*/
            });
            Children.Add(new ItemsPage()                { IconImageSource = "ButtonChats512x512.png"    , BackgroundImageSource = "Background.png", HeightRequest =96 } ); ;//, Title = "Подписки"
            if (App.setup.isProgrammMode())
                Children.Add(new MainJobTabbedPage()        { IconImageSource = "Button_Tasks_512x512.png"  , BackgroundImageSource = "Background.png"}  ); ;//, Title = "Задачи"  settings64.png

            //      Children.Add(new JobsViewPage() { IconImageSource = "settings64.png" }); ;//, Title = "Задачи"  settings64.png
            //      Children.Add(new JobsViewPage() { IconImageSource = "settings64.png" }); ;//, Title = "Задачи"  settings64.png
            //Children.Add(new ItemsPage() { IconImageSource = "todo48.png", Title = "Подписки" }); ;
            //Children.Add(new ContactsPage() { IconImageSource = "user48.png", Title = "Контакты" }); ;
            //Children.Add(new ContactsPage() { IconImageSource = "JI48.png", Title = "Задачи" }); ;


            App.ddd.OnStatusConectChanged += OnStatusConectChanged;
        }

        private void OnStatusConectChanged(StatusConected statusConected)
        {
            SetConnectIcon(statusConected);
        }

        internal async Task NavigateFromMenu(int id)
        {
            if (!MenuPages.ContainsKey(id))
            {
                switch (id)
                {
                    case (int)MenuItemType.Browse:
                        MenuPages.Add(id, new NavigationPage(new ItemsPage()));
                        break;
                    case (int)MenuItemType.About:
                        MenuPages.Add(id, new NavigationPage(new AboutPage()));
                        break;
                    case (int)MenuItemType.Contacts:
                        MenuPages.Add(id, new NavigationPage(new ContactsPage("All")));
                        break;
                    case (int)MenuItemType.ConnectServer:
                        MenuPages.Add(id, new NavigationPage(new ConnectServerPage()));
                        break;
                    case (int)MenuItemType.MessageList:
                        MenuPages.Add(id, new NavigationPage(new MessageListViewPage(new GroupChat() { ObjId = 1, Text = "Проба", TypeId = MsgObjType.PublicChat })));
                        break;
                    case (int)MenuItemType.ParkingPass:
                        MenuPages.Add(id, new NavigationPage(new ParkingPassPage()));
                        break;


                }
            }

            var newPage = MenuPages[id];

            if (newPage != null && this.SelectedItem != newPage)
            {
               this.SelectedItem  = newPage;

                if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
                    await Task.Delay(100);

          //      IsPresented = false;
            }
        }




        bool b_firsConn = true;
        private void SetConnectIcon(StatusConected statusConected)
        {
            try
            {
                if (statusConected == StatusConected.Open)
                {
                    //StatusConnectIcon.IconImageSource = "logo.png";
                    //StatusConnectIcon.BindingContext = "";
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        StatusConnectIcon.IconImageSource = null;
                        StatusConnectIcon.IconImageSource = ImageSource.FromFile("IconOnline512x512.png"); ;
                        //if (App.ddd.WS != null)
                        //    App.ddd.WS.OnWSReciveNewMessage += WS_OnWSReciveNewMessage;
                    });

                    //if ( b_firsConn == true)
                    //    DependencyService.Get<IXROGiToast>().ShortAlert("Соединение восстановлено");
                    // b_firsConn = false;
                }
                else
                {
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        StatusConnectIcon.IconImageSource = null;
                        StatusConnectIcon.IconImageSource = ImageSource.FromFile("IconNotOnline512x512.png"); ;
                        //if (App.ddd.WS != null)
                        //    App.ddd.WS.OnWSReciveNewMessage -= WS_OnWSReciveNewMessage;
                    });
                    //if (b_firsConn == true)
                    //    DependencyService.Get<IXROGiToast>().ShortAlert("Соединение прервалось");
                    //StatusConnectIcon.IconImageSource = "xamarin_logo.png";
                }
            }
            catch (Exception err)
            {

            }
        }

        object lockAutoconnect = new object();
        private async Task StatusConnectIcon_ClickedAsync(object sender, EventArgs e)
        {
            lock (lockAutoconnect)
            {

                try
                {
                    if (App.ddd != null)
                        if (App.ddd.connectInterface != null)
                        {
                            if (App.ddd.connectInterface.isSetup() == false)
                            {

                                App.ddd.Fill();
                            }
                            if (App.ddd.connectInterface.isSetup() == true)
                            {
                                if (App.ddd.isOpen()==false)
                                {
                                    //if (App.ddd.statusConectWS == StatusConected.None)
                                    //{
                                    //    DependencyService.Get<IXROGiToast>().ShortAlert("Соединение не инициализировано");
                                    //    return;
                                    //}
                                     App.ddd.Open();

                                    
                                    {
                                      
                                        if (App.ddd.isOpen()==false)
                                        {
                                            DependencyService.Get<IXROGiToast>().ShortAlert("Соединение не установлено");
                                        }
                                        
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
                        }

                    //        return true; // True = Repeat again, False = Stop the timer
                }
                catch (Exception err)
                {

                }
            }
            //   return false;
        }

        private void TabbedPage_CurrentPageChanged(object sender, EventArgs e)
        {
            if (SelectedItem is ContactsPage us)
            {
                us.LoadUsers();
            }
        }

        private async void StatusConnectIcon_Clicked(object sender, EventArgs e)
        {
           // if (App != null)
            {
                if (App.ddd != null)
                {
                    if (App.ddd.SR != null)
                        await App.ddd.SR.Open();
                }
            }

        }
    }
}