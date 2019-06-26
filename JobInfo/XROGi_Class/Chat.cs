using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobInfo.WS_JobInfo;
using JobInfo.XROGi_Extensions;

namespace JobInfo.XROGi_Class
{
    public enum ChatEventType {onUnknownEvent, onErrorEvent, onMessagesListUpdate, onMessagesReciverUpdate, onChatStatisticChanged, onFilterChanged };
    public class Chat
    {
        bool b_Selected;
        public bool Selected
        {
            get { return b_Selected; }
            set
            {
                if (b_Selected != value)
                {
                    b_Selected = value;
                    OnChatSelectedChanged(this, b_Selected);
                }

            }
        }
        private WS_JobInfo.Obj chat;
        public UserChatInfo _statistic;
        public UserChatInfo statistic
        {
            get { return _statistic; }
            set
            {
                if (value == null && _statistic != null)
                {

                }



                if ((_statistic != null && value != null)
                        && (_statistic.LastShownObjId != value.LastShownObjId
                                        ||
                            _statistic.LastObjId != value.LastObjId
                                        )
                    )
                {
                    _statistic = value;
                    OnChatEvent(this, ChatEventType.onChatStatisticChanged, _statistic);
                }
                else // if null
                {
                    _statistic = value;
                    if (value!=null)
                        OnChatEvent(this, ChatEventType.onChatStatisticChanged, _statistic);
                }


            }
        }
        public SortedList<int,WS_JobInfo.Obj> MessMessageArray;
        private int Hash_MessagesLast;

        public List<WS_JobInfo.User> users = new List<WS_JobInfo.User>();

      //  public delegate Task OnChatEventDelegate(Chat chat, ChatEventType et, object Tag);
      //  public event OnChatEventDelegate OnChatEvent;//= delegate { };

        public delegate void OnChatEventDelegate(Chat chat, ChatEventType et, object Tag);
        public event OnChatEventDelegate OnChatEvent= delegate { };


        public delegate void OnChatSelectedChangedDelegate(Chat sender, bool SelectedState);
        public event OnChatSelectedChangedDelegate OnChatSelectedChanged = delegate { };


        public delegate void OnChatRequestLastStatisticDelegate(Chat sender);
        public event OnChatRequestLastStatisticDelegate OnChatRequestLastStatistic = delegate { };


        private string _Text ;
        public string Text {
            get
            { if (_Text != null && _Text != String.Empty)
                    return _Text;
                else
                    if (chat != null)
                {
                    _Text = chat.GetString(GraphicsChatExtensions.XTypeStringInfo.Text_String);
                    return _Text;
                }
                else
                    return "Объект еще не определён";
            }
            set {  }
        }
        /*

        public Chat(WS_JobInfo.Obj chat)
        {
            this.chat = chat;
            this.statistic = null;
            Init();
        }
        */
        public Chat(WS_JobInfo.Obj chat, UserChatInfo stst)
        {
            this.chat = chat;
            this.statistic = stst;
            Init();
        }

        private void Init()
        {
            MessMessageArray = new SortedList<int, WS_JobInfo.Obj>();
            Hash_MessagesLast = 0;
            b_Selected = false;
        }

        public bool isMessageShown(int MsgId)
        {
            if (statistic == null)
                return false;
            if (statistic.LastShownObjId >= MsgId) return true;
            else return false;
        }
        public WS_JobInfo.Obj ObjId
        {
            get
            {
                return chat;
            }
            internal set
            {
                chat = value;
            }
        
        }
        public int chatId
        {
            get
            {
                return (int)chat.ObjId;
            }
        }
        /*
        internal int GetId()
        {
            return (int) chat.ObjId;
        }*/


        internal    void MessageDataArrayAddRange(asyncReturn_Messages ret)
        {
            if (ret.ErrorCount == 0)
            {
                if (ret.InParam_chatid == chatId && ret.ListMessages?.Length>0)
                {
                    foreach (WS_JobInfo.Obj o in ret.ListMessages)
                    {
                        if (MessMessageArray.ContainsKey((int)o.ObjId) == true)
                            MessMessageArray[(int)o.ObjId] = o;
                        else
                        {
                            MessMessageArray.Add((int)o.ObjId, o);
                        }
                    }
                 /*   var myEvent = OnChatEvent;
                    if (myEvent != null)
                        await Task.WhenAll(Array.ConvertAll(myEvent.GetInvocationList()
                        ,
                        e => ((OnChatEventDelegate)e).Invoke(this, ChatEventType.onMessagesReciverUpdate, ret.ListMessages)));
               
                        */
            //        IAsyncResult result =  OnChatEvent.BeginInvoke(this, ChatEventType.onMessagesReciverUpdate, ret.ListMessages);
                    //await 
                        OnChatEvent(this, ChatEventType.onMessagesReciverUpdate, ret.ListMessages);
                }
            }
        }

        internal void MessageArrayAddRange(int[] messageArray)
        {

            if (messageArray.Length > 0)
            {
       //         if (MessMessageArray.Count > 0)
                {
                    foreach (int key in messageArray)
                    {
                        if (MessMessageArray.ContainsKey(key)==false)
                                MessMessageArray.Add(key, null);
                    }
                    int last = messageArray[messageArray.Length - 1];
                    if (last> Hash_MessagesLast)
                        Hash_MessagesLast = messageArray[messageArray.Length - 1];
                    try
                    {
                        if (statistic==null)
                        {
                            OnChatRequestLastStatistic(this);// Запросить у контроллера последнюю статистику событием
                        }
                        OnChatEvent(this, ChatEventType.onMessagesListUpdate, messageArray);
                    }catch (Exception err)
                    {

                    }
                }
                //else
                //{

                //}
            }
            else
            {

            }
        }

        internal int Hash_GetMessagesLast()
        {
            return Hash_MessagesLast;
        }

        internal void SetMode_FileFilter(bool @checked)
        {
             
           OnChatEvent(this, ChatEventType.onFilterChanged, null);
        }

        internal void Close()
        {
            MessMessageArray.Clear();
            _statistic = null;
        }

        internal void Refresh_Statistic(Job job)
        {
            job.Chat_GetLastStatistic(this);
        }
    }
}
