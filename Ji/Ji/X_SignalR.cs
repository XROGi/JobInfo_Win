using Ji.ClassSR;
using Ji.Models;
using Ji.Services;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Ji
{
    public class X_SignalR : INotifyPropertyChanged
    {
        bool b_Debug = true;
        bool b_OnEventAfterTokenRecive = true;
        public String ClassName { get; set; }
        public ObservableCollection<SRMessageLog> MsgsLog { get; set; } 

        private HubConnection hubConnection;
        public Command SendMessageCommand { get; set; }

        public Command CommandConnect { get; set; }
        public Command CommandGetToken { get; set; }

        ConnectInfo connect;
        public int UserId
        {
            get
            {

                return connect.UserId;
            }

        }

        public X_SignalR()
        {
            ClassName = "X_SignalR";
            MsgsLog = new ObservableCollection<SRMessageLog>();
            
            CommandConnect = new Command(async () => 
                                        await Open().ConfigureAwait(true), () => IsConnected
                            );
            CommandGetToken = new Command(async () => 
                                        await RequestTokenNow("").ConfigureAwait(true), () => IsConnected
                            );
        }

        public event PropertyChangedEventHandler propertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }



        // идет ли отправка сообщений
        bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    OnPropertyChanged("IsBusy");
                }
            }
        }

      


        // осуществлено ли подключение
        bool isConnected;

        internal async  Task ReciveParkingStep1(string text)
        {
           
            try
            {
                IsBusy = true;
                //     string userQR = "<cmd xmlns=\"http://localhost/xrogi\" pid=\"14622\" clientname=\"ru.svod_int.SvodInf 1.0.34\" vers=\"2\" name=\"gettoken\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><user name=\"76E4DF6C-6FA1-4797-AE94-B3BCD8F4EB72\"><device name=\"Blackview BV6000S_RU\" OSVersion=\"Android 7.0\" IMEI=\"24:5E:4D:92:AD:DD\" devicetype=\"Phone\"><Token_Counter>1</Token_Counter></device></user></cmd>";
                await hubConnection.InvokeAsync("parking_step1", text );
                SendLocalMessage(String.Empty, "ReciveParkingStep1  запрошен...");

            }
            catch (Exception ex)
            {
                SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        internal async Task ReciveParkingStep2(string parkingNumber, string text1, string text2, string text3, string text4)
        {
            try
            {
                IsBusy = true;



                //if (res2 != null)
                //{
                //    StatusStep2.Text = res2.Status;
                //    DateOut.Text = res2.DateOut.ToString();
                //    //  PassNumber.Text = res2.ParkingNumber;
                //}
                //else
                //{
                //    PassNumber.Text = "Произошла ошибка";
                //}
                //Result.IsVisible = true;//ViewStates.Invisible



                //     string userQR = "<cmd xmlns=\"http://localhost/xrogi\" pid=\"14622\" clientname=\"ru.svod_int.SvodInf 1.0.34\" vers=\"2\" name=\"gettoken\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><user name=\"76E4DF6C-6FA1-4797-AE94-B3BCD8F4EB72\"><device name=\"Blackview BV6000S_RU\" OSVersion=\"Android 7.0\" IMEI=\"24:5E:4D:92:AD:DD\" devicetype=\"Phone\"><Token_Counter>1</Token_Counter></device></user></cmd>";
                await hubConnection.InvokeAsync("parking_step2",   parkingNumber,  text1,  text2,  text3,  text4);
           //     await hubConnection.InvokeAsync("parkink_step2", text);
                
                SendLocalMessage(String.Empty, "parkink_step2  запрошен...");

            }
            catch (Exception ex)
            {
                SendLocalMessage(String.Empty, $" parkink_step2 Ошибка отправки: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        internal async Task Request_Job_Create(int parentChatId, int typeChatId, string name, string Description, int[] users, bool isJobNative, bool isJobSvod, int[] objId)
        {
            int nRepeat = 3;

            while (nRepeat > 0)
            {
                try
                {
                    IsBusy = true;
                    //result on chat_append
                    await hubConnection.InvokeAsync("job_create", parentChatId, typeChatId, name, Description, users
                        , isJobNative, isJobSvod, objId

                        ).ConfigureAwait(false);
                    SendLocalMessage(String.Empty, "Request_Message_List  запрошен...");
                    nRepeat = 0;
                }
                catch (System.TimeoutException ex)
                {
                    //{System.TimeoutException: Server timeout (30000,00ms) elapsed without receiving a message from the server.   at Microsoft.AspNetCore.SignalR.Client.HubConnection.InvokeCoreAsyncCore(
                    SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
                    nRepeat--;
                }
                catch (Exception ex)
                {
                    //"Failed to invoke 'chat_create' due to an error on the server. HubException: Method does not exist."
                    SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        /// <summary>
        /// wait    return Clients.Caller.SendAsync("user_param_upd", param, value);
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="Param"></param>
        /// <param name="valuecmd"></param>
        internal async void Request_User_Param_Set(int userId, string Param, string valuecmd)
        {
            int nRepeat = 3;

            while (nRepeat > 0)
            {
                try
                {
                    IsBusy = true;
                    //result on chat_append
                    await hubConnection.InvokeAsync("user_param_set", userId,  Param,  valuecmd  ).ConfigureAwait(false);
                    SendLocalMessage(String.Empty, "user_param_set  запрошен...");
                    nRepeat = 0;
                }
                catch (System.TimeoutException ex)
                {
                    //{System.TimeoutException: Server timeout (30000,00ms) elapsed without receiving a message from the server.   at Microsoft.AspNetCore.SignalR.Client.HubConnection.InvokeCoreAsyncCore(
                    SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
                    nRepeat--;
                }
                catch (Exception ex)
                {
                    //"Failed to invoke 'chat_create' due to an error on the server. HubException: Method does not exist."
                    SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        public bool IsConnected
        {
            get => isConnected;
            set
            {
                if (isConnected != value)
                {
                    isConnected = value;
                    OnPropertyChanged("IsConnected");
                }
            }
        }



  //      private ClientWebSocket  client;
  //      CancellationToken cts_Connect;

        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
       

         CancellationToken cts_Read;
         CancellationToken cts_Write;
         Task TaskRecive;

        public delegate void OnLogConnecttionAddDelegate(ConnectionLog log);
        public event OnLogConnecttionAddDelegate OnLogConnecttionAdd;


        public delegate void OnWSConnectedDelegate(X_SignalR sender);
        public event OnWSConnectedDelegate OnWSConnected /* = delegate { }*/;

        public delegate void OnWSReConnectedDelegate(X_SignalR sender);
        public event OnWSReConnectedDelegate OnWSReConnected ;

        public delegate void OnWSDisConnectedDelegate(X_SignalR sender, Exception err);
        public event OnWSDisConnectedDelegate OnWSDisConnected /* = delegate { }*/;

        public delegate void OnWSReciveDelegate(X_SignalR sender, WS_EventType type, string Msg);
        public event OnWSReciveDelegate OnWSRecive;

        public delegate void OnWSReciveNewMessageDelegate(X_SignalR sender, string Msg, int ChatId, int NewMsgId);
        public event OnWSReciveNewMessageDelegate OnWSReciveNewMessage;
        /*---------------------------------*/
        public delegate void OnWSReciveNewChatStatisticDelegate(int ChatId, string json);
        public event OnWSReciveNewChatStatisticDelegate OnWSReciveStatisicUpdate;

        public delegate void OnReciveChatListDelegate(int ChatId, string json);
        public event OnReciveChatListDelegate OnReciveChatList;

        public delegate void OnReciveChatAppendDelegate(int ChatId, string json);
        public event OnReciveChatAppendDelegate OnReciveChatAppend;

        public delegate void OnReciveMessageUpdateDelegate(int ObjId, string json);
        public event OnReciveMessageUpdateDelegate OnReciveMessageUpdate;

        public delegate void OnReciveUserParamUpdateDelegate(int userid, int useridFavorite,string  param, string value);
        public event OnReciveUserParamUpdateDelegate OnReciveUserParamUpdate ;
 

        public delegate void OnReciveChatUserLeaveDelegate(int ChatId, int userid);
        public event OnReciveChatUserLeaveDelegate OnReciveChatUserLeave;


        public delegate void OnUnknownErrorDelegate(string Error);
        public event OnUnknownErrorDelegate OnUnknownError;



        public delegate void OnReciveUserListDelegate(int maxVers, int nPage,  string json);
        public event OnReciveUserListDelegate OnReciveUserList;

        public delegate void OnReciveMessagePageListDelegate(int ChatId, string json);
        public event OnReciveMessagePageListDelegate OnReciveMessagePageList;

        public delegate void OnReciveMessageDelegate(int ChatId, string json);
        public event OnReciveMessageDelegate OnReciveMessage;

        public delegate void OnReciveMessageAppendDelegate(int ChatId, string json);
        public event OnReciveMessageAppendDelegate OnReciveMessageAppend;

        public delegate void OnReciveMessageStatusDelegate(int ChatId, string json);
        public event OnReciveMessageStatusDelegate OnReciveMessageStatus;

        public delegate void OnReciveMessageAllStatusDelegate(int ChatId, int ObjId, string json);
        public event OnReciveMessageAllStatusDelegate OnReciveMessageAllStatus;

        /*---------------------------------*/

        public delegate void OnWSReciveChatEventDelegate(X_SignalR sender, string Msg);
        public event OnWSReciveChatEventDelegate OnWSReciveChatEvent;
        

        public delegate void OnWSRecivePongDelegate(X_SignalR sender);
        public event OnWSRecivePongDelegate OnWSRecivePong;

        public delegate void OnWSReciveTokenDelegate(X_SignalR sender,string tokenSeance);

 

        public event OnWSReciveTokenDelegate OnWSReciveToken;

        public delegate void OnResultReciveParkingStep1Delegate(string Number, string jsonResult);
        public event OnResultReciveParkingStep1Delegate OnResultReciveParkingStep1;

        public delegate void OnResultReciveUserParamsDelegate (int UserId, string jsonResult);
        public event OnResultReciveUserParamsDelegate OnResultReciveUserParams;

        

        public delegate void OnResultReciveParkingStep2Delegate(string Number, string jsonResult);
        public event OnResultReciveParkingStep2Delegate OnResultReciveParkingStep2;


        public delegate void OnWSReciveTokenClosedDelegate(X_SignalR sender);
        public event OnWSReciveTokenClosedDelegate OnWSReciveTokenClosed;
        public event PropertyChangedEventHandler PropertyChanged;
        // Отключение от чата
        public async Task Disconnect()
        {
            if (!IsConnected)
                return;

            await hubConnection.StopAsync();
            IsConnected = false;
            SendLocalMessage(String.Empty, "Вы покинули чат...");
        }



        ConnectionLog logconnection = null;

        DateTime DateIn=DateTime.MinValue;
        bool  bLockOnOpen = false;
        string url;
        public string Url { get => url; set => url=value; }


        public async Task<bool> WaitOpen()
        {
            if (hubConnection != null)
            {
                if (IsConnected)
                    return IsConnected;
                else
                {
                    int waitSec = 5;
                    for (int i = 0; i <= waitSec * 10; i++)
                    {
                        if (IsConnected)
                            return IsConnected;
                        await Task.Delay(100);
                    }
                    throw (new JiException("Нет соединения с сервером"));
                }
            }
            return false;
        }
            public async Task<bool> Open()
        {
            if ((DateTime.Now - DateIn).TotalSeconds > 10.0)
            {//Timeout;
            }
            if (bLockOnOpen == true && !((DateTime.Now - DateIn).TotalSeconds > 40.0))
            {
                
                return false;
            }
            try 
            {
                DateIn = DateTime.Now;
                 bLockOnOpen = true;
                if (isOpen() == false && Url != null && Url != "")
                {
                    try
                    {
                        if (hubConnection != null)
                        {
                            try
                            {
                                return await ReConnect();
                            }
                            catch (Exception err)
                            {
                            }
                            return true;
                            // return;
                        }


                        IsConnected = false;    // по умолчанию не подключены
                        IsBusy = false;         // отправка сообщения не идет
                                                //     http://194.190.100.194/XSignalR/Service.asmx
                        hubConnection = new HubConnectionBuilder()
                        //.WithUrl(new Uri("http://192.168.1.100:49249/conhub"))
                       .WithUrl(new Uri("http://194.190.100.194/XSignalR/conhub"))
                       //.WithAutomaticReconnect()
                       .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromSeconds(10) })
                       .Build();
                        //https://stackoverflow.com/questions/50336870/how-to-increase-timeout-setting-in-asp-net-core-signalr-v2-1

                        //hubConnection.ConfigureAwait(
                        hubConnection.HandshakeTimeout = new TimeSpan(0, 0, 30);
                        hubConnection.ServerTimeout = new TimeSpan(0, 30, 0);
                        hubConnection.KeepAliveInterval = TimeSpan.FromMinutes(60);




                        hubConnection.Reconnected += async (error) =>
                        {
                            try
                            {
                                logconnection.DateReConnected = DateTime.Now;
                                //                            logconnection.nReConnected = nReconnect;
                                logconnection.Event = "Пересоединено";
                                //        OnWSReConnected?.Invoke(this);
                            }
                            catch (Exception err)
                            {

                            }

                        };
                        hubConnection.Reconnecting += async (error) =>
                        {//https://docs.microsoft.com/en-us/aspnet/core/signalr/dotnet-client?view=aspnetcore-3.1&tabs=visual-studio
                            try
                            {
                                logconnection.DateDisConnected = DateTime.Now;
                                logconnection.Event = "На пересоединение";
                                logconnection = new ConnectionLog();
                                App.ddd.LogConnecttionAdd(logconnection);
                                logconnection.DateBeginReConnect = DateTime.Now;
                                nReconnect++;
                                logconnection.nReConnected = nReconnect;
                            }
                            catch (Exception err)
                            {

                            }
                        };
                        hubConnection.Closed += async (error) =>
                        {//imeoutException: Server timeout (30000,00ms) elapsed without receiving a message from the server.     at Microsoft.AspNetCore.SignalR.Client.HubConnection.InvokeCoreAsyncCore(
                            try
                            {
                                //       SendLocalMessage(String.Empty, "Подключение закрыто...");
                                IsConnected = false;
                                logconnection.DateDisConnected = DateTime.Now;
                                if (error != null)
                                {
                                    logconnection.ErrorException = error;
                                    logconnection.Event = "Закрыто:" + error.InnerException.Message.ToString();
                                }
                                else
                                {
                                    logconnection.Event = "Соединение закрыто";
                                }



                                OnWSDisConnected?.Invoke(this, error);
                                //             await Task.Delay(1500);
                                //             await ReConnect();
                            }
                            catch (Exception err)
                            {

                            }
                        };
                        hubConnection.On<string, int>("Token", (Token, _UserId) =>
                        {
                            try
                            {
                                string ggg = Token.ToString();
                                connect = new ConnectInfo() { nMessage = 22, TokenSeance = Token, UserId = _UserId };
                                SendLocalMessage(String.Empty, "ConnectedToken !..." + Token);
                                App.ddd.UserId = _UserId;
                                if (b_OnEventAfterTokenRecive)
                                {
                                    try
                                    {
                                        OnWSReciveToken(this, ggg);
                                    }
                                    catch (Exception err)
                                    {
                                        SendLocalMessage(String.Empty, $"Ошибка On Token: {err.Message}");
                                    }
                                }
                            }
                            catch (Exception err)
                            {
                                SendLocalMessage(String.Empty, $"Ошибка On Token: {err.Message}");
                            }

                        });

                        hubConnection.On<string>("TokenError", (data) =>
                        {//user_param token error
                            try
                            {
                                if (data != "user_param token error")
                                {
                                    App.Log(new Exception("UnknownError:" + data));
                                    SendLocalMessage(String.Empty, "UnknownError:" + data);
                                    OnUnknownError?.Invoke(data);
                                }
                                else

                                {
                                }
                            }
                            catch (Exception err)
                            {

                            }
                        });
                        hubConnection.On<string>("UnknownError", (data) =>
                        {//user_param token error
                            try
                            {
                                if (data == "token_error")
                                {
                                    string cmd = App.ddd.RequestTokenSeanceString();
                                    SendLocalMessage(String.Empty, "UnknownError:" + data);
                                    RequestTokenNow(cmd);
                                }
                                if (data != "user_param token error")
                                {
                                    App.Log(new Exception("UnknownError:" + data));
                                    SendLocalMessage(String.Empty, "UnknownError:" + data);
                                    OnUnknownError?.Invoke(data);
                                }
                                else

                                {
                                }
                            }
                            catch (Exception err)
                            {

                            }
                        });
                        /*RRRRRRRRRRRRRRRR названо по ошибке parkinК_step1*/
                        hubConnection.On<string, string>("parkink_step1", (number, json) =>
                            {
                                try
                                {
                                    SendLocalMessage(String.Empty, "chat_statistic:" + json);
                                    OnResultReciveParkingStep1(number, json);
                                }
                                catch (Exception err)
                                {

                                }
                            });
                        hubConnection.On<string, string>("parkink_step2", (number, json) =>
                        {
                            try
                            {
                                SendLocalMessage(String.Empty, "parkink_step2:" + json);
                                OnResultReciveParkingStep2(number, json);
                            }
                            catch (Exception err)
                            {

                            }
                        });
                        hubConnection.On<int, string>("chat_statistic", (chatid, json) =>
                         {
                             try
                             {
                                 SendLocalMessage(String.Empty, "chat_statistic:" + json);
                                 OnWSReciveStatisicUpdate(chatid, json);
                                //SendLocalMessage(String.Empty, "chat_statistic/" + chatid.ToString());
                            }
                             catch (Exception err)
                             {

                             }
                         });
                        hubConnection.On<int>("chat_statistic_updated", (chatid) =>
                        {
                            try
                            {
                                SendLocalMessage(String.Empty, "chat_statistic_updated:" + chatid.ToString());
                                _ = Request_Chat_Statisic(chatid);
                                //  SendLocalMessage(String.Empty, "chat_statistic_updated/" + chatid.ToString());
                            }
                            catch (Exception err)
                            {

                            }
                        });
                        hubConnection.On<int, string>("chat_list", (chatid, json) =>
                        {
                            try
                            {
                                SendLocalMessage(String.Empty, "chat_list:" + json);
                                OnReciveChatList(chatid, json);

                            }
                            catch (Exception err)
                            {

                            }
                        });
                        hubConnection.On<int, int>("chat_user_leave", (chatid, userid) =>
                        {
                            try
                            {
                                SendLocalMessage(String.Empty, "chat_user_leave:");
                                OnReciveChatUserLeave?.Invoke(chatid, userid);
                            }
                            catch (Exception err)
                            {

                            }
                        });
                        hubConnection.On<int, int, string>("user_list", (maxVers, nPage, json) =>
                       {
                           try
                           {
                               SendLocalMessage(String.Empty, "user_list:" + json);
                               OnReciveUserList(maxVers, nPage, json);
                           }
                           catch (Exception err)
                           {

                           }
                       });
                        hubConnection.On<int, string, string>("user_param", (UserId, ParamName, ParamValue) =>
                    {
                        try
                        {
                            SendLocalMessage(String.Empty, "!!NotEvent user_params:" + ParamName + " " + ParamValue);

                        }
                        catch (Exception err)
                        {

                        }
                    });
                        hubConnection.On<int, string>("user_params", (userid, json) =>
                        {
                            try
                            {
                                SendLocalMessage(String.Empty, "user_params:" + json);
                                OnResultReciveUserParams(userid, json);
                            }
                            catch (Exception err)
                            {

                            }
                        });
                        hubConnection.On<int, string>("chat_append", (chatid, json) =>
                        {
                            try
                            {

                                //{"ObjId":588581,"MessageNum":0,"PageNum":0,"ShownState":14,"sgTypeId":7,"sgGruupId":0,"MsgId":588902,"sgClassId":6,"Type":"\u041E\u0431\u0449\u0438\u0439 \u0447\u0430\u0442","Guid":"","Parent_Guid":null,"Deep":0,"period":{"dtb":"2020-01-28T11:11:14.77","dte":null,"dtc":"2020-01-28T11:11:14.77","dtd":"2020-01-28T11:11:14.77"},"xml":"\u043E\u043E","links":null,"userid":4,"userCreater":null,"UsersInChat":[4],"Vers":0}
                                SendLocalMessage(String.Empty, "chat_append:" + json);
                                if (json != "{}")
                                    OnReciveChatAppend(chatid, json);
                            }
                            catch (Exception err)
                            {

                            }
                        });
                        hubConnection.On<int, string>("message_status", (chatid, json) =>
                             {
                                 try
                                 {
                                     SendLocalMessage(String.Empty, "message_status:" + json);
                                     if (json != "[]")
                                         OnReciveMessageStatus?.Invoke(chatid, json);
                                 }
                                 catch (Exception err)
                                 {

                                 }
                             });
                        hubConnection.On<int, int, string>("message_statusall", (chatid, objid, json) =>
                        {
                            try
                            {
                                SendLocalMessage(String.Empty, "message_statusall:" + json);
                                if (json != "[]")
                                    OnReciveMessageAllStatus?.Invoke(chatid, objid, json);

                            }
                            catch (Exception err)
                            {

                            }
                        });
                        hubConnection.On<int, string>("message_update", (objid, json) =>
                        {
                            try
                            {
                                SendLocalMessage(String.Empty, "message_update:" + json);
                                if (json != "[]")
                                    OnReciveMessageUpdate.Invoke(objid, json);

                            }
                            catch (Exception err)
                            {

                            }
                        });
                        hubConnection.On<int, int, string, string>("user_param_upd", (userid, useridFavorite, param, value) =>
                        {
                            try
                            {
                                SendLocalMessage(String.Empty, "user_param_upd:" + userid.ToString());
                                //  if (json != "[]")
                                OnReciveUserParamUpdate.Invoke(userid, useridFavorite, param, value);

                            }
                            catch (Exception err)
                            {
                                SendLocalMessage(String.Empty, $"Ошибка On user_param_upd: {err.Message}");
                            }
                        });
                        hubConnection.On<int, string>("message_pagelist", (chatid, json) =>
                        {
                            try
                            {
                                SendLocalMessage(String.Empty, "message_pagelist:" + json);
                                if (json != "[]")
                                    OnReciveMessagePageList(chatid, json);
                            }
                            catch (Exception err)
                            {

                            }
                        });
                        hubConnection.On<int, string>("message_obj", (objid, json) =>
                        {
                            try
                            {
                                SendLocalMessage(String.Empty, "message_obj:" + json);
                                if (json != "[]")
                                    OnReciveMessage?.Invoke(objid, json);
                            }
                            catch (Exception err)
                            {

                            }
                        });
                        hubConnection.On<int, string>("message_append", (chatid, json) =>
                        {
                            try
                            {
                                SendLocalMessage(String.Empty, "message_append:" + json);
                                if (json != "{}")
                                    OnReciveMessageAppend(chatid, json);
                            }
                            catch (Exception err)
                            {

                            }
                        });
                        hubConnection.On<int, string>("message_status", (chatid, json) =>
                        {
                            try
                            {
                                SendLocalMessage(String.Empty, "message_status:" + json);
                                if (json != "{}")
                                    OnReciveMessageStatus(chatid, json);
                            }
                            catch (Exception err)
                            {

                            }
                        });
                        hubConnection.On<string>("pong", (data) =>
                        {
                            try
                            {
                                SendLocalMessage(String.Empty, "pong:" + data);
                            }
                            catch (Exception err)
                            {
                            }
                            ;
                        });
                        //SendMessageCommand = new Command(async () => await RequestTokenNow(), () => IsConnected);
                        bool b = await ReConnect();
                        return b;
                    }
                    catch (Exception err)
                    {
                        if (logconnection != null)
                        {
                            logconnection.DateErrorConnected = DateTime.Now;
                            logconnection.ErrorException = err;
                        }
                        SendLocalMessage(String.Empty, "MainOn Exception:" + err.Message.ToString());
                        OnWSDisConnected?.Invoke(this, err);
                        return false;
                    }
                }
                else

                {
                    if (App.ddd.UserId == 0)
                    {
                        OnWSConnected?.Invoke(this);
                    }
                }
                return true;
            }
            finally
            {
                bLockOnOpen = false ;
            }
            
        }


        private void SendLocalMessage(string MessageFunction = "" , string ParamFunction="Empty Param list"
            ,[System.Runtime.CompilerServices.CallerMemberName] string memberName = ""
            )
        {
            try
            {
                if (b_Debug)
                {
                    MsgsLog.Add(new SRMessageLog() { FunctionParam = ParamFunction, FunctionName = memberName });
                    OnPropertyChanged(nameof(MsgsLog));
                }
            } 
            catch (Exception err)
            {
            }
        }



        public async Task RequestTokenNow(string cmd)
        {
            try
            {
                IsBusy = true;
                string userQR = cmd;
                /*"<cmd xmlns=\"http://localhost/xrogi\" pid=\"14622\" clientname=\"ru.svod_int.SvodInf 1.0.34\" vers=\"2\" name=\"gettoken\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
                                "<user name=\"76E4DF6C-6FA1-4797-AE94-B3BCD8F4EB72\">"+
                                "<device name=\"Blackview BV6000S_RU\" OSVersion=\"Android 7.0\" IMEI=\"24:5E:4D:92:AD:DD\" devicetype=\"Phone\">"+"<Token_Counter>1</Token_Counter></device></user></cmd>";
                                */

                /*
                 
                 <?xml version="1.0" encoding="utf-8"?>
<cmd xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" name="gettoken" vers="2" clientname="ru.svod_int.SvodInf 1.2.412" clientvers="1.2.412" pid="6156" xmlns="http://localhost/xrogi">
  <user name="F9107DCE-7587-4561-8ACF-6C530C38F51C">
    <device name="Google Android SDK built for x86" devicetype="Phone" IMEI="90b8a3acf806c991" OSVersion="Android 9">
      <Token_Counter>1</Token_Counter>
    </device>
  </user>
</cmd>
                 */

                SendLocalMessage(String.Empty, "Токен запрошен...");
                await hubConnection.InvokeAsync("token_get", userQR);

            }
            catch (Exception ex)
            {
                SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }


        public async Task Request_User_List(int maxVers, int nPage, String Filter)
        {
            int ntry = 3;
            while (ntry > 0)
            {
                
                try
                {
                    int DedLockTimeout = 25;
                    while (isOpen() == false)
                    {
                        await Task.Delay(new Random().Next(0, 5) * 200);
                        DedLockTimeout--;
                        if (DedLockTimeout <= 0)
                            break;
                    }
                    if (isOpen())
                    {
                        IsBusy = true;
                        SendLocalMessage(String.Empty, "user_getlist");
                        //     string userQR = "<cmd xmlns=\"http://localhost/xrogi\" pid=\"14622\" clientname=\"ru.svod_int.SvodInf 1.0.34\" vers=\"2\" name=\"gettoken\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><user name=\"76E4DF6C-6FA1-4797-AE94-B3BCD8F4EB72\"><device name=\"Blackview BV6000S_RU\" OSVersion=\"Android 7.0\" IMEI=\"24:5E:4D:92:AD:DD\" devicetype=\"Phone\"><Token_Counter>1</Token_Counter></device></user></cmd>";
                        //      await hubConnection.InvokeAsync("user_getlist", maxVers, nPage, Filter);
                        await hubConnection.InvokeAsync("user_getlist_filter", maxVers, nPage, Filter);
                        
                        return;
                    }
                    else
                    {
                        await hubConnection.StartAsync();

                    }
                }
                catch (Exception ex)
                {
                    SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
                }
                finally
                {
                    IsBusy = false;
                }
                ntry--;
            }
        }
        public async Task Request_UserParams(int UserId)
        {
            try
            {
                IsBusy = true;
                SendLocalMessage(String.Empty, "user_params  запрошен...");
                await hubConnection.InvokeAsync("user_params", UserId);
            }
            catch (Exception ex)
            {
                SendLocalMessage(String.Empty, $"user_params Ошибка отправки: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task Request_Chat_Statisic(int Chatid)
        {
            try
            {
                IsBusy = true;
                SendLocalMessage(String.Empty, "Request_Chat_Statisic  запрошен...");

                //     string userQR = "<cmd xmlns=\"http://localhost/xrogi\" pid=\"14622\" clientname=\"ru.svod_int.SvodInf 1.0.34\" vers=\"2\" name=\"gettoken\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><user name=\"76E4DF6C-6FA1-4797-AE94-B3BCD8F4EB72\"><device name=\"Blackview BV6000S_RU\" OSVersion=\"Android 7.0\" IMEI=\"24:5E:4D:92:AD:DD\" devicetype=\"Phone\"><Token_Counter>1</Token_Counter></device></user></cmd>";
                await hubConnection.InvokeAsync("chat_statistic", Chatid);
            }
            catch (Exception ex)
            {
                SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task Request_Message_Shown(int Chatid,    int ObjId)
        {
            try
            {
                IsBusy = true;
                int[] a = new int[] { ObjId };
                SendLocalMessage(String.Empty, "Request_Message_Shown  запрошен...");
                await hubConnection.InvokeAsync("message_shown", Chatid, a);
                
            }
            catch (Exception ex)
            {
                SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        internal async Task Request_Message_Status(int Chatid, int objId)
        {
            try
            {
                IsBusy = true;
                SendLocalMessage(String.Empty, "Request_Message_Status  запрошен...");
                await hubConnection.InvokeAsync("message_status", Chatid, objId);
                
            }
            catch (Exception ex)
            {
                SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        public async Task SendMessageAsync(string message)
        {
            try
            {
                //if (!CanSendMessage(message))                return;
                if (isOpen()==false)
                {
                    SendLocalMessage(String.Empty, "> SendMessageAsync  Open запрошен...");
                    bool x = await Open();
                }
                if (isOpen() )
                {
                    SendLocalMessage(String.Empty, "> InvokeAsync "+ message + "  запрошен...");
                    await hubConnection.InvokeAsync(message);
                    
                    //     var byteMessage = Encoding.UTF8.GetBytes(message);
                    //    var segmnet = new ArraySegment<byte>(byteMessage);
                    //await client.SendAsync(segmnet, WebSocketMessageType.Text, true, cts_Write);
                    //     await hubConnection.s.SendAsync(segmnet, WebSocketMessageType.Text, true, cts_Write);
                }
            }
            catch (Exception err)
            {
                OnWSDisConnected?.Invoke(this, err);
            }
        }

        internal async Task Request_Message_Send(int Chatid, string xml)
        { try
            {
                IsBusy = true; 
                SendLocalMessage(String.Empty, "message_send  запрошен...");
                await hubConnection.InvokeAsync("message_send", Chatid, xml);
                
            }
            catch (Exception ex)
            {
                SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        public async Task Request_Message_SendFile(int Chatid, byte [] filedata,string NameFile)
        {
            try
            {
                IsBusy = true;
                SendLocalMessage(String.Empty, "UploadFilePost  запрошен...");
                await UploadFilePost( Chatid, filedata,  NameFile);
              //  await hubConnection.InvokeAsync("message_sendfile", Chatid,NameFile);
               
            }
            catch (Exception ex)
            {
                SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task UploadFilePost(int chatid, byte[] filedata, string nameFile)
        {

            HttpContent content = GetContent(chatid, filedata, nameFile);
            var httpClient = new HttpClient();

            string url = App.ddd.SOAP_SoapServer() + "/xml/SendFileData.ashx?" + "tiket=" + "3545647155565&nmessage=78&id=" + chatid.ToString();// +"&s="+toke;
            var response = await httpClient.PostAsync(new Uri(url), content).ConfigureAwait(false);

            string status = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        private HttpContent GetContent(int chatid, byte[] filedata, string nameFile)
        {
            MultipartFormDataContent c = new MultipartFormDataContent();
            c.Add(new StreamContent(new MemoryStream(filedata)), "\"file\"", nameFile);
//            c.Add(new Content()    "\"chatid\"", chatid);
            return null;
        }



        bool b_InReconnect = false;


        int nReconnect = 0;
       // object bIsEnterToConnect= new object();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
       public async Task<bool> ReConnect()
        {

            await _semaphore.WaitAsync();//https://inrecolan.ru/blog/ilya-koshevoy/421-net4part3
            try
            {  
                if (b_InReconnect) return false;
                b_InReconnect = true;
                {
                    logconnection = new ConnectionLog();
                    App.ddd.LogConnecttionAdd(logconnection);
                    
                   // while (true)
                    {
                        // !!!! https://docs.microsoft.com/ru-ru/aspnet/signalr/overview/guide-to-the-api/handling-connection-lifetime-events
                        if (IsConnected)
                            if (
                                hubConnection.State == HubConnectionState.Connecting
                                ||
                                hubConnection.State == HubConnectionState.Disconnected
                                )
                                IsConnected = false;

                     //   if (IsConnected)
                     //       break;
                        try
                        {
                            if (hubConnection.State == HubConnectionState.Connecting ||
                                hubConnection.State == HubConnectionState.Reconnecting)
                            {
                                return false;//           hubConnection.StopAsync().ConfigureAwait(true);
                            }
                            //           if (hubConnection.State == HubConnectionState.Disconnected)
                            logconnection.DateBeginConnect = DateTime.Now;
                            logconnection.Event = "Соединение...";
                            await hubConnection.StartAsync().ConfigureAwait(true);
                            //   SendLocalMessage(String.Empty, "Вы вошли в чат...");

                            if (hubConnection.State == HubConnectionState.Connected)
                            {
                                logconnection.DateConnected = DateTime.Now;
                                logconnection.Event = "Подключено";
                                IsConnected = true;
                                OnWSConnected?.Invoke(this);

                                //string cmd = App.ddd.RequestTokenSeanceString();
                                //RequestTokenNow(cmd);
                                return true;
                                //  break;
                            }
                            else
                            {
                            }
                             //   hubConnection.StopAsync().ConfigureAwait(true);

                        }
                        catch (Exception ex)
                        {
                            logconnection.DateErrorConnected = DateTime.Now;
                            logconnection.ErrorException = ex;
                            logconnection.Event = "Ошибка:" + ex.InnerException.Message.ToString();
                                ;
                            //   SendLocalMessage(String.Empty, $"Ошибка подключения: {ex.Message}");
                            //System.InvalidOperationException: The HubConnection cannot be started if it is not in the Disconnected state.
                            //await Connect();
                        }
                 //       Task.Delay(1000).ConfigureAwait(true);
                    }
                }
            }
            finally
            {
                _semaphore.Release();
                b_InReconnect = false;
            }
            return false; 
        }


        object parseReadmessage = new object();
        async Task ReadMessage(ClientWebSocket client)
        {

            try
            {
                WebSocketReceiveResult result;
              //  var message = new ArraySegment<byte>(new byte[4096]);
                do
                {
                    //using (
                    ArraySegment<byte> message = new ArraySegment<byte>(new byte[4096]);
                        //)
                    {

                        result = await client.ReceiveAsync(message, cts_Read);
                        if (result.MessageType != WebSocketMessageType.Text)
                            break;
                        lock (parseReadmessage)
                        {
                            bool b_isChatUpdateMsg = false;
                            var messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
                            string receivedMessage = Encoding.UTF8.GetString(messageBytes);

                            WS_EventType type = WS_EventType.msgUnknown;

                            try
                            {
                                type = WS_EventType_Decode(receivedMessage);
                           //     OnWSRecive?.Invoke(this, type, receivedMessage);
                            }
                            catch (Exception err)
                            {

                            }
                            if (receivedMessage == "pong")
                            {
                                OnWSRecivePong?.Invoke(this);
                                continue;
                            }
                            if (receivedMessage.StartsWith("CHATSTATUSUPDATE["))
                            {
                             //   continue;
                            }
                            int toke = receivedMessage.IndexOf("OK[");
                            int end = receivedMessage.IndexOf("]");
                            if (toke >= 0)
                            {

                                string token = receivedMessage.Substring(toke + 3, end - toke - 3).Trim();
                                if (token == "!!??!!")
                                {

                                }
                                if (string.IsNullOrEmpty(token) == false)
                                {
                                    OnWSReciveToken?.Invoke(this, token);
                                    continue;
                                }

                            }
                            {
                                int indexMsg = -1;
                                 indexMsg = receivedMessage.IndexOf("CHATSTATUSUPDATE[");
                                 if (indexMsg >= 0 && end > indexMsg)
                                 {
                                     try
                                     {
                                         b_isChatUpdateMsg = true;
                                         string[] strs = receivedMessage.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                                         // foreach (string str in strs)
                                         {

                                             if (strs.Length == 2)
                                             {

                                                 int chat = Convert.ToInt32(strs[1]);

                                                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                                                {
                                                    //if (OnWSReciveNewChatStatistic != null)
                                                    //    OnWSReciveNewChatStatistic.Invoke(this, chat);
                                                });


                                            }

                                         }
                                     }
                                     catch (Exception err)
                                     {

                                     }
                                     continue;

                                 }
                            }

                            //"MSG[25546,372856]"
                            {
                                int indexMsg = -1;
                                indexMsg = receivedMessage.IndexOf("MSG[");
                                if (indexMsg >= 0 && end > indexMsg)
                                {
                                    string[] strs = receivedMessage.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                                    // foreach (string str in strs)
                                    {
                                        try
                                        {
                                            if (strs.Length == 2)
                                            {
                                                String[] data = strs[1].Split(',');
                                                int chat = Convert.ToInt32(data[0]);
                                                int msg = Convert.ToInt32(data[1]);
                                                if (OnWSReciveNewMessage != null)
                                                    OnWSReciveNewMessage.Invoke(this, receivedMessage, chat, msg);
                                            }
                                        }
                                        catch (Exception err)
                                        {

                                        }
                                    }

                                }
                            }
                            {

                                if (b_isChatUpdateMsg == false)
                                {
                                    bool isChatCmd = receivedMessage.StartsWith("CHAT");// if (o.Cmd == "CHAT" || o.Cmd == "CHATLEAVE" || o.Cmd == "CHATADD" || o.Cmd == "CHATRENAME") if (o.Cmd == "CHATSTATUSUPDATE")
                                    if (isChatCmd)
                                    {
                                        try
                                        {
                                            if (OnWSReciveNewMessage != null)
                                                OnWSReciveChatEvent.Invoke(this, receivedMessage);
                                        }
                                        catch (Exception err)
                                        {

                                        }
                                    }
                                }
                            }

                            //                Console.WriteLine("Received: {0}", receivedMessage);
                        }
                    }
                }
                while (!result.EndOfMessage);
            }catch (Exception err)
            {
                OnWSDisConnected?.Invoke(this, err);
            }
        }

        private WS_EventType WS_EventType_Decode(string receivedMessage)
        {
            WS_EventType val = WS_EventType.msgUnknown;
            try
            {
                string cmd = receivedMessage.ToLower();
                if (cmd == "pong".ToLower())
                {
                    return WS_EventType.msgPong;
                }
                if (cmd.StartsWith("OK[".ToLower()))
                {
                    return WS_EventType.msgOK;
                }
                if (cmd.StartsWith("CHATSTATUSUPDATE[".ToLower()))
                {
                    return WS_EventType.msgCHATSTATUSUPDATE;
                }
                if (cmd.StartsWith("MSG[".ToLower()))
                {
                    return WS_EventType.msgMSG;
                }
                if (cmd.StartsWith("CHAT[".ToLower()))
                {
                    return WS_EventType.msgChat;
                }
                
            }
            catch (Exception err) 
            {
            }
            return val;
            
        }
        public bool isOpen()
        {
            try
            {
                if (hubConnection==null) 
                    return false;

                if (hubConnection.State == HubConnectionState.Connected)
                    return true;
                
            }
            catch (Exception err)
            {
            }
            return false;
        }

        public async void Close()
        {
            /*cancelTokenSource.Cancel();
            client = null;
            cancelTokenSource = null;*/
            await hubConnection.StopAsync();
           // throw new Exception("Нет соединения");
        }

        public async Task Request_Job_List(int Chatid, string Filter)
        {
            try
            {
                IsBusy = true;
                await hubConnection.InvokeAsync("job_getlist", Chatid, Filter);
                SendLocalMessage(String.Empty, "Request_Job_List  запрошен...");
            }
            catch (Exception ex)
            {
                SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        public async Task Request_Chat_List(int Chatid)
        {
            try
            {
                IsBusy = true;
                SendLocalMessage(String.Empty, "chat_getlist  запрошен...");
                await hubConnection.InvokeAsync("chat_getlist", Chatid);
               
            }
            catch (Exception ex)
            {
                SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        internal async Task Request_Job_SetStatus(int ChatId, int ObjId, string NewStatus)
        {
            try
            {
                IsBusy = true; SendLocalMessage(String.Empty, "job_setstatus  запрошен...");
                await hubConnection.InvokeAsync("job_setstatus", ChatId, ObjId, NewStatus).ConfigureAwait(false);
                
            }
            catch (Exception ex)
            {
                SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }


        public async Task Request_Message_Page(int Chatid,int [] nPages)
        {
            int nRepeat = 3;

            while (nRepeat > 0)
            {
                try
                {
                    if (await WaitOpen())
                    {
                        IsBusy = true; SendLocalMessage(String.Empty, "message_getpagelist  запрошен...");
                        await hubConnection.InvokeAsync("message_getpagelist", Chatid, nPages);
                     //   SendLocalMessage(String.Empty, "Request_Message_Page  запрошен...");
                        nRepeat = 0;
                    }
                }
                catch (JiException ex)
                {
                    SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
                    nRepeat--;
                }
                catch (Exception ex)
                {
                    SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
                    nRepeat--;
                }

                finally
                {
                    IsBusy = false;
                }
            }
        }
        public async Task Request_Message_List(int Chatid, int[] MsgIds)
        {
            try
            {
                IsBusy = true; SendLocalMessage(String.Empty, "message_getlist  запрошен...");
                await hubConnection.InvokeAsync("message_getlist", Chatid, MsgIds);
             //   SendLocalMessage(String.Empty, "Request_Message_List  запрошен...");
            }
            catch (Exception ex)
            {
                SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task Request_Chat_Create(int parentChatId, int typeChatId, string name, string Description, int[] users)
        {
            int nRepeat = 3;
            
            while (nRepeat > 0)
            {
                try
                {
                    IsBusy = true;
                    SendLocalMessage(String.Empty, "chat_create  запрошен...");
                    await hubConnection.InvokeAsync("chat_create", parentChatId, typeChatId, name, Description, users).ConfigureAwait(false);
                   // SendLocalMessage(String.Empty, "Request_Message_List  запрошен...");
                    nRepeat = 0;
                }
                catch (System.TimeoutException ex)
                {
                    //{System.TimeoutException: Server timeout (30000,00ms) elapsed without receiving a message from the server.   at Microsoft.AspNetCore.SignalR.Client.HubConnection.InvokeCoreAsyncCore(
                    SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
                    nRepeat--;
                }
                catch (Exception ex)
                {
                    //"Failed to invoke 'chat_create' due to an error on the server. HubException: Method does not exist."
                    SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        public  async Task Request_Chat_Leave(int chatId)
        {
            int nRepeat = 3;

            while (nRepeat > 0)
            {
                try
                {
                    IsBusy = true;
                    SendLocalMessage(String.Empty, "chat_leave  запрошен...");
                    await hubConnection.InvokeAsync("chat_leave", chatId).ConfigureAwait(false);
                  //  SendLocalMessage(String.Empty, "Request_Chat_Leave  запрошен...");
                    nRepeat = 0;
                }
                catch (System.TimeoutException ex)
                {
                    //{System.TimeoutException: Server timeout (30000,00ms) elapsed without receiving a message from the server.   at Microsoft.AspNetCore.SignalR.Client.HubConnection.InvokeCoreAsyncCore(
                    SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
                    nRepeat--;
                }
                catch (Exception ex)
                {
                    //"Failed to invoke 'chat_create' due to an error on the server. HubException: Method does not exist."
                    SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }



        public async Task Register_Param(string ParamName, string ParamValue)
        {
            int nRepeat = 3;

            while (nRepeat > 0)
            {
                try
                {
                    IsBusy = true;
                    Open();
                    {
                        SendLocalMessage(String.Empty, "register_param  запрошен...");
                        await hubConnection.InvokeAsync("register_param", ParamName, ParamValue).ConfigureAwait(false);
                      //  SendLocalMessage(String.Empty, "Register_Param  запрошен...");
                        nRepeat = 0;
                    }
                }
                catch (System.TimeoutException ex)
                {
                    //{System.TimeoutException: Server timeout (30000,00ms) elapsed without receiving a message from the server.   at Microsoft.AspNetCore.SignalR.Client.HubConnection.InvokeCoreAsyncCore(
                    SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
                    nRepeat--;
                }
                catch (System.SystemException se)
                {
                    SendLocalMessage(String.Empty, $"Ошибка отправки: {se.Message}");
                }
                catch (Exception ex)
                {
                    //"Failed to invoke 'chat_create' due to an error on the server. HubException: Method does not exist."
                    SendLocalMessage(String.Empty, $"Ошибка отправки: {ex.Message}");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }
    }
}
