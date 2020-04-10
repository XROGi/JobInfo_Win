using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using JIcon.Properties;
using JobInfo.WS_JobInfo;
using JobInfo.XROGi_Class;
using JobInfo.XROGi_Extensions;
using WebSocket4Net;

namespace JobInfo
{

    //enum JobEventType { onMessagesListUpdate, onMessagesUpdate, onDisconnected, onConnected };
    enum xrTypeSubscribe { GuestUserInChat };
    public class Job
    {
        public DateTime _DateCreateObject = DateTime.Now;//private
        public DateTime _DatePing;
        public DateTime _DatePong;
        public string[]  Setup_Params;

        public WS_JobInfo.StatusInfo[] statusInfo = null;

        private string Url;
        XROGi_Net net;
        long LastIdShow = 0;
        long LastIdStop = 0;
        
        public bool b_Connected;//private
        private xConnectStatus statusConnect;
        private bool _b_ReconnectAutomatical = true;
        WS_JobInfo.User user_self = null;

        public List<ObjOu> ou_data = new List<ObjOu>();
        ImageList il = new ImageList();
        

        public bool b_ReciveLoop = false;

        public Device this_device;
        User this_user;
        public List<Chat> Chats;
        public List<WS_JobInfo.Obj> MessgesList;

        public List<WS_JobInfo.User> users = new List<WS_JobInfo.User>();

        Chat Selected_Chat;

        List<Chat> Selected_Chats = new List<Chat>();

        public bool b_ReconnectAutomatical { get => _b_ReconnectAutomatical; set => _b_ReconnectAutomatical = value; }
        public xConnectStatus StatusConnect { get => statusConnect;
            set => statusConnect = value;
        }

        public delegate void OnJobClassEventDelegate(Job _job, string FunctionName);
        public event OnJobClassEventDelegate OnJobClassEvent = delegate { };

        public delegate void OnDebugClassEventDelegate(object sender, string FunctionName, string param_values);
        public event OnDebugClassEventDelegate OnDebugClassEvent = delegate { };


        public delegate void OnChatListChangedDelegate(Job _job);
        public event OnChatListChangedDelegate OnChatListChanged = delegate { };


        public delegate void OnConnecedDelegate(Job _job);
        public event OnConnecedDelegate OnConneced = delegate { };

        public delegate void OnTokenReciveDelegate(Job _job);
        public event OnTokenReciveDelegate OnTokenRecive = delegate { };

        public delegate void OnMsgReciveDelegate(Job _job, String cmd, long chatid, long msgid);
        public event OnMsgReciveDelegate OnMsgRecive = delegate { };


        public delegate void OnChatUpdateReciveDelegate(Job _job, String cmd, long chatid, long msgid);
        public event OnChatUpdateReciveDelegate OnChatUpdateRecive = delegate { };


        public delegate void OnDisconnecedDelegate(Job _job, Exception err);
        public event OnDisconnecedDelegate OnDisconneced = delegate { };

        public delegate void OnSocketSendDelegate(Exception err, string data);
        public event OnSocketSendDelegate OnSocketSend = delegate { };


        public delegate void OnChatEventDelegate(Chat chat, ChatEventType et, object Tag);
        public event OnChatEventDelegate OnChatEvent = delegate { };


        public delegate void OnMessage_GetFileAsyncDelegate(asyncReturn_GetFile ret, int ObjId, byte[] data);
        public event OnMessage_GetFileAsyncDelegate OnMessage_GetFileAsync = delegate { };

        public delegate void OnUser_GetListAllAsyncDelegate(object sender, User_GetListAllCompletedEventArgs e);
        public event OnUser_GetListAllAsyncDelegate OnUser_GetListAllAsync = delegate { };

        /// <summary>
        /// Делегант для подписки на обновления сообщений. Нужен для подписки копиям обьектов Chat.
        /// </summary>
        /// <param name="ChatId"></param>
        /// <param name="MessageArray"></param>
        public delegate void Message_GetListIDDelegate(int ChatId, int[] MessageArray);
        public event Message_GetListIDDelegate onMessage_GetListID = delegate { };

        public delegate void Message_ReciveListObjIdDelegate(asyncReturn_Messages ret);
        public event Message_ReciveListObjIdDelegate Message_ReciveListObjId = delegate { };


        public delegate void OnPingPongDelegate(Job job, DateTime Ping, DateTime Pong);
        public event OnPingPongDelegate OnPingPong = delegate { };


        /* public delegate void OnJobEventDelegate(Job _job,JobEventType et, string info);
         public event OnJobEventDelegate OnJobEvent = delegate { };


         */
        public delegate void OnBackWorkBeginDelegate(string Message);
        public event OnBackWorkBeginDelegate OnBackWorkBegin = delegate { };

        public delegate void OnObjStatusChangedDelegate(ObjStatus[] ObjStatus);
        public event OnObjStatusChangedDelegate OnObjStatusChanged = delegate { };

        public Job(string url)
        {
            b_Connected = false;
            StatusConnect = xConnectStatus.b_Created;
            this.Url = url;
            net = new XROGi_Net(Url);
            net.OnIncomeMessage += OnJob_IncomeMessage;
            net.OnErrorConneced += OnErrorConneced;
            net.OnSendCompleat += OnSendCompleat;
            net.OnConneced += OnConnected;
            net.OnConneced4 += OnConnected4;
            net.OnDisconneced += OnDisconnecedWS;
            net.onMessage_GetListID += Net_onMessage_GetListID;
            net.onMessage_GetListArray += Net_onMessage_GetListArray;
            net.OnMessage_GetFileAsync += Net_OnMessage_GetFileAsync;
            net.OnUser_GetListAllAsync += Net_OnUser_GetListAllAsync;
            net.OnErrorAsyncFunctionRun += Net_OnErrorAsyncFunctionRun;
            MessgesList = new List<WS_JobInfo.Obj>();
            Chats = new List<Chat>();
            b_ReconnectAutomatical = true;
            il.ImageSize = new Size(64, 64);
        }

        private void Net_OnErrorAsyncFunctionRun(Exception err, string Message)
        {
            OnJobClassEvent(this, string.Format("- Catch\t{0}. Debug\t{1} \t{2}", MethodBase.GetCurrentMethod().Name, Message,err.Message.ToString()));
        }

        private void Net_OnUser_GetListAllAsync(object sender, User_GetListAllCompletedEventArgs e)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));
            users.Clear();
            if (e.Result != null)
            {
                users.AddRange(e.Result.OrderBy(s => s.UserName));
                OnUser_GetListAllAsync(sender, e);
            }
        }

        public bool isConneced()
        {
            if (StatusConnect == xConnectStatus.b_Connected)
                return true;
            else
                return false;
        }
        private void Net_onMessage_GetListArray(asyncReturn_Messages ret)
        {
            
            Message_ReciveListObjId(ret);

        }

        private void Net_onMessage_GetListID(int ChatId, int[] MessageArray)
        {

  //          Chat c = Chats.Find(s => s.chatId == ChatId);
  //          c?.MessageArrayAddRange(MessageArray);
            onMessage_GetListID(ChatId, MessageArray);
        }

        private void OnConnected4(WebSocket4Net.WebSocket ws)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));
            OnConneced(this);
            b_Connected = true;
            StatusConnect = xConnectStatus.b_Connected;

            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));

            /*
                       if (RegisterUser(userName))
                       {
                           #region Регистрируем вход с компьютера
                           Device d = new Device(machineName);
                           bool resul = RegisterComputer(d);
                           #endregion


                       }*/

        }

        DateTime DateDisconnced = DateTime.Now;
        private void OnDisconnecedWS(ClientWebSocket ws)
        {
            try
            {
                OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));

                b_Connected = false;
                StatusConnect = xConnectStatus.b_Disconnected;
                b_ReciveLoop = false;

                this.this_device.Token_Clear();
                OnTokenRecive(this);
                OnDisconneced(this, null);
                DateDisconnced = DateTime.Now;
          //      ReconnectBegin();
            }catch (Exception err2)
            {

            }
        }

        internal UserStatus [] User_GetUsersStatus(int[] users)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));
            return net.User_GetUsersStatus(this_device.TokenId, this_device.Token_Counter, users);
        }

        public  void ReconnectBegin()
        {
            if (b_ReconnectAutomatical)
            {
                if ((DateTime.Now - DateDisconnced).TotalSeconds > 45)
                {
                    ConnectToServer(GetServerName());
                }
                else
                {

                }
            }
        }

        private void OnConnected(ClientWebSocket ws)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));
            OnConneced(this);
            b_Connected = true;
            StatusConnect = xConnectStatus.b_Connected;
        }

        private void OnSendCompleat(string Message)
        {
            b_ReciveLoop = true;
            WaitIncome();
            try
            {
                OnSocketSend(null, Message);
            }
            catch (Exception err)
            { }
        }

        private void OnErrorConneced(Exception err, string Message)
        {
            try
            {
                b_Connected = false;
                b_ReciveLoop = false;

                this.this_device.Token_Clear();
                StatusConnect = xConnectStatus.b_Disconnected;
                OnDisconneced(this, err);
            }catch (Exception err2)
            {

            }
        }

        private void OnJob_IncomeMessage(string msg_in)
        {
            try
            {
                try
                {
                    try
                    {


                        //        ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
                        // WebSocketReceiveResult result = await ws.ReceiveAsync(bytesReceived, CancellationToken.None);
                        //   resultsr = ReciveString(bytesReceived); ;
               //         Console.WriteLine(msg_in);
                        string msg = msg_in.ToUpper();
                        string cmd = GetSocketCMD(msg);
                        
                        if (cmd.StartsWith("CHAT"))
                        //if (cmd == "CHAT" || cmd == "CHATLEAVE" || cmd == "CHATADD" || cmd == "CHATRENAME")
                        {
                            string text = GetSocketText(msg);
                            int val = Convert.ToInt32(text);
                            // string GetNumber(t)
                            try
                            {
                                if (cmd == "CHATADD")
                                {//  подгрузить новый чат
                                    RequestChat(val);
                                }

                                if (val!=0)
                                    OnChatUpdateRecive(this, cmd, val, 0); //long (text
                            }catch (Exception err)
                            {

                            }
                        }
                        if (cmd == "MSG")
                        {
                            string text = GetSocketText(msg);
                            int ChatId = 0;
                            int MsgId = 0;

                            string[] vals = text.Split(',');
                            if (vals.Length == 2)
                            {
                                if (int.TryParse(vals[0], out ChatId))
                                {
                                    if (int.TryParse(vals[1], out MsgId))
                                    {
                                        OnMsgRecive(this, cmd, ChatId, MsgId); //long (text
                                    }

                                }
                            }
                            else
                           if (int.TryParse(text, out ChatId)) // не должно быть !
                            {
                                OnMsgRecive(this, cmd, 0, ChatId); //long (text
                            }
                            else
                                OnMsgRecive(this, cmd, 0, 0); // не должно быть !
                        }
                        if (cmd == "OK")
                        {

                            string text = GetSocketText(msg);


                            if (text != "")
                            {
                                if (text == "!!??!!")
                                {

                                    this.this_device.Token_Clear();
                                    b_Connected = false;
                                    try
                                    {
                                        OnDisconneced(this, null);
                                    }catch (Exception err2)
                                    {

                                    }
                                }
                                else
                                {
                                    this.this_device.Token_Set(Convert.ToInt64(text));
                                    statusConnect = xConnectStatus.b_Connected;
                                    OnTokenRecive(this);
                                }
                                //                            return;
                            }
                            else
                            {//Токен не выдан

                            }
                        }
                    }
                    catch (Exception err)
                    {

                    }
                }
                finally
                {
                    //     if (b_ReciveLoop)
                    //         net.ReciveAllBegin(); // повтор

                }
            } catch (Exception err)
            {

            }
        }

        private string GetSocketText(string msg)
        {
            int cb = msg.IndexOf("[");
            int ce = msg.IndexOf("]");
            if (cb > 0 && ce > 0 && cb < ce)
            {
                return msg.Substring(cb + 1, ce - cb - 1); ;
            }
            else
            {
                return String.Empty;
            }
        }
        private string GetSocketCMD(string msg)
        {
            if (msg=="PONG")
            {
                _DatePong = DateTime.Now;
                OnPingPong(this, _DatePing , _DatePong);
            }
            int cb = msg.IndexOf("[");
            if (cb > 0)
            {
                return msg.Substring(0, cb).ToUpper(); ;
            }
            else
            {
                return String.Empty;
            }
        }
   /*     public Job()
        {

        }*/
        internal ObjStatus[] Message_SetStatus(int ObjId, int Status)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));

            ObjStatus[] ret =net.Message_SetStatus(this_device.TokenId, this_device.Token_Counter, ObjId, Status);
            OnObjStatusChanged(ret);
            return ret; 
        }

        internal void Spravka_GetStatus()
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));
            WS_JobInfo.StatusInfo[] ret = net.Spravka_GetStatus(this_device.TokenId, this_device.Token_Counter );
            statusInfo = ret;
        }
        

        internal void Message_Shown(long chatid, long id)
        {

            var r = Chats.Where(s => s.ObjId.ObjId == chatid).FirstOrDefault();
            Chat c = Update_CurrentChat(r);

            if (c != null)
            {
                if (!c.isMessageShown((int)id))
                {

                    net.Message_Shown(this_device.TokenId, this_device.Token_Counter, chatid, id, 0);
                    /*Прямая редакция. Хоорошо бы перезапрашивать с сервера!!!*/
                    //c.statistic.LastShownObjId = (int)id;
    //20190609                c.statistic =  Chat_GetMyStatistic((int)chatid);
                }
                else
                {
                    net.Message_Shown(this_device.TokenId, this_device.Token_Counter, chatid, id, 1);

                    //возможно убраьть, порсто скопироваое сверху
          //20190301          c.statistic = Chat_GetMyStatistic((int)id);
                }
            }

            //   if ()
            {

            }
        }

        private Chat Update_CurrentChat(Chat chat)
        {
            if (Selected_Chat == null)
            {
                Selected_Chat = chat;
            }
            else
            {
                if (Selected_Chat == chat)
                {
                    return Selected_Chat;
                }
                else
                {
                    Selected_Chat = chat;
                }
            }
            return Selected_Chat;
        }

        internal bool RegisterComputer(string wsUrl, Device d)
        {
            if (this_user.isValid)
            {
                this_user.SetDevice(d);
                this_device = d;
                StatusConnect = xConnectStatus.b_Identity;
                Task ff = net.ConnectToToken(wsUrl, this_user, d);
                SetupParam_GetList();
               
            }
            return false;
        }

        public string []   SetupParam_GetList()
        {
            try
            {
                if (net != null)
                {
                    Setup_Params = net.Get_Setup_Params();
                    if (Setup_Params==null)
                    {
                        Setup_Params = new[] { "MainWEBServer=ws://jobinfo/xml/", "RunWinClient=false", "RunShowPageGroups=false" };
                    }
                    return Setup_Params;
                }
            }
            catch (Exception err)
            {
                //       throw err;
                Setup_Params = new[] { "MainWEBServer=ws://jobinfo/xml/", "RunWinClient=false", "RunShowPageGroups=false" };
            }
            return null;
        }

        internal int GetMyUserId()
        {
            if (this_user.isValid)
            {
                if (user_self != null)
                    return user_self.UserId;
                else
                    return -1;
                
            }
            return -1;
        }
        internal WS_JobInfo.User GetMyUser()
        {
            if (this_user.isValid)
            {
                if (user_self != null)
                    return user_self;
                else
                    return null;

            }
            return null;
        }

        internal bool RegisterUser(string userName)
        {
            this_user = new User(userName);
            if (true)
            {
                this_user.isValid = true;
                //проверка валидности юзера
                return true;
            }
            else
            {
                this_user.isValid = false;
                return false;
            }

        }

        internal void Connected(bool b_Connected)
        {
            this.b_Connected = b_Connected;
        }
        internal Chat[] GetChatList_FromCache()
        {
            return GetChatList_FromCache(0, 0);
        }

        /// <summary>
        /// Удалить из спсиска чатов чат и загрузить чат 
        /// </summary>
        /// <param name="ChatId"></param>
        /// <returns></returns>
        internal bool UpdateChat(int ChatId/*, int sgTypeId*/)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));
            WS_JobInfo.Obj updatedchat = net.Chat_Get (this_device.TokenId, this_device.Token_Counter, ChatId/*, sgTypeId*/);
            //WS_JobInfo.Obj[] os = net.Chat_GetList(this_device.TokenId, this_device.Token_Counter, ChatId/*, sgTypeId*/);
            if (updatedchat != null)
            {
     //           var list = os.Where(s => s.ObjId == ChatId);
   //             foreach (WS_JobInfo.Obj chat in list)
                {
                    try
                    {
                        var x = Chats.Where(s => s.chatId == ChatId).ToList();
                        foreach (Chat c in x)
                            Chats.Remove(c);
                    }catch (Exception err)
                    {

                    }
                    Chat_RegisterInList(updatedchat);
                }
            }

            return  true;
        }
        public bool UpdateChats(int ParentChatId/*, int sgTypeId*/)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));
            /*
            ParameterInfo[] parameters = MethodBase.GetCurrentMethod().GetParameters();

            
            for (int x = 0; x < parameters.Length; x++)
            {
                ParameterInfo p=  parameters[x];
           
                
            }
           */

            Chats.Clear();

            WS_JobInfo.Obj[] os = net.Chat_GetList(this_device.TokenId, this_device.Token_Counter, ParentChatId/*, sgTypeId*/);

            if (os != null)
            {
                foreach (WS_JobInfo.Obj chat in os)
                {
                    string ChatName = chat.GetText();
                    OnBackWorkBegin("Запрос сообщений для чата: " + ChatName);
                    if (chat.ObjId == 158)
                        continue;
                    Chat c = Chat_RegisterInList(chat);
                    
        


                }
                OnBackWorkBegin("");
            }
            else
                return false;


            return true;

        }

        private Chat Chat_RegisterInList(WS_JobInfo.Obj chat)
        {
            
            if (Chats.Where(s => s.chatId == chat.ObjId).Any())
            {
                //Чат уже зарегистрирован
                Chat c = Chats.Where(s => s.chatId == chat.ObjId).First();
                //c.OnChatEvent += JobOnChatEvent;
                var stat = net.Chat_GetMyStatistic(this_device.TokenId, this_device.Token_Counter, chat.ObjId);
                c.statistic = stat;
                //Chats.Add(c);
     //20190627            Message_GetListIDsNow(c);
                return c;
            }
            else
            {
            //    var stat = net.Chat_GetMyStatistic(this_device.TokenId, this_device.Token_Counter, chat.ObjId);
            //    if (Chats.Where(s => s.ObjId == (int)chat.ObjId).Any() == false)
                Chat c = new Chat(chat);//, stat);
                c.OnChatEvent += JobOnChatEvent;
                c.OnChatSelectedChanged += JobOnChatSelectedChangedEvent;
                c.OnChatRequestLastStatistic += OnChatRequestLastStatistic;
                c.OnChatRequestListIDs += Chat_OnChatRequestListIDs;
                //            c.statistic = stat;
                Chats.Add(c);
                c.GetLastStatistic();
                //20190627           Message_GetListIDsNow(c);
                return c;
            }
        }

        public string GetServerName()
        {
            SetupParam_GetList();
            /*if (Setup_Params == null)
                SetupParam_GetList();
                */

            if (Setup_Params != null)
            {
                string g = Setup_Params.Where(s => s.StartsWith("MainWEBServer=")).FirstOrDefault().Substring("MainWEBServer=".Length);
                return g;
            }else
            {
                return "";
            }
        }


        /// <summary>
        /// Запросить список сообщений
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="showMsgObjId"></param>
        /// <param name="showDirection"></param>
        /// <param name="b_async"></param>
        private void Chat_OnChatRequestListIDs(Chat sender, int showMsgObjId, ShowMessagePosition showDirection, bool b_async)
        {
            if (sender != null)
            {
                OnJobClassEvent(this, string.Format("{0}.{1}\tChatid={2}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name, sender.chatId));
                int Direction = 0;
                int maxCount = 0;// 0  - all
                if (showDirection == ShowMessagePosition.LastOnBootom) Direction = -1;
                if (showDirection == ShowMessagePosition.LastInTop) Direction = 1;

                net.Message_GetListIDs(this_device.TokenId, this_device.Token_Counter, sender.chatId, showMsgObjId, Direction , maxCount ,!b_async);
            }
        }

        private void OnChatRequestLastStatistic(Chat sender)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}\tChatid={2}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name, sender.chatId));

            var stat = net.Chat_GetMyStatistic(this_device.TokenId, this_device.Token_Counter, sender.chatId);

            if (stat != null && sender.chatId == sender.chatId)
            {
                sender.statistic = stat;
                if (sender.statistic == stat)
                { //пои идее не нужно
                    OnChatEvent(sender, ChatEventType.onChatStatisticChanged, stat);
                }
                else
                {

                }
            }
        }

        private void JobOnChatSelectedChangedEvent(Chat sender, bool SelectedState)
        {
            if (SelectedState == false)
            {
                Selected_Chats.Remove(sender);
            }
            else
                Selected_Chats.Add(sender);
        }

        /// <summary>
        /// Запросить данные чата с сервера
        /// </summary>
        /// <param name="chatid"></param>
        internal void RequestChat(int chatid)
        {
            WS_JobInfo.Obj new_chat = net.Chat_Get(this_device.TokenId, this_device.Token_Counter, chatid);
            if (new_chat != null)
            {
                Chat_RegisterInList(new_chat);
            };
        }

        private void  JobOnChatEvent(Chat chat, ChatEventType et, object Tag)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name+"\t"+et.ToString()+"\t"+Tag.ToString()));
            OnChatEvent(chat, et, Tag);
        }

        internal WS_JobInfo.Obj[] Chat_GetList_PublicByType(int ParentChatId, int [] sgTypeId)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));
        
            WS_JobInfo.Obj[] os = net.Chat_GetList_PublicByType(this_device.TokenId, this_device.Token_Counter, ParentChatId, sgTypeId);
            return os;
        }

        internal WS_JobInfo.User GetUser(int userid)
        {
            if (users.Where(s => s.UserId == userid).Any() == false)
            {
                var ee = net.Get_User(this_device.TokenId, this_device.Token_Counter, userid);
                if (ee != null)
                {
                    users.Add(ee);
                    return ee;
                }
            }
            else
            {
                return users.Where(s => s.UserId == userid).FirstOrDefault();
            }
            return null;
        }

        internal Chat[] GetChatList_FromCache(int ParentChatId, int sgTypeId)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));
            /*
            ParameterInfo[] parameters = MethodBase.GetCurrentMethod().GetParameters();

            
            for (int x = 0; x < parameters.Length; x++)
            {
                ParameterInfo p=  parameters[x];
           
                
            }
           */

            /*  bool b_change = false;
              WS_JobInfo.Obj[] os = Chats.Select(s => s.ObjId ).ToArray();// net.Chat_GetList(this_device.TokenId, this_device.Token_Counter, ParentChatId, sgTypeId);
              */
            return Chats.ToArray();


            /* foreach (WS_JobInfo.Obj o in  os)
             {
                 if (ChatList.Where(s => s.id == o.ObjId).Any() == false) ;
                 {
                     Obj lo = new Obj() { id = o.ObjId, temp_string = o.Type, type = MsgType.chat };
                     ChatList.Add(lo);
                     b_change = true;
                 }

             }
                 //if (b_change)                     OnChatListChanged(this);
             return ChatList.Where(s => s.type == MsgType.chat).ToArray();*/
        }
        internal void Message_GetFile(int objId, string guid, int TypeImage)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name
                + " "+objId.ToString()+" " + guid + " " + TypeImage.ToString()
                ));

      
            net.Message_GetFile_Async(this_device.TokenId, this_device.Token_Counter, objId, guid, TypeImage);
        }

        private void Net_OnMessage_GetFileAsync(asyncReturn_GetFile ret, int ObjId, byte[] data)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name
               + " " + ret.InParam_ObjId.ToString() + " " + ret.TypeRequest.ToString()
               ));

            OnMessage_GetFileAsync(ret, ObjId, data);
        }

        internal Image Message_Image_Get(int objId, string guid, int TypeImage)
        {
           return byteArrayToImage(net.Message_Image_Get(this_device.TokenId, this_device.Token_Counter, objId, guid, TypeImage));
        }
        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn != null && byteArrayIn.Length>0)
            {
                try
                {
                    Image returnImage;
                    MemoryStream ms = new MemoryStream(byteArrayIn, 0, byteArrayIn.Length);
                    ms.Write(byteArrayIn, 0, byteArrayIn.Length);
                    returnImage = Image.FromStream(ms, true);//Exception occurs here
                    return returnImage;
                }
                catch { }
            }
            
            return null;
        }

         internal void Message_GetListArray(int ParentChatId,  int [] msgids)
        {
      //      Cursor c = Cursor.Current;
            try
            {
                try
                {
             //       Chat_GetMyStatistic(ParentChatId);
            //        Cursor.Current = Cursors.AppStarting;
                    string s = String.Join(",", msgids);

                    OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name + "\t" + s));



                //    await Task.Run(() => {
                        //await
                        net.Message_GetListArray(this_device.TokenId, this_device.Token_Counter, ParentChatId, msgids);
//                    });

                    
                }catch (Exception err)
                {

                }
            }
            finally
            {
             //   Cursor.Current = c;
            }
        }
        /*
        internal WS_JobInfo.ObjStatus[] Message_GetListArray(int ObjId)
        {
            Cursor c = Cursor.Current;
            try
            {
                OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name  ));
                WS_JobInfo.ObjStatus[] ret = net.Message_GetStatus(this_device.TokenId, this_device.Token_Counter, ObjId);
                return  ret;
            }
            finally
            {
                Cursor.Current = c;
            }
        }*/
        internal WS_JobInfo.view_ObjStatus_HistoryInfo[] Message_GetStatusHistory(int ObjId)
        {
            Cursor c = Cursor.Current;
            try
            {
                OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));
                WS_JobInfo.view_ObjStatus_HistoryInfo[] ret = net.Message_GetStatusHistory(this_device.TokenId, this_device.Token_Counter, ObjId);
                return ret;
            }
            finally
            {
                Cursor.Current = c;
            }
        }
        


        internal WS_JobInfo.Obj[] GetMessages(long ParentChatId, int Get_AfterMesageId, int CountDelta)
        {
            Cursor c = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.AppStarting;

                OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));
                //        MessgesList.Clear();

                WS_JobInfo.Obj[] os = net.GetMessages(this_device.TokenId, this_device.Token_Counter, ParentChatId, Get_AfterMesageId, CountDelta);

                if (os != null)
                {
                    
                    //    var Rest = MessgesList.Union();
                    //          MessgesList.Clear();

                    //        MessgesList.AddRange(os.Where(s => s.ObjId > Get_AfterMesageId).ToArray());//.AddRange(os);
                }
           //     else
            //        MessgesList.Clear();

                return os;
            }finally
            {
                Cursor.Current = c;
            }
        }

        internal WS_JobInfo.Obj[] GetMessages(long ParentChatId, WS_JobInfo.UserChatInfo cui, int CountDelta)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));
            //        MessgesList.Clear();
            bool b_change = false;
            long CurrentPosition = 0;
            if (cui != null)
            {
                CurrentPosition = cui.LastShownObjId;
            }
            WS_JobInfo.Obj[] os = net.GetMessages(this_device.TokenId, this_device.Token_Counter, ParentChatId, CurrentPosition, CountDelta);
            if (os != null)
            {
                //    var Rest = MessgesList.Union();
                //          MessgesList.Clear();

       //         MessgesList.AddRange(os);//.AddRange(os);
            }
       //     else
       //         MessgesList.Clear();

            return os;
        }

        
        internal void Message_GetListIDs(int ParentChatId, int afterId)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));
            net.Message_GetListIDs(this_device.TokenId, this_device.Token_Counter, ParentChatId, afterId,1,0,false);
        }


        internal void Message_GetListIDs(Chat c)
        {
            int last = c.Hash_GetMessagesLast();
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name+" "+last.ToString()));
     
            net.Message_GetListIDs(this_device.TokenId, this_device.Token_Counter, c.chatId, last,1,0, false);
        }

        internal void Message_GetListIDsNow(int chatid, int LastObjId)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));
            net.Message_GetListIDs(this_device.TokenId, this_device.Token_Counter, chatid, LastObjId,1,0, true);
        }
        internal void Message_GetListIDsNow(Chat c)
        {
            if (c != null)
            {
                OnJobClassEvent(this, string.Format("{0}.{1}\tChatid={2}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name,c.chatId));
                net.Message_GetListIDs(this_device.TokenId, this_device.Token_Counter, c.chatId, c.Hash_GetMessagesLast(),1,0, true);
            }
        }
        internal void Message_GetListIDsNow(int chatid)
        {
            Chat c = Chats.Where(s => s.chatId == chatid).FirstOrDefault();
            Message_GetListIDsNow(c);
        }

        internal WS_JobInfo.Obj[] GetMessages(long ParentChatId, int nScrollMsgCount)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));

            {

   //             MessgesList.Clear();
                bool b_change = false;
                long CurrentPosition = 0;
                try
                {
                    Chat_UpdateMyStatistic((int)ParentChatId);

                    WS_JobInfo.Obj[] os = net.GetMessages(this_device.TokenId, this_device.Token_Counter, ParentChatId, CurrentPosition, nScrollMsgCount);

                    return os;
                } catch (Exception err)
                {
                    return null;
                }
                /*
                foreach (WS_JobInfo.Obj o in os)
                {
                    if (MessgesList.Where(s => s.id == o.ObjId).Any() == false)
                    {
                        Obj lo = new Obj() { id = o.ObjId, temp_string = "id="+o.ObjId.ToString()+" "+ o.xml.ToString(), type = MsgType.msg };
                        MessgesList.Add(lo);
                        b_change = true;
                    }

                }*/
            }
            //if (b_change)                OnMsgListChanged(this);

            //MessgesList = new List<Obj>();
            //     return MessgesList.Where(s => s.type == MsgType.msg).ToArray();






            /*            objects.Add_onlyIfNotExists(new Obj() { id = 7, type = MsgType.msg ,temp_string ="d"});
                        objects.Add_onlyIfNotExists(new Obj() { id = 8, type = MsgType.msg, temp_string = "msg Проверка\r\nString 2\r\nString 3\r\nString 3\r\nString 3\r\nString 3" });
                        objects.Add_onlyIfNotExists(new Obj() { id = 9, type = MsgType.msg, temp_string ="d"});*/
            //  ChatList.Add_onlyIfNotExists(new Obj() { id = 10, type = MsgType.chat ,temp_string ="d"});
            //  return ChatList.ToArray();
        }



        internal void UnregisterComputer(Device d)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));

            ///Task ff = net.TokenTest(this_user, d);
            Task ff =
                  net.TokenDisable(this_device.TokenId, this_user, d);

            //         ff.Wait(); // Blocks current thread until GetFooAsync task completes
            // For pedagogical use only: in general, don't do this!
            //   var result = ff..Result;

        }

        internal void LoadObjects()
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));



            /*     if (objects == null)
                     objects = new List<Obj>();
                 objects.Add(new Obj() { id = 1, type = "chat" , temp_string = "Проверка" });
                 objects.Add(new Obj() { id = 2, type = "chat", temp_string = "Проверка\r\nString 2\r\nString 3" });
                 objects.Add(new Obj() { id = 3, type = "chat", temp_string = "Проверка String StringStringString String String  String ONE STRING" });
                 objects.Add(new Obj() { id = 4, type = "chat", temp_string = "Проверка\r\nString 2\r\nString 3\r\nString 3\r\nString 3\r\nString 3" });
                 objects.Add(new Obj() { id = 5, type = "chat" });
                 objects.Add(new Obj() { id = 6, type = "msg", temp_string = "Проверка\r\nString 2\r\nString 3\r\nString 3\r\nString 3\r\nString 3" });
                 objects.Add(new Obj() { id = 7, type = "msg" });
                 objects.Add(new Obj() { id = 8, type = "msg", temp_string = "Проверка\r\nString 2\r\nString 3\r\nString 3\r\nString 3\r\nString 3" });
                 objects.Add(new Obj() { id = 9, type = "msg" });
                 objects.Add(new Obj() { id = 10, type = "chat" });
                 return;*/
        }
        /*
        internal Obj[] GetLoadedMessages(object p)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));

            return MessgesList.ToArray();
        }*/


        internal void Job_SetMsgLast(Obj o)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));

            if (o.isShown() == false)
            {

            }
            LastIdStop = o.id;
            if (LastIdStop > LastIdShow)
            {
                LastIdShow = LastIdStop;
            }
        }
        //internal JobLastShow Job_GetMsgLast()
        //{
        //    JobLastShow last = new JobLastShow(0, 0);
        //    return last;

        //}


        internal void Job_Leave(long chatid)
        {

            net?.Job_Leave(this_device.TokenId, this_device.Token_Counter, chatid);
        }

        internal void GetUsers()
        {
            if (users.Count == 0)
            {
                //var ee =  
                    net.Get_UserListAsync(this_device.TokenId, this_device.Token_Counter);
                /*if (ee != null)
                {
                    users.AddRange(ee);
                }*/
            }
        }

        internal void WaitIncome()
        {
            try
            {
                //          net.ReciveAllBegin();
            } catch (Exception err)
            {

            }
        }

        internal void SendMessage(string v1, string v2)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));

            //        XDocument doc1 = XDocument.Parse("");
            XNamespace xr = "http://localhost/xrogi";
            //         var ns = doc1.Root.Name.Namespace;
            XDocument result = new XDocument(new XElement(xr + "cmd", new XAttribute("name", v1)));
            result.Root.Add(new XElement(xr + "msg") { Value = v2 });

            string cs = result.ToString();
            net.SendXML2Socket(cs);
        }

        private  void ConnectToServer(string wsUrl)
        {
            if (this_user!=null)
                if (this_device!=null)
                {
                    ConnectToServer(wsUrl, this_device.Name, this_user.userName);
                }
        }
        public void ConnectToServer(string wsUrl, string machineName, string userName)
        {

            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));


            if (RegisterUser(userName))
            {
                #region Регистрируем вход с компьютера
                Device d = new Device(machineName);
                bool resul = RegisterComputer(wsUrl, d);
                #endregion


            }
        }

        public void Close()
        {
            UnregisterComputer(this_device);

            foreach (Chat c in Chats)
            {
                c.Close();
            }
            Chats?.Clear();
            ou_data?.Clear();
            users?.Clear();
            MessgesList?.Clear();

            //if (this_user!=null)
            net.WS_CloseConnect();
            b_Connected = false;
            //      throw new NotImplementedException();
      //      _job = null;
        }

        public void CloseWS()
        {
            UnregisterComputer(this_device);//Если зарегистрирован то разорвём  регистрацию
            net.WS_CloseConnect();
            b_Connected = false;
            //      throw new NotImplementedException();
            //      _job = null;
        }

        internal WS_JobInfo.ChatTypes[] GetTypeChatList()
        {

            return net.GetTypeChatList(this_device.TokenId, this_device.Token_Counter);
        }

        internal int  Chat_Add(long ParentChatId, int sgTypeId, string nameChat, string ChatComment)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));

            return net.Chat_Add(this_device.TokenId, this_device.Token_Counter, ParentChatId, sgTypeId, nameChat, ChatComment);
        }

        internal int Chat_CreateAndSubscribe(long ParentChatId, int sgTypeId, string nameChat, string ChatComment, int[] users)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));

            return net.Chat_CreateAndSubscribe(this_device.TokenId, this_device.Token_Counter, ParentChatId, sgTypeId, nameChat, ChatComment, users);
        }
        
        internal void Chat_AddCorporative(long ParentChatId, int sgTypeId, String UID_Subdiv)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));

            //net.ChatAdd(this_device.TokenId, this_device.Token_Counter, ParentChatId, sgTypeId, nameChat, ChatComment);
        }

        internal void Add_Message(int Chatid, string text)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));

            net.MessageAdd(this_device.TokenId, this_device.Token_Counter, Chatid, text);
        }
        internal void Add_MessageSmile(long id, string text)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));

            net.Message_AddSmile(this_device.TokenId, this_device.Token_Counter, id, text);
        }

        internal void Chat_Rename(Chat o, String NewName)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));

            net.Chat_Rename(this_device.TokenId, this_device.Token_Counter, o.ObjId.ObjId, NewName);
        }
        internal void Chat_CreateLink(int NewParentChatId, int CurrentChatId)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));
            net.Chat_CreateLink(this_device.TokenId, this_device.Token_Counter, NewParentChatId, CurrentChatId);
        }
        internal void Chat_CreateMainLink(int NewParentChatId, int CurrentChatId)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));
            net.Chat_CreateMainLink(this_device.TokenId, this_device.Token_Counter, NewParentChatId, CurrentChatId);
        }
        internal int Chat_GetSelected()
        {
            int chatid = net.Chat_GetSelected(this_device.TokenId, this_device.Token_Counter);
            return chatid;
        }
        internal bool Chat_Selected(long id)
        {
            OnSocketSend(null, "ChatSelected= "+id.ToString());
            bool b_changed = false;
            try
            {
            
            if (id<=0)
            {
                Chats.Clear();
                Selected_Chat = null;
                 b_changed = true;
                return b_changed;
            }
            
            if (Selected_Chat != null && Selected_Chat.ObjId!=null &&  Selected_Chat.ObjId.ObjId!=id)
            {
                b_changed = true;
                MessgesList.Clear();
            }
            net.Chat_Selected(this_device.TokenId, this_device.Token_Counter,id);
           
                Chat ch = Chats.Where(s => s.ObjId != null && s.ObjId.ObjId == id).FirstOrDefault();
              //  Chat c = 
                    Update_CurrentChat(ch);

            }
            catch (Exception err)
            {

            }
            return b_changed;

        }



        internal WS_JobInfo.UserChatInfo Chat_GetLastStatistic(Chat c, bool b_Now = true)
        {
            OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name + " for one chat chatid=\t" + c.chatId.ToString()));
            int LastObjId = 0;
            if (c?.statistic != null)
            {
                LastObjId = c.statistic.LastObjId;
            }
            WS_JobInfo.UserChatInfo cui = net.Chat_GetMyStatistic(this_device.TokenId, this_device.Token_Counter, c.chatId, b_Now);
            c.statistic = cui;
            if (cui !=null && (LastObjId != cui.LastObjId || cui.LastObjId == cui.StartShownObjId)
                ||
                (c.MessMessageArray.Count>0 && c.MessMessageArray?.Last()!=null &&    LastObjId != c.MessMessageArray?.Last().Key)
                )
            {
                if (b_Now)
                    Message_GetListIDsNow(c);
                else
                    Message_GetListIDs(c);
            }
            //            c.statistic = null;
            return cui;
            /*2
            WS_JobInfo.UserChatInfo cui = net.Chat_GetMyStatistic(this_device.TokenId, this_device.Token_Counter, c.chatId,b_Now);
            //foreach (Chat c in Chats.Where(s => s.chatId == id).ToArray())
            if (cui!=null)
            {
                c.statistic = null;
                c.statistic = cui;
                Message_GetListIDs(c);
            }
            */
            //return cui;
        }

        internal UserChatInfo Chat_GetMyStatistic(long id, bool b_Now=true)
        {
            try
            {
                OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name + " chatid=\t" + id.ToString()));

                Chat[] rr = Chats.Where(s => s.chatId == id).ToArray();
                foreach (Chat c in rr)
                {
                 return Chat_GetLastStatistic(c, b_Now);
                }
                
                
            }catch (Exception err)
            {
                //Внимание! Разобраться кто вызвал и не обработал Exception
                // Тут обработчика быть не должно!

                // при переименовании чата иногда!
                /* ****Недопустимая операция в нескольких потоках: попытка доступа к элементу управления 'treeView1' не из того потока, в котором он был создан.
                    JobInfo.exe!JobInfo.Job.Chat_GetMyStatistic(long id)Строка 1068 C#
                    JobInfo.exe!JobInfo.Job.Message_GetListArray(int ParentChatId, int[] msgids)Строка 637  C#
                    JobInfo.exe!JobInfo.XROGi_Class.ChatMessagesPanel.Display_MessageIndex(int indexMessMessageArray)Строка 996 C#
                        JobInfo.exe!JobInfo.XROGi_Class.ChatMessagesPanel.Display_Message(int lastShownObjId, JobInfo.XROGi_Class.ChatMessagesPanel.ShowMessagePosition lastOnBootom)Строка 1111	C#
                    JobInfo.exe!JobInfo.XROGi_Class.ChatMessagesPanel.OnChatEvent(JobInfo.XROGi_Class.Chat chat, JobInfo.XROGi_Class.ChatEventType et, object Tag)Строка 915	C#
                    JobInfo.exe!JobInfo.XROGi_Class.Chat.MessageArrayAddRange(int[] messageArray)Строка 165	C#
                    JobInfo.exe!JobInfo.Job.Net_onMessage_GetListID(int ChatId, int[] MessageArray)Строка 127	C#
                    JobInfo.exe!JobInfo.XROGi_Net.Message_GetListIDs(long tokenId, long token_Counter, int parentChatId, int AfterPosition, bool b_Now)Строка 395	C#
                    JobInfo.exe!JobInfo.Job.Message_GetListIDsNow(JobInfo.XROGi_Class.Chat c)Строка 734	C#
                    JobInfo.exe!JobInfo.Job.Message_GetListIDsNow(int chatid)Строка 740	C#
                    JobInfo.exe!JobInfo.XROGi_Class.ChatMessagesPanel.On_Job_ChatUpdateRecive(JobInfo.Job _job, string cmd, long chatid, long msgid)Строка 1215	C#
                    JobInfo.exe!JobInfo.Job.OnJob_IncomeMessage(string msg_in)Строка 243	C#
                    JobInfo.exe!JobInfo.XROGi_Net.OnWSIncomeMessage(string Message)Строка 606	C#
                    JobInfo.exe!JobInfo.WS.Receive(object sender, WebSocket4Net.MessageReceivedEventArgs e)Строка 98	C#

                */

            }
            return null;
        }

        internal UserChatInfo Chat_UpdateMyStatistic(long id)
        {
            return Chat_GetMyStatistic(id);
       /*     Chat c = Chats.Where(s => s.ObjId.ObjId == id).FirstOrDefault();
            if (c != null)
            {
                WS_JobInfo.UserChatInfo cui = net.Chat_GetMyStatistic(this_device.TokenId, this_device.Token_Counter, c.ObjId.ObjId);

                if (c.statistic == null) c.statistic = cui;  

                if (
                   (
                            cui!=null 
                   //     &&
                   //         (
                   //         cui.LastShownObjId != c.statistic.LastShownObjId
                   //         || cui.LastObjId != c.statistic.LastObjId
                   //         || cui.CountNew != c.statistic.CountNew
                   //         )
                    )
                    )
                {
                    c.statistic = null;
                    c.statistic = cui; //изменилась статистика
                }
                return cui;
            }
            return null;*/
        }

        internal WS_JobInfo.Obj[] GetOU(string root)
        {
            WS_JobInfo.Obj[] ou= net.OU_GetList(this_device.TokenId, this_device.Token_Counter, root);
            return ou;
        }

        internal WS_JobInfo.User[] GetOU_Users(string Guid)
        {
            WS_JobInfo.User[] ou = net.OU_GetUserList(this_device.TokenId, this_device.Token_Counter, Guid);
            return ou;
        }

        internal  string Chat_AddCorporative(WS_JobInfo.Obj obj_ou, string text)
        {
            string ou = net.Chat_AddCorporative(this_device.TokenId, this_device.Token_Counter, obj_ou, text);
            return ou;
        }

        internal WS_JobInfo.User[] GetChat_Users(int objId)
        {
            WS_JobInfo.User[] ou = net.Chat_GetUserList(this_device.TokenId, this_device.Token_Counter, objId);
            return ou;
        }

        internal WS_JobInfo.User[] User_FindFull(string text)
        {
            WS_JobInfo.User[] ou = net.User_FindFull(this_device.TokenId, this_device.Token_Counter, text);
            return ou;
        }

        internal bool Chat_SubscribeUser( long objId, WS_JobInfo.User user, xrTypeSubscribe typeSubscribe)
        {
            //WS_JobInfo.User[] ou =
            bool  res=  net.Chat_SubscribeUser(this_device.TokenId, this_device.Token_Counter, objId , user, 21);
            return res;
        }
        internal bool Chat_UnSubscribeUser(WS_JobInfo.Obj obj, long objId, WS_JobInfo.User user, int typeSubscribe)
        {
            //WS_JobInfo.User[] ou =
            bool res = net.Chat_UnSubscribeUser(this_device.TokenId, this_device.Token_Counter, objId, user, 21);
            return res;
        }
        internal view_tbl_HashTag [] Hash_GetList( string find, int ObjId, int UserId)
        {
            //WS_JobInfo.User[] ou =
            view_tbl_HashTag [] res = net.Hash_GetList(this_device.TokenId, this_device.Token_Counter, find, ObjId, UserId);
            return res;
        }


        internal void Hash_Update(view_tbl_HashTag h)
        {
            //view_tbl_HashTag[] res =
                net.Hash_Update(this_device.TokenId, this_device.Token_Counter, h);
            //return res;
        }


        internal Chat GetSelectedChat()
        {
            if (Selected_Chat != null)
            {
                XROGi_Class.Chat c = Chats.Where(s => s.ObjId.ObjId == Selected_Chat.ObjId.ObjId).FirstOrDefault();
                return c;

            }
            else
                return null;
        }

        internal void Add_Image(long objId, Image image1, string Comment)
        {
            net.Add_Image(this_device.TokenId, this_device.Token_Counter, objId, image1, Comment);
        }


        private void E(ChatWsFunctionException err)
        {
          
            {
                System.Diagnostics.Debug.WriteLine(err.Message.ToString());

              
                //       MessageBox.Show(err.Message.ToString(), "ChatWsFunctionException", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void E(ChatDisconnectedException err)
        {
         
            {
                System.Diagnostics.Debug.WriteLine(err.Message.ToString());
             
                //          MessageBox.Show(err.Message.ToString(), "ChatDisconnectedException", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void E(Exception err)
        {
            {
                System.Diagnostics.Debug.WriteLine(err.Message.ToString());
        
            }

        }
        internal void LoagAllOU()
        {
            try
            {

                if (ou_data.Count!=0) return;
                ou_data.Clear();
                WS_JobInfo.Obj[] ous = GetOU("");
                ObjOu[] arr = ous.Select(s => new ObjOu(s, null)).ToArray();
                ou_data.AddRange(arr);
           /*    
                


                if (ous != null)
                    foreach (WS_JobInfo.Obj ou in ous)
                    {
                       
                            LoadOU(ou);
                    }
              */
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err);   }
        }
        internal void LoagAllUsers()
        {
            try
            {

                if (ou_data.Count != 0) return;
                ou_data.Clear();
                WS_JobInfo.Obj[] ous = GetOU("");
                ObjOu[] arr = ous.Select(s => new ObjOu(s, null)).ToArray();
                ou_data.AddRange(arr);
                /*    



                     if (ous != null)
                         foreach (WS_JobInfo.Obj ou in ous)
                         {

                                 LoadOU(ou);
                         }
                   */
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); }
        }

        private void LoadOU(WS_JobInfo.Obj ou)
        {
            try
            {
                string root = "";
                WS_JobInfo.Obj o = ou;
                if (o != null)
                    root = o.Guid;
                WS_JobInfo.Obj[] ous = GetOU(root);
                if (ous != null)
                {
                    ObjOu[] arr = ous.Select(s => new ObjOu(s, o)).ToArray();
                    ou_data.AddRange(arr);

                    foreach (WS_JobInfo.Obj ou2 in ous)
                    {
                        LoadOU(ou2);
                    }
                }
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err);    }
            catch (Exception err) { E(err); }
        }

        public  WS_JobInfo.User UserGetSelf()
        {

            user_self =  net.UserGetSelf(this_device.TokenId, this_device.Token_Counter );
            return user_self;
        }

        internal void SetUserParameter(WS_JobInfo.User user, UserDopParameter parPhone, int identityParamToServerChanged, string newValue)
        {
           

                net.User_SetParameter(this_device.TokenId, this_device.Token_Counter
                , user.UserId
                , identityParamToServerChanged
                ,(int)parPhone
                ,newValue
                );
          
        }



        static public byte[] BitmapDataFromBitmap(Bitmap objBitmap, ImageFormat imageFormat)

        {

            MemoryStream ms = new MemoryStream();

            objBitmap.Save(ms, imageFormat);

            return (ms.GetBuffer());

        }

        static public byte[] BitmapDataFromBitmap(Image objBitmap, ImageFormat imageFormat)

        {

            MemoryStream ms = new MemoryStream();

            objBitmap.Save(ms, imageFormat);

            return (ms.GetBuffer());

        }
        internal bool User_GetFoto(WS_JobInfo.User u)
        {
            if (u.foto == null)
            {
                u.foto = net.User_GetFoto(this_device.TokenId, this_device.Token_Counter ,u.UserId);
                if (u.foto==null)
                {
                    try
                    {
                        if (!(il.Images.IndexOfKey("X_user_50px") >= 0))
                        {
                            var bmp = new Bitmap(Resources.user_50px);
                            //var bmp = new Bitmap(50,50);
                            u.foto = BitmapDataFromBitmap(bmp, ImageFormat.Png);
                            il.Images.Add("X_user_50px", bmp);
                            
                        }
                        else
                        u.foto = BitmapDataFromBitmap(il.Images["X_user_50px"], ImageFormat.Png);  
                    }
                    catch (Exception err)
                    {

                    }
                }
                return true;
            }
            else
            {

            }
            return false;
        }
        internal fn_GetUserParametersResult[]  User_GetParameters(int UserId)
        {


            return net.User_GetParameters(this_device.TokenId, this_device.Token_Counter
            , UserId

            );

        }

        internal void Chat_SetPublic(Chat o, bool b_SetPubic)
        {
            net.Chat_SetPublic(this_device.TokenId, this_device.Token_Counter
          , o.chatId, b_SetPubic );
        }

        public void PingWinSocket()
        {

            try
            {
                if (isConneced())
                {
                    net.PingWinSocket();
                    _DatePing = DateTime.Now;
                }
            }
            catch (Exception err)
            { E(err); };
        }

        internal void Message_Replay(XMessageCtrl newmsg)
        {
            try
            {
                OnJobClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name));

                string textxml = "<root>" + newmsg.Data2String() + "</root>";

                net.MessageAdd(this_device.TokenId, this_device.Token_Counter, newmsg.chatId, textxml);
            }

            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); }
            catch (Exception err) { E(err); };
            ///et.MessageReplayAdd();
            ///net.MessageReplayAdd(newmsg);
        }




        internal Chat[] Get_PrivateChats()
        {
            int sgTypeId = 8;
            return Chats.Where(s => s.ObjId.sgTypeId == sgTypeId
                             //      && s.ObjId.UsersInChat.Where(s2 => s2 == !=null)
                             ).ToArray();
        }
    internal Chat FindPrivateChat(int userId)
        {

      
            int sgTypeId = 8;
            if (Chats.Where(s => s.ObjId.sgTypeId == sgTypeId
                                    && s.ObjId.UsersInChat.Where(s2 => s2 == userId).Any()
                                ).Any() == true)
            {
             return Chats.Where(s => s.ObjId.sgTypeId == sgTypeId
                                  && s.ObjId.UsersInChat.Where(s2 => s2 == userId).Any()).FirstOrDefault();
            }

        
            /*
            foreach (Chat c in Chats.Where(s=>s.ObjId.sgTypeId==8 && s.users.Where(ss=>ss.UserId== userId).Any()))
            {
           
                if (//c.ObjId.links.Where(s=>s.objid_to== userId ).Any()                    && 
                    c.ObjId.sgTypeId==8
                    //&& c.ObjId.userid==this.user_self.UserId
                    )
                { 

                    return c;
                }
            }*/
            return null;
        }
    }
    public enum xConnectStatus { b_Created, b_Connecting, b_Identity, b_Connected , b_Disconnecting, b_Disconnected };
    public static class MyExtensions
    {

        public static void Add_onlyIfNotExists<Obj>(this List<Obj> list, Obj item)
        {
          
            //if (list.FindIndex(item)<0)
            list.Add( item);
        }
    }
}