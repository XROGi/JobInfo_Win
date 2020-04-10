using System;
using System.Text;
using System.Threading.Tasks;

namespace Ji.Models
{
   interface OnLineInterface
    {
        //HubConnection hubConnection;
        /*private ClientWebSocket client;
        CancellationToken cts_Connect;

        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();


        CancellationToken cts_Read;
        CancellationToken cts_Write;
        Task TaskRecive;
        */
    /*   public delegate void OnWSConnectedDelegate(OnLineInterface sender);
        public event OnWSConnectedDelegate OnWSConnected ;

        public delegate void OnWSDisConnectedDelegate(X_WS sender, Exception err);
        public event OnWSDisConnectedDelegate OnWSDisConnected;

        public delegate void OnWSReciveDelegate(X_WS sender, WS_EventType type, string Msg);
        public event OnWSReciveDelegate OnWSRecive;

        public delegate void OnWSReciveNewMessageDelegate(X_WS sender, string Msg, int ChatId, int NewMsgId);
        public event OnWSReciveNewMessageDelegate OnWSReciveNewMessage;

        public delegate void OnWSReciveNewChatStatisticDelegate(X_WS sender, int ChatId);
        public event OnWSReciveNewChatStatisticDelegate OnWSReciveNewChatStatistic;

        public delegate void OnWSReciveChatEventDelegate(X_WS sender, string Msg);
        public event OnWSReciveChatEventDelegate OnWSReciveChatEvent;


        public delegate void OnWSRecivePongDelegate(X_WS sender);
        public event OnWSRecivePongDelegate OnWSRecivePong;

        public delegate void OnWSReciveTokenDelegate(X_WS sender, string tokenSeance);
        public event OnWSReciveTokenDelegate OnWSReciveToken;

        public delegate void OnWSReciveTokenClosedDelegate(X_WS sender);
        public event OnWSReciveTokenClosedDelegate OnWSReciveTokenClosed;

        public string Url { get; set; }*/
       /* public async Task Open();

        public async void SendMessageAsync(string message);

        async Task ReadMessage();

        private WS_EventType WS_EventType_Decode(string receivedMessage);
       
        public bool isOpen();
        public void Close();
         */
    }
}
