using Ji.Models;
using Ji.Services;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ji
{
    public class X_WS
    {
        private ClientWebSocket  client;
         CancellationToken cts_Connect;

        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
       

         CancellationToken cts_Read;
         CancellationToken cts_Write;
         Task TaskRecive;

        public delegate void OnWSConnectedDelegate(X_WS sender);
        public event OnWSConnectedDelegate OnWSConnected /* = delegate { }*/;

        public delegate void OnWSDisConnectedDelegate(X_WS sender, Exception err);
        public event OnWSDisConnectedDelegate OnWSDisConnected /* = delegate { }*/;

        public delegate void OnWSReciveDelegate(X_WS sender, WS_EventType type, string Msg);
        public event OnWSReciveDelegate OnWSRecive;

        public delegate void OnWSReciveNewMessageDelegate(X_WS sender, string Msg, int ChatId, int NewMsgId);
        public event OnWSReciveNewMessageDelegate OnWSReciveNewMessage;

        public delegate void OnWSReciveNewChatStatisticDelegate(X_WS sender,  int ChatId);
        public event OnWSReciveNewChatStatisticDelegate OnWSReciveNewChatStatistic;

        public delegate void OnWSReciveChatEventDelegate(X_WS sender, string Msg);
        public event OnWSReciveChatEventDelegate OnWSReciveChatEvent;
        

        public delegate void OnWSRecivePongDelegate(X_WS sender);
        public event OnWSRecivePongDelegate OnWSRecivePong;

        public delegate void OnWSReciveTokenDelegate(X_WS sender,string tokenSeance);
        public event OnWSReciveTokenDelegate OnWSReciveToken;

        public delegate void OnWSReciveTokenClosedDelegate(X_WS sender);
        public event OnWSReciveTokenClosedDelegate OnWSReciveTokenClosed;

        bool  bLockOnOpen = false;
        string url;
        public string Url { get => url; set => url=value; }
        

        public async Task Open()
        {
            if (bLockOnOpen == true) 
                return;
            try 
            {
                 bLockOnOpen = true;
                if (isOpen() == false && Url != null && Url != "")
                {
                    try
                    {
                        /*

                        if (client != null)
                        {
                            Close();
                            //    return;

                            //await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "", cts_Connect);
                        }*/
                        //  else
                        {
                            client = new ClientWebSocket();
                            cancelTokenSource = new CancellationTokenSource();
          //                  cancelTokenSource.CancelAfter(2000);
                        }
                        //else
                        //    {

                        //}
                        Uri url = new Uri(Url);
                        await client.ConnectAsync(url, cancelTokenSource.Token); //new Uri(App.ddd.connectInterface.Server_WS)

                        OnWSConnected?.Invoke(this);

                        var TaskRecive = await Task.Factory.StartNew(async () =>
                         {
                             try
                             {
                                 while (true)
                                 {
                                     if (isOpen())
                                     {
                                         await ReadMessage(client);
                                     }
                                     else
                                     {
                                         break;
                                         //cancelTokenSource.Cancel();// Task.Factory.
                                         //Thread.Sleep(1500);
                                     }
                                 }
                             }
                             catch (Exception err)
                             {
                                
                                 OnWSDisConnected?.Invoke(this, err);
                             }

                         }, cancelTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                        // Do something with WebSocket


                        //WebSocketReceiveResult result;
                        //var message = new ArraySegment<byte>(new byte[4096]);
                        //result = await client.ReceiveAsync(message, cts);
                        //var messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
                        //string receivedMessage = Encoding.UTF8.GetString(messageBytes);
                        //SendMessageAsync("ping");
                        //result = await client.ReceiveAsync(message, cts1);
                        //var messageBytes1 = message.Skip(message.Offset).Take(result.Count).ToArray();
                        //string receivedMessage1 = Encoding.UTF8.GetString(messageBytes);
                    }
                    catch (Exception err)
                    {
              //          bLockOnOpen = false;
                        OnWSDisConnected?.Invoke(this, err);
                    }
                }
            }
            finally
            {
                bLockOnOpen = false ;
            }
        }

        public async void SendMessageAsync(string message)
        {
            try
            {
                //if (!CanSendMessage(message))                return;
                if (isOpen()==false)
                {
                    Open();
                }
                if (isOpen() )
                {
                    var byteMessage = Encoding.UTF8.GetBytes(message);
                    var segmnet = new ArraySegment<byte>(byteMessage);
                    await client.SendAsync(segmnet, WebSocketMessageType.Text, true, cts_Write);
                }
            }
            catch (Exception err)
            {
                OnWSDisConnected?.Invoke(this, err);
            }
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
                                                    if (OnWSReciveNewChatStatistic != null)
                                                        OnWSReciveNewChatStatistic.Invoke(this, chat);
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
                if (client == null)
                    return false;
                if (client.State == WebSocketState.Open)
                    return true;
            }
            catch (Exception err)
            {
            }
            return false;
        }

        public void Close()
        {
            cancelTokenSource.Cancel();
            client = null;
            cancelTokenSource = null;
           // throw new Exception("Нет соединения");
        }
    }
}
