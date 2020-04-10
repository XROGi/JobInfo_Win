using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using Ji.Models;
using Ji.Views;
using System.Threading;
using Ji.Droid;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace Ji.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<GroupChat> ChatItems { get; set; }
        public Command LoadItemsCommand { get; set; }
        bool b_FirstInitData = false;
        public bool b_ShowJob { get; set; }

        bool _IsMySelected;
        bool _IsFromMySelected;
        public bool IsMySelected { get { return _IsMySelected; } set { _IsMySelected = value;OnPropertyChanged(); OnPropertyChanged(nameof(IsMyUnselected)); } }
        public bool IsMyUnselected
        {
            get { return !_IsMySelected; }
        }
        public bool IsFromMySelected { get { return _IsFromMySelected; } set { _IsFromMySelected = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsFromMyUnselected)); } }
     

        public bool IsFromMyUnselected
        {
            get { return !_IsFromMySelected;  }
        }


        int _inChatId;
        public ItemsViewModel( )
        {
            // Title = "Чат открытый";
            b_ShowJob = false;
            ChatItems = new ObservableCollection<GroupChat>();
            //LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            
            MessagingCenter.Subscribe<NewItemPage, GroupChat>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as GroupChat;
                ChatItems.Add(newItem);
                await DataStore.AddItemAsync(newItem);
            });
     
            App.ddd.OnReciveChatAppend += Ddd_OnReciveChatAppend; ;
            App.ddd.OnReciveStatisticUpdate += Ddd_OnReciveStatisticUpdate; ;
            //        Task.Run(async () => { await ExecuteLoadItemsCommand(); });
        }
        List<xFilterAttribute> attrList = new List<xFilterAttribute>();
        public ItemsViewModel(List<xFilterAttribute> _attrList)
        {
            // Title = "Чат открытый";
            b_ShowJob = false;
            ChatItems = new ObservableCollection<GroupChat>();
            attrList = _attrList;
            //LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            
            MessagingCenter.Subscribe<NewItemPage, GroupChat>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as GroupChat;
                ChatItems.Add(newItem);
                await DataStore.AddItemAsync(newItem);
            });
     
            App.ddd.OnReciveChatAppend += Ddd_OnReciveChatAppend; ;
            App.ddd.OnReciveStatisticUpdate += Ddd_OnReciveStatisticUpdate; ;
            //        Task.Run(async () => { await ExecuteLoadItemsCommand(); });
        }

        internal void LeaveChat(int chatId)
        {
            for (int i = 0; i < ChatItems.Count; i++)
            {
                if (ChatItems[i].ObjId == chatId)
                {
                    ChatItems.Remove(ChatItems[i]);
                    break;
                }
            }
            OnPropertyChanged(nameof(ChatItems));
        }

        private void Ddd_OnReciveStatisticUpdate(ChatStatisticUser stat)
        {
           
            foreach (GroupChat chat in ChatItems)
            {
                if (stat.ChatId == chat.ObjId)
                {
                    chat.Statistic = stat;
                       //chat.CountNewMessage = stat.CountNew.ToString();
                    SortChatListByNewMsg(chat);
                    break;
                }
                /*ChatItems.Add(new GroupChat
                {
                    Text = chat.xml,
                    ObjId = chat.ObjId,
                    period = chat.period,
                    TypeId = chat.SgTypeId,
                    UserList = chat.UsersInChat,
                    Description = "sds"
                }); ;
                */
            }
            
        }

        private void SortChatListByNewMsg(GroupChat chat)
        {
            try
            {
                for (int i = ChatItems.Count - 1; i >= 0; i--)
                {
                    for (int j = 1; j <= i; j++)
                    {
                        GroupChat o1 = ChatItems[j - 1];
                        GroupChat o2 = ChatItems[j];
                        if (o1.Statistic != null && o2.Statistic != null)
                        {
                            if (o1.Statistic.LastObjIdDateCreate < o2.Statistic.LastObjIdDateCreate)
                            {
                                try
                                {
                                    ChatItems.Remove(o1);
                                    ChatItems.Insert(j, o1);
                                }
                                catch (Exception err)
                                {
                                }
                            }
                        }
                        else
                        {
                            if (o1.Statistic == null && o2.Statistic != null) // если нет статистики то вниз
                            {
                                try
                                {
                                    ChatItems.Remove(o1);
                                    ChatItems.Insert(j, o1);
                                }
                                catch (Exception err)
                                {
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
            }
            OnPropertyChanged(nameof(ChatItems));
        }
        
        private void Ddd_OnReciveChatAppend(int chatId, Obj objtemp)
        {
            try
            {
                IsBusy = true;

                if (objtemp.ObjId != 0 && objtemp.xml == null)
                {

                }

                if ((MsgObjType)objtemp.sgTypeId == MsgObjType.Job && b_ShowJob == false)
                    return;
                    ChatItems.Insert(0,new GroupChat(objtemp)
                    //{
                    //    Text = objtemp.xml,
                    //    ObjId = objtemp.ObjId,
                    //    period = objtemp.period,
                    //    TypeId = (MsgObjType)objtemp.sgTypeId,
                    //    UserList = objtemp.UsersInChat,
                    //    Description = "sds"
                    //}
                    
                    ); ;
                 
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


        public class ReverseComparer : IComparer<GroupChat>
        {
            public int Compare(GroupChat x, GroupChat y)
            {
                try
                {
                    if (x.Statistic == null && y.Statistic == null) return 0;
                    if (x.Statistic != null && y.Statistic == null) return 1;
                    if (x.Statistic == null && y.Statistic != null) return -1;

                    if (x.Statistic.LastObjIdDateCreate > y.Statistic.LastObjIdDateCreate)
                        return 1;
                    if (x.Statistic.LastObjIdDateCreate < y.Statistic.LastObjIdDateCreate)
                        return -1;
                    return 0;
                }
                catch (Exception err)
                {

                }
                return 0;
            }


        }

        private GroupChat[] SortChatListByDateLast(GroupChat [] chatlist)
        { 
            
            Array.Sort(chatlist, new ReverseComparer());
            return chatlist;

            /*

            List <Obj> inMass = new List<Obj>()  ;
            inMass.AddRange(chatlist);

            for (int i = inMass.Count  - 1; i >= 0; i--)
            {
                for (int j = 1; j <= i; j++)
                {
                    GroupChat o1 = inMass.(();//[j - 1];
                    GroupChat o2 = inMass[j];
                    if (o1.Statistic != null && o2.Statistic != null)
                    {
                        if (o1.Statistic.LastObjIdDateCreate < o2.Statistic.LastObjIdDateCreate)
                        {
                            inMass.Remove(o1);
                            inMass.Insert(j, o1);
                        }
                    }
                    else
                    {
                        if (o1.Statistic == null && o2.Statistic != null) // если нет статистики то вниз
                        {
                            inMass.Remove(o1);
                            inMass.Insert(j, o1);
                        }
                    }
                }
            }
            OnPropertyChanged(nameof(ChatItems));*/
        }
        private   void Ddd_OnReciveChatList(Obj[] chatlist)
        {
            try
            {
                IsBusy = true;
                List<GroupChat> n = new List<GroupChat>();

                
                foreach (Obj chat in chatlist )
                {
                    try
                    {
                        //если не показывать работы и это работа - пропустить
                        if ((MsgObjType)chat.sgTypeId == MsgObjType.Job && b_ShowJob == false)
                            continue;

                        // если показывать работы и это не работа пропустить
                        if (((MsgObjType)chat.sgTypeId != MsgObjType.Job && b_ShowJob == true))
                            continue;
                        {
                            GroupChat gchat = new GroupChat(chat);
                            //{
                            //    Text = chat.xml,
                            //    ObjId = chat.ObjId,
                            //    period = chat.period,
                            //    TypeId = (MsgObjType)chat.sgTypeId,
                            //    UserList = chat.UsersInChat,
                            //    Description = "sds"
                            //};
                            n.Add(gchat); ;

                            //if (gchat.Statistic == null)
                            //    Request_Chat_Statistic(gchat);
                        }
                    }
                    catch (Exception err)
                    {
                        Debug.WriteLine(err);
                    }
                }
                GroupChat [] res = SortChatListByDateLast(n.ToArray());
                ChatItems.Clear();
                foreach (GroupChat c in res)
                {
                    ChatItems.Add(c);
                }
                OnPropertyChanged( nameof(ChatItems));

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                App.ddd.OnReciveChatList -= Ddd_OnReciveChatList;
                IsBusy = false;
            }
        }

    internal void OnStatusChanged(StatusConected statusConected)
        {

            if (b_FirstInitData == false &&  statusConected == StatusConected.Open)
            {
                Task.Run(
                    async () => {
                        await ExecuteLoadItemsCommand(); 
                    }
                    );
            }
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
               
                if (App.ddd.SR==null || App.ddd.SR.isOpen()==false)
                {
                    return;
                }
                App.ddd.OnReciveChatList += Ddd_OnReciveChatList;
                App.ddd.SR.OnUnknownError += SR_OnUnknownError; ;
                if (attrList.Contains(xFilterAttribute.JobMy))
                {
                    await App.ddd.SR.Request_Job_List(0, "My").ConfigureAwait(true);
                }
                if (attrList.Contains(xFilterAttribute.JobFromMy))
                {
                    await App.ddd.SR.Request_Job_List(0, "FromMy").ConfigureAwait(true);
                }

                if (attrList.Count==0)
                    await App.ddd.SR.Request_Chat_List(0).ConfigureAwait(true);
                ;
                //    while (App.ddd.statusConectWS != Droid.StatusConected.Open)
                //{
                //    Thread.Sleep(300);
                //    t++; if (t >= 50)
                //    {
                //        break;
                //    }
                //}
                /*
                 ChatItems.Clear();
                int t = 0;
                //var items = await DataStore.GetItemsAsync(true);
       //         if (App.ddd.statusConectWS == Droid.StatusConected.Open)
                {
                    //   Thread.Sleep(1000);
                    App.ddd.Chat_UpdateStatistic();//.GetChatList();
                    if (App.ddd.StatisticChat == null)
                        App.ddd.Chat_UpdateStatistic();
                    var items = App.ddd.GetChatList();
                    if (items != null)
                    {
                        if (App.ddd.StatisticChat != null)
                        {
                            foreach ( var stat in App.ddd.StatisticChat.OrderByDescending(s=>s.LastObjIdDateCreate))
                            {
                                GroupChat chat =   items.Where(s => s.ObjId == stat.ChatId).FirstOrDefault();
                                if (chat != null)
                                {
                                    chat.Statistic = stat;
                                    chat.CountNewMessage = stat.CountNew!=null? stat.CountNew.ToString():"0";
                                    ChatItems.Add(chat);
                                    items.Remove(chat);
                                }
                            }
                            foreach (var item in items)
                            {
                                ChatItems.Add(item);
                            }
                        }
                        else
                        {
                            foreach (var item in items)
                            {
                                ChatItems.Add(item);
                            }
                        }

                      
                        b_FirstInitData = true;
                    }
                    else

                    {
                        var items1 = App.ddd.GetChatList();
                        if (items1 != null)
                        {
                            foreach (var item in items1)
                            {
                                
                                ChatItems.Add(item);
                            }
                            b_FirstInitData = true;
                        }
                        else

                        {
                        }
                    }
                }
               
    */
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
               // SetProperty(ref ChatItems, ChatItems);
                IsBusy = false;
            }
        }

        public void SR_OnUnknownError(string Error)
        {
            IsBusy = false;
            App.ddd.SR.OnUnknownError -= SR_OnUnknownError;

            //DependencyService.Get<IXROGiToast>().ShortAlert("Соединение восстановлено");
        }

        internal async void Request_Chat_Statistic(GroupChat chat)
        {
            try
            {
                await App.ddd.SR.Request_Chat_Statisic(chat.ObjId).ConfigureAwait(false);//// .Request_Chat_List(0).ConfigureAwait(true);
            }
            catch (Exception err)
            {
            }
        }
    }
}