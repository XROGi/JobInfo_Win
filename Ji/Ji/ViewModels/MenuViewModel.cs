using Ji.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Xamarin.Forms;

namespace Ji.ViewModels
{
    public class MenuViewModel : INotifyPropertyChanged
    {
        bool b_addMenu = false;
        public ObservableCollection<HomeMenuItem> MenuItems
        {
            get;
            set;
        }
        public MenuViewModel()
        {
            b_addMenu = false;

            MenuItems = new ObservableCollection<HomeMenuItem>();
            MenuItems.Add(new HomeMenuItem { Id = MenuItemType.SetupPage, Title = "Настройки" });
            if (App.setup.isProgrammMode())
            {
                MenuItems.Add(new HomeMenuItem { Id = MenuItemType.Browse, Title = "Основное" });
                /*  new HomeMenuItem {Id = MenuItemType.Contacts, Title="Контакты" },
                   new HomeMenuItem {Id = MenuItemType.MessageList, Title="Сообщения" },*/

                MenuItems.Add(new HomeMenuItem { Id = MenuItemType.ConnectServer, Title = "Сервер" });
                MenuItems.Add(new HomeMenuItem { Id = MenuItemType.SetupPage, Title = "Настройки" });
            //new HomeMenuItem {Id = MenuItemType.UpdateServerPage, Title="Обновить версию.." }
            };
            if (System.Diagnostics.Debugger.IsAttached)
            {
                MenuItems.Add(new HomeMenuItem { Id = MenuItemType.DebugPage, Title = "DebugPage" });
            }
            MenuItems.Add(new HomeMenuItem { Id = MenuItemType.About, Title = "About" });

        }


        internal void SetPPSMenu(bool b_IsPPSEnabled)
        {
            if (b_IsPPSEnabled == true)
            {
                if (b_addMenu == false)
                {
                    MenuItems.Insert(1, new HomeMenuItem { Id = MenuItemType.ParkingPass, Title = "Парковка" });
                    b_addMenu = true;
                }
            }
        }



        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T backingStore, T value,
           [CallerMemberName]string propertyName = "",
           Action onChanged = null)
        {
            try
            {
                if (EqualityComparer<T>.Default.Equals(backingStore, value))
                    return false;

                backingStore = value;
                onChanged?.Invoke();
                OnPropertyChanged(propertyName);
                return true;
            }
            catch (Exception err)
            {
            }
            return false;
        }



        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}