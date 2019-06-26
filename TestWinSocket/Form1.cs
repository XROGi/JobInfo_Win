using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestWinSocket
{
    public partial class Form1 : Form
    {
        ClientWebSocket ws ;
        CancellationTokenSource cts_Connection = new CancellationTokenSource();

        public Form1()
        {
            InitializeComponent();
            listBox1.Items.Clear();
            ws = new ClientWebSocket();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           listBox1.Items.Clear();
           
        //   ClientWebSocket ws = 
                ConnectToServer(textBox1.Text, cts_Connection);

                
         
        }

        public     void ConnectToServer(string url, CancellationTokenSource cts_Connection)
        {
            ws = new ClientWebSocket();
            {
                // https://www.websocket.org/echo.html
                //          Uri serverUri = new Uri("ws://127.0.0.1:53847/ChatHandler.ashx");
                //                Uri serverUri = new Uri("ws://localhost:53847/ChatHandler.ashx");
                Uri serverUri = new Uri(url);
                //Uri serverUri = new Uri("wss://demos.kaazing.com/echo");
                /*

                ws.Options.Proxy = new WebProxy("proxy")
                {
                    Credentials = new NetworkCredential("iu.smirnov@ghp.lc", "mou773nitnap*")
                };
                */



                CancellationTokenSource cts = new CancellationTokenSource();

                //       string g = ws.SubProtocol;
                listBox1.Items.Add(ws.State.ToString());
                var st = ws.State;
                  ws.ConnectAsync(serverUri, CancellationToken.None);
                int circle = 0;
                while (ws.State != WebSocketState.Open)
                {
                    circle++;
                    if (ws.State == WebSocketState.Closed)
                        break;
                    if (ws.State == WebSocketState.Open)
                        break;
                    if (st != ws.State)
                    {
                        listBox1.Items.Add(ws.State.ToString() + " iterators=" + circle.ToString());
                        st = ws.State;
                    }
                }
                listBox1.Items.Add(ws.State.ToString() + " iterators=" + circle.ToString());
             //   return _ws;
            }
        }

            private void timer1_Tick(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                textBox2.Text = DateTime.Now.ToLongTimeString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            while (ws.State != WebSocketState.Open)
            {
               // ws = 
                       ConnectToServer(textBox1.Text, cts_Connection);
                if (ws == null) return;
            }

            while (ws.State == WebSocketState.Open)
            {
                /*Console.Write("Input message ('exit' to exit): ");
                string msg = Console.ReadLine();
                if (msg == "exit")
                {
                    break;
                }
                */
                var newString = String.Format("I am server,this is test.Msg[{0}]", textBox2.Text ); //DateTime.Now.ToString()
                Byte[] bytes = System.Text.Encoding.UTF8.GetBytes(newString);

                ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(newString));
                ws.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
           /*     ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
                var result = ws.ReceiveAsync(bytesReceived, CancellationToken.None);*/
                //   Console.WriteLine(Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            cts_Connection.Cancel();
            cts_Connection.Dispose();
        }
    }
}
