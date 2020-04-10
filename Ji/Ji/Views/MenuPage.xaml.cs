using Ji.Models;
using Ji.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;

namespace Ji.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MenuPage : ContentPage
    {
        MasterDetailPage RootPage { get => Application.Current.MainPage as MasterDetailPage; }
        MenuViewModel model ;

        //        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        //      TabbedPage RootTabbedPage { get => Application.Current.MainPage as TabbedPage; }
        List<HomeMenuItem> menuItems;
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
        
        public MenuPage()
        {
            InitializeComponent();

            BindingContext = model = new MenuViewModel();

            App.MainMenu = model;

     //       ListViewMenu.ItemsSource = menuItems;
            
          //  ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                try
                {
                    if (e.SelectedItem == null)
                        return;
                    if ((e.SelectedItem as HomeMenuItem).Id == MenuItemType.ParkingPass)
                    {
                     //   await DisplayAlert("Извините", "Функция временно отключена. Функция в  API в процессе переноса.. (", "OK");
                     //   return;
                        if (App.ddd.b_IsPPSEnabled == false)
                        {
                            //var t = menuItems.Where(s => s.Id == MenuItemType.ParkingPass).FirstOrDefault();
                            //if (t != null)
                            //    menuItems.Remove(t);
                            //ListViewMenu.ItemsSource = menuItems;
                            await DisplayAlert("Извините", "У Вас нет доступа к этой функции", "OK");
                            return;
                            //e.SelectedItem = ;
                        }
                        else
                        {
                            NavigationPage np = this.RootPage.Detail as NavigationPage;
                            await np.PushAsync(new ParkingPassPage());
                        }
                    }
                     if ((e.SelectedItem as HomeMenuItem).Id == MenuItemType.Browse)
                    {
                        

                        //NavigationPage np = this.RootPage.Detail as NavigationPage;

                    }
                    if ((e.SelectedItem as HomeMenuItem).Id == MenuItemType.ConnectServer)
                    {
                        //this.RootPage.Detail = new NavigationPage( new ConnectServerPage());
                        NavigationPage np = this.RootPage.Detail as NavigationPage;
                        await np.PushAsync(new ConnectServerPage());
                    }
                    if ((e.SelectedItem as HomeMenuItem).Id == MenuItemType.SetupPage)
                    {
                      
                        NavigationPage np = this.RootPage.Detail as NavigationPage;
                        await np.PushAsync(new SetupViewPage());
                    }
                    if ((e.SelectedItem as HomeMenuItem).Id == MenuItemType.About)
                    {
                        //this.RootPage.Detail = new NavigationPage(new AboutPage());
                        NavigationPage np = this.RootPage.Detail as NavigationPage;
                        
                    //         await np.PushAsync(new SignalRTestPage());
                      await np.PushAsync(new AboutPage());
                    }
                    if ((e.SelectedItem as HomeMenuItem).Id == MenuItemType.DebugPage)
                    {
                        //this.RootPage.Detail = new NavigationPage(new AboutPage());
                        NavigationPage np = this.RootPage.Detail as NavigationPage;
                        //await np.PushAsync(new DebugViewPage());


                        await np.PushAsync(new DebugConnectLogPage());
                    }


                    //    var id = (int)((HomeMenuItem)e.SelectedItem).Id;
                    //await RootPage.NavigateFromMenu(id);

                    //////////if (RootPage != null)
                    //////////    await RootPage.NavigateFromMenu(((HomeMenuItem)e.SelectedItem).Id);
                    //////////if (RootTabbedPage != null)
                    //////////    await (RootTabbedPage as MainTabbedPage) .NavigateFromMenu((int)((HomeMenuItem)e.SelectedItem).Id);
                    ///


                    RootPage.IsPresented = false;
                }
                catch (Exception err)
                {
                    App.Log(err);
                }
            };
        }
    }
}