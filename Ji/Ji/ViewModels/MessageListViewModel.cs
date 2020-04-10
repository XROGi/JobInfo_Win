using Ji.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Ji.ViewModels
{
    public class MessageListViewModel : INotifyPropertyChanged
    {
        
        public ObservableCollection<ObjMsg> ItemsMsgs { 
            get; 
            set; 
        }

        List<int> objs = new List<int>();

        public Action<ObjMsg> OnArticoloScannerizzato { get; set; }

        public Action<ObjMsg> OnCopyScrollPosition { get; set; }

        public Action<int> OnBeginRefresh { get; set; }

        public Action<int> OnEndRefresh { get; set; }
        public Action<int> OnScrollToObjId { get; set; }

        int b_FirstInitPosition = 0;

        public GroupChat chat { get; set; }
        
        public int Page_Start;
        public int Page_Stop;
        public bool b_sendMessageAndWaitFromServer ;
        internal void Close()
        {
            try
            {
                if (ItemsMsgs != null)
                    ItemsMsgs.CollectionChanged -= ItemsMsgs_CollectionChanged;
                if (pc != null)
                {
                    pc.OnChat_ShowMsgInList -= Pc_OnChat_ShowMsgInList;
                    pc.OnChat_PageCompleat -= Pc_OnChat_PageCompleat;
                    pc.Close();
                }
                if (chat != null)
                    chat.OnReciveStatisticUpdate -= Ddd_OnReciveStatisticUpdate;


                if (App.ddd != null)
                {
                    //App.ddd.OnReciveStatisticUpdate -= Ddd_OnReciveStatisticUpdate;
                    App.ddd.OnReciveMessagePageList -= Ddd_OnReciveMessagePageList;
                    App.ddd.OnReciveMessageStatus -= Ddd_OnReciveMessageStatus;
                    App.ddd.OnReciveMessageAppend -= Ddd_OnReciveMessageAppend;
                }
                chat = null;
            }
            catch (Exception err)
            {

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
        
        public Command SelectedItemMsg { get; set; }
        public MessageListViewModel()
        {
            SelectedItemMsg = new Command(async () => await ExecuteLoadItemsCommand());
         
        }


        
        public ICommand CommandBtnMoveToLastShown
        {
            get
            {
                return new Command(async () =>
                {
                  //  IsRefreshing = true;

                //    await ExecuteLoadItemsCommand();

                  //  IsRefreshing = false;
                });
            }
        }
        // public Command JobCompleatCommand { get; set; }






        bool _b_addMode;
        public bool b_addMode
        {
            get { return _b_addMode; }
            set
            {
                if (_b_addMode != value)
                {
                    _b_addMode = value;
                }
                OnPropertyChanged(nameof(b_addMode));
            }

        }


        int _nPage;
        public int nPage
        {
            get { return _nPage; }
            set
            {
                _nPage = value;
            }
        }

        //public bool IsBusy { get; private set; }

        bool b_isBusyLoadPage = false;
        public bool isBusyLoadPage
        {
            get { return b_isBusyLoadPage; }
            set { b_isBusyLoadPage = value;
               OnPropertyChanged();
            }
        }

        public ChatPagesController pc { get; set; }
        
        

        public MessageListViewModel(GroupChat g )
        {
            try
            {
                chat = g;
                b_addMode = false;
                b_sendMessageAndWaitFromServer = false;

                ItemsMsgs = new ObservableCollection<ObjMsg>();
                objs = new List<int>();
                Xamarin.Forms.BindingBase.EnableCollectionSynchronization(ItemsMsgs, null, ObservableCollectionCallback);
        //        BindingOperations.EnableCollectionSynchronization(_points, _lock);

                ItemsMsgs.CollectionChanged += ItemsMsgs_CollectionChanged;
                int ChatId = g.ObjId;
                pc = new ChatPagesController(ChatId);
                pc.OnChat_ShowMsgInList += Pc_OnChat_ShowMsgInList;
                pc.OnChat_PageCompleat += Pc_OnChat_PageCompleat;

                
                App.ddd.OnReciveMessagePageList += Ddd_OnReciveMessagePageList;
                App.ddd.OnReciveMessageStatus += Ddd_OnReciveMessageStatus;
                App.ddd.OnReciveMessageAllStatus += Ddd_OnReciveMessageAllStatus; ;
                
                App.ddd.OnReciveMessageAppend += Ddd_OnReciveMessageAppend; ;
                App.ddd.OnReciveMessage += Ddd_OnReciveMessage; ; ;

          //      App.ddd.OnReciveStatisticUpdate += Ddd_OnReciveStatisticUpdate;
                if (chat!=null)
                    chat.OnReciveStatisticUpdate += Ddd_OnReciveStatisticUpdate;
                App.ddd.OnReciveStatisticUpdate += Ddd_OnReciveStatisticUpdate1;
                Task.Run(async () =>
                {
                    try
                    {
                  //      await Request_Chat_Statistic(chat);
                        await ExecuteLoadItemsCommand();
                    }
                    catch (Exception err)
                    {
                    }
                });
            }
            catch (Exception err)
            {
            }
        }

        private void Ddd_OnReciveStatisticUpdate1(ChatStatisticUser stat)
        {
            if (stat.ChatId == chat.ObjId)
            {
                chat.Statistic = stat;
            }
        }

        private void Ddd_OnReciveMessage(ObjMsg[] list)
        {
            try
            {
                foreach (ObjMsg o in list)
                {
                    ObjMsg item = ItemsMsgs.Where(s => s.ObjId == o.ObjId).FirstOrDefault();
                    if (item != null)
                    {

                        o.Chat = chat;
                        if (o.isMessgeJobTemplate())
                        {

                            o.JobBeginCommand =
                                 new Command(async (arg) =>
                                 {
                                     try
                                     {
                                         int id = (int)arg;
                                         await App.ddd.SR.Request_Job_SetStatus(chat.ObjId, id, "work");
                                         Console.WriteLine(arg);
                                     }
                                     catch (Exception err)
                                     {
                                     }

                                 });
                            o.JobCompleatCommand =
                                  new Command(async (arg) =>
                                  {
                                      try
                                      {
                                          int id = (int)arg;
                                          await App.ddd.SR.Request_Job_SetStatus(chat.ObjId, id, "compleat");
                                          Console.WriteLine(arg);
                                      }
                                      catch (Exception err)
                                      {
                                      }

                                  });


                            o.JobCancelCommand =
                          new Command(async (arg) =>
                          {
                              try
                              {
                                  int id = (int)arg;
                                  await App.ddd.SR.Request_Job_SetStatus(chat.ObjId, id, "cancel2");
                                  Console.WriteLine(arg);
                              }
                              catch (Exception err)
                              {
                              }

                          });
                            //   App.ddd.SR.Request_Message_Status(chat.ObjId, item.ObjId);
                        }


                        int index = ItemsMsgs.IndexOf(item);
                        ItemsMsgs.RemoveAt(index);
                        ItemsMsgs.Insert(index, o);
                        App.ddd.SR.Request_Message_Status(chat.ObjId, o.ObjId);




                        //ItemsMsgs.Add(msg_append);
                    }
                }
                OnPropertyChanged(nameof(ItemsMsgs));
            }
            catch (Exception err)
            { 
            }
        }

        private void Ddd_OnReciveMessageAppend(ObjMsg msg_append)
        {
            b_addMode = true;
            try
            {
                if (msg_append.PageNum == Page_Stop || msg_append.MessageNum == ItemsMsgs[ItemsMsgs.Count - 1].MessageNum + 1)
                {
                    if (msg_append.userid != App.ddd.SR.UserId)
                    {
                        ItemsMsgs.Add(msg_append);
                    }
                    else
                        ItemsMsgs.Add(msg_append); // своё сообщение в последней странице - покажем
                }
                else
                { //не загружена последняя страница. Удалить все загрузки и загрузить
                    ItemsMsgs.Add(msg_append);
                }
                OnArticoloScannerizzato?.Invoke(msg_append);

            }
            catch (Exception err)
            {
            }
            finally
            {
                b_addMode = false;
            }

            OnPropertyChanged(nameof(ItemsMsgs));
        }
        private void Ddd_OnReciveMessageAllStatus(int chatId, int ObjId, ClassSR.ObjStatus[] status)
        {
            try
            {
                /*if (chatId != chat.ObjId)
                    return;*/
                for (int t = ItemsMsgs.Count - 1; t >= 0; t--)
                {
                    if (ItemsMsgs[t].ObjId == ObjId)
                    {
                        var obj = ItemsMsgs[t];


                        ClassSR.ObjStatus[] statsorder = status.OrderByDescending(s => s.dtb).ToArray();
                        obj.Status = statsorder;
                            
                        //foreach (ClassSR.ObjStatus stat in statsorder)
                        //{
                            
                        //    obj.ShownState = sgStatus;
                        //    break;
                        //}

                    }
                }
            }
            catch (Exception err)
            {
            }
        }
        private void Ddd_OnReciveMessageStatus(int ObjId, int sgStatus)
        {
            try
            {
                for (int t = ItemsMsgs.Count - 1; t >= 0; t--)
                {
                    if (ItemsMsgs[t].ObjId == ObjId)
                    {
                        var obj = ItemsMsgs[t];
                        ////int index = ItemsMsgs.IndexOf(selectedItem);
                        //ItemsMsgs.Remove(obj);
                        obj.ShownState = sgStatus;
                        //ItemsMsgs.Insert(t, obj);
                      
                        //OnPropertyChanged(nameof(ItemsMsgs));
                        //OnPropertyChanged(nameof(obj));
                        break;
                        
                    }
                }

                
                /*
                foreach (ObjMsg m in ItemsMsgs)
                {


                    if (m.ObjId == ObjId)
                    {
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                        {
                            try
                            {

                                m.ShownState = sgStatus;
                            }
                            catch (Exception err)
                            {
                            }
                        });
                        break;
                    }
                }*/
                //ObjMsg obj = ItemsMsgs.Where(s => s.ObjId == ObjId).SingleOrDefault();
                //if (obj != null)
                //{
                //    obj.ShownState = sgStatus;
                //}
            }
            catch (Exception err)
            {
            }
        }

        int LastPageInList = -1;
        int LastLoadedObjIdInInList = -1;
        private void Ddd_OnReciveMessagePageList(ObjMsg[] list)
        {  
            ObjMsg LastShown=null; 
            int r=0;
            try
            {
                isBusyLoadPage = true;
                if (list != null)
                {
                    foreach (ObjMsg m in list)
                    {
                        m.Chat = chat;
                        if (m.isMessgeJobTemplate())
                        {

                            m.JobBeginCommand =
                                 new Command(async (arg) =>
                                 {
                                     try
                                     {
                                         int id = (int)arg;
                                         await App.ddd.SR.Request_Job_SetStatus(chat.ObjId, id, "work");
                                         Console.WriteLine(arg);
                                     }
                                     catch (Exception err)
                                     {
                                     }

                                 });
                            m.JobCompleatCommand =
                                  new Command(async (arg) =>
                                 {
                                      try
                                      {
                                          int id = (int)arg;
                                          await App.ddd.SR.Request_Job_SetStatus(chat.ObjId, id, "compleat");
                                          Console.WriteLine(arg);
                                      }
                                      catch (Exception err)
                                      {
                                      }

                                  });
                            m.JobCancelCommand =
                               new Command(async (arg) =>
                               {
                                   try
                                   {
                                       int id = (int)arg;
                                       await App.ddd.SR.Request_Job_SetStatus(chat.ObjId, id, "cancel");
                                       Console.WriteLine(arg);
                                   }
                                   catch (Exception err)
                                   {
                                   }

                               });
                            App.ddd.SR.Request_Message_Status(chat.ObjId, m.ObjId);
                        }
                        if (m.PageNum > LastPageInList)
                        {
                            LastPageInList = m.PageNum;
                        }
                        if (LastLoadedObjIdInInList < m.ObjId)
                        {
                            LastLoadedObjIdInInList = m.ObjId;
                        }
                    }

                    //     OnBeginRefresh?.Invoke(r);
                    if (list[0].PageNum >= Page_Stop)
                    {
                        foreach (ObjMsg m in list.OrderBy(s => s.MessageNum))
                        {
                            if (ItemsMsgs.Any(s => s.ObjId == m.ObjId) == false)
                            {
                                ItemsMsgs.Add(m);
                                if (Page_Stop < m.PageNum)
                                {
                                    Page_Stop = m.PageNum;
                                }
                                if (m.ObjId == chat.Statistic.LastShownObjId)
                                    LastShown = m;
                            }
                        }
                    }

                    if (list[0].PageNum < Page_Start)
                    {
                        ObjMsg[] res = list.OrderByDescending(s => s.MessageNum).ToArray();
                        foreach (ObjMsg m in res)
                        {
                            ItemsMsgs.Insert(0, m);
                            if (Page_Start > m.PageNum)
                            {
                                Page_Start = m.PageNum;
                            }
                            if (m.ObjId == b_FirstInitPosition)
                                LastShown = m;
                        }
                    }
                }// if !null
              
            }
            finally
            {
                b_addMode = false;
                isBusyLoadPage = false;
                
                if (LastShown != null)
                {
                    if (b_NeedChangeCursor)
                        OnArticoloScannerizzato?.Invoke(LastShown);
                    b_FirstInitPosition = 0;
                    LastShown = null;
                }
                b_NeedChangeCursor = true;
            }
            OnPropertyChanged(nameof(ItemsMsgs));


     //       OnEndRefresh?.Invoke(r);

            //   OnPropertyChanged(nameof(ItemsMsgs));
        }


        public bool _bVisibleCountNewMessage;

        public bool bVisibleCountNewMessage
        {
            get { return _bVisibleCountNewMessage; }
            set { _bVisibleCountNewMessage = value; OnPropertyChanged(); }
        }

        string _CountNewMessage;
        public String CountNewMessage
        {
            get { return _CountNewMessage; }
            set
            {
                if (_CountNewMessage != value)
                {
                    _CountNewMessage = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(bVisibleCountNewMessage));
                }
            }
        }
        public bool b_NeedChangeCursor = true;
        private async void Ddd_OnReciveStatisticUpdate(ChatStatisticUser stat)
        {
            if (chat.ObjId == stat.ChatId)
            {
                CountNewMessage = stat.CountNew.ToString();
                if (LastLoadedObjIdInInList < stat.LastObjId)
                {
                    if (stat.CountNew!=0)
                        bVisibleCountNewMessage = true;
                }
                if (LastPageInList == stat.PageLastObj-1
                     && LastLoadedObjIdInInList < stat.LastObjId
                    )
                {
                    b_NeedChangeCursor = false;
                    await Request_Message_Page(stat.PageLastObj - 1).ConfigureAwait(false);
                    await Request_Message_Page(stat.PageLastObj).ConfigureAwait(false);
                }
                if (LastPageInList == stat.PageLastObj
                    && LastLoadedObjIdInInList < stat.LastObjId
                    )
                {
                    b_NeedChangeCursor = false;
                    await Request_Message_Page(stat.PageLastObj).ConfigureAwait(false);
                }
                // chat.Statistic = stat;
            }
    }

        private void ObservableCollectionCallback(IEnumerable collection, object context, Action accessMethod, bool writeAccess)
        {
            lock (collection)
            {
                accessMethod?.Invoke();
            }
        }

        private void ItemsMsgs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add: // если добавление
                  //  User newUser = e.NewItems[0] as User;
                  //  Console.WriteLine($"Добавлен новый объект: {newUser.Name}");
                    break;
                case NotifyCollectionChangedAction.Remove: // если удаление
                                                           // User oldUser = e.OldItems[0] as User;
                                                           //   Console.WriteLine($"Удален объект: {oldUser.Name}");
                    break;
                case NotifyCollectionChangedAction.Replace: // если замена
                                                            //    User replacedUser = e.OldItems[0] as User;
                                                            //User replacingUser = e.NewItems[0] as User;
                                                            //Console.WriteLine($"Объект {replacedUser.Name} заменен объектом {replacingUser.Name}");
                    break;
            }
        }

        private void Pc_OnChat_PageCompleat(int chat_objId, int PageNum)
        {
           
        }

        volatile int Items_ObjId_CurrentMin = -1;
        volatile int Items_ObjId_CurrentMax = -1; 

        public bool b_send_Begin { get; set; }

        public void SetMinMaxList(int ObjId)
        {
            //         Items_ObjId_CurrentMax = ObjId;
            if (Items_ObjId_CurrentMax < ObjId)
            {
                Items_ObjId_CurrentMax = ObjId;
            }
        }

        private void Pc_OnChat_ShowMsgInList(int chat_objId, int PageNum, ObjMsg[] _msgs)
        {
            ObjMsg[] msgs = _msgs.ToArray();
            //  return;
            MainThread.BeginInvokeOnMainThread(  () =>
            {
                bool bAddEndMode = false;
                bool bAddStartMode = false;
                //  int [] ids= ItemsMsgs.Select(ee => ee.ObjId).ToArray();
                ObjMsg[] updateMsg = msgs.Where(s => objs.Contains(s.ObjId)).ToArray();
               ObjMsg[] newMsg = msgs.Where(s => !objs.Contains(s.ObjId)).ToArray();
               if (updateMsg.Length  > 0)
               {

               }
               if (newMsg.Length > 0)
               {
                    int min = newMsg.Min(s => s.ObjId);
                    int max = newMsg.Max(s => s.ObjId);
                    if (objs.Count == 0)
                    {
                        bAddEndMode = true;
                    }
                    else
                    {
                        int ExistMax = objs.Max();
                        int ExistMin = objs.Min();
                        if (ExistMax < min)
                        {
                            bAddEndMode = true;
                        }
                        if (ExistMin > max)
                        {
                            bAddStartMode = true;
                        }
                    }
                }
               else
               {
                   return;
               }

               //msgs = msgs.Where(s => !objs.Contains(s.ObjId)).Select(s => s.ObjId);
               try
                {
             
                    /*if (msgs == null || msgs.Length == 0)
                        return;*/
                    /*if (ItemsMsgs.Count == 0)
                    {
                        Items_ObjId_CurrentMin = int.MaxValue;
                        Items_ObjId_CurrentMax = int.MinValue;
                    }
                    */
                 /*   if (msgs[0].ObjId == Items_ObjId_CurrentMax)
                    {
                        bAddEndMode = true;
                        //     return;
                        //
                    }
                    if (msgs.Max(s => s.ObjId) > Items_ObjId_CurrentMax)
                    {
                        bAddEndMode = true;
                        //
                    }

                    */
                    if (bAddEndMode)
                    {// добавим в конец списка
                         
                        //if (b_send_Begin)// удаляем временые об отправке
                        //{
                        //    /*  ObjMsg [] item_sendbegin = Items.Where(s => s.ObjId == -1).ToArray();//.FirstOrDefault();
                        //      foreach  (var item in item_sendbegin )
                        //      {
                        //          Items.Remove(item);
                        //          b_send_Begin = false;
                        //      }*/
                        //}
                       
                       
                       foreach (var t in newMsg.OrderBy(s=>s.ObjId))
                        {
                            if (t != null && t.ObjId>0)
                            {
                                SetMinMaxList(t.ObjId);


                      //         if (ItemsMsgs.Where(s => s.ObjId == t.ObjId).Any() == false)
                               {
                                   ItemsMsgs.Add(t);
                                   objs.Add(t.ObjId);
                               }

                            }
                        }
                    }
                    if (bAddStartMode == true)
                    {
                       // добавим в начало списка
                       foreach (var t in newMsg.OrderByDescending(s=>s.ObjId))
                       // foreach (var t in )
                        {
                            SetMinMaxList(t.ObjId);

                 //          if (ItemsMsgs.Where(s => s.ObjId == t.ObjId).Any() == false)
                           {
                               ItemsMsgs.Insert(0, t);
                               objs.Add(t.ObjId);
                           }
                             /*0912*/
                        //    ItemsMsgs.Insert(0, t);
                        }
                    }
                }
                catch (Exception err)
                {
                }
           }
           );
         //  Task.Delay(1000);
        }

        internal async Task Request_Message_Shown(ObjMsg msgs)
        {
            try
            {
                if (App.ddd.SR != null)
                {
                    int[] pages = new int[] { nPage };
                    await App.ddd.SR.Request_Message_Shown(chat.ObjId, msgs.ObjId);
                }
            }
            catch (Exception err)
            { }
        //    msgs.ShownState = 14;
//            msgs.PropertyChanged(typeof(ShownState));
            
        }

        private void OpenPopup()
        {
      
        }

        public async  Task Request_Message_Page(int nPage)
        {
            int[] pages = new int[] { nPage };
            await App.ddd.SR.Request_Message_Page(chat.ObjId, pages);
        }
        public async Task Request_Message_Page(int nPage1, int nPage2)
        {
            int[] pages = new int[] { nPage , nPage2 };
            await App.ddd.SR.Request_Message_Page(chat.ObjId, pages);
        }


        internal async Task Request_Message_Send(string xml)
        {
            int[] pages = new int[] { nPage };
            await App.ddd.SR.Request_Message_Send(chat.ObjId, xml);
        }

        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }
        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;

                    await ExecuteLoadItemsCommand();

                    IsRefreshing = false;
                });
            }
        }
        internal async Task Request_Chat_Statistic(GroupChat chat)
        {
            try
            {
                await App.ddd.SR.Request_Chat_Statisic(chat.ObjId).ConfigureAwait(false);//// .Request_Chat_List(0).ConfigureAwait(true);
            }
            catch (Exception err)
            {
            }
        }
        async private Task ExecuteLoadItemsCommand()
        {
            try
            {
                if (IsRefreshing != true)
                {
                    if (isBusyLoadPage)
                        return;
                }
                try
                {

                    int DellayCount = 0;
                    isBusyLoadPage = true;
                    while (DellayCount < 20 )
                    {
                        if (chat.Statistic == null)
                        {
                            if (DellayCount == 0)
                            {
                                await Request_Chat_Statistic(chat);
                            }
                        }
                        else
                            break;
                        Thread.Sleep(200);

                        DellayCount++;
                    }
                    if (chat.Statistic == null) 
                        return;

                        int[] pages;
                    if (chat.Statistic != null)
                    {
                        
                            pages = new int[] { chat.Statistic.PageLastShownObj };
                    }
                    else
                        pages = new int[] { 0 };

                    for (int p = chat.Statistic.PageLastShownObj - 1; p <= chat.Statistic.PageLastShownObj; p++)
                    {
                        if (p >= 0)
                        {
                            pages = new int[] { p };
                            await App.ddd.SR.Request_Message_Page(chat.ObjId, pages);
                        }
                    }

                    if (chat.Statistic != null)
                        b_FirstInitPosition = chat.Statistic.LastShownObjId;


                    //await App.ddd.SR.Request_Message_Page(chat.ObjId, pages);
                    //ChatStatisticUser stat = App.ddd.Chat_GetMyStatistic(chat.ObjId);
                    //if (stat != null)
                    //    chat.Statistic = stat;
                    //pc.SetMaxPageNum(chat.Statistic.PageLastObj);
                    // Load
                }
                finally
                {
                   
                }
            }
            catch (Exception err)
            {
                isBusyLoadPage = false;
            }
        }

    
    }
}
