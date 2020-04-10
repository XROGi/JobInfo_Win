using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.ClientEngine;
using WebSocket4Net;
 

namespace JobInfo
{
    class WS_XROGi
    {
        public bool IsConnected;
 
        string URL_Server;

        WebSocket ClientWebSocket;
        public delegate void OnConnecedDelegate();
        public event OnConnecedDelegate OnConneced = delegate { };

        public delegate void OnDisConnecedDelegate();
        public event OnDisConnecedDelegate OnDisConneced = delegate { };

        public delegate void OnIncomeMessageDelegate(string Message);
        public event OnIncomeMessageDelegate OnIncomeMessage = delegate { };

        public delegate void OnExceptionErrorDelegate(Exception error);
        public event OnExceptionErrorDelegate OnExceptionError = delegate { };

        public void Send(string id)
        {
            WebSocket webSocket = ClientWebSocket;
            //if (webSocket!=null)
            webSocket?.Send(id);
        }

        public async Task<bool> ConnectAsync(string url, bool isreconnect = false)
        {
            try
            {
                URL_Server = url;
                //работаем с локальной копией вебсокета, чтобы избежать его закрытия из другого потока во время асинхронной операции
                //(маловероятно, но возможно)
                WebSocket webSocket = ClientWebSocket;

                // Проверяем что не поделючены
                if (!IsConnected)
                {
                    //получаем адресс сервера (ws://myserver/app.ashx")
                    var uri = ServerUri();
                    webSocket = new WebSocket(uri.ToString());
                    //устанавливаем обработчики
                    webSocket.Error += webSocket_Error;
                    webSocket.MessageReceived += Receive;
                    webSocket.Closed += webSocket_Closed;
                    //соединение не асинхронное, поэтому "асинхронизируем" его принудительно
                    var tcs = new TaskCompletionSource<bool>();
                    webSocket.Opened += (s, e) =>
                    {
                        //устанавливаем в переменную класса только после успешного подключения
                        ClientWebSocket = webSocket;
                        IsConnected = true;
                        if (OnConneced != null)
                            OnConneced();        //сообщаем, что мы подключились

                        else tcs.SetResult(true);

                    };
                    //webSocket.Closed 
                   webSocket.Open();

                    return await tcs.Task;

                }

                return false;
            }
            catch (Exception ex)
            {
                //что-то не так
                return false;
            }

        }

        private void Connected()
        {
           
        }

        private void webSocket_Closed(object sender, EventArgs e)
        {
            IsConnected = false;
            if (OnDisConneced!=null)
                        OnDisConneced();
            
        }

        private void Receive(object sender, MessageReceivedEventArgs e)
        {
              OnIncomeMessage(e.Message);
            //      OnIncomeMessageDelegate temp = OnIncomeMessage;
          
        }

        private void webSocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            if (OnExceptionError!=null)
            OnExceptionError(e.Exception);
        }

        private Uri ServerUri()
        {
            //            return new Uri("ws://jobinfo/xml/ChatHandler.ashx");
            return new Uri(URL_Server); //"ws://localhost:53847/ChatHandler.ashx")
        }

        internal void CloseConnect()
        {
            WebSocket webSocket = ClientWebSocket;

            webSocket.Close();
            webSocket = null;

            IsConnected = false;
            if (OnDisConneced != null)
                OnDisConneced();

            
        }
    }
}

