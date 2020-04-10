/*
  if (tokenId < 0) throw new ChatDisconnectedException();
  if (  TokenId < 0) throw new ChatDisconnectedException();
  throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using JobInfo.WS_JobInfo;
using WebSocket4Net;
using WebSocket = WebSocket4Net.WebSocket;

namespace JobInfo
{
    enum UserDopParameter { parPhone, parSkill};
    internal class XROGi_Net
    {
        private string service_url;
        private string ws_url;
        ClientWebSocket ws_ClientWebSocket;
        WebSocket websocket4;

        WS_XROGi ws_socket;

        //        JobInfo.WS_JobInfo.GetUserInfo ui;
        bool b_Connected = false;
        private string ns_str = "http://localhost/xrogi";
        Uri serverUri;
        CancellationTokenSource cts;

        private static readonly ReaderWriterLockSlim LockerRecive = new ReaderWriterLockSlim();
        CancellationToken ct_recive;
        CancellationToken ct_send;


        public delegate void OnIncomeMessageDelegate(string Message);
        public event OnIncomeMessageDelegate OnIncomeMessage = delegate { };




        public delegate void OnConnecedDelegate(ClientWebSocket ws);
        public event OnConnecedDelegate OnConneced = delegate { };

        public delegate void OnConneced4Delegate(WebSocket4Net.WebSocket ws);
        public event OnConneced4Delegate OnConneced4 = delegate { };

        public delegate void OnDisconnecedDelegate(ClientWebSocket ws);
        public event OnDisconnecedDelegate OnDisconneced = delegate { };

        public delegate void OnDisconnecedDelegateWS4(WebSocket ws);
        public event OnDisconnecedDelegateWS4 OnDisconneced4 = delegate { };

        public delegate void OnErrorConnecedDelegate(Exception err, string Message);
        public event OnErrorConnecedDelegate OnErrorConneced = delegate { };


        public delegate void OnErrorAsyncFunctionRunDelegate(Exception err, string Message);
        public event OnErrorAsyncFunctionRunDelegate OnErrorAsyncFunctionRun = delegate { };



        public delegate void OnSendCompleatDelegate(string Message);
        public event OnSendCompleatDelegate OnSendCompleat = delegate { };

        public delegate void Message_GetListIDDelegate(int ChatId, int[] MessageArray);
        public event Message_GetListIDDelegate onMessage_GetListID = delegate { };


        public delegate void OnMessage_GetListArrayDelegate(asyncReturn_Messages ret);
        public event OnMessage_GetListArrayDelegate onMessage_GetListArray = delegate { };

        public delegate void OnMessage_GetFileAsyncDelegate(asyncReturn_GetFile ret, int ObjId, byte[] data);
        public event OnMessage_GetFileAsyncDelegate OnMessage_GetFileAsync = delegate { };


        public delegate void OnUser_GetListAllAsyncDelegate(object sender, User_GetListAllCompletedEventArgs e);
        public event OnUser_GetListAllAsyncDelegate OnUser_GetListAllAsync = delegate { };



        public XROGi_Net(string url)
        {


            //       OnIncomeMessage = () => { };
            #region MyRegion
            //this.url = "http://jobinfo/xml/";
            //this.url = "http://localhost:53847/";
            //ws_url = "ws://localhost:53847/ChatHandler.ashx"; //ws://jobinfo/xml/ChatHandler.ashx

            #endregion            this.url = url;

            ws_url = url.Replace("http:", "ws:") ;
            if (ws_url.EndsWith("/"))
            {
                ws_url += "ChatHandler.ashx";
            }
            else
            {
                ws_url=  ws_url.Replace("GetUserInfo.asmx", "ChatHandler.ashx");
            }
            service_url = url.Replace("ws:", "http:");
            if (service_url.EndsWith("/"))
            {
                service_url += "GetUserInfo.asmx";
            }
            serverUri = new Uri(ws_url);    //"ws://jobinfo/xml/ChatHandler.ashx"
                                            //ws://localhost:53847/ChatHandler.ashx
            cts = new CancellationTokenSource();
            ct_recive = cts.Token;

            ct_send = cts.Token;

            //  ct_recive = new CancellationToken();
        }

        private GetUserInfo GetService(string v)
        {
            GetUserInfo ui=null;
            //   if (ui == null)
            if (ws_socket != null
          //      && ws_socket.IsConnected == true
                )

            {
                Console.WriteLine("GetService. Create New for Method "+v);
                ui = new JobInfo.WS_JobInfo.GetUserInfo();
                
                ui.Timeout = 100* 1000; 
                ui.Url = service_url;// @"http://jobinfo/xml/xml/GetUserInfo.asmx";
            }
            
   //         else
    //            throw new ChatDisconnectedException();


            return ui;
        }
        private GetUserInfo GetService(string v, List<object> param)
        {
            GetUserInfo ui = GetService(v);
            Console.WriteLine("GetService. Create New for Method " + v+ " params= " + GetParamString(param));
            ui.Timeout = 100 * 1000;
            ui.Url = service_url;// @"http://jobinfo/xml/xml/GetUserInfo.asmx";
            //    Console.WriteLine(v + " GetService. Create New " + GetParamString (param) );
            return ui;
        }

        private string GetParamString(List<object> param)
        {
            if (param != null)
                return string.Join(",", param);
            return "";
        }

        internal async Task WS_CloseConnect(User this_user, Device d)
        {
            try
            {
                ws_socket.CloseConnect();
                if (websocket4 != null)
                {
                    if (websocket4.State == WebSocket4Net.WebSocketState.Closing) // System.Net.WebSockets.WebSocketState.Aborted
                    {

                        //string g = ws.SubProtocol;
                        WebSocketCloseStatus cs = new WebSocketCloseStatus();
                        await ws_ClientWebSocket.CloseAsync(cs, "user manual kill token", cts.Token);
                        b_Connected = false;
                        cts.Cancel();
                        websocket4 = null;
                        OnDisconneced4(websocket4);

                    }/*
                    else

                    if (ws_ClientWebSocket.State == System.Net.WebSockets.WebSocketState.Open )
                    {
                        
                        //string g = ws.SubProtocol;
                        WebSocketCloseStatus cs = new WebSocketCloseStatus();
                     
                        b_Connected = false;
                        cts.Cancel();
                        OnDisconneced(ws_ClientWebSocket);
                //        await ws.CloseAsync(cs, "user manual kill token", cts.Token);

                    }*/

                }

            }
            catch (Exception err)
            {
                ws_ClientWebSocket = null;
                b_Connected = false;
                OnErrorConneced(err, err.Message.ToString());
                //throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }
        }

        internal async Task WS4_CloseConnect(User this_user, Device d)
        {
            try
            {
                if (websocket4 != null)
                {
                    if (websocket4.State == WebSocket4Net.WebSocketState.Closing)
                    {

                        //string g = ws.SubProtocol;
                        WebSocketCloseStatus cs = new WebSocketCloseStatus();
                        websocket4.Close();//.CloseAsync(cs, "user manual kill token", cts.Token);
                        b_Connected = false;
                        cts.Cancel();
                        websocket4 = null;
                        OnDisconneced4(websocket4);

                    }
                    else

                    if (websocket4.State == WebSocket4Net.WebSocketState.Open)
                    {

                        //string g = ws.SubProtocol;
                        WebSocketCloseStatus cs = new WebSocketCloseStatus();

                        b_Connected = false;
                        cts.Cancel();
                        OnDisconneced4(websocket4);
                        //        await ws.CloseAsync(cs, "user manual kill token", cts.Token);

                    }

                }

            }
            catch (Exception err)
            {
                websocket4 = null;
                b_Connected = false;
                OnErrorConneced(err, err.Message.ToString());
                //throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }
        }


        internal async Task TokenDisable(long tokenId, User this_user, Device d)
        {
            try
            {
                //       WS4_BeginConnect();
                //                await WS_BeginConnect();
                {
                    bool b_send = false; string resultsr = "";
                    int nErr = 0;
                    /*if (ws_socket!=null)
                    {
                        ws_socket.Send()
                    }*/
                    //                   if (websocket4 != null && websocket4.State == WebSocket4Net.WebSocketState.Open && b_send == false)
                    if (ws_socket != null && ws_socket.IsConnected == true)

                    {
                        //Console.Write("Input message ('exit' to exit): ");
                        //string msg = Console.ReadLine();
                        //if (msg == "exit")
                        //{
                        //    break;
                        //}

                        //var newString = SendString(d.toXml());//String.Format("Hello, ! Time {0}", DateTime.Now.ToString())
                        //Byte[] bytes = System.Text.Encoding.UTF8.GetBytes(newString);

                        double totalMill = (new TimeSpan(DateTime.Now.Ticks)).TotalMilliseconds;
                        long ddd = Convert.ToInt64(totalMill);
                        //  ArraySegment<byte> bytesToSend = SendString(d.toXml());
                        string u_cs = this_user.toXml();//d.toXml()

                        try
                        {
                            //Добавить комманду к данным
                            XDocument doc1 = XDocument.Parse(u_cs);
                            XNamespace xr = "http://localhost/xrogi";
                            var ns = doc1.Root.Name.Namespace;
                            XDocument result = new XDocument(new XElement(xr + "cmd"
                                , new XAttribute("name", "cleartoken")
                                , new XAttribute("pid", Process.GetCurrentProcess().Id.ToString())
                                , new XAttribute("clientname", Process.GetCurrentProcess().ProcessName.ToString())
                                , new XAttribute("TokenId", tokenId.ToString())
                                
                                ));
                            result.Root.Add(doc1.Root);
                            u_cs = result.ToString();
                        }
                        catch (Exception err)
                        {

                        }   /**/

                        SendXML2Socket(u_cs);

                    }

                }


            }
            catch (Exception err)
            {
                websocket4 = null;
                b_Connected = false;
                OnErrorConneced(err, err.Message.ToString());
                //     MessageBox.Show(err.Message.ToString());
            }


        }

        internal UserStatus[] User_GetUsersStatus(long tokenId, long token_Counter, int[] users)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();
            try
            {

                List<object> param = new List<object>(); param.Add(token_Counter); param.Add(users);
                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name, param);
                UserStatus[] h = ws.User_GetUsersStatus(tokenId, token_Counter, users );
                return h;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }
            return null;
        }

        internal void Message_Shown(long tokenId, long token_Counter, long chatid, long id, int TypeShown)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();
            try
            {

                List<object> param = new List<object>(); param.Add(token_Counter); param.Add(chatid); param.Add(id);
                //if (tokenId < 0) return;

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name, param);
                //WS_JobInfo.Obj[] h =
                ws.Message_ShownAsync(tokenId, token_Counter, chatid, id, TypeShown);
                ws.Message_ShownCompleted += Message_ShownCompleted;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }

        }

        private void Message_ShownCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
                OnErrorAsyncFunctionRun(e.Error, MethodBase.GetCurrentMethod().Name);
            //if (e.Error!=null)
            //throw new ChatWsFunctionException(e.Error, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

        }

        internal WS_JobInfo.Obj[] GetMessages(long tokenId, long token_Counter, long parentChatId, long currentPosition, int CountDelta)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();
            try
            {

                List<object> param = new List<object>(); param.Add(token_Counter); param.Add(parentChatId); param.Add(currentPosition);

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name, param);
                WS_JobInfo.Obj[] h = ws.Message_GetList(tokenId, token_Counter, parentChatId, currentPosition, 0, CountDelta);
                return h;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }
            return null;
        }
        internal WS_JobInfo.ObjStatus[] Message_GetStatus(long tokenId, long token_Counter, int ObjId)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();
            try
            {

                List<object> param = new List<object>(); param.Add(token_Counter); param.Add(ObjId);

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name, param);
                WS_JobInfo.ObjStatus[] h = ws.Message_GetStatus(tokenId, token_Counter, ObjId);
                return h;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }
            return null;
        }

        internal WS_JobInfo.view_ObjStatus_HistoryInfo[] Message_GetStatusHistory(long tokenId, long token_Counter, int ObjId)
        { //основной метод
            if (tokenId < 0) throw new ChatDisconnectedException();
            try
            {

                List<object> param = new List<object>(); param.Add(token_Counter); param.Add(ObjId);

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name, param);
                view_ObjStatus_HistoryInfo[] h = ws.Message_GetStatusHistory(tokenId, token_Counter, ObjId);
                return h;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }
            return null;
        }

        internal WS_JobInfo.StatusInfo[] Spravka_GetStatus(long tokenId, long token_Counter)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();
            try
            {
                List<object> param = new List<object>(); param.Add(token_Counter);
                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name, param);
                WS_JobInfo.StatusInfo[] h = ws.Spravka_GetStatus(tokenId, token_Counter);
                return h;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }
            return null;
        }

        internal ObjStatus[] Message_SetStatus(long tokenId, long token_Counter, int ObjId, int Status)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();
            try
            {

                List<object> param = new List<object>(); param.Add(token_Counter); param.Add(ObjId);

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name, param);
                ObjStatus[] ret = ws.Message_SetStatus(tokenId, token_Counter, ObjId, Status);
                return ret;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }
            return null;
        }

        async internal Task<WS_JobInfo.Obj[]> Message_GetListArray(long tokenId, long token_Counter, int parentChatId, int[] msgids)
        {
            if (tokenId < 0)
                throw new ChatDisconnectedException();
            try
            {
                string s = String.Join(",", msgids);

                List<object> param = new List<object>(); param.Add(token_Counter); param.Add(parentChatId); param.Add(s);

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name, param);
                ws.Message_GetListArrayCompleted += Ws_Message_GetListArrayCompleted;

             //   await Task.Run(() => {
                    ws.Message_GetListArrayAsync(tokenId, token_Counter, parentChatId, msgids);
             //   });
               
                
                //                WS_JobInfo.Obj[] h = ws.Message_GetListArray(tokenId, token_Counter, parentChatId, msgids);

                //                return h;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }
            return null;
        }

        internal string [] Get_Setup_Params()
        {
            //if (tokenId < 0) throw new ChatDisconnectedException();
            try
            {

              List<object> param = new List<object>();// param.Add(token_Counter); param.Add(ObjId);

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name, param);
                return ws.Setup_Params("");
                
            }
            catch (ChatDisconnectedException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }
            
            return null;
        }

        private void Ws_Message_GetListArrayCompleted(object sender, Message_GetListArrayCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (e.Error != null)
                    OnErrorAsyncFunctionRun(e.Error, MethodBase.GetCurrentMethod().Name);

                throw new ChatWsFunctionException(e.Error, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

                return;
            }
            if (e.Result != null)
            {
                asyncReturn_Messages ret = e.Result;
                onMessage_GetListArray(ret);
            }
        }

        internal void Message_GetListIDs(long tokenId, long token_Counter, int parentChatId, int ObjId, int direction, int maxCount, bool b_Now)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();
            try
            {
                if (ObjId == 0)
                {}
                List<object> param = new List<object>(); param.Add(token_Counter); param.Add(parentChatId); param.Add(ObjId);
                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name, param);
                if (b_Now == false)
                {
                    // ws.Message_GetListIDs_V2Completed += Ws_Message_GetListIDs_V2Completed;
                    // ws.Message_GetListIDs_V2Async(tokenId, token_Counter, parentChatId, ObjId);
                    ws.Message_GetListIDs_V3Completed += Ws_Message_GetListIDs_V3Completed;
                    ws.Message_GetListIDs_V3Async(tokenId, token_Counter, parentChatId, ObjId, direction, maxCount);

                }
                else
                {
                    asyncReturn_MessagesIDs ret =
                    ws.Message_GetListIDs_V3(tokenId, token_Counter, parentChatId, ObjId, direction, maxCount);
                    int[] h = ret.ListObjID;

                    //Передать результаты
                    onMessage_GetListID(parentChatId, h);
                }

            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }
            //   return null;
        }

        private void Ws_Message_GetListIDs_V3Completed(object sender, Message_GetListIDs_V3CompletedEventArgs e)
        {
            ///if (temp_ChatId != 0)
            {
                //                int _f = temp_ChatId;                temp_ChatId = 0;
                if (e != null || e.Result != null || e.Result.ErrorCount == 0)
                {
                    //Передать результаты
                    onMessage_GetListID(e.Result.InParam_chatid, e.Result.ListObjID);
                }
                else
                {

                }
                if (e.Error != null)
                    OnErrorAsyncFunctionRun(e.Error, MethodBase.GetCurrentMethod().Name);

                //temp_ChatId = 0;
            }
        }

        internal void Message_GetListIDs_v2(long tokenId, long token_Counter, int parentChatId, int AfterPosition, bool b_Now)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();
            try
            {
                //                if (temp_ChatId != 0)
                //                {
                ////                    List<object> param = new List<object>(); param.Add(token_Counter); param.Add(parentChatId); param.Add(AfterPosition); param.Add("SKIPPED");
                //                    return;
                //                }

                if (AfterPosition == 0)
                {

                }
                List<object> param = new List<object>(); param.Add(token_Counter); param.Add(parentChatId); param.Add(AfterPosition);

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name, param);
                //       
                if (b_Now == false)
                {
                    //             temp_ChatId = parentChatId;
                    //                    ws.Message_GetListIDsCompleted += Ws_Message_GetListIDsCompleted;
                    ws.Message_GetListIDs_V2Completed += Ws_Message_GetListIDs_V2Completed;
                    /*      Console.WriteLine("Called service.");
                          Console.ReadLine();  // Wait, otherwise console app will just exit.
                          */
                    ws.Message_GetListIDs_V2Async(tokenId, token_Counter, parentChatId, AfterPosition);
                }
                else
                {

                    asyncReturn_MessagesIDs ret =
                    ws.Message_GetListIDs_V2(tokenId, token_Counter, parentChatId, AfterPosition);
                    int[] h = ret.ListObjID;
                    onMessage_GetListID(parentChatId, h);
                }

            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }
            //   return null;
        }
        private void Ws_Message_GetListIDs_V2Completed(object sender, Message_GetListIDs_V2CompletedEventArgs e)
        {
            ///if (temp_ChatId != 0)
            {
                //                int _f = temp_ChatId;                temp_ChatId = 0;
                if (e != null || e.Result != null || e.Result.ErrorCount == 0)
                {
                    onMessage_GetListID(e.Result.InParam_chatid, e.Result.ListObjID);
                }
                else
                {

                }

                //temp_ChatId = 0;
            }
            if (e.Error != null)
                OnErrorAsyncFunctionRun(e.Error, MethodBase.GetCurrentMethod().Name);

        }

        //int temp_ChatId;
        private void Ws_Message_GetListIDsCompleted(object sender, Message_GetListIDsCompletedEventArgs e)
        {
            //if (temp_ChatId != 0)
            {
                //    int _f = temp_ChatId;
                //    temp_ChatId = 0;
                if (e != null || e.Result != null || e.Result.Length > 0)
                {
                    onMessage_GetListID(99887766, e.Result);
                }
                else
                {

                }

                //  temp_ChatId = 0;
            }

            if (e.Error != null)
                OnErrorAsyncFunctionRun(e.Error, MethodBase.GetCurrentMethod().Name);

        }

        internal WS_JobInfo.Obj[] Chat_GetList(long tokenId, long token_Counter, int parentChatId/*, int sgTypeId*/)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();
            try
            {

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                WS_JobInfo.Obj[] h = ws.Chat_GetList(tokenId, token_Counter, parentChatId);
                return h;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }

        }

        internal WS_JobInfo.Obj Chat_Get(long tokenId, long token_Counter, int ChatId/*, int sgTypeId*/)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();
            try
            {

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                WS_JobInfo.Obj[] h = ws.Chat_Get(tokenId, token_Counter, ChatId);
                if (h != null)
                    return h.FirstOrDefault();
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }
            return null;
        }
        internal WS_JobInfo.Obj[] Chat_GetList_PublicByType(long tokenId, long token_Counter, int parentChatId, int[] sgTypeId)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();
            try
            {

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                WS_JobInfo.Obj[] h = ws.Chat_GetList_PublicByType(tokenId, token_Counter, parentChatId, sgTypeId);
                return h;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }

        }
        private async Task WS_BeginConnect()
        {
            try
            {
                if (ws_ClientWebSocket == null)
                {
                    ws_ClientWebSocket = new ClientWebSocket();

                }
                if (ws_ClientWebSocket.State != System.Net.WebSockets.WebSocketState.Open)
                {
                    string g = ws_ClientWebSocket.SubProtocol;
                    await ws_ClientWebSocket.ConnectAsync(serverUri, cts.Token);
                    b_Connected = true;
                    OnConneced(ws_ClientWebSocket);
                }
            }
            catch (Exception err)
            {
                ws_ClientWebSocket = null;
                b_Connected = false;
                OnErrorConneced(err, err.Message.ToString());
                //throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }

        }

        private async Task WS4_BeginConnect()
        {
            try
            {
                if (ws_socket == null)
                {
                    ws_socket = new WS_XROGi();
                    ws_socket.OnConneced += onWSConnected;
                    ws_socket.OnIncomeMessage += OnWSIncomeMessage;
                    ws_socket.OnDisConneced += onWSDisConnected;
                    ws_socket.OnExceptionError += Ws_socket_OnExceptionError;
                    Task<bool> bb = ws_socket.ConnectAsync(serverUri.ToString(), true);

                }
                else
                {
                    if (ws_socket.IsConnected == false)
                    {
                        Task<bool> bb = ws_socket.ConnectAsync(serverUri.ToString(), true);
                    }
                }
                /*
                                if (websocket4 == null)
                                {
                                    websocket4 = new WebSocket(serverUri.ToString());
                                    websocket4.Opened += onWSOpened;
                                    websocket4.MessageReceived += MessageRecived;
                                    websocket4.Closed += OnClosedSocket;

                                }
                                if (websocket4.State != WebSocket4Net.WebSocketState.Open)
                                {
                                    //string g = websocket4.Version.ToString();
                                    websocket4.Open();//..ConnectAsync(serverUri, cts.Token);
                                    b_Connected = true;
                           //         OnConneced4(websocket4);
                                }*/
            }
            catch (Exception err)
            {
                websocket4 = null;
                b_Connected = false;
                OnErrorConneced(err, err.Message.ToString());
                //throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }

        }

        private void Ws_socket_OnExceptionError(Exception error)
        {
            OnErrorConneced(error, error.Message.ToString());
        }

        private void onWSDisConnected()
        {
            OnDisconneced(null);
        }

        internal void Message_GetFile_Async(long tokenId, long token_Counter, int objId, string guid, int TypeImage)
        {

            if (tokenId < 0) throw new ChatDisconnectedException();
            try
            {
                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                ws.Message_GetFileCompleted += Ws_Message_GetFileCompleted; ;
                ws.Message_GetFileAsync(tokenId, token_Counter, objId, guid, TypeImage);
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

            }
        }

        private void Ws_Message_GetFileCompleted(object sender, Message_GetFileCompletedEventArgs e)
        {
            if (e != null)
                if (e.Result != null)
                    OnMessage_GetFileAsync(e.Result, e.Result.InParam_ObjId, e.Result.Data);
            if (e.Error != null)
                OnErrorAsyncFunctionRun(e.Error, MethodBase.GetCurrentMethod().Name);

        }

        internal byte[] Message_Image_Get(long tokenId, long token_Counter, int objId, string guid, int TypeImage)
        {

            if (tokenId < 0) throw new ChatDisconnectedException();
            try
            {

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                return ws.Message_Image_Get(tokenId, token_Counter, objId, guid, TypeImage);

            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

            }
        }

        private void OnWSIncomeMessage(string Message)
        {
            try
            {
                OnIncomeMessage(Message);
            }
            catch (Exception err)
            {
                OnDisconneced4(websocket4);
            }
        }

        private void onWSConnected()
        {
            b_Connected = true;
            OnConneced4(null);
            //     string f = "<cmd name=\"gettoken\" xmlns=\"http://localhost/xrogi\">\r\n  <user xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" name=\"iu.smirnov\" xmlns=\"http://localhost/xrogi\">\r\n    <device name=\"PARK-IT-PC444\" devicetype=\"Microsoft Windows NT 6.2.9200.0&#x9;Win32NT\" TokenId=\"-1\" Token_Counter=\"0\">\r\n      <period dtb=\"0001-01-01T00:00:00\" dte=\"0001-01-01T00:00:00\" dtc=\"2019-02-06T07:38:43.7205576+03:00\" dtd=\"0001-01-01T00:00:00\" />\r\n    </device>\r\n  </user>\r\n</cmd>";
            //     ws.Send(f);
        }

        private void OnClosedSocket(object sender, EventArgs e)
        {
            //throw new NotImplementedException();

        }

        private void MessageRecived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                OnIncomeMessage(e.Message);
            } catch (Exception err)
            {
                OnDisconneced4(websocket4);
            }
        }

        private void onWSOpened(object sender, EventArgs e)
        {

        }

        internal async Task ConnectToToken(User this_user, Device d)
        {
            try
            {
                //    await WS_BeginConnect();
                WS4_BeginConnect();
                //         TestSendDevice(d);
                if (b_Connected == false)
                {
                    return;
                }
                //using (ClientWebSocket ws = new ClientWebSocket())
                {

                    //Uri serverUri = new Uri("wss://demos.kaazing.com/echo");

                    /*
                    ws.Options.Proxy = new WebProxy("proxy")
                    {
                        Credentials = new NetworkCredential("iu.smirnov@ghp.lc", "mou773nitnap*")
                    };
                */


                    // MessageBox.Show("ConnectAsync compleat");
                    bool b_send = false;
                    string resultsr = "";
                    int nErr = 0;
                    //              if (websocket4.State == WebSocket4Net.WebSocketState.Open && b_send == false)
                    //      ws.
                    {
                        //Console.Write("Input message ('exit' to exit): ");
                        //string msg = Console.ReadLine();
                        //if (msg == "exit")
                        //{
                        //    break;
                        //}

                        //var newString = SendString(d.toXml());//String.Format("Hello, ! Time {0}", DateTime.Now.ToString())
                        //Byte[] bytes = System.Text.Encoding.UTF8.GetBytes(newString);

                        //          double totalMill = (new TimeSpan(DateTime.Now.Ticks)).TotalMilliseconds;
                        //           long ddd = Convert.ToInt64(totalMill);
                        //  ArraySegment<byte> bytesToSend = SendString(d.toXml());


                        string test = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><cmd xmlns=\"http://localhost/xrogi\" pid=\"5168\" clientname=\"ru.svod_int.Ji\" vers=\"2\" name=\"gettoken\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><user name=\"09A5EEAA-E57B-4422-82CA-65B2AC7DE565\"><device name=\"Blackview BV6000S_RU\" OSVersion=\"Android 7.0\" devicetype=\"Physical\"><Token_Counter>1</Token_Counter></device></user></cmd>\"";

//                        string u_cs = test; // this_user.toXml();//d.toXml()
                        string u_cs = this_user.toXml();//d.toXml()

                        try
                        {
                            string Vers = "--.--.--.--";
                            try
                            {
                                var ttt = Assembly.GetExecutingAssembly().GetName().Version;
                                Vers = ttt.ToString();
                            }
                            catch (Exception err)
                            {

                            }
                            //Добавить комманду к данным
                            XDocument doc1 = XDocument.Parse(u_cs);
                            XNamespace xr = ns_str;//"http://localhost/xrogi";
                            var ns = doc1.Root.Name.Namespace;
                            XDocument result = new XDocument(new XElement(xr + "cmd", new XAttribute("name", "gettoken")
                                , new XAttribute("pid", Process.GetCurrentProcess().Id.ToString())
                                , new XAttribute("clientname", Process.GetCurrentProcess().ProcessName.ToString())
                                , new XAttribute("clientvers", Vers)
                                , new XAttribute("os", System.Environment.OSVersion.Platform.ToString())
                                

                                ));

                   //         var rrrr = Process.GetCurrentProcess();
                       /*     var assemblyFullName = Assembly.GetExecutingAssembly().FullName;
                            var version = assemblyFullName.Split(',')[1].Split('=')[1];*/
                           
                            result.Root.Add(doc1.Root);
                            u_cs = result.ToString();
                        }
                        catch (Exception err)
                        {

                        }   /**/

                        try
                        {

                            XDocument doc1 = XDocument.Parse(u_cs);
                            var ns = doc1.Root.Name.Namespace;

                            XmlNamespaceManager xnm = new XmlNamespaceManager(new NameTable());
                            xnm.AddNamespace("x", ns_str);
                            //XmlReader books = XmlReader.Create(xml);
                            var cmd = doc1.Element(ns + "cmd");
                            string g44 = cmd.Attribute("name").Value;

                            //  var ns = doc1.Root.Name.Namespace;

                            var rr = cmd.Element(ns + "user");
                            var val = rr.Attribute("name").Value;
                            var dev = rr.Element(ns + "device");
                            var dev_name = dev.Attribute("name").Value;
                            try
                            {
                                var dev_token = dev.Attribute("devicetoken");
                                if (dev_token != null && dev_token.Value == "!!??!!")
                                {

                                }
                            } catch (Exception err)
                            {

                            }

                        }
                        catch (Exception err)
                        {

                        }  /**/
                        SendXML2Socket(u_cs);
                        /*             await ws.SendAsync(SendString(u_cs), WebSocketMessageType.Text, true, CancellationToken.None);
                                     OnSendCompleat(u_cs);
                                     */
                        /*
                        byte[] ret_arr = await ReciveAll();
                        resultsr = ReciveString(ret_arr); ;

                        //        ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
                        // WebSocketReceiveResult result = await ws.ReceiveAsync(bytesReceived, CancellationToken.None);
                        //   resultsr = ReciveString(bytesReceived); ;
                        Console.WriteLine(resultsr);
                        if (resultsr.ToUpper().StartsWith("OK"))
                        {
                            int cb = resultsr.IndexOf("[");
                            int ce = resultsr.IndexOf("]");
                            if (cb > 0 && ce > 0 && cb < ce)
                            {
                                string number = resultsr.Substring(cb + 1, ce - cb - 1);
                                d.Token = number;
                                try
                                {
                                    d.Token_id = Convert.ToInt64(number);
                                }catch (Exception err)
                                {

                                }
                                b_send = true;
                                return;
                            }
                            else
                            {//Токен не выдан

                            }
                        }
                        nErr++;
                        if (nErr >= 3)
                            break;
                            */
                    }

                }



            } catch (Exception err)
            {
                MessageBox.Show(err.Message.ToString());
            }

        }



        internal async Task TokenTest_old(User this_user, Device d)
        {
            try
            {
                //    await WS_BeginConnect();
                WS4_BeginConnect();
                //         TestSendDevice(d);
                if (b_Connected == false)
                {
                    return;
                }
                //using (ClientWebSocket ws = new ClientWebSocket())
                {

                    //Uri serverUri = new Uri("wss://demos.kaazing.com/echo");

                    /*
                    ws.Options.Proxy = new WebProxy("proxy")
                    {
                        Credentials = new NetworkCredential("iu.smirnov@ghp.lc", "mou773nitnap*")
                    };
                */


                    // MessageBox.Show("ConnectAsync compleat");
                    bool b_send = false; string resultsr = "";
                    int nErr = 0;
                    if (ws_ClientWebSocket.State == System.Net.WebSockets.WebSocketState.Open && b_send == false)
                    {
                        //Console.Write("Input message ('exit' to exit): ");
                        //string msg = Console.ReadLine();
                        //if (msg == "exit")
                        //{
                        //    break;
                        //}

                        //var newString = SendString(d.toXml());//String.Format("Hello, ! Time {0}", DateTime.Now.ToString())
                        //Byte[] bytes = System.Text.Encoding.UTF8.GetBytes(newString);

                        //          double totalMill = (new TimeSpan(DateTime.Now.Ticks)).TotalMilliseconds;
                        //           long ddd = Convert.ToInt64(totalMill);
                        //  ArraySegment<byte> bytesToSend = SendString(d.toXml());
                        string u_cs = this_user.toXml();//d.toXml()

                        try
                        {
                            //Добавить комманду к данным
                            XDocument doc1 = XDocument.Parse(u_cs);
                            XNamespace xr = ns_str;//"http://localhost/xrogi";
                            var ns = doc1.Root.Name.Namespace;
                            XDocument result = new XDocument(new XElement(xr + "cmd", new XAttribute("name", "gettoken")));
                            result.Root.Add(doc1.Root);
                            u_cs = result.ToString();
                        }
                        catch (Exception err)
                        {

                        }   /**/

                        try
                        {

                            XDocument doc1 = XDocument.Parse(u_cs);
                            var ns = doc1.Root.Name.Namespace;

                            XmlNamespaceManager xnm = new XmlNamespaceManager(new NameTable());
                            xnm.AddNamespace("x", ns_str);
                            //XmlReader books = XmlReader.Create(xml);
                            var cmd = doc1.Element(ns + "cmd");
                            string g44 = cmd.Attribute("name").Value;

                            //  var ns = doc1.Root.Name.Namespace;

                            var rr = cmd.Element(ns + "user");
                            var val = rr.Attribute("name").Value;
                            var dev = rr.Element(ns + "device");
                            var dev_name = dev.Attribute("name").Value;
                            try
                            {
                                var dev_token = dev.Attribute("devicetoken");
                                if (dev_token != null && dev_token.Value == "!!??!!")
                                {

                                }
                            }
                            catch (Exception err)
                            {

                            }

                        }
                        catch (Exception err)
                        {

                        }  /**/
                        SendXML2Socket(u_cs);
                        /*             await ws.SendAsync(SendString(u_cs), WebSocketMessageType.Text, true, CancellationToken.None);
                                     OnSendCompleat(u_cs);
                                     */
                        /*
                        byte[] ret_arr = await ReciveAll();
                        resultsr = ReciveString(ret_arr); ;

                        //        ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
                        // WebSocketReceiveResult result = await ws.ReceiveAsync(bytesReceived, CancellationToken.None);
                        //   resultsr = ReciveString(bytesReceived); ;
                        Console.WriteLine(resultsr);
                        if (resultsr.ToUpper().StartsWith("OK"))
                        {
                            int cb = resultsr.IndexOf("[");
                            int ce = resultsr.IndexOf("]");
                            if (cb > 0 && ce > 0 && cb < ce)
                            {
                                string number = resultsr.Substring(cb + 1, ce - cb - 1);
                                d.Token = number;
                                try
                                {
                                    d.Token_id = Convert.ToInt64(number);
                                }catch (Exception err)
                                {

                                }
                                b_send = true;
                                return;
                            }
                            else
                            {//Токен не выдан

                            }
                        }
                        nErr++;
                        if (nErr >= 3)
                            break;
                            */
                    }

                }



            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message.ToString());
            }

        }
        internal void MessageAdd(long tokenId, long token_Counter, int parentChatId, string text)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();
            try
            {

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                int id = ws.Message_Add(tokenId, token_Counter, parentChatId, text);

            }
            catch (Exception err)
            {
                if (err != null)
                    OnErrorAsyncFunctionRun(err, MethodBase.GetCurrentMethod().Name);

            }

        }



        internal void Get_UserListAsync(long tokenId, long token_Counter)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();

            try
            {


                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);

                ws.User_GetListAllCompleted += Ws_User_GetListAllCompleted;


                ws.User_GetListAllAsync(tokenId, token_Counter);
                
              //  return;// 

            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }
            

        }

        private void Ws_User_GetListAllCompleted(object sender, User_GetListAllCompletedEventArgs e)
        {
            if (e.Error != null)
                OnErrorAsyncFunctionRun(e.Error, MethodBase.GetCurrentMethod().Name);

            OnUser_GetListAllAsync(sender, e);
        }

        internal WS_JobInfo.User[] Get_UserList(long tokenId, long token_Counter)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();

            try
            {


                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                return ws.User_GetListAll(tokenId, token_Counter);

            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

            }

        }


        internal WS_JobInfo.User Get_User(long tokenId, long token_Counter, int UserId)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();

            try
            {


                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);

                return ws.User_Get(tokenId, token_Counter, UserId);

            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }
            return null;
        }

        internal void Add_Image(long tokenId, long token_Counter, long objId, Image image1, string comment)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();

            try
            {

                byte[] b = ImageToByteArray(image1);

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);

                ws.Message_Image_Add(tokenId, token_Counter, objId, b, "jpg", comment);

            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }

        }
        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }
        internal void Message_AddSmile(long tokenId, long token_Counter, long parentChatId, string text)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();

            try
            {

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                ws.Message_AddSmile(tokenId, token_Counter, parentChatId, text);

            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

            }

        }
        internal int Chat_Add(long tokenId, long token_Counter, long parentChatId, int sgTypeId, string nameChat, string ChatComment)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();

            try
            {

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                int h = ws.Chat_Add(tokenId, token_Counter, parentChatId, sgTypeId, nameChat, ChatComment);
                return h;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

            }
            return 0;
        }
        internal int Chat_CreateAndSubscribe(long tokenId, long token_Counter, long parentChatId, int sgTypeId, string nameChat, string ChatComment, int[] users)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();

            try
            {

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                int h = ws.Chat_CreateAndSubscribe(tokenId, token_Counter, parentChatId, sgTypeId, nameChat, ChatComment, users);
                return h;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

            }
            return 0;
        }
        /*
        internal string Chat_CreateAndSubscribe(long tokenId, long token_Counter, long parentChatId, int sgTypeId, string nameChat, string ChatComment, int [] UsersIdSubscribe)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();

            try
            {

                WS_JobInfo.GetUserInfo ws = GetService("Chat_CreateAndSubscribe");
                string h = ws.Chat_CreateAndSubscribe(tokenId, token_Counter, parentChatId, sgTypeId, nameChat, ChatComment, UsersIdSubscribe);
                return h;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

            }
            return null;
        }

        */
        internal WS_JobInfo.ChatTypes[] GetTypeChatList(long TokenId, long Token_Counter)
        {
            if (TokenId < 0) throw new ChatDisconnectedException();

            try
            {

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                WS_JobInfo.ChatTypes[] h = ws.Chat_GetTypes(TokenId, Token_Counter);
                return h;
            } catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

            }

        }

        internal void Chat_Rename(long TokenId, long Token_Counter, long id, string newName)
        {
            if (TokenId < 0) throw new ChatDisconnectedException();
            try
            {

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                // WS_JobInfo.ChatTypes[] h =
                ws.Chat_Rename(TokenId, Token_Counter, id, newName);

            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

            }

        }

        internal void Chat_CreateLink(long TokenId, long Token_Counter, int NewParentChatId, int CurrentChatId)
        {
            if (TokenId < 0) throw new ChatDisconnectedException();
            try
            {

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                // WS_JobInfo.ChatTypes[] h =
                ws.Chat_CreateLink(TokenId, Token_Counter, NewParentChatId, CurrentChatId);

            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

            }

        }
        internal void Chat_CreateMainLink(long TokenId, long Token_Counter, int NewParentChatId, int CurrentChatId)
        {
            if (TokenId < 0) throw new ChatDisconnectedException();
            try
            {

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                // WS_JobInfo.ChatTypes[] h =
                ws.Chat_CreateMainLink(TokenId, Token_Counter, NewParentChatId, CurrentChatId);

            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

            }

        }

        internal WS_JobInfo.User UserGetSelf(long tokenId, long token_Counter)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();
            try
            {

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                // WS_JobInfo.ChatTypes[] h =
                WS_JobInfo.User user = ws.User_GetSelf(tokenId, token_Counter);
                return user;

            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

            }
        }

        internal byte[]  User_GetFoto(long tokenId, long token_Counter, int userId)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();
            try
            {

                List<object> param = new List<object>(); param.Add(token_Counter); param.Add(userId); 
                //if (tokenId < 0) return;

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name, param);
               
                byte[] res =     ws.User_GetFoto(tokenId, token_Counter,  userId,1);
                return res;
                
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }
        }
    internal void User_SetParameter(long tokenId, long token_Counter, int userId, int identityParamToServerChanged, int parPhone, string newValue)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();
            try
            {

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                // WS_JobInfo.ChatTypes[] h =
                ws.User_SetParameter(tokenId, token_Counter
                       , userId
                , identityParamToServerChanged
                , parPhone
                , newValue
                    );
               

            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

            }
        }

        internal void Chat_SetPublic(long tokenId, long token_Counter, int chatId, bool b_SetPubic)
        {

            if (tokenId < 0) throw new ChatDisconnectedException();
            try
            {

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                // WS_JobInfo.ChatTypes[] h =
                ws.Chat_SetPublic(tokenId, token_Counter
                       , chatId
                , b_SetPubic
                    );


            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

            }
        }

        internal fn_GetUserParametersResult[] User_GetParameters(long tokenId, long token_Counter, int userId)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();
            try
            {

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                // WS_JobInfo.ChatTypes[] h =
             fn_GetUserParametersResult [] ret =    ws.User_GetParameters(tokenId, token_Counter
                       , userId
                    );

                return ret;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

            }
            return null;
        }
        internal async void SendXML2Socket(string u_cs)
        {
            /*     ArraySegment<byte> varr = SendString(u_cs);
                 List<ArraySegment<byte>> ff = new List<ArraySegment<byte>>();
                 ff.Add(varr);
                 websocket4.Send(ff);*/
     //       Thread.Sleep(1000);
 //           byte [] arr  = SendString2(u_cs);
            ws_socket.Send(u_cs);

            //        await ws_ClientWebSocket.SendAsync(SendString(u_cs), WebSocketMessageType.Text, true, CancellationToken.None);
            OnSendCompleat(u_cs);
        }

        internal int Chat_GetSelected(long TokenId, long token_Counter)
        {
            if (TokenId < 0) throw new ChatDisconnectedException();

            try
            {

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                // WS_JobInfo.ChatTypes[] h =
              int ret = ws.Chat_GetSelected(TokenId, token_Counter);

                //  tbl_ChatUserInfo cui = ws.Chat_GetMyStatistic(TokenId, token_Counter, chatid);
                return ret;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }
            return 0;
        }

        internal bool Chat_Selected(long TokenId, long token_Counter,long chatid)
        {
            if (TokenId < 0) throw new ChatDisconnectedException();

            try
            {
           
                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                // WS_JobInfo.ChatTypes[] h =
                ws.Chat_Selected (TokenId, token_Counter, chatid);

                //  tbl_ChatUserInfo cui = ws.Chat_GetMyStatistic(TokenId, token_Counter, chatid);
                return true;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }
           
        }

        internal void MessageReplayAdd(XMessageCtrl newmsg)
        {
          
        }

        internal void PingWinSocket()
        {
            ws_socket.Send("ping");
        }

        internal UserChatInfo Chat_GetMyStatistic(long TokenId, long token_Counter,  long chatId, bool b_Now = true)
        {
            if (TokenId < 0) throw new ChatDisconnectedException();

            try
            {
                
                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                UserChatInfo cui = ws.Chat_GetMyStatistic(TokenId, token_Counter, chatId);
                return cui;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

            }
            return null;
        }
 
        internal async void ReciveAllBegin()
        {
            try
            {
                byte[] buf = await ReciveAll();
                //LogStatus(true, buffer, result.Count);
                if (buf != null)
                {
                    try
                    {
                        OnIncomeMessage(ReciveString(buf));
                    }
                    catch (Exception err)
                    {

                        throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
                    }
                }
            } catch (Exception err)

            {
                OnDisconneced(ws_ClientWebSocket);
          //      throw new ChatDisconnectedException();
            }
        }


       

        private async Task<byte[]> ReciveAll()
        {
            try
            {
                if (!(ws_ClientWebSocket.State == System.Net.WebSockets.WebSocketState.Open)) return null;
                if (LockerRecive.RecursiveWriteCount > 0)
                    return null;
                int bufferSize = 1000;
                var buffer = new byte[bufferSize];
                var offset = 0;
                var free = buffer.Length;
                int maxFrameSize = 5 * 1000 * 1024;
                LockerRecive.EnterWriteLock();
                try
                {
                    while (true)
                    {
                        //CancellationToken.None
                        //ct_recive.Token.



                        var result = await ws_ClientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer, offset, free), ct_recive);
                        offset += result.Count;
                        free -= result.Count;
                        if (result.EndOfMessage) break;
                        if (free == 0)
                        {
                            // No free space
                            // Resize the outgoing buffer
                            var newSize = buffer.Length + bufferSize;
                            // Check if the new size exceeds a limit
                            // It should suit the data it receives
                            // This limit however has a max value of 2 billion bytes (2 GB)
                            if (newSize > maxFrameSize)
                            {
                                throw new Exception("Maximum size exceeded");
                            }
                            var newBuffer = new byte[newSize];
                            Array.Copy(buffer, 0, newBuffer, 0, offset);
                            buffer = newBuffer;
                            free = buffer.Length - offset;
                        }

                    }
                }
                finally { LockerRecive.ExitWriteLock(); };
                var newBuffer3 = new byte[offset];
                Array.Copy(buffer, 0, newBuffer3, 0, offset);
                return newBuffer3;
            }
            catch (Exception err) when (err.InnerException is System.Net.Sockets.SocketException)
            {
                OnDisconneced(ws_ClientWebSocket); //"from server
                ws_ClientWebSocket.Dispose();
                ws_ClientWebSocket = null; 
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }
            return null;
        }

        internal bool Chat_SubscribeUser(long tokenId, long token_Counter, long chatid, WS_JobInfo.User user, int typeSubscribe)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();


            try
            {
              
                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                 ws.Chat_SubscribeUser(tokenId, token_Counter, chatid, user.UserId,  typeSubscribe);
                return true;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }
        }
        internal bool Chat_UnSubscribeUser(long tokenId, long token_Counter, long chatid, WS_JobInfo.User user, int typeSubscribe)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();


            try
            {

                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                ws.Chat_UnSubscribeUser(tokenId, token_Counter, chatid, user.UserId, typeSubscribe);
                return true;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }
        }
        internal WS_JobInfo.User[] Chat_GetUserList(long tokenId, long token_Counter, int objId)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();

            try
            {
               
                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                WS_JobInfo.User[] cui = ws.Chat_GetUsers(tokenId, token_Counter, objId);
                return cui;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }
 
        }
        internal WS_JobInfo.User[] User_FindFull(long tokenId, long token_Counter, string text)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();

            try
            {
           
                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                WS_JobInfo.User[] cui = ws.User_FindFull(tokenId, token_Counter, text);
                return cui;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }
        }
        internal string  Chat_AddCorporative(long tokenId, long token_Counter, WS_JobInfo.Obj obj_ou, string text)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();

            try
            {
                
                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                string cui = ws.Chat_AddCorporative(tokenId, token_Counter, obj_ou.Guid);
                return cui;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);
            }
            
        }
        internal WS_JobInfo.User[] OU_GetUserList(long tokenId, long token_Counter, string guid)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();

            try
            {
                 
                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                WS_JobInfo.User[] cui = ws.OU_GetUsers(tokenId, token_Counter, guid);
                 
                return cui;
                
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

            }
            
        }
        internal WS_JobInfo.Obj[] OU_GetList(long TokenId, long token_Counter, string root)
        {
            if (TokenId < 0) throw new ChatDisconnectedException();

            try
            {
      
                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name);
                WS_JobInfo.Obj[] cui = ws.Chat_GetListCorporate(TokenId, token_Counter, root);
                return cui;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

            }
           
        }

        #region HashTag 

        internal void Hash_Update(long tokenId, long token_Counter, view_tbl_HashTag h)
        {
            if (tokenId < 0) throw new ChatDisconnectedException();

            try
            {
                
                    WS_JobInfo.GetUserInfo ws = GetService("Hash_Update");
                    ws.Hash_Update(tokenId, token_Counter, h);
                 
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

            }

        }



        internal view_tbl_HashTag [] Hash_GetList(long TokenId, long token_Counter, string find, int ObjId, int UserId)
        {
            if (TokenId < 0) throw new ChatDisconnectedException();

            try
            {
                if (find != "")
                {
                    WS_JobInfo.GetUserInfo ws = GetService("Hash_Find");
                    view_tbl_HashTag[] cui = ws.Hash_Find(TokenId, token_Counter, find, 1);
                    return cui;
                }
                if (ObjId != 0)
                {
                    WS_JobInfo.GetUserInfo ws = GetService("Hash_FindByObjId");
                    view_tbl_HashTag[] cui = ws.Hash_FindByObjId (TokenId, token_Counter, ObjId, 1);
                    return cui;
                }
                if (UserId != 0)
                {
                    WS_JobInfo.GetUserInfo ws = GetService("Hash_FindByObjId");
                    view_tbl_HashTag[] cui = ws.Hash_FindByObjId(TokenId, token_Counter, UserId, 1);
                    return cui;
                }
                return null;
            }
            catch (Exception err)
            {
                throw new ChatWsFunctionException(err, MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name);

            }

        }
        #endregion


        /// <summary>
        ///Шифровка и т.п. может быть сделана тут
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        //        private ArraySegment<byte> SendString2(string v)
        private byte [] SendString2(string v)
        {
            return Encoding.UTF8.GetBytes(v);
         //return     new ArraySegment<byte>(Encoding.UTF8.GetBytes(v));
        }

        /// <summary>
        ///РасШифровка и т.п. может быть сделана тут
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private string ReciveString(byte[] bytesReceived)
        {if (bytesReceived != null)
                return System.Text.Encoding.Default.GetString(bytesReceived);
            //   return new ArraySegment<byte>(Encoding.UTF8.GetBytes(bytesReceived.Array));
            else
                return String.Empty;
        }


        private void TestSendDevice(Device d)
        {
            try
            {
                //string xml = d.toXml();

                //string uri = url + "/SendDevice.aspx";
                //var req = WebRequest.Create(uri);
                ////req.Proxy = WebProxy.GetDefaultProxy(); // Enable if using proxy
                //req.Method = "POST";        // Post method
                //req.ContentType = "text/xml";     // content type
                //                                  // Wrap the request stream with a text-based writer
                //StreamWriter writer = new StreamWriter(req.GetRequestStream());
                //// Write the XML text into the stream
                //writer.WriteLine(xml);
                //writer.Close();
                //// Send the data to the webserver
                //WebResponse rsp = req.GetResponse();
            }catch (Exception err)
            {

            }
        }

        internal void Job_Leave(long tokenId, long token_Counter, long ChatId)
        {
            try
            {
                WS_JobInfo.GetUserInfo ws = GetService(MethodBase.GetCurrentMethod().Name); 
                //WS_JobInfo.ChatTypes[] h = 
                    ws.Chat_Leave(tokenId, token_Counter, ChatId);
               
            }
            catch (Exception err)
            {

            }
     //       return null;
        }
    }
}