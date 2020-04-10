//using JobInfo;
//using JobInfo.XROGi_Class;
using JobInfo;
using JobInfo.XROGi_Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JIcon
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        private static extern
       bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern
            bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);


        [DllImport("user32.dll")]
        private static extern
            bool ShowOwnedPopups(IntPtr hWnd, bool fShow);


        [DllImport("user32.dll")]
        private static extern
            bool IsIconic(IntPtr hWnd);

        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOWNOACTIVATE = 4;
        private const int SW_RESTORE = 9;
        private const int SW_SHOWDEFAULT = 10;


        Job job;
        public delegate void OnConnecedToJobServerDelegate(Job _job);
        public event OnConnecedToJobServerDelegate OnConnecedToJobServerMethod = delegate { };
        public delegate void OnTokenReciveDelegate(Job job);
        public event OnTokenReciveDelegate OnTokenReciveMethod = delegate { };
        public delegate void OnMsgReciveDelegate(Job _job, string cmd, long chatid, long msgid);
        public event OnMsgReciveDelegate OnMsgReciveDelegateMethod = delegate { };
        public delegate void OnDisConnecedToJobServerWithErrDelegate(Job _job, Exception err);
        public event OnDisConnecedToJobServerWithErrDelegate OnDisConnecedWithErrMethod = delegate { };

        bool cClose;
        bool b_ShowConnectInfo = false;
        bool b_ShowIncommeMessage = true;
        public Form1()
        {
            InitializeComponent();
            this.OnConnecedToJobServerMethod += OnConnecedToJobServerCall;
            this.OnTokenReciveMethod += OnTokenReciveMethodCall;
            this.OnMsgReciveDelegateMethod += OnMsgReciveDelegateCall;
            this.OnDisConnecedWithErrMethod += OnDisConnecedWithErrMethodCall;
            label6.Text = "";
            label5.Text = "";
            label2.Text = "";
            label4.Text = "";
            cClose = true;
            if (Environment.UserName=="iu.smirnov")
            {
                //             b_ShowConnectInfo = true;
                button2.Visible = true;
            }
            else
                button2.Visible = false;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            StartMain();

            //
        }

        private void StartMain()
        {
            //    
            bool RunWinClient = false;
            foreach (string par in job.Setup_Params)
            {
                if (par == "RunWinClient=true")
                {
                    RunWinClient = true;
                }
            }
            if (RunWinClient==false)
            {
                ShowInfoBallon("Информация", "Запуск мессенджера временно недоступен.\r\n Инф тел. 4296");
                return;
            }
            try
            {
                try
                {

                    //Проверяем на наличие мутекса в системе
                    Mutex.OpenExisting("JobInfo");
                    ShowOnTopProcess("JobInfo");
                }
                catch
                {
                    //Если получили исключение значит такого мутекса нет, и его нужно создать
               
                  
                    {
                        RunNewProcessMain();
             
                    }
                    
                }
                Close();

             /*   string shortcutName = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.Programs),
                     //                "\\", publisher_name,
                     "\\", "SvodDocs", ".appref-ms");
                //СводИнтернешнл\SvodDocs.appref - ms
                Process.Start("file://soft/svoddocs$/JobInfo/JobInfo.application");////file://soft/svoddocs$/SvodDocs/SvodDocs.application
                */
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message.ToString(), "Диагностика ошибки");
            }
        }

        private void RunNewProcessMain()
        {
            //      string shortcutName = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.Programs),
            //                "\\", publisher_name,
            //   "\\", "SvodDocs", ".appref-ms");
            //СводИнтернешнл\SvodDocs.appref - ms
            Process.Start("file://soft/svoddocs$/JobInfo/JobInfo.application");////file://soft/svoddocs$/SvodDocs/SvodDocs.application
        }

        private void ShowOnTopProcess(string v)
        {
            string proc = Process.GetCurrentProcess().ProcessName;
            proc = v;
            // get the list of all processes by that name
            Process[] processes = Process.GetProcessesByName(proc);
            // if there is more than one process...
            if (processes.Length >= 1)
            {
                // Assume there is our process, which we will terminate, 
                // and the other process, which we want to bring to the 
                // foreground. This assumes there are only two processes 
                // in the processes array, and we need to find out which 
                // one is NOT us.

                // get our process
                Process p = Process.GetCurrentProcess();
                int n = 0;        // assume the other process is at index 0
                                  // if this process id is OUR process ID...
                if (processes[0].Id == p.Id)
                {
                    // then the other process is at index 1
                    n = 1;
                }
                processes[0].Refresh();
                // get the window handle
                IntPtr hWnd = processes[n].MainWindowHandle;
                if (hWnd != IntPtr.Zero)
                {
                    // if iconic, we need to restore the window
                    if (IsIconic(hWnd))
                    {
                        ShowWindowAsync(hWnd, SW_RESTORE);
                    }
                    var placement = Window_StaticClass.GetPlacement(hWnd);
                    //       MessageBox.Show(placement.showCmd.ToString());
            //        Text = placement.showCmd.ToString();


                    ShowOwnedPopups(hWnd, true);
                    ;
                    // bring it to the foreground
                    SetForegroundWindow(hWnd);
                    // exit our process
                }
                else
                {

                    Window_StaticClass. UnhideProcess(v);
/*
                    //              RunNewProcessMain();
                    var ret = Window_StaticClass.GetProcessWindows(processes[n].Id);
                    foreach ( var t in ret)
                    {
                       string cl = Window_StaticClass.GetClassNameOfWindow(t);
                       {

                        }*
                    }*/
               //     processes[n].mainw
                }
                return;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            ConnectedNow();
 
        }

        private void ConnectedNow()
        {
            string prif = GetServer_ChatURL();
            Job_Run1(prif);
        }

        int AntiRecurse = 0;
        private void Job_Run1(string prif)
        { 
            bool b_FirstRun; //e170f42b-b55d-46bd-ba07-a6b9625a2ece
            string guid = Marshal.GetTypeLibGuidForAssembly(Assembly.GetExecutingAssembly()).ToString();
            Mutex mutexObj = new Mutex(true, guid, out b_FirstRun);
            if (b_FirstRun == true)
            {
                MainJobStart(prif);
            }
            else
            {
                ShowInfoBallon("Внимание", "Уже запущенн процесс");
            }

        }

        private void MainJobStart(string prif)
        {
            while (true)
            {
                job = new Job(prif);
                // job.ConnectToServer();
                Setup.MachineName = Environment.MachineName;
                Setup.UserLogin = Environment.UserName;
                string Prif = GetServer_ChatURL();
                job.OnConneced += OnConnected;
                //job.OnConneced += OnConnecedToJobServerCompleat;
                job.OnDisconneced += OnDiscnnecedJobServer;
                job.OnTokenRecive += OnTokenRecive;
                job.OnMsgRecive += OnMsgRecive;
                job.OnChatListChanged += OnChatListChanged;
                job.OnJobClassEvent += ShowJobLogEvents;
                job.OnChatUpdateRecive += OnChatUpdateRecive;
                job.OnSocketSend += OnSocketSend;
                Setup.MachineName = Environment.MachineName;
                Setup.UserLogin = Environment.UserName;
                string returl = job.GetServerName();
                job.ConnectToServer(returl ,Setup.MachineName, Setup.UserLogin);
               
                if (returl != prif && AntiRecurse != 1)
                {
                    job.Close();
                    Setup.URLServer = returl;
                    try
                    {
                        AntiRecurse = 1;
                        //Job_Run1(Setup.URLServer);
                        job = null;
                        continue;
                    }
                    catch (Exception err)
                    {

                    }
                }
                AntiRecurse = 0;
                return;
            }
        }

        private void ShowInfoBallon(string Caption, string _Text)
        {
            string s = Caption;
            //            tn.Text = s+ " (" + c.statistic.CountNew.ToString() + ")";
            string t = _Text;
            notifyIcon_ji.BalloonTipText = _Text;
            string ca = "Ji [" + s + "]";
            if (s=="")
                ca = "Ji";
            //notifyIcon_ji.BalloonTipTitle = "Hi";
            notifyIcon_ji.ShowBalloonTip(0, ca, t, ToolTipIcon.Info);
        }

        private void OnSocketSend(Exception err, string data)
         {
           // throw new NotImplementedException();
        }

        private void OnChatUpdateRecive(Job _job, string cmd, long chatid, long msgid)
        {
           // throw new NotImplementedException();
        }

        private void ShowJobLogEvents(Job _job, string FunctionName)
        {
          //  throw new NotImplementedException();
        }

        private void OnChatListChanged(Job _job)
        {
           // throw new NotImplementedException();
        }

        private void OnMsgRecive(Job _job, string cmd, long chatid, long msgid)
        {
            // throw new NotImplementedException();
            this.BeginInvoke(OnMsgReciveDelegateMethod, _job, cmd, chatid, msgid);
        }


        private void OnMsgReciveDelegateCall(Job _job, string cmd, long chatid, long msgid)
        {
            try
            {


                Chat c = job.Chats.Where(s => s.chatId == chatid).FirstOrDefault();

                if (c != null)
                {
                    string chatName = c.Text;
                    string t = "В чате появилось новое сообщение\r\nВсего новых сообщений в чате:" + c.statistic.CountNew.ToString();
                    ShowChatsStatistivInfo(chatName, t);
                    

                }


                /*
                DebugInfo("OnMsgRecive " + cmd + " " + chatid + " " + msgid.ToString());
                XROGi_Class.Chat chat = GetCurrentChatObj();

                mp.Show_Message((int)chatid, (int)msgid);
                if (chat != null)
                {
                    int currentchatid = chat.GetId();
                    if (currentchatid == chatid)
                    {
                        try
                        {
                            Chat_GetMyStatistic(chat.chatId);
                            //         chat = GetCurrentChatObj();
                            TreeNodeUpdateCounter(chat.chatId);
                        }
                        catch (Exception err)
                        {

                        }
                        if (chat.statistic == null)
                        {

                        }
                        if (chat.statistic != null)
                        {
                            Chat_GetMyStatistic(chat.chatId);
                            UpdateStatusChatInTree(chat);
                            ButtonHaveNewMessages_Update(chat.statistic);
                            //          notifyIcon_ji.BalloonTipText = "Новых сообщений "+ chat.statistic.CountNew.ToString();

                            if (chat.statistic.CountNew < 15)
                            {// стоял в конце списка можно загрузить 
                                try
                                {
                                    //       Thread.Sleep(0300);
                                    var r = GetMessages(chat.chatId, chat.statistic, chat.statistic.CountNew);
                                    AddToEndList(r, (int)msgid);
                                    
                                    //tbl_ChatUserInfo newstat =  
                                    //         job.Chat_UpdateMyStatistic(chat.chatId);
                                    //ShowJob(job, 0, listBox_msg);//, r
                                }
                                catch (Exception err)
                                {

                                }
                            }
                            else
                            {


                                flowLayoutPanel1.Controls.Clear();


                                {

                                    int CurrentLastId = chat.statistic.LastObjId;
                                   
                                    WS_JobInfo.Obj[] r1 = GetMessages(chat.chatId, CurrentLastId, -10); //msg_inChat =

                                    AddToEndList(r1, 0);
                                    WS_JobInfo.Obj[] r = GetMessages(chat.chatId, CurrentLastId, 0); //msg_inChat =
                                    AddToEndList(r, CurrentLastId);
                                }
                            }

                        }

                    }
                    else
                    {
                        Chat_GetMyStatistic(chatid);
                        TreeNodeUpdateCounter((int)chatid);



                    }
                }
                else
                {
                    Chat_GetMyStatistic(chatid);
                    TreeNodeUpdateCounter((int)chatid);


                }*/
            }

            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_DisplayClearOnChatChange(); }
            catch (Exception err) { E(err); }
        }

        private void ShowChatsStatistivInfo(string ChatName, string t)
        {
            //Chat c = job.Chats.Where(s => s.chatId == chatid).FirstOrDefault();
          
            string res = GetChatStatisticString();

            if (b_ShowIncommeMessage)
                ShowInfoBallon(ChatName, t + Environment.NewLine + "-----------------------------" + Environment.NewLine + res);
        }

        private string GetChatStatisticString()
        {
            
                int count_chat = 0;
            int count_msg = 0;
            string res = "";
            foreach (Chat c in job.Chats)
            {
                if (c?.statistic?.CountNew > 0)
                {
                    if (count_chat < 5)
                        res += c.Text + ": " + c.statistic.CountNew.ToString() + " сообщений" + Environment.NewLine;

                    count_chat++;
                    count_msg += c.statistic.CountNew;
                    if (count_chat == 5)
                    {
                        res += "..." + Environment.NewLine;
                    }
                }
            }
            label2.Text = count_chat.ToString();
            label4.Text = count_msg.ToString();
            return res;
        }

        private void OnTokenRecive(Job _job)
        {
            this.BeginInvoke(OnTokenReciveMethod, _job);
            //  throw new NotImplementedException();
        }

        private void OnTokenReciveMethodCall(Job _job)
        {
            try
            {
                //ContentMenu_Devices(job);
                /*if (_job != null && _job.this_device != null)
                    DebugInfo("Recive Token " + _job.this_device.TokenId.ToString());
                */
                //if (debug)
                {
                    label6.Text = _job.this_device.TokenId.ToString();
                    if (b_ShowConnectInfo)
                    ShowInfoBallon("", "Авторизация пройдена успешно...");
                    label5.Text = "Соединён с сервером [" + DateTime.Now.ToLongTimeString()+"]";
                    /*  tokenToolStripMenuItem.Text = _job.this_device.TokenId.ToString();
                      serverConnectedToolStripMenuItem.Text = Setup.URLServer;
                      mashineNameToolStripMenuItem.Text = Setup.MachineName;
                      userLoginToolStripMenuItem.Text = Setup.UserLogin;
                      */
                }
                try
                {
                    _job.UserGetSelf();
                }
                catch (Exception err)
                {

                }

                //toolStripStatusLabel2.Text = "Подключен к серверу";
                if (_job.this_device.TokenId != -1)
                {
                    Load_Personal_Chats(_job);
                    //Tree_Load_Chats_All(_job);
                  //  int selectedchat = job.Chat_GetSelected();
                  //  FindAndSelectInTreeChatById(selectedchat);
                  //  job.Spravka_GetStatus();
                    //         job.GetUsers();
                }

                //       throw new NotImplementedException();
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_DisplayClearOnChatChange(); }
            catch (Exception err) { E(err); }
        }
        private void Load_Personal_Chats(Job _job)
        {
            try
            {
                _job.UpdateChats(0);
                GetChatStatisticString();
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_DisplayClearOnChatChange(); }
            catch (Exception err) { E(err); }
        }
        private void OnDiscnnecedJobServer(Job _job, Exception err)
        {
            //   throw new NotImplementedException();
            this.BeginInvoke(OnDisConnecedWithErrMethod, _job, err);
        }
        private void OnDisConnecedWithErrMethodCall(Job _job, Exception errIn)
        {
            try
            {
                if (b_ShowConnectInfo)
                    ShowInfoBallon("Внимание", "Нет соединения с сервером");
                label6.Text = "";
                label5.Text = "Нет соединения с сервером";
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_DisplayClearOnChatChange(); }
            catch (Exception err) { E(err); }
        }


        private void OnConnecedToJobServerCompleat(Job _job)
        {
        //    this.BeginInvoke(OnConnecedToJobServerMethod, _job);
        }

        private void OnConnected(Job _job)
        {
            this.BeginInvoke(OnConnecedToJobServerMethod, _job);
        }
        private void OnConnecedToJobServerCall(Job _job)
        {
            try
            {
                //DebugInfo("<<<<<<<<<<<<<<<<<<<<<<<<<<< OnConnecedToJobServerCompleat >>>>>>>>>>>>>>>>>>>>>>>>>>>> ");

                if (b_ShowConnectInfo)
                    ShowInfoBallon("Внимание", "Подключен к серверу.Идентификация...");
           
          

                label5.Text = "Соединён с сервером.Идентификация";

                job.ConnectToServer(job.GetServerName(), Environment.MachineName, Environment.UserName);

                //    RequestChats(job);
                foreach (string par in job.Setup_Params)
                {
                    if (par == "RunWinClient=false")
                    {
                        button1.Enabled = false;
                    }
                    if (par == "RunWinClient=true")
                    {
                        button1.Enabled = true;
                    }
                }

            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_DisplayClearOnChatChange(); }
            catch (Exception err) { E(err); }
        }

        private void E(Exception err)
        {
           // throw new NotImplementedException();
        }

        private void E(ChatDisconnectedException err)
        {
           // throw new NotImplementedException();
        }

        private void Chat_DisplayClearOnChatChange()
        {
            //throw new NotImplementedException();
        }

        private void E(ChatWsFunctionException err)
        {
            //throw new NotImplementedException();
        }

        private string GetServer_ChatURL()
        {
            string Prif = "ws://localhost:53847/";

            
              Prif = "ws://jobinfo/xml/";
            Prif = "ws://ghp-sql/xml/";
            //http://jobinfo/xml/xml/GetUserInfo.asmx

            //          Prif = "ws://localhost:53847/";
            Setup.URLServer = Prif;
            return Prif;
        }
      

        private void NotifyIcon_ji_Click(object sender, EventArgs e)
        {
            Show();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = cClose;
            Hide();
            //e.CloseReason=CloseReason.
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            
            {

                Hide();
                ConnectedNow();
            }
         

        }

        private void ПереподключитьсяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectedNow();
        }

        private void ЗапуститьМессенджерToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartMain();
        }

        private void ВыгрузитИзПамятиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cClose = false;
            Close();
        }

        private void NotifyIcon_ji_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            StartMain();
        }

        private void JnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartMain();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    if (File.Exists(@"\\soft\svoddocs$\StopApp\JIcon.txt")) {
                        cClose = false;
                        Close();
                    }
                    if (File.Exists(@"\\soft\svoddocs$\StopApp\JIcon_Reconnect.txt"))
                    {
                        if (job != null)
                        {
                            if (job.StatusConnect == xConnectStatus.b_Created
                                    ||
                                    job.StatusConnect == xConnectStatus.b_Disconnected
                                    )
                            {
                                //string prif = job.GetServerName();
                                //MainJobStart(prif);
                                job.SetupParam_GetList();
                                job?.ReconnectBegin();
                                return;
                            }
                        }

                    }
                }
                catch (Exception err)
                {

                }
                //Проверяем на наличие мутекса в системе
                //Если запущен основной мессенджер, то не показыват сообщения
                using (Mutex m = Mutex.OpenExisting("JobInfo"))
                {
                 //   m.ReleaseMutex();
                    b_ShowIncommeMessage = false;
                }
                
            }
            catch
            {

                b_ShowIncommeMessage = true;
            }
            notifyIcon_ji.Visible = b_ShowIncommeMessage;

            try
            {
                if (job != null)
                {
                    if (job.StatusConnect == xConnectStatus.b_Created
                            ||
                            job.StatusConnect == xConnectStatus.b_Disconnected
                            )
                    {
                        job?.ReconnectBegin();
                       // DebugInfo("Timer Reconect begin");
                       //    ConnectedNow();
                    }
                }
                try
                {
                    if (job != null)
                        job.PingWinSocket();
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message.ToString());
                }
                /*
                if (job!=null)
                {
                    if (job.StatusConnect == xConnectStatus.b_Disconnected)
                    {

                    }
                }*/
            }
            catch
            {

            }

        }

        private void Button10_Click(object sender, EventArgs e)
        {
             
                //      string shortcutName = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.Programs),
                //                "\\", publisher_name,
                //   "\\", "SvodDocs", ".appref-ms");
                //СводИнтернешнл\SvodDocs.appref - ms
                Process.Start("file://soft/svoddocs$/Help/jobinfo/help1.docx");
            //\\soft\svoddocs$\Help\jobinfo
        }

        private void Button2_Click_1(object sender, EventArgs e)
        {
            job.CloseWS();
        }
    }
}
