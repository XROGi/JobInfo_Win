using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using Ji.Models;
using Ji.Views;
using System.Collections.Generic;
using System.Linq;

namespace Ji.ViewModels
{
    public class ContactsViewModel : BaseViewModel_User
    {
        private string Filer;
        public ObservableCollection<UserChat> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        
        Command tapCommand;
        public Command TapCommand
        {
            get { return tapCommand; }
        }
        void OnTapped(object s)
        {
            //taps++;
            Debug.WriteLine("parameter: " + s);
        }

        public delegate void OnLoadUsersCompleatDelegate();
        public event OnLoadUsersCompleatDelegate OnLoadUsersCompleat;

        bool _b_OnlySelcted;

        public ContactsViewModel(int[] listUsersAdd, bool b_OnlySelcted)
        {
            if (b_OnlySelcted == true)
                Filer = "Selected";
            else
                Filer = "All";

               Title = "Пользователи";
            Items = new ObservableCollection<UserChat>();
            SetSelectedUser(listUsersAdd);
            _b_OnlySelcted = b_OnlySelcted; 
            App.ddd.OnReciveUserList += Ddd_OnReciveUserList;
            //LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            MessagingCenter.Subscribe<NewItemPage, UserChat>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as UserChat;
                Items.Add(newItem);
                await DataStore.AddItemAsync(newItem);
            });

       

            //Task.Run(async () => { await ExecuteLoadItemsCommand(); });
        }
        public ContactsViewModel(string _Filter)
        {
            Title = "Пользователи";
            Filer = _Filter;
            Items = new ObservableCollection<UserChat>();
   
            //LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            MessagingCenter.Subscribe<NewItemPage, UserChat>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as UserChat;
                Items.Add(newItem);
                await DataStore.AddItemAsync(newItem);
            });
            //   Task.Run(async () => { await ExecuteLoadItemsCommand(); });
        }
        internal UserChat[] GetSelected()
        {
            if (Items != null)
                return Items.Where(s => s.Selected == true).ToArray();
            else
                return null;
        }

        internal void Close()
        {
            
            if (App.ddd != null)
            {
                App.ddd.OnReciveUserList -= Ddd_OnReciveUserList;
            }


        }
        private void Ddd_OnReciveUserList(int maxVers, int nPage, UserChat[] users)
        {
            try

            {
                Items.Clear();
                foreach (UserChat m in users)
                {
                    try
                    {

                        if (listUsersSelected!=null && listUsersSelected.Contains(m.UserId))
                        {
                            m.Selected = true;
                        }
                        else
                            m.Selected = false;
                        if (_b_OnlySelcted)
                        {
                            if (m.Selected)
                            {
                                Items.Add(m);
                            }
                        }
                        else
                            Items.Add(m);
                    }
                    catch (Exception err)
                    {
                    
                    }


                }
                App.ddd.OnReciveUserList -= Ddd_OnReciveUserList;
                IsBusy = false;
                OnLoadUsersCompleat?.Invoke();
                if (users.Length>0)
                 OnPropertyChanged(nameof(Items));
            }
            catch (Exception err)
            {
            }
            
        }

       

        private void Ddd_OnReciveUserList1(int maxVers, int nPage, UserChat[] users)
        {
             
        }

        async Task ExecuteLoadItemsCommand()
        {

           // System.Threading.Thread.Sleep(15000);
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                App.ddd.OnReciveUserList += Ddd_OnReciveUserList;
                await App.ddd.Request_User_List(0, 0, Filer);
                #region MyRegion

                ////var items = await DataStore.GetItemsAsync(true);
                //if (App.ddd.connectInterface.TokenSeanceId != null)
                //{
                //    List<UserChat> items = App.ddd.GetUserList();
                //    if (items != null)
                //    {
                //        List < UserChat > addlist;
                //        if (_b_OnlySelcted && listUsersSelected != null)
                //            addlist = items.Where(s => listUsersSelected.Contains(s.UserId)).OrderBy(s => s.FIO).ToList();
                //        else
                //            addlist = items.OrderBy(s => s.FIO).ToList();

                //        foreach (var item in addlist)
                //        {
                //            if (listUsersSelected!=null)
                //                if (listUsersSelected.Contains(item.UserId) == true)
                //                    item.Selected = true;
                //            Items.Add(item);
                //        }
                //    }
                //}
                //else

                //{

                //} 
                #endregion
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
               IsBusy = false;
            }
     
        }

        internal void Refresh()
        {
            Task.Run(async () => { await ExecuteLoadItemsCommand(); });
        }

        int[] listUsersSelected;
        internal void SetSelectedUser(int [] listUsersAdd)
        {
            listUsersSelected = listUsersAdd;
        }
    }
}