using Ji.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Device = Xamarin.Forms.Device;

namespace Ji.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : MasterDetailPage
    {
        Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
        public MainPage()
        {
            InitializeComponent();

            MasterBehavior = MasterBehavior.Popover;
            //ZXing .Net.Mobile.Forms.Android.Platform.Init();
         //   MenuPages.Add((int)MenuItemType.Browse, (NavigationPage)Detail);

 

            /*
            var options = new ZXing.Mobile.MobileBarcodeScanningOptions();

            options.PossibleFormats = new List<ZXing.BarcodeFormat>() {
                ZXing.BarcodeFormat.CODE_128

            };

            var scanPage = new ZXingScannerPage(options);
            scanPage.AutoFocus();
            scanPage.OnScanResult += (result) =>
            {
                // Stop scanning
                scanPage.IsScanning = false;
                scanPage.AutoFocus(0, 500);

                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopModalAsync();
                    //DisplayAlert("Scanned Barcode", result.Text, "OK");
                //    scanres.Text = result.Text;
                });
            };
             Navigation.PushModalAsync(scanPage);

            */

        }
        public async void RemobePPSPage()
        {
         //   ListViewMenu.
            MenuPages.Remove((int)MenuItemType.ParkingPass);
      
            if (Device.RuntimePlatform == Device.Android)
                await Task.Delay(100);
        }
        public async Task NavigateFromMenu(int id)
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
                        MenuPages.Add(id, new NavigationPage(new MessageListViewPage(new GroupChat() { ObjId = 1, Text = "Проба", TypeId = MsgObjType.PrivateChatd })));
                        break;
                    case (int)MenuItemType.ParkingPass:
                        MenuPages.Add(id, new NavigationPage(new ParkingPassPage()));
                        break; 


                }
            }
            
            var newPage = MenuPages[id];

            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
        }
        public async Task NavigateFromMenu(MenuItemType page)
        {
            if (!MenuPages.ContainsKey((int)page))
            {
                switch (page)
                {
                    case MenuItemType.Browse:
                        MenuPages.Add((int)page, new NavigationPage(new ItemsPage()));// {BackgroundColor = Color.Red }
//                        MenuPages.Add((int)page, new NavigationPage(new ItemsPage()));// {BackgroundColor = Color.Red }
  
                        break;
                    case MenuItemType.About:
                        MenuPages.Add((int)page, new NavigationPage(new AboutPage()));
                      //  MenuPages.Add((int)page, new NavigationPage(new SignalRTestPage()));
                        
                        break;
                    case MenuItemType.Contacts:
                        MenuPages.Add((int)page, new NavigationPage(new ContactsPage("All")));
                        break;
                    case MenuItemType.ConnectServer:
                        MenuPages.Add((int)page, new NavigationPage(new ConnectServerPage()));
                        break;
                    case MenuItemType.MessageList:
                        MenuPages.Add((int)page, new NavigationPage(new MessageListViewPage(new GroupChat() { ObjId = 1, Text = "Проба", TypeId = MsgObjType.PrivateChatd })));
                        break;
                    case MenuItemType.ParkingPass:
                        {
                            if (App.ddd.b_IsPPSEnabled)
                            {
                                MenuPages.Add((int)page, new NavigationPage(new ParkingPassPage()));
                            }
                            else
                            {
                                await DisplayAlert("Внимание", "У Вас нет доступа к этому функционалу", "OK");
                            }
                        }
                        break;
                    case MenuItemType.UpdateServerPage:
                        MenuPages.Add((int)page, new NavigationPage(new UpdateServerPage()));
                        break;
                }
            }
            try
            {
                if (MenuPages.ContainsKey((int)page))
                {
                    var newPage = MenuPages[(int)page];

                    if (newPage != null && Detail != newPage)
                    {
                        Detail = newPage;
                        try
                        {
                            if (Device.RuntimePlatform == Device.Android)
                                await Task.Delay(100);
                        }
                        catch (Exception err)
                        {

                        }
                        IsPresented = false;
                    }
                }

            }catch (Exception err )
            {

            }
        }
    }
}