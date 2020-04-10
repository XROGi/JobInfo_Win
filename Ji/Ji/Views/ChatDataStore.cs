using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ji.ClassSR;

//using Android.OS;
using Ji.Models;
using Ji.Services;
using SQLite;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Ji.Droid
{


    public enum StatusConected { None, Open, Close };
    public class ChatDataStore //: IChatDataStore
    {
        public ObservableCollection<ConnectionLog> Log_ConnecionSR = new ObservableCollection<ConnectionLog>();

        public delegate void OnLogConnecttionAddDelegate(ConnectionLog log);
        public event OnLogConnecttionAddDelegate OnLogConnecttionAdd;

        public void LogConnecttionAdd(ConnectionLog log)
        {
            OnLogConnecttionAdd?.Invoke(log);
        }

        public ConcurrentQueue<Log_DataSting> log_income = new ConcurrentQueue<Log_DataSting>();
   
        //  public IMyApplicationInfo deviceIn;
        private bool bCacheUser;
        private bool bCacheMessage;
        private bool bCacheChatList;

        bool _b_IsPPSEnabled;
        public bool b_IsPPSEnabled { get { return _b_IsPPSEnabled; } set { _b_IsPPSEnabled = value;
                if (_b_IsPPSEnabled)
                {
                    App.MainMenu.SetPPSMenu(_b_IsPPSEnabled);
                }
            } }

        public delegate void OnStatusConectChangedDelegate(StatusConected statusConected);


        public delegate ParkResult OnParking_Step1Delegate(string sender);

        public event OnParking_Step1Delegate OnParking_Step1;

        public delegate ParkResult OnParking_Step2Delegate(string PassNumber, string Out_Who, string Out_CarType, string Out_CarNumber, string Out_Reason);

        //List<ObjMsg> 
        public delegate ChatStatisticUser OnChat_GetMyStatisticDelegete(int chat_objId);
        public event OnChat_GetMyStatisticDelegete OnChat_GetMyStatistic;

        public delegate Task<bool> OnRegister_FBDelegete(string token);
        public event OnRegister_FBDelegete OnRegister_FB;

        DateTime StatisticChatVersion;

        private ChatStatisticUser[] _StatisticChat;
        public ChatStatisticUser[] StatisticChat
        {
            get { return _StatisticChat; }
            set
            {
                _StatisticChat = value;
                if (_StatisticChat != null)
                {
                    DateTime oldStatisticChatVersion = StatisticChatVersion;
                    StatisticChatVersion = StatisticChat.Max(s => s.LastObjIdDateCreate);
                    if (oldStatisticChatVersion != StatisticChatVersion)
                    {
                        if (OnStatistic_Changed != null)
                        {
                            OnStatistic_Changed(oldStatisticChatVersion);
                        }
                    }

                }
            }
        }

        public delegate void OnNewPageReciveDelegete(ObjMsg[] msgs);
        public event OnNewPageReciveDelegete OnNewPageRecive;

        public void NewPageRecive(ObjMsg[] msgs)
        {
            OnNewPageRecive(msgs);
        }


        private ChatPages[] DB_Chat_GetPages(int chatId)
        {
            var table = db.Table<ChatPages>();

            ChatPages[] pages = table.Where(s => s.ChatId == chatId).OrderBy(s => s.PageNumber).ToArray();
            return pages;
            //return data.Select(s => new ObjMsg { ObjId = s.ObjId, PageNum = s.PageNum, Data = s.Text, userid = s.userid, DateCreate = s.DateCreate, ShownState = -1 }).ToArray();

            //return null;
        }
       
        private bool DB_Page_isExists(int chatId)
        {
            return true;
        }

        internal void Chat_Select(int objId)
        {

        }

        public delegate ChatStatisticUser[] OnChat_GetStatisticDelegete();
        public event OnChat_GetStatisticDelegete OnChat_GetStatistic;

        public delegate void Chat_ListRefreshDelegate(int ChatId);
        public event Chat_ListRefreshDelegate OnChat_ListRefresh;
        public void Chat_ListRefresh(int v)
        {
            try
            {
                this.OnChat_ListRefresh?.Invoke(v);
            }
            catch (Exception err)
            {
                App.Log(err);
            }
        }

        public   delegate Task Chat_LeaveDelegate(int ChatId);
        public event Chat_LeaveDelegate OnChat_Leave;

        public delegate int Chat_CreateAndSubscribeDelegate(int UserId, string Name);

        internal void Log(Exception errIn)
        {
            try
            {
                log_income.Enqueue(new Log_DataSting() { Text = errIn.Message.ToString() });
                
                if (App.setup != null)
                {
                    if (App.setup.b_ShowExceptionText)
                        DependencyService.Get<IXROGiToast>().ShortAlert(errIn.Message.ToString());
                }
                else
                    DependencyService.Get<IXROGiToast>().ShortAlert(errIn.Message.ToString());
            }
            catch (Exception err)
            {

            }

        }

        internal async Task Request_User_List(int maxVers, int nPage, String Filter)
        {
            try
            {
                
                //if (usersList.Count == 0)
                //{
                //    OnGetUserList(0);
                //}
                
                if (usersList.Count != 0)
                {
                    if (Filter == "Favorite")
                    {
                        OnReciveUserList(maxVers, nPage, usersList.Where(s => s.bFavorite == true).ToArray());
                    }
                    else
                    {
                      if( Filter=="All")
                            OnReciveUserList(maxVers, nPage, usersList.ToArray());
                    }
                }
                else
                {
                    if (App.ddd.SR != null)
                    {
                        await SR.Request_User_List(maxVers, nPage, Filter );
                    }
                    else
                    {
                    }
                }
            }
            catch (Exception err)
            { 
            }
        }

        public event Chat_CreateAndSubscribeDelegate OnChat_CreateAndSubscribe;

        public  delegate  Task OnChat_Create_PublicDelegate(int ParentChatId, string Name, string Comment, int[] usersList);
        public event OnChat_Create_PublicDelegate OnChat_Create_Public;

        public delegate void Chat_UnSubscribeUserDelegate(int ChatId, int WhoDeleteUserId, int id);
        public event Chat_UnSubscribeUserDelegate OnChat_UnSubscribeUser;

        public delegate void Chat_SubscribeUserDelegate(int ChatId, int WhoDeleteUserId, int id);
        public event Chat_SubscribeUserDelegate OnChat_SubscribeUser;



        public delegate void Message_ShownDelegate(int ChatId, int ObjId, int UserId);
        public event Message_ShownDelegate OnMessage_Shown;

        //    List<UserChat> users = null;




        internal int Chat_CreateAndSubscribe(UserChat user)
        {
            if (OnChat_CreateAndSubscribe != null)
                return OnChat_CreateAndSubscribe(user.UserId, user.FIO);
            return -2;
        }



        internal bool isConnectSetup()
        {
            if (connectInterface == null
                    || string.IsNullOrEmpty(App.ddd.connectInterface.Server_SOAP)
                    )
                return false;
            else
            {
                return true;
            }
        }

        internal void Chat_UnSubscribeUser(GroupChat newGroup, int id)
        {
            if (OnChat_UnSubscribeUser != null)
            {
                //OnChat_UnSubscribeUser(newGroup.ObjId, SelfUser.UserId, id);

                if (SelfUser != null)
                    OnChat_UnSubscribeUser(newGroup.ObjId, SelfUser.UserId, id);
                else
                    OnChat_UnSubscribeUser(newGroup.ObjId, App.ddd.SR.UserId, id);
            }

        }

        public async Task SendMessageAsync(string v)
        {
            try
            {
                if (SR != null)
                {
                    await sr.SendMessageAsync(v).ConfigureAwait(false);
                }
            }
            catch (Exception err)
            {
            }
        }

        internal void Chat_SubscribeUser(GroupChat newGroup, int id)
        {
            if (OnChat_SubscribeUser != null)
            {
                if (SelfUser != null)
                    OnChat_SubscribeUser(newGroup.ObjId, SelfUser.UserId, id);
                else
                    OnChat_SubscribeUser(newGroup.ObjId, App.ddd.SR.UserId, id);
            }
        }


        //internal bool isPPSEnabled()
        //{

        //}

        internal void Chat_Create_Public(int ParentChatId, string Name, string Comment, int[] usersList)
        {
            if (OnChat_Create_Public != null)
                 OnChat_Create_Public(ParentChatId, Name, Comment, usersList);
            //return -2;
        }




        public delegate ObjMsg[] OnMessage_GetPageDelegate(int ChatId, int nPage, bool bReload);
        public event OnMessage_GetPageDelegate OnMessage_GetPage;

        List<string> SetupParams;

        public delegate string[] OnSetupParam_LoadDelegate();
        public event OnSetupParam_LoadDelegate OnSetupParam_Load;

        internal bool SetupParam_Load()
        {
            if (SetupParams == null)
                SetupParams = new List<string>();
            return true;
        }

        public delegate bool OnMessage_SendDelegate(int ChatId, String MessageText);
        public event OnMessage_SendDelegate OnMessage_Send;

        public delegate bool OnMessageFile_SendDelegate(int ChatId, String MessageText, byte[] File);
        public event OnMessageFile_SendDelegate OnMessageFile_Send;


        public delegate void OnStatistic_ChangedDelegate(DateTime oldLastVersion);

        internal string SOAP_SoapServer()
        {
            return "http://194.190.100.194";
        }

        public event OnStatistic_ChangedDelegate OnStatistic_Changed;




        /// <summary>
        /// Вызывать когда данные нужны но не особо, в том числе первичный вызов. 
        /// </summary>
        /// <param name="objId"></param>
        /// <returns></returns>
        internal ChatStatisticUser Chat_GetMyStatistic(int objId)
        {
            try
            {
                //OnChat_GetMyStatistic view.OnRefreshObjectExplorerClicked += async (s, e) => await RefreshObjectExplorerAsync();
                /*      if ((DateTime.Now - StatisticChatVersion).TotalSeconds > 3 * 60)
                      {
                          StatisticChat = OnChat_GetStatistic();
                      }
                      */

                return OnChat_GetMyStatistic(objId);
            }
            catch (Exception err)
            {
            }
            return null;
        }




        /// <summary>
        /// Вызывать когда надо точно запросить данные (новый чат, новое сообщение уведомлением и.т.п.)
        /// </summary>
        internal void Chat_UpdateStatistic()
        {
            try
            {
                //OnChat_GetMyStatistic view.OnRefreshObjectExplorerClicked += async (s, e) => await RefreshObjectExplorerAsync();
                if (OnChat_GetMyStatistic != null)
                {
                    ///OnChat_GetStatistic.BeginInvoke();
                    StatisticChat = OnChat_GetStatistic();

                }
            }
            catch (Exception err)
            {
            }
        }


        internal GroupChat Chat_GetChat(int chatid)
        {
            return OnChatGetOne(chatid);
        }

        internal ObjMsg[] Message_GetPage(int ChatId, int nPage)
        {
            return OnMessage_GetPage(ChatId, nPage, true);//true????
        }

        public event OnParking_Step2Delegate OnParking_Step2;


        internal ParkResult Parking_Step1(string skypas_id)
        {
            return OnParking_Step1?.Invoke(skypas_id);
        }


        public void Register_FB(string v)
        {
            try
            {
                if(connectInterface!=null)
                    connectInterface.Fb_Token = v;
                if (OnRegister_FB != null)
                    OnRegister_FB?.Invoke(v);
            } catch (Exception err)
            {

            }
        }


        internal ParkResult Parking_Step2(string PassNumber, string Out_Who, string Out_CarType, string Out_CarNumber, string Out_Reason)
        {
            return OnParking_Step2?.Invoke(PassNumber, Out_Who, Out_CarType, Out_CarNumber, Out_Reason);
        }

        public event OnStatusConectChangedDelegate OnStatusConectChanged;

        public delegate List<GroupChat> OnGetChatListDelegate(X_WS sender);
        public event OnGetChatListDelegate OnGetChatList;

        public delegate GroupChat OnChat_GetChatDelegate(int ChatId);
        public event OnChat_GetChatDelegate OnChatGetOne;

        public delegate List<UserChat> OnGetUserListDelegate(long MaxVersion);
        public event OnGetUserListDelegate OnGetUserList;


        public delegate void OnReciveChatUserLeaveDelegate(int ChatId, int userid);
        public event OnReciveChatUserLeaveDelegate OnReciveChatUserLeave;

        public delegate void OnReciveMessageUpdateDelegate(int ObjId, Obj objtemp);
        public event OnReciveMessageUpdateDelegate OnReciveMessageUpdate;
        

        StatusConected _statusConect;

        StatusConected _statusConectWS;

        //private StatusConected statusConectWS
        //{
        //    set
        //    {
        //        if (_statusConectWS != value)
        //        {
        //            _statusConectWS = value;
        //            OnStatusConectChanged?.Invoke(_statusConectWS);
        //        };
        //        if (_statusConectWS != StatusConected.Open)
        //            TokenSeance = "";
        //    }
        //    get { return _statusConectWS; }
        //}

        public StatusConected statusConect
        {
            set
            {
                if (_statusConect != value)
                {
                    _statusConect = value;
                    OnStatusConectChanged?.Invoke(_statusConect);
                };
                if (_statusConect != StatusConected.Open)
                    TokenSeance = "";
            }
            get { return _statusConect; }
        }
        public bool isOpen()
        {
            //if (App.ddd.ws != null)
            //{
            //    if (App.ddd.statusConectWS == StatusConected.Close || App.ddd.statusConectWS == StatusConected.None)
            //        return false;
            //    else return true;
            //}
            if (App.ddd.SR != null)
            {
                return App.ddd.SR.isOpen();
/*                if (App.ddd.statusConect == StatusConected.Close || App.ddd.statusConect == StatusConected.None)
                    return false;
                else return true;
                */
            }
            return false;
        }

        SQLiteConnection db=null;
        string dbPath = "";
        Services.Device device;
        ConnectInterface _connectInterface;
        private string TokenSeance;
        public ChatDataStore()
        {
            Init();
        }
        
        
        public void Close_OnDestroy()
        {
          
        }

        

        private void Init()
        {
            try
            {
                bCacheUser = true;
                bCacheMessage = true;
                bCacheChatList = true;
                //var users = db.Table<Tbl_Users>();
                usersList = new List<UserChat>();

                device = new Services.Device();

            //    statusConectWS = StatusConected.Close;
                TokenSeance = "";
                StatisticChatVersion = DateTime.MinValue;
                b_IsPPSEnabled = false;
                ///    StatisticChat = null;
                //   RequestTokenSeance();
            }
            catch (Exception err)
            {


            }
        }

        public async void Open()
        {
            //if (App.ddd.WS != null)
            //{
            //    await App.ddd.WS.Open();
            //}
            if (App.ddd.SR != null)
            {
                await App.ddd.SR.Open();
            }
        }
        public void Fill()
        {
            int Step = 00;
            try
            {

                //_ =
                
                LoadConnectAsync();
                Step = 1;
                device.Fill(App.deviceIn);
                Step = 2;

            }
            catch (Exception err)
            {

            }
        }

        internal async Task Setup_RegisterNewConnectAsync(string TokenReqId, string Server_SOAP)
        {
            ClearConnect();

            //Xamarin.Forms.Application.Current.Properties["TokenReqId"] = TokenReqId;
            //Xamarin.Forms.Application.Current.Properties["Server_SOAP"] = Server_SOAP;
            //Xamarin.Forms.Application.Current.SavePropertiesAsync();

            ConnectInterface connectInterfac = new ConnectInterface();
            //if (System.Diagnostics.Debugger.IsAttached)
            //{
            //    connectInterfac.TokenReqId = "F9107DCE-7587-4561-8ACF-6C530C38F51C";
            //    connectInterfac.Server_Name = "http://194.190.100.194/xml/";
            //    connectInterfac.Server_SOAP = "http://194.190.100.194/xml/GetUserInfo.asmx";
            //    connectInterfac.Server_WS = "ws://194.190.100.194/xml/ChatHandler.ashx";

            //}
            //else
            {
                connectInterfac.TokenReqId = TokenReqId;
                connectInterfac.Server_Name = Server_SOAP;
                App.ddd.connectInterface = connectInterfac;
                await SecureStorage.SetAsync("TokenReqId", connectInterfac.TokenReqId).ConfigureAwait(false);
                await SecureStorage.SetAsync("ServerName", connectInterfac.Server_Name).ConfigureAwait(false);
                await SecureStorage.SetAsync("ServerSOAP", connectInterfac.Server_SOAP).ConfigureAwait(false);
                await SecureStorage.SetAsync("ServerWS", connectInterfac.Server_WS).ConfigureAwait(false);
            }
            //            WS
            //                                if (_connectInterface != null)
            //                if (_connectInterface.Server_WS != null)
            ws.Url = connectInterfac.Server_WS;
            //App.ddd.WS.Url
        }

        public void ClearConnect()
        {
            try
            {
                //Xamarin.Forms.Application.Current.Properties["TokenReqId"] = "";
                //Xamarin.Forms.Application.Current.Properties["Server_SOAP"] = "";
                //Xamarin.Forms.Application.Current.SavePropertiesAsync();

                SecureStorage.SetAsync("TokenReqId", "");
                SecureStorage.SetAsync("ServerSOAP", "");
                SecureStorage.SetAsync("ServerWS", "");
                SecureStorage.SetAsync("ServerName", "");


                if (App.ddd.connectInterface != null)
                {
                    App.ddd.connectInterface.Server_SOAP = "";
                    App.ddd.connectInterface.Server_WS = "";
                    App.ddd.connectInterface.TokenReqId = "";
                    App.ddd.connectInterface.TokenSeanceId = "";
                };

                if (OpenSQL_DB())
                {
                    if (IsTableExists("Tbl_Connect"))
                    {
                        db.Execute("DELETE FROM Tbl_Connect");
                        db.DropTable<Tbl_Connect>();
                    }
                    if (IsTableExists("UserChat"))
                    {
                        db.Execute("DELETE FROM UserChat");
                        db.DropTable<UserChat>();
                    }

                }
                //  db.Table<Tbl_Connect>().Delete();
            }
            catch (Exception err)
            {

            }
        }

        internal int DB_MessageChatPage_Clear(int chatId, int _pageNum)
        {
            int CountDeleted = -1;
            if (bCacheMessage)
            {

                {
                    var table = db.Table<DbMsg>();
                    if (table != null)
                    {
                        CountDeleted = table.Delete(s => s.ChatId == chatId && s.PageNum == _pageNum);
                    }
                }
                {
                    var table = db.Table<ChatPages>();
                    if (table != null)
                    {
                        table.Delete(s => s.ChatId == chatId && s.PageNumber == _pageNum);
                    }
                }
            }
            return CountDeleted;
        }

        public ConnectInterface connectInterface
        {
            get
            {
                //if (_connectInterface==null)
                //{
                // //   _connectInterface= LoadConnectAsync().Result;
                //}
                return _connectInterface;
            }
            set { _connectInterface = value;
                //       SaveConncet();
                //      Setup_RegisterNewConnectAsync()
            }
        }

        X_SignalR sr;
        public X_SignalR SR
        {
            get => sr; set
            {
                if (value != null)
                    sr = value;
                if (_connectInterface != null)
                    if (_connectInterface.Server_WS != null)
                        sr.Url = _connectInterface.Server_WS;
            }
        }


        public delegate void OnReciveMessagePageListDelegate(ObjMsg[] list);
        public event OnReciveMessagePageListDelegate OnReciveMessagePageList;

        public void ReciveMessagePageList(ObjMsg[] list)
        {
            OnReciveMessagePageList?.Invoke(list);
        }


        public delegate void OnReciveStatisticUpdateDelegete(ChatStatisticUser stat);
        public event OnReciveStatisticUpdateDelegete OnReciveStatisticUpdate;

        public void ReciveStatisticUpdate(ChatStatisticUser stat)
        {
            OnReciveStatisticUpdate?.Invoke(stat);
        }

        private X_WS ws;
        public X_WS WS
        {
            get => ws; set
            {
                if (value != null)
                    ws = value;
                if (_connectInterface != null)
                    if (_connectInterface.Server_WS != null)
                        ws.Url = _connectInterface.Server_WS;
            }
        }

        public List<GroupChat> GetChatList()
        {
            return OnGetChatList(null);
        }
        public List<UserChat> GetUserList()
        {
            long MaxVersionUserChanged = 0;
            if (usersList != null && usersList.Count > 0)
            {
                return usersList;
            }
            if (OnGetUserList != null)
            {

                usersList = OnGetUserList(MaxVersionUserChanged);
                return usersList;
            }
            return null;

        }

        private string _SOAP_AboutVersino;
        public string SOAP_AboutVersion { get => _SOAP_AboutVersino; set => _SOAP_AboutVersino = value; }
        
        private List<UserChat> usersList;
        public List<UserChat> UsersList
        {
            get => usersList;
            set
            {
                usersList = value;
                Sqlite_UsersUpdate(usersList);
            }
        }

        private UserChat SelfUser { get; set; }
        public int UserId { get; internal set; }
        public Ji.ClassSR.ViewGetUserParameters[] SelfParams { get;   set; }

        public void RequestTokenSeance()
        {
            //   if (statusConectWS== StatusConected.Open)
            {
                if (String.IsNullOrEmpty(TokenSeance) == true)
                {
                    /*https://www.c-sharpcorner.com/article/how-to-get-app-version-and-build-number-in-xamarin-forms/*/
                    /*  var context = Android.App.Application.Context;  
                        var VersionNumber = context.PackageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.MetaData).VersionName;  
                        var BuildNumber = context.PackageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.MetaData).VersionCode.ToString();
                    */

                    Cmd cmd = new Cmd() { name = "gettoken", Vers = "2", pid = App.deviceIn.Get_MyPid(), clientname = App.deviceIn.Get_clientname(), clientvers = App.deviceIn.Get_clientvers() };

                    if (cmd.pid == "")
                    {
                        cmd.pid = "111";
                    }
                    Services.User user = new Services.User() { name = connectInterface.TokenReqId };
                    cmd.user = user;
                    cmd.user.device = device;
                    //string devicexml = device.GetXMLString();
                    string cmdstr = cmd.toXml();//.GetXMLString();
                    ws.SendMessageAsync(cmdstr);
                }
            }
        }

        string _ConncentXMLForTokenRequest = "";
        public string  RequestTokenSeanceString()
        {
            //   if (statusConectWS== StatusConected.Open)
            if (!String.IsNullOrEmpty(_ConncentXMLForTokenRequest))
                return _ConncentXMLForTokenRequest;


                if (String.IsNullOrEmpty(TokenSeance) == true)
                {
                    /*https://www.c-sharpcorner.com/article/how-to-get-app-version-and-build-number-in-xamarin-forms/*/
                    /*  var context = Android.App.Application.Context;  
                        var VersionNumber = context.PackageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.MetaData).VersionName;  
                        var BuildNumber = context.PackageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.MetaData).VersionCode.ToString();
                    */

                    Cmd cmd = new Cmd() { name = "gettoken", Vers = "2", pid = App.deviceIn.Get_MyPid(), clientname = App.deviceIn.Get_clientname(), clientvers = App.deviceIn.Get_clientvers() };

                    if (cmd.pid == "")
                    {
                        cmd.pid = "111";
                    }
                Services.User user = new Services.User() { name = connectInterface.TokenReqId };
                    cmd.user = user;
                    cmd.user.device = device;
                    //string devicexml = device.GetXMLString();
                    string cmdstr = cmd.toXml();//.GetXMLString();
                _ConncentXMLForTokenRequest = cmdstr;
                     return _ConncentXMLForTokenRequest;
            }
            return "";
        }
        internal void Request_GetMyStatistic(GroupChat chat)
        {
            // Request_GetMyStatistic(chat.ObjId, )
        }


        internal bool Message_Send(int objId, string textToSend, byte[] foto)
        {
            return OnMessageFile_Send(objId, textToSend, foto);
        }

        [ObsoleteAttribute]
        internal bool Message_Send(int objId, string textToSend)
        {
            return OnMessage_Send(objId, textToSend);
        }

        internal async Task Chat_Leave(GroupChat g)
        {
            if (OnChat_Leave != null)
                  await      OnChat_Leave.Invoke(g.ObjId);
    //        return false;

        }

        private async Task LoadConnectAsync()
        {

            // write secret
            try
            {
                // https://ru.stackoverflow.com/questions/681382/%D0%98%D1%81%D0%BF%D0%BE%D0%BB%D1%8C%D0%B7%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5-configureawaitfalse
                //      вол удаления
                ConnectInterface c = new ConnectInterface();

                string LastStart = await GetPrivateParam("LastStart").ConfigureAwait(false);
                await SecureStorage.SetAsync("LastStart", DateTime.Now.ToLongTimeString()).ConfigureAwait(false);
                string LastStart2 = await GetPrivateParam("LastStart").ConfigureAwait(false);

                c.TokenReqId = await GetPrivateParam("TokenReqId").ConfigureAwait(false);
                c.Server_SOAP = await GetPrivateParam("ServerSOAP").ConfigureAwait(false);
                c.Server_WS = await GetPrivateParam("ServerWS").ConfigureAwait(false);
                c.Server_Name = await GetPrivateParam("ServerName").ConfigureAwait(false);

                if (System.Diagnostics.Debugger.IsAttached && (String.IsNullOrEmpty(LastStart) || c.TokenReqId == null))
                {
                    c.TokenReqId = "F9107DCE-7587-4561-8ACF-6C530C38F51C";
                    c.Server_Name = "http://194.190.100.194/xml/";
                    c.Server_SOAP = "http://194.190.100.194/xml/GetUserInfo.asmx";
                    c.Server_WS = "ws://194.190.100.194/xml/ChatHandler.ashx";
                    await SecureStorage.SetAsync("TokenReqId", c.TokenReqId).ConfigureAwait(false);
                    await SecureStorage.SetAsync("ServerName", c.Server_Name).ConfigureAwait(false);
                    await SecureStorage.SetAsync("ServerSOAP", c.Server_SOAP).ConfigureAwait(false);
                    await SecureStorage.SetAsync("ServerWS", c.Server_WS).ConfigureAwait(false);
                }
                else

                {
                    c.TokenReqId = await GetPrivateParam("TokenReqId").ConfigureAwait(false);
                    c.Server_SOAP = await GetPrivateParam("ServerSOAP").ConfigureAwait(false);
                    c.Server_WS = await GetPrivateParam("ServerWS").ConfigureAwait(false);
                    c.Server_Name = await GetPrivateParam("ServerName").ConfigureAwait(false);
                }
                this._connectInterface = c;
                //     _connectInterface;
            }
            catch (Exception err)
            {
                //     return null;
            }

            /*
            if (OpenSQL_DB())
            {
                var table = db.Table<Tbl_Connect>();
               
                
                if (table != null && IsTableExists("Tbl_Connect")==true)
                {
                    try
                    {
                        Tbl_Connect[] t = table.ToArray();
                        foreach (Tbl_Connect s in t)
                        {
                            _connectInterface = new ConnectInterface();
                            _connectInterface.TokenReqId = s.TokenReqId;
                            _connectInterface.Server_SOAP = s.Server_SOAP;
                            _connectInterface.Server_WS = s.Server_WS;

                            //   break;
                        }
                    }
                    catch (Exception err)
                    {

                    }

                }
                else

                    return null;

                return _connectInterface;
            }*/
            //    return null;
        }

        public delegate void OnReciveMessageDelegate(ObjMsg[] list);
        public event OnReciveMessageDelegate OnReciveMessage;
        public void ReciveMessage(ObjMsg[] list)
        {
            OnReciveMessage?.Invoke(list);
        }

        public void ReciveMessageUpdate(int objId, Obj objtemp)
        {
            OnReciveMessageUpdate?.Invoke(objId, objtemp);
        }

        public void ReciveChatUserLeave(int chatId, int userid)
        {
            OnReciveChatUserLeave?.Invoke(chatId, userid);
        }

        private async Task<string> GetPrivateParam(string v)
        {
            try
            {
                //       await SecureStorage.SetAsync(v+"1", "secret-oauth-token-value");
                string ret = await SecureStorage.GetAsync(v);
                return ret;
            } catch (Exception er)
            {
                return String.Empty;
            }
        }

        public delegate void OnReciveChatAppendDelegate(int chatId, Obj objtemp);
        public event OnReciveChatAppendDelegate OnReciveChatAppend ;
        public void ReciveChatAppend(int chatId, Obj objtemp)
        {
            OnReciveChatAppend?.Invoke(chatId, objtemp);
            
        }

        public delegate void OnReciveUserListDelegate(int maxVers, int nPage, UserChat[] users);
        public event OnReciveUserListDelegate OnReciveUserList;
        public void ReciveUserList(int maxVers, int nPage, UserChat[] users)
        {
            if (UsersList.Count == 0)
            {
                if (users.Length>100)
                    UsersList = users.ToList();
            }
            OnReciveUserList?.Invoke(maxVers, nPage, users);
        }

        public delegate void OnReciveMessageAppendDelegate(ObjMsg msg_append);
        public event OnReciveMessageAppendDelegate OnReciveMessageAppend ;
        public void ReciveMessageAppend(ObjMsg msg_append)
        {
            OnReciveMessageAppend?.Invoke(msg_append);
        }



        public delegate void OnReciveMessageStatusDelegate(int ObjId, int sgStatus);
        public event OnReciveMessageStatusDelegate OnReciveMessageStatus;
        public void ReciveMessageStatus(int ObjId, int sgStatus)
        {
            OnReciveMessageStatus?.Invoke(ObjId, sgStatus);
        }

        public int DB_MessageChatPage_Clear(int chatId)
        {
            int CountDeleted = -1;
            if (bCacheMessage)
            {
                
                {
                    var table = db.Table<DbMsg>();
                    if (table != null)
                    {
                        CountDeleted = table.Delete(s => s.ChatId == chatId);
                    }
                }
                {
                    var table = db.Table<ChatPages>();
                    if (table != null)
                    {
                        table.Delete(s => s.ChatId == chatId);
                    }
                }
            }
            return CountDeleted;
        }

        public delegate void OnReciveChatListDelegete(Obj[] chatlist);
        public event OnReciveChatListDelegete OnReciveChatList;

        public void ReciveChatList(Obj[] chatlist)
        {
            OnReciveChatList?.Invoke(chatlist);
        }


        public delegate void OnReciveMessageAllStatusDelegete(int chatId, int objId, ObjStatus[] status);
        public event OnReciveMessageAllStatusDelegete OnReciveMessageAllStatus; 
        public void ReciveMessageAllStatus(int chatId, int objId, ObjStatus[] status)
        {
            OnReciveMessageAllStatus?.Invoke(chatId,  objId, status);
        }

        public ObjMsg[] DB_MessageGetPage(int chatId, int nPage)
        {
            if (bCacheMessage)
            {
                var table = db.Table<DbMsg>();

                DbMsg [] data = table.Where(s => s.ChatId == chatId && s.PageNum == nPage).OrderBy(s=>s.DateCreate).ToArray();
                return data.Select(s => new ObjMsg { ObjId = s.ObjId, PageNum = s.PageNum, Data = s.Text, userid = s.userid, DateCreate = s.DateCreate , ShownState=12 }).ToArray();
            }
            return null;
        }
        public ChatPages[] DB_MessageGetPages(int chatId )
        {
            if (bCacheMessage)
            {
                OpenSQL_DB();


                if (IsTableExists("ChatPages") == true)
                {
                    TableQuery<ChatPages> table = db.Table<ChatPages>();

                    //  if (table.Count > 0)
                    {
                        ChatPages[] data = table.Where(s => s.ChatId == chatId).OrderBy(s => s.PageNumber).ToArray();
                        return data;//.Select(s =>s.PageNumber).ToArray();
                    }
                }
                else
                {
                    DB_CreateMessageTables();
                }
            }
            return null;
        }

        private void DB_CreateMessageTables()
        {
            if (IsTableExists("ChatPages") == false)
            {
                db.CreateTable<ChatPages>();
            }
            if (IsTableExists("DbMsg") == false)
            {
                db.CreateTable<DbMsg>();
            }
        }

        public void DB_Page_Add(ChatPages page)
        {
            if (bCacheMessage == true)
            {
                if (OpenSQL_DB())
                {
                    // по идее не надо
                    //if (IsTableExists("ChatPages") == false)
                    //{
                    //    db.CreateTable<ChatPages>();
                    //    return false;
                    //}
                    //var table = db.Table<ChatPages>();
                    db.Insert(page);

                }
            }
        }



        public bool DB_Page_isExists(int ChatId, int nPage)
        {
             
            if (bCacheMessage == true)
            {

                if (OpenSQL_DB())
                {
                    try
                    {
                        
                        if (IsTableExists("DbMsg") == false)
                        {
                            db.CreateTable<DbMsg>();
                        }

                        if (IsTableExists("ChatPages") == false)
                        {
                            db.CreateTable<ChatPages>();

                            if (IsTableExists("ObjMsg") == false)
                            {
                                db.CreateTable<ObjMsg>();
                                return false;
                            }
                            return false;
                        }
                        else
                        {
                            var table = db.Table<ChatPages>();
                            ChatPages data = table.Where(s => s.ChatId == ChatId && s.PageNumber == nPage).FirstOrDefault();
                            if (data == null)
                            {
                                return false;
                            }
                            else
                            {
                                if (data.DateDownload == null) return false;
                                return true;
                            }

                        }
                    }
                    catch (Exception err)
                    {

                    }
                }
            }
           return false;
        }

        private void SaveConncet()
        {
            return;
            try
            {
                if (OpenSQL_DB())
                {
                    var ConnectList = db.Table<Tbl_Connect>();


                    if (IsTableExists("Tbl_Connect") == false)
                    {
                        db.CreateTable<Tbl_Connect>();

                    }
                    //      ConnectList.Delete();
                    //db.CreateTable<Tbl_Connect>();
                    //  db.CreateTableAsync<Tbl_Connect>().Wait();
                    //       if (db.Table<Tbl_Connect>().Count() == 0)
                    {
                        // only insert the data if it doesn't already exist
                        var newStock = new Tbl_Connect()
                        {
                            Server_SOAP = _connectInterface.Server_SOAP,
                            Server_WS = _connectInterface.Server_WS,
                            TokenReqId = _connectInterface.TokenReqId
                        };
                        db.Insert(newStock);
                    }

                }
            }catch (Exception err)
            {

            }
        }

        public void DB_MessageSavePage(int _ChatId , int PageNum, ObjMsg[] msgs)
        {
          if (bCacheMessage==true && msgs!=null && msgs.Length>0)
            {
                try
                {
                    DbMsg[] arr = msgs.Select(s => new DbMsg() { ChatId = _ChatId, userid = s.userid, DateCreate = s.DateCreate, ObjId = s.ObjId, PageNum = s.PageNum, Text = s.Data }).ToArray();
                    if (db == null)
                    {
                        OpenSQL_DB();
                    }
                    if (db != null)
                    {
                        db.Table<DbMsg>().Delete(s => s.PageNum == PageNum && s.ChatId == _ChatId);
                        db.Table<ChatPages>().Delete(s => s.PageNumber == PageNum && s.ChatId == _ChatId);
                        db.InsertAll(arr);
                        db.Insert(new ChatPages() { ChatId = _ChatId, PageNumber = PageNum, DateDownload = DateTime.Now });
                    }
                }
                catch (Exception err)
                {

                }
            }
        }
        public void DB_MessageAddNewInPage(int _ChatId, int PageNum, ObjMsg[] msgs)
        {
            if (bCacheMessage == true && msgs != null && msgs.Length > 0)
            {
                try
                {
                    DbMsg[] arr = msgs.Select(s => new DbMsg() { ChatId = _ChatId, userid = s.userid, DateCreate = s.DateCreate, ObjId = s.ObjId, PageNum = s.PageNum, Text = s.Data }).ToArray();
                    db.Table<DbMsg>().Delete(s => s.PageNum == PageNum && s.ChatId == _ChatId);
                    db.Table<ChatPages>().Delete(s => s.PageNumber == PageNum && s.ChatId == _ChatId);

                    db.InsertAll(arr);
                    db.Insert(new ChatPages() { ChatId = _ChatId, PageNumber = msgs[0].PageNum, DateDownload = DateTime.Now });
                }
                catch (Exception err)
                {

                }
            }
        }

        internal void Message_Shown(int ChatId, int objId)
        {
             
            if (OnMessage_Shown != null)
            {
                 OnMessage_Shown(ChatId,objId, SelfUser.UserId);
            }
            else
            {

            }
            int time = 65464;
        //    Console.WriteLine($"zxzxz {time.ToString()}");
        }

        private void AddUser( int UserId , byte[] data, string UserName)
        {
            if (OpenSQL_DB())
            {
                var ConnectList = db.Table<Tbl_Users>();
                //      ConnectList.Delete();
                //db.CreateTable<Tbl_Connect>();
                //  db.CreateTableAsync<Tbl_Connect>().Wait();
                //       if (db.Table<Tbl_Connect>().Count() == 0)
                {
                    // only insert the data if it doesn't already exist
                    var Tbl_Users = new Tbl_Users()
                    {
                        UserId = UserId,
                        Bytes = data,
                        UrlImage = "",
                        UserName = "Тест"
                    };
                    db.Insert(Tbl_Users);
                }
            }
        }


        private async void Sqlite_UsersUpdate(List<UserChat> usersList)
        {
            if (OpenSQL_DB())
            {
                try
                {
                
                    DateTime start = DateTime.Now;
                    if (IsTableExists("UserChat") == false)
                        db.CreateTable<UserChat>();
                    else
                    {
                        //db.DropTable<UserChat>();
                        //db.CreateTable<UserChat>();
                    }
                    foreach (var user in usersList)
                    {
                        AddUser(user);
                    }
                    db.Close();
                    DateTime stop = DateTime.Now;
                    TimeSpan ts = stop - start;
                    double TotalMS = ts.TotalMilliseconds;
                }
                catch (Exception err)
                {

                }
            }
           
        }

        public bool IsTableExists(string tableName)
        {
            if (OpenSQL_DB())
            {
                try
                {
                    var tableInfo = db.GetTableInfo(tableName);
                    if (tableInfo.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        private void AddUser(UserChat user )
        {
            
            //if (OpenSQL_DB())
            //{
            //    try
            //    {
            //        if (IsTableExists("UserChat") == false)
            //                  db.CreateTable<UserChat>();
                      db.Insert(user);

                //} catch (Exception err)
                //{

                //}
            //}
        }


        
        private bool OpenSQL_DB()
        {
            if (db == null)
            {
                try
                { //              data/user/0/ru.Ji/files/ji.db3"
                    string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ji.db3");
                    db = new SQLiteConnection(dbPath);
                }catch (Exception err)
                {
                    App.Log(err);
                    db = null;
                }
            }
            return db != null;
        }

        public void SQL_LoadUsersList(long MaxVersion)
        {
            try
            {
                if (OpenSQL_DB())
                {
                    if (IsTableExists("UserChat") == true)
                    {
                        var table = db.Table<UserChat>();
                        if (table != null)
                        {

                            try
                            {
                                UserChat[] t = table.ToArray();
                                var Test = t.Where(s => s.UserId == 4).FirstOrDefault();
                                if (Test != null)
                                {
                            //        if (Test.Params!=null && Test.Params.Count()>0) // Это временно, перечитать из базы если нет заполненного поля парамтров
                                    {
                                        usersList.AddRange(t);
                                    }

                                }
                                /*foreach (UserChat s in t)
                                {
                                    users
                                }*/
                            }
                            catch (Exception err)
                            {

                            }
                        }
                    }
                }
            //            Tbl_Users[] Tbl_UsersList = db.Table<Tbl_Users>().ToArray();
            //        //      ConnectList.Delete();
            //        //db.CreateTable<Tbl_Connect>();
            //        //  db.CreateTableAsync<Tbl_Connect>().Wait();
            //        if (Tbl_UsersList != null && Tbl_UsersList.Length > 0)
            //        {
            //            // only insert the data if it doesn't already exist
            //            //var t = Tbl_UsersList[0];
            //            //Tbl_Users tbl_Users = t;// new Tbl_Users();
            //            foreach (Tbl_Users u in Tbl_UsersList)
            //            {

            //            };

            //        }
            //    }
            }catch( Exception err)
            {

            }
        }

        public async void Log_IncomeMsgAsync(string msg)
        {
            try
            {

                log_income.Enqueue(new Log_DataSting() { Text = msg }) ;

            }
            catch 
            {

                
            }
        }

       
    }
}