/*
              }        
            catch  (ChatWsFunctionException err){E(err);}
            catch (ChatDisconnectedException err){E(err);Chat_UnSelect();}
            catch (Exception err) {        E(err);  }

 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using JobInfo.WS_JobInfo;
using JobInfo.XROGi_Class;
//using WebSocketSharp;
using JobInfo.XROGi_Extensions;
using XROGiClassLibrary;
using XWpfControlLibrary;

namespace JobInfo
{
    public enum xEnumUserFiler { xFilterNone, xFilterAll, xFilterOnline, xFilterSubscribe };

    public partial class JI_Form : Form
    {
        string Vers = "--.--.--.--";

        //  https://www.codeproject.com/Articles/134358/TRichTextBox-A-universal-RichTextBox-which-can-dis

        bool debug = true;
        Job job;

        int AutoSelect_ChatId = 0;

        #region NotifyIcon

        int lastDeactivateTick;
        bool lastDeactivateValid;

        public int OnChatAddMessageSource { get; private set; }

        #endregion
        int maxH = 0;


        List<WS_JobInfo.Obj> Objs_Chat = new List<WS_JobInfo.Obj>();
        List<int> list = new List<int>();
        TreeNode tn_Personal;
        TreeNode tn_OU;
        TreeNode tn_user;
        TreeNode tn_hash;
        TreeNode tn_ChatsType7;
        TreeNode tn_ChatsType9;
        TreeNode tn_ChatsType10;

        public delegate void OnDebugInfoDelegate(string Message);
        public event OnDebugInfoDelegate OnDebugInfo = delegate { };

        public delegate void OnTokenReciveDelegate(Job job);
        public event OnTokenReciveDelegate OnTokenReciveMethod = delegate { };

        public delegate void OnMsgReciveDelegate(Job _job, string cmd, long chatid, long msgid);
        public event OnMsgReciveDelegate OnMsgReciveDelegateMethod = delegate { };
        /*
        public delegate void OnConnecedToJobServerDelegate(Job _job, string cmd, long chatid, long msgid);
        public event OnConnecedToJobServerDelegate OnConnecedToJobServerMethod = delegate { };
        */
        public delegate void OnDisConnecedToJobServerDelegate(Job _job);
        public event OnDisConnecedToJobServerDelegate OnDisConnecedToJobServerMethod = delegate { };

        public delegate void OnDisConnecedToJobServerWithErrDelegate(Job _job, Exception err);
        public event OnDisConnecedToJobServerWithErrDelegate OnDisConnecedWithErrMethod = delegate { };

        public delegate void OnConnecedToJobServerDelegate(Job _job);
        public event OnConnecedToJobServerDelegate OnConnecedToJobServerMethod = delegate { };

        public delegate void OnChatUpdateReciveDelegate(Job _job, string cmd, long chatid, long msgid);
        public event OnChatUpdateReciveDelegate OnChatUpdateReciveMethod = delegate { };

        public delegate void OnChatUpdateReciveDelegateI(Job _job, string cmd, long chatid, long msgid);
        public event OnChatUpdateReciveDelegateI OnChatUpdateReciveCallInvoke = delegate { };

        public delegate void OnPingPongDelegate(Job job, DateTime Ping, DateTime Pong);
        public event OnPingPongDelegate OnJob_PingPongCallInvoke = delegate { };

        public delegate void Job_OnBackWorkBeginCall(String message);
        public event Job_OnBackWorkBeginCall Job_OnBackWorkBeginCallInvoke = delegate { };





        public List<Job> jobsArxive = new List<Job>();
        bool b_boss = false;
        bool b_ChatAdmin = false;
        public JI_Form()
        {

            InitializeComponent();
            string guid = Marshal.GetTypeLibGuidForAssembly(Assembly.GetExecutingAssembly()).ToString(); //88e6c12a-7f34-43e9-bcf5-10cb41c174ae
            bool b_FirstRun;
            Mutex mutexObj = new Mutex(true, guid, out b_FirstRun);
            if (b_FirstRun == false)
            {
                //MessageBox.Show("Поддерживается лишь один запуск")
            }
            bool b_DebugFile = false;
            if (File.Exists("c:\\temp\\jidebug.txt"))
            {
                b_DebugFile = true;
            }

            if (Environment.UserName == "iu.smirnov" || b_DebugFile)
            {
                b_boss = true;
                mp.b_boss = b_boss;
                b_ChatAdmin = true;
                if (Control.ModifierKeys == Keys.Shift)
                    DebugForm.Checked = true;
            }
            else
            {
                ToolStripMenuItemSetup.Visible = false;
                tabControl1.TabPages.Remove(tabPage1);
                DeleteMesageToolStripMenuItem.Enabled = false;
            }
            




            if (b_ChatAdmin == true)
            {
                DeleteMesageToolStripMenuItem.Visible = true;
            }
            else
                DeleteMesageToolStripMenuItem.Visible = false;


            Setup.StartInit();
            AutoConnect.Checked = Setup.bAutoConnect;
            toolStripStatusLabel2.Text = "";
            treeView1.NodeMouseClick += (sender, args) => treeView1.SelectedNode = args.Node;
            panel_msg.Controls.Clear();
            panel_msg.AutoScroll = false;
            panel_msg.VerticalScroll.Enabled = true;
            panel_msg.VerticalScroll.Visible = true;
            MainChatName_Cmd.Text = "";
            panel_msg.MouseWheel += MyMouseWheel;
            mp.MouseWheel += Mp_MouseWheel;
            mp.DebugInfoDraw = b_boss;
            mp.OnMouseMessageClick += Mp_OnMouseMessageClick;

            up.onUser_NeedFoto += UserPanel_onUser_NeedFoto;
            up.onUser_NeedUserStatusInfo += onUser_NeedUserStatusInfo;

            up.OnDebugClassEvent += OnDebugClassEvent;
            up.OnUserListUpdate += Up_OnUserListUpdate;
            up.OnMessages_GetCountNotRead += Up_OnMessages_GetCountNotRead;

            mp.OnDebugClassEvent += OnDebugClassEvent;

            if (panel_msg.AutoScrollMargin.Width < 5 || panel_msg.AutoScrollMargin.Height < 5)
            {
                panel_msg.SetAutoScrollMargin(5, 5);
            };

            //for (int i = 0; i < 100; ++i)
            //{
            //    MessageChatControl c = new MessageChatControl();
            //    c.Width = panel4.Width;
            //    c.Height= 40;
            //    c.Text = i.ToString () + " = 12316354564";
            //    c.Top = i * 40;
            //    panel4.Controls.Add(c);
            //    list.Add(i);
            //}
            panel_msg.AutoScroll = true;

            HideTabs(tabControl2);
            this.OnDebugInfo += OnDebugInfoFunction;
            this.OnTokenReciveMethod += OnTokenReciveMethodCall;
            this.OnMsgReciveDelegateMethod += OnMsgReciveDelegateCall;
            this.OnConnecedToJobServerMethod += OnConnecedToJobServerCall;
            this.OnChatUpdateReciveMethod += OnChatUpdateReciveCall;
            this.OnChatUpdateReciveCallInvoke += OnChatUpdateReciveCallInvokeMethod;
            this.OnJob_PingPongCallInvoke += OnJob_PingPongCallInvokeMethod;
            this.Job_OnBackWorkBeginCallInvoke += JI_Form_Job_OnBackWorkBeginCallInvokeMethod;


            //this.OnDisConnecedToJobServerMethod += OnDiscnnecedJobServer;
            this.OnDisConnecedWithErrMethod += OnDisConnecedWithErrMethodCall;
            //dataGridView1.RowCount = list.Count;
            //dataGridView1.CellValueNeeded += GetData;
            dataGridView_Users.Rows.Clear();
            dataGridView_Users.DataSource = null;
            notifyIcon_ji.BalloonTipClicked += OnClickInviteBalloonTip;


            button9.FlatStyle = FlatStyle.Flat;
            button9.FlatAppearance.BorderSize = 0;
            //flowLayoutPanel1.HorizontalScroll.Enabled = false;
            //flowLayoutPanel1.AutoScroll =  true;
            up.UserContextMenuStrip = contextMenu_userchat;

            try
            {
                var ttt = Assembly.GetExecutingAssembly().GetName().Version;
                Vers = ttt.ToString();
            }
            catch (Exception err)
            {

            }
            VersionMenu.Text = "Vers:" + Vers;
            try
            {

                var ctr = (elementHost1.Child as XWpfControlLibrary.UserControl1);
                if (ctr == null)
                    return;
                ctr.KeyDown += ctr_KeyDown;
            }
            catch (Exception)
            {


            }
        }

        private void Up_OnMessages_GetCountNotRead(XUsersCtrl sender, ChatUserPanel userPanel)
        {
            if (userPanel.live.PublicChatId != null)
            {
                Chat c = job.Chats.Where(s => s.chatId == userPanel.live.PublicChatId).FirstOrDefault();
                if (c != null)
                {
                    userPanel.live.statistic = c.statistic;
                }
            }
        }

        private void Up_OnUserListUpdate(XUsersCtrl sender)
        {
            // job.public



            Chat[] all = job.Get_PrivateChats();
            foreach (Chat c in all)
            {
                //                ChatUserPanel
                int self = job.GetMyUserId();
                var userid = c.ObjId.UsersInChat.Where(s => s != self).FirstOrDefault();
                if (userid != null)
                {
                    UserLive ul = sender.Data.Live.Where(s => s.user.UserId == userid).FirstOrDefault();
                    if (ul != null)
                    {
                        ul.PublicChatId = c.chatId;

                    }
                }
            }


        }

        private void OnDebugClassEvent(object sender, string FunctionName, string param_values)
        {
            DebugInfo(FunctionName + " \t " + param_values);

        }

        private void JI_Form_Job_OnBackWorkBeginCallInvokeMethod(string message)
        {
            toolStripStatusLabel2.Text = message;
            toolStripStatusLabel2.Visible = true;
            toolStripStatusLabel2.Invalidate();
        }

        private void onUser_NeedUserStatusInfo(XUsersCtrl sender, int[] users)
        {
            try
            {
                if (users.Count() > 0)
                {
                    var ee = job.User_GetUsersStatus(users);
                    up.AddStatus(ee);
                }

            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }


        }

        private void UserPanel_onUser_NeedFoto(XUsersCtrl sender, ChatUserPanel user)
        {
            //  user.fo
            if (user.b_FotoSkipped == false && job.User_GetFoto(user.u))
            {
                //ok. Ничего недклаю
            }
            else
            {
                user.b_FotoSkipped = true;// сервер фото не давал, больше не грузим
            }


        }

        private void OnJob_PingPongCallInvokeMethod(Job job, DateTime Ping, DateTime Pong)
        {
            try
            {
                int val = Convert.ToInt16((Pong - Ping).TotalMilliseconds);
                if (val > 2 || DebugForm.Checked)
                {
                    PingPongStatusLabel.Text = "Ping=" + val.ToString() + " mc";
                    PingPongStatusLabel.Visible = true;
                }
                else
                    PingPongStatusLabel.Visible = false;
            }
            catch (Exception err)
            {
                E(err);
            }
        }

        private void ShowInfoBallon(string Caption, string _Text)
        {
            string s = Caption;
            //            tn.Text = s+ " (" + c.statistic.CountNew.ToString() + ")";
            string t = _Text;
            notifyIcon_ji.BalloonTipText = _Text;

            //notifyIcon_ji.BalloonTipTitle = "Hi";
            notifyIcon_ji.ShowBalloonTip(0, "JI [" + s + "]", t, ToolTipIcon.Info);
        }
        private void Mp_OnMouseMessageClick(XMessageCtrl sender, ChatPanelElement el, MessageRegion mr)
        {
            try
            {
                if (el.ElementType == ChatPanelElementType.pnlMessageTextRegion)
                {
                    //         MessageChatControl mc =  el.Get_Tag_MessageChatControl();
                    if (sender != null)
                    {
                        if (sender.b_ImageMsg)
                            if (sender.files.Count >= 1)
                            {
                                sender.LoadContainFiles(job, 0);//0 - image  1- icon
                                                                //         Thread.Sleep(10000);
                                pictureBox_image.Image = job.Message_Image_Get((int)sender.MessageObj.ObjId
                                        , sender.files[0].Item3.ToString(), 0); // плохо. нет иконки
                                tabControl2.SelectedTab = tabPage2;
                            }

                        if (mr == MessageRegion.xrLike || mr == MessageRegion.xrDisLike || mr == MessageRegion.xrWorkBtn)
                        {
                            int status = 0;
                            if (mr == MessageRegion.xrLike)
                                status = 45;
                            if (mr == MessageRegion.xrDisLike)
                                status = 46;
                            if (mr == MessageRegion.xrWorkBtn)
                                status = 47;
                            XMessageCtrl mc = el.Get_Tag_MessageChatControl();
                            if (status == 47)
                            {
                                MessageBox.Show("Извините, окно создания задачи из сообщения в разработке.", "Создание задачи из сообщения", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            if (status != 0)
                            {

                                ObjStatus[] ret = job.Message_SetStatus((int)mc.MessageObj.ObjId, status);

                                sender.Set_ObjStatus(ret);
                            }
                        }
                    }
                }
                //if (region == MessageRegion.xrImage)
                //{
                //    if (sender.files.Count >= 1)
                //    {

                //        //                    sender.files[0].Item1  = job.Message_Image_Get((int)sender.MessageObj.ObjId, sender.files[2].ToString(), 0); // плохо. нет иконки


                //    }
                //}
                //if (region == MessageRegion.xrFoto)
                //{
                //    //   MessageChatControl userc = sender as MessageChatControl;
                //    ShowUserInfoParam(sender.user);
                //}
            }
            catch (Exception err)
            {

            }
        }

        private void Mp_MouseWheel(object sender, MouseEventArgs e)
        {

        }

        private void Chat_LoadToControlObj(WS_JobInfo.Obj o)
        {
            if (o != null)
            {
                try
                {
                    Chat_ShowUsersToControls(o);
                    /*          Chat_ShowMessagesToControls(o);
                              // Разрешим отправку
                              button_Send.Enabled = true;*/
                    MainChatName_Cmd.Visible = true;
                    //if (o.ObjId>0)
                    MainChatName_Cmd.Text = o.GetText();
                    Text = "Корпоративный мессенджер [" + MainChatName_Cmd.Text + "]";
                }
                catch (Exception err)
                {

                }
            }
            else
            {
                Text = "Корпоративный мессенджер";
                //       button_Send.Enabled = false;
            }
        }
        private void Chat_LoadToControls(Chat c)
        {
            if (c == null)
            {
                Chat_UnSelect();
                //mp.Chat_UnSelect();
                //panel_users.
                return;
            }
            WS_JobInfo.Obj o = c.ObjId;
            //            return;
            if (o != null)
            {
                try
                {
                    Chat_ShowUsersToControls(c);
                    /*          Chat_ShowMessagesToControls(o);
                              // Разрешим отправку
                              button_Send.Enabled = true;*/
                    MainChatName_Cmd.Visible = true;
                    //if (o.ObjId>0)
                    MainChatName_Cmd.Text = o.GetText();
                    Text = "Корпоративный мессенджер [" + MainChatName_Cmd.Text + "]";
                }
                catch (Exception err)
                {

                }
            }
            else
            {
                Text = "Корпоративный мессенджер";
                //         button_Send.Enabled = false;
            }
        }

        private void onResizeMessagePanel(object sender, EventArgs e)
        {
            //   throw new NotImplementedException();
        }

        private void OnClickInviteBalloonTip(object sender, EventArgs e)
        {
            Application_ShowOnDesctop();
        }

        private void OnDisConnecedWithErrMethodCall(Job _job, Exception errIn)
        {
            try
            {
                DebugInfo("<<<<<<<<<<<<<<<<<<<<<<<<<<< OnDiscnnecedJobServer >>>>>>>>>>>>>>>>>>>>>>>>>>>> ");
                if (errIn != null)
                    DebugObject(errIn);
                if (debug)
                    Text = _job.this_device.TokenId.ToString();

                toolStripStatusLabel2.Text = "Нет соединения с сервером";
                if (ShowStatusConnect.Checked)
                {
                    ShowInfoBallon("Внимание", "Нет соединения с сервером");
                }
                //     listView_chats.Items.Clear();
                listBox_msg.DataSource = null;
                //  GetCurrentChat();
                Chat_Select(0);
                //        button_Send.Enabled = false;
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }
        }

        private void OnDisConnecedToJobServerCall(Job _job)
        {
            Chat_Select(0);
        }

        private void OnDebugInfoFunction(string Message)
        {
            DebugInfo(Message);
        }

        private void HideTabs(TabControl tabCon)
        {
            tabCon.Appearance = TabAppearance.FlatButtons;
            tabCon.ItemSize = new Size(0, 1);
            tabCon.SizeMode = TabSizeMode.Fixed;
        }

        private void MyMouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                panel_msg.ScrollUp(1);
            if (e.Delta > 0)
                panel_msg.ScrollUp(-1);


        }

        private void GetData(object sender, DataGridViewCellValueEventArgs e)
        {
            // If this is the row for new records, no values are needed.
            //   if (e.RowIndex == this.dataGridView1.RowCount - 1) return;


            e.Value = e.RowIndex.ToString();

        }

        int AntiRecurse = 0;
        private void Job_Run(string prif)
        {
            if (prif == "")// неверолятно
                prif = Setup.URLServer;

            if (String.IsNullOrEmpty(prif))
                return;
            toolStripStatusLabel2.Text = "Подключение к серверу..";
            if (job != null)
            {
                //job.ReconnectNow()
                //      job.ReconnectBegin();
                job.Close();
                job.OnConneced -= OnConnecedToJobServerCompleat;
                job.OnDisconneced -= OnDiscnnecedJobServer;
                job.OnTokenRecive -= OnTokenRecive;
                job.OnMsgRecive -= OnMsgRecive;
                job.OnChatListChanged -= OnChatListChanged;

                job.OnJobClassEvent -= ShowJobLogEvents;
                job.OnChatUpdateRecive -= OnChatUpdateRecive;
                job.OnSocketSend -= OnSocketSend;
                job.statusInfo = null;
                job = null;/**/
                mp.Close();
                up.Close();
                /*  */
            }
            if (job == null)
            {
                tn_Personal = treeView1.Nodes["TreeNode_Personal"];
                tn_OU = treeView1.Nodes["TreeNode_OU"];
                tn_user = treeView1.Nodes["TreeNode_Users"];
                tn_hash = treeView1.Nodes["TreeNode_HashTag"];
                tn_ChatsType7 = treeView1.Nodes.Find("tn_ChatsType7", true).FirstOrDefault();
                tn_ChatsType9 = treeView1.Nodes["tn_ChatsType9"];
                tn_ChatsType10 = treeView1.Nodes["tn_ChatsType10"];
                //            tn_ChatsType7 = treeView1.Nodes["tn_ChatsType7"];
                job = new Job(prif);
                jobsArxive.Add(job);
                job.OnConneced += OnConnecedToJobServerCompleat;
                job.OnDisconneced += OnDiscnnecedJobServer;
                job.OnTokenRecive += OnTokenRecive;
                job.OnMsgRecive += OnMsgRecive;
                job.OnChatListChanged += OnChatListChanged;

                job.OnJobClassEvent += ShowJobLogEvents;
                job.OnChatUpdateRecive += OnChatUpdateRecive;
                job.OnSocketSend += OnSocketSend;
                job.OnBackWorkBegin += Job_OnBackWorkBegin;
                //            job.ConnectToServer(Environment.MachineName, Environment.UserName);
                Setup.MachineName = Environment.MachineName;
                Setup.UserLogin = Environment.UserName;
                if (defaultToolStripMenuItem.Checked != true)
                {
                    if (botToolStripMenuItem.Checked)
                    {
                        Setup.UserLogin = "bot";
                    }
                }
                mp.SetJob(job);
                job.OnUser_GetListAllAsync += Job_OnUser_GetListAllAsync;


                job.OnConneced += mp.On_Job_Conneced;
                job.OnTokenRecive += mp.On_Job_TokenRecive;
                job.OnDisconneced += mp.On_Job_Disconneced;

                job.OnChatUpdateRecive += mp.On_Job_ChatUpdateRecive;
                job.OnChatEvent += ForTree_OnChatEvent;
                job.onMessage_GetListID += mp.onMessage_GetListIDMethod;
                job.Message_ReciveListObjId += mp.Message_ReciveListObjIdMethod;
                job.OnPingPong += OnJob_PingPong;

                //
                job.ConnectToServer(Setup.MachineName, Setup.UserLogin);

                string returl = job.GetServerName();
                if (returl != prif && AntiRecurse != 1)
                {
                    Setup.URLServer = returl;
                    try
                    {
                        AntiRecurse = 1;
                        Job_Run(Setup.URLServer);
                    }
                    catch (Exception err)
                    {

                    }
                }
                AntiRecurse = 0;
            }


            //        JobLastShow last = job.Job_GetMsgLast();
            //            int nScrollMsgCount = 2;


        }

        private void Job_Message_ReciveListObjId(asyncReturn_Messages ret)
        {
            throw new NotImplementedException();
        }

        private void Job_OnBackWorkBegin(string Message)
        {
            try
            {
                this.BeginInvoke(Job_OnBackWorkBeginCallInvoke, Message);
            }
            catch (Exception err)
            {

            };
        }

        private void OnJob_PingPong(Job job, DateTime Ping, DateTime Pong)
        {
            try
            {
                this.BeginInvoke(OnJob_PingPongCallInvoke, job, Ping, Pong);
            }
            catch (Exception err)
            {

            };
        }

        private void Job_OnUser_GetListAllAsync(object sender, User_GetListAllCompletedEventArgs e)
        {
            try
            {
                up.SetUsers(e.Result);
                FilterUser();
            }
            catch (Exception err)
            {

            };
        }

        private void ForTree_OnChatEvent(Chat chat, ChatEventType et, object Tag)
        {
            try
            {
                if (et == ChatEventType.onChatStatisticChanged)
                {
                    JobInfo.WS_JobInfo.UserChatInfo obj = Tag as JobInfo.WS_JobInfo.UserChatInfo;
                    up.UpdateStatisticChat(chat, obj);
                    if (Tree_Update_ChatTextDataOnly(chat.chatId))
                    {
                        treeView1.Refresh();
                    }

                }
                else
                {

                }
            }
            catch (Exception err)
            {

            };
        }

        private bool Tree_Update_ChatTextDataOnly(long chatid)
        {
            bool b_changed = false;
            TreeNode[] nodes = treeView1.Nodes.Find("chat_" + chatid.ToString(), true);
            foreach (TreeNode tn in nodes)
            {
                if (tn.Tag != null)
                    if ((tn.Tag as Chat).ObjId.ObjId != 0)
                    {
                        //                job.UpdateChat((int)chatid);


                        XROGi_Class.Chat c = job.Chats.Where(s => s.ObjId.ObjId == chatid).FirstOrDefault();
                        //        mp.Chat_Select(c);
                        if (c != null)
                        {
                            string text = c.Text;
                            if (c?.statistic?.CountNew > 0)
                            {
                                if (!text.Contains(")"))
                                {
                                    text += (c.statistic == null ? "" : " \t(" + c.statistic?.CountNew.ToString() + ")").ToString();
                                }
                                else
                                {

                                }
                            }
                            if (text != tn.Text)
                            {
                                tn.Text = text;
                                b_changed = true;
                            }
                        }
                        else
                        { // вышел из чата или выкинули.
                          //20190314                           tn.Remove();
                          //           tn.Tag = null;
                        }




                    }

            }
            return b_changed;

        }
        private void OnSocketSend(Exception err, string data)
        {
            try
            {
                if (data != "")
                    DebugInfo(data);
            }
            catch (Exception err33)
            {

            };
        }

        private void OnChatUpdateReciveCall(Job _job, string cmd, long chatid, long msgid)
        {
            try
            {
                this.BeginInvoke(OnChatUpdateReciveCallInvoke, _job, cmd, chatid, msgid);
            }
            catch (Exception err)
            {

            };

        }
        private void OnChatUpdateReciveCallInvokeMethod(Job _job, string cmd, long chatid, long msgid)
        {
            try
            {
                DebugInfo("OnChatUpdateRecive " + cmd + " " + chatid + " " + msgid.ToString());

                if (cmd == "CHATSTATUSUPDATE")
                {
                    //        Chat c = _job.Chats.Where(s => s.chatId == chatid).FirstOrDefault();
                    //         c?.GetLastStatistic();
                }

                if (cmd == "CHATADD")
                {



                    Chat[] cl = _job.GetChatList_FromCache();
                    TreeView_Load_Personal_Chats(cl);
                    /**/
                    //        if (cmd == "CHATADD")
                    {

                        Chat[] cc = cl.Where(s => s.chatId == chatid).ToArray();
                        if (cc.Length > 1)
                        {

                        }
                        if (cc.Length == 1)
                        {

                            Chat c = cc[0];
                            ShowInfoBallon("Информация", "Вы добавлены в чат [" + c.Text + "]");
                            mp.Chat_Select(ref c);
                            Chat_LoadToControls(c);
                            Tree_Update_ChatTextData(c.chatId);
                            FilterUser();
                            TreeNode[] nodes = treeView1.Nodes.Find("chat_" + chatid.ToString(), true);
                            foreach (TreeNode tn in nodes)
                            {
                                treeView1.SelectedNode = tn;
                                mp.Show_Message(c.chatId, c.statistic.LastObjId);
                                mp.Refresh();
                                break;
                            }
                            if (AutoSelect_ChatId == chatid)
                            {
                                SelectChat(c);
                                AutoSelect_ChatId = 0;
                                BackToChatTable();
                            }
                        }
                    }
                }

                if (cmd == "CHATLEAVE")
                {
                    try
                    {
                        mp.Chat_UnSelect();
                        _job.Chats.Remove(_job.Chats.Where(s => s.chatId == chatid).FirstOrDefault());
                        Chat[] cl = _job.GetChatList_FromCache();
                        {
                            ShowInfoBallon("Информация", "Вы вышли из чата");
                            TreeView_Load_Personal_Chats(cl);//.Where(s => s.chatId != chatid).ToArray()
                        }
                        Chat_UnSelect();
                        FilterUser();
                        //mp.Chat_UnSelect();
                    }
                    catch (Exception err)
                    {

                    }
                }
                if (cmd == "CHAT")
                {
                    //ситуация переименование
                    _job.Chats.Remove(_job.Chats.Where(s => s.chatId == chatid).FirstOrDefault());
                    _job.RequestChat((int)chatid);

                    Chat[] cl = _job.GetChatList_FromCache();
                    if (cl != null)
                    {
                        TreeView_Load_Personal_Chats(cl);
                    }
                }
                if (cmd == "CHATRENAME")
                {
                    //ситуация переименование
                    _job.Chats.Remove(_job.Chats.Where(s => s.chatId == chatid).FirstOrDefault());
                    _job.RequestChat((int)chatid);

                    Chat[] cl = _job.GetChatList_FromCache();
                    if (cl != null)
                    {
                        ShowInfoBallon("Информация", "Чат был переимнован");
                        TreeView_Load_Personal_Chats(cl);
                    }
                }
                treeView1.Refresh();//?
            }
            catch (Exception err)
            {

            }
        }

        private bool Tree_Update_ChatTextData(long chatid)
        {
            bool b_changed = false;
            TreeNode[] nodes = treeView1.Nodes.Find("chat_" + chatid.ToString(), true);
            foreach (TreeNode tn in nodes)
            {
                if (tn.Tag != null)
                    if ((tn.Tag as Chat).ObjId.ObjId != 0)
                    {
                        job.UpdateChat((int)chatid);


                        XROGi_Class.Chat c = job.Chats.Where(s => s.ObjId.ObjId == chatid).FirstOrDefault();
                        mp.Chat_Select(ref c);
                        if (c != null)
                        {
                            string text = c.Text;
                            if (c?.statistic?.CountNew > 0)
                            {
                                if (!text.Contains(")"))
                                {
                                    text += (c.statistic == null ? "" : " \t(" + c.statistic?.CountNew.ToString() + ")").ToString();
                                }
                                else
                                {

                                }
                            }
                            if (text != tn.Text)
                            {
                                tn.Text = text;
                                b_changed = true;
                            }
                        }
                        else
                        { // вышел из чата или выкинули.
                            tn.Remove();
                            tn.Tag = null;
                        }




                    }

            }
            return b_changed;

        }

        private void OnChatUpdateRecive(Job _job, string cmd, long chatid, long msgid)
        {
            try
            {
                this.BeginInvoke(OnChatUpdateReciveMethod, _job, cmd, chatid, msgid);
            }
            catch (Exception err)
            {

            };

        }

        List<String> ListLogs = new List<string>();
        private void ShowJobLogEvents(Job _job, string FunctionName)
        {
            try
            {
                DebugInfo(FunctionName);
            }
            catch (Exception err)
            {

            }
        }

        private void ddddd(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            notifyIcon_ji.BalloonTipText = "Запущен JobInfo";
            //notifyIcon_ji.BalloonTipTitle = "Hi";
            notifyIcon_ji.ShowBalloonTip(1000);


            Setup.URLServer = GetServer_ChatURL(); ; // "http://localhost:53847/"
            try
            {
                Job_Run(Setup.URLServer);
            }catch (Exception err )
            {
                E(err);

            }
            //   WindowState = FormWindowState.Minimized;
        }

        DrawItemEventHandler eh = null;
        MeasureItemEventHandler mi = null;
        private void ShowJob(Job _job, long Chatid, ListBox lb)//, WS_JobInfo.Obj [] msgs
        {
            #region FormMsg Delegate

            if (mi == null)
            {
                mi = new MeasureItemEventHandler(listBox1_MeasureItem);
            }
            else
            {
                lb.MeasureItem -= mi;
                mi = new MeasureItemEventHandler(listBox1_MeasureItem);
            }
            lb.MeasureItem += mi;

            if (eh == null)
                eh = new DrawItemEventHandler(listBox1_DrawItem2);
            else
            {
                lb.DrawItem -= eh;
                eh = new DrawItemEventHandler(listBox1_DrawItem2);
            }
            lb.DrawItem += eh;
            #endregion
        }

        private XMessageCtrl AddToStartList(WS_JobInfo.Obj[] obj, int selectes = 0)
        {
            int i = 0;
            if (obj == null)
                return null;
            XMessageCtrl FirsSelected = null;
            foreach (WS_JobInfo.Obj o in obj.OrderByDescending(s => s.ObjId))
            {
                if (selectes > 0)
                    i++;

                try
                {

                    //        if (o.ObjId <= job.LastMessageAdd)
                    {
                        //          continue;
                    }

                    XMessageCtrl c = new XMessageCtrl();

                    Set_MessageChatToPanel(c, o);

                    /*
                    c.Width = flowLayoutPanel1.Width - 30;//; c.Data_Width;
                    c.Calculate_Size(o, c.Width);
                    c.Text = o.GetText() ;
                    if (Environment.UserName == "iu.smirnov" && Control.ModifierKeys == Keys.Shift)
                        c.Text += " id=" + o.ObjId.ToString();

                    //       c.Top = maxH;
                    //c.Top;
                    c.Height = c.Data_Height;
                    //           maxH += c.Height + 25 + 5;
                    c.ContextMenuStrip = context_msg;
                    c.OnMessageShownEvent += OnMessageShownEvent;
                    c.OnClickMessageRegionEvent += OnClickMessageRegionEvent;
                   
                    //    panel_msg.Controls.Add(c);
                    flowLayoutPanel1.Controls.Add(c);
                    flowLayoutPanel1.Controls.SetChildIndex(c, 0);
                    c.Tag = o;
                    if (o.userid== job.GetMyUserId())
                            c.Position = MessagePosition.xrMyMessage;
                    //if (c.Top>)
                    if (i>0&& i== selectes)
                    {
             //          flowLayoutPanel1.ScrollControlIntoView(c);
                    }
                    job.LastMessageAdd = o.ObjId;

                    **/


                }
                catch (Exception err)
                {

                    E(err);
                }
            }
            return FirsSelected;
        }

        private void Set_MessageChatToPanel(XMessageCtrl c, WS_JobInfo.Obj o)
        {
            //     c.Width = flowLayoutPanel1.Width - 30;//; c.Data_Width;
            c.Text = o.GetText();
            if (Environment.UserName == "iu.smirnov" && (Control.ModifierKeys == Keys.Shift || idcheck.Checked))
                c.Text += " id=" + o.ObjId.ToString();
            if (o.userid == job.GetMyUserId())
                c.Position = MessagePosition.xrMyMessage;
            c.SetWidth(c.Width);
            c.MessageObj = o;
            c.GenBitmap_HTML();
            c.ContextMenuStrip = context_msg;
            c.Height = c.Data_Height;
            c.ContextMenuStrip = context_msg;
            c.OnMessageShownEvent += OnMessageShownEvent;
            c.OnClickMessageRegionEvent += OnClickMessageRegionEvent;
            c.LoadContainFiles(job, 1);
            if (o.isImage())
            {
                string[] file = o.GetFiles();
                if (file != null)
                {
                    foreach (string guid in file)
                    {
                        if (guid.Trim() != "")
                        {
                            Image img = null; //job.Message_Image_Get((int)o.ObjId, guid,0); // плохо. нет иконки
                            Image imgicon = job.Message_Image_Get((int)o.ObjId, guid, 1);// icon
                            if (imgicon == null)
                            {
                                imgicon = job.Message_Image_Get((int)o.ObjId, guid, 0); // плохо. нет иконки
                            }
                            else
                            {

                            }

                            if (img != null || null != imgicon)
                            {
                                c.files.Add(new Tuple<Image, Image, string>(img, imgicon, guid));
                            }
                            else
                            {
                                //вообще нет ничего
                            }

                        }
                    }
                }
            }
            c.Tag = o;
            c.user = GetUser(o.userid);
        }

        private void OnClickMessageRegionEvent(XMessageCtrl sender, MouseEventArgs e, MessageRegion region)
        {
            if (region == MessageRegion.xrImage)
            {
                if (sender.files.Count >= 1)
                {

                    //                    sender.files[0].Item1  = job.Message_Image_Get((int)sender.MessageObj.ObjId, sender.files[2].ToString(), 0); // плохо. нет иконки

                    pictureBox_image.Image = job.Message_Image_Get((int)sender.MessageObj.ObjId, sender.files[0].Item3.ToString(), 0); // плохо. нет иконки
                    tabControl2.SelectedTab = tabPage2;
                }
            }
            if (region == MessageRegion.xrFoto)
            {
                //   MessageChatControl userc = sender as MessageChatControl;
                ShowUserInfoParam(sender.user);
            }
        }

        private void AddToEndList(WS_JobInfo.Obj[] obj, int ObjId_SelectToShow)
        {
            //           flowLayoutPanel1.VerticalScroll.SmallChange = 35;
            if (obj == null)
                return;
            int num = 0;
            XMessageCtrl c_last = null;
            foreach (WS_JobInfo.Obj o in obj.OrderBy(s => s.ObjId))
            {
                try
                {
                    num++; // передвинем только на первое сообщение

                    //        if (o.ObjId <= job.LastMessageAdd)
                    {
                        //          continue;
                    }

                    XMessageCtrl c = new XMessageCtrl();
                    //      c.ImageListMsg = ImageListMsg;
                    Set_MessageChatToPanel(c, o);
                    if (c_last == null)
                    {
                        c_last = c;
                    }
                    /*
                    c.Width = flowLayoutPanel1.Width - 30;//; c.Data_Width;

                    c.Text = o.GetText() ;
                    if (Environment.UserName=="iu.smirnov" && Control.ModifierKeys == Keys.Shift)
                        c.Text +=  " id=" + o.ObjId.ToString();


                    c.Calculate_Size(o, c.Width);
                    c.MessageObj = o;
                    //       c.Top = maxH;
                    //c.Top;
                    c.Height = c.Data_Height;
                    //           maxH += c.Height + 25 + 5;
                    c.ContextMenuStrip = context_msg;
                    c.OnMessageShownEvent += OnMessageShownEvent;
                    c.OnClickMessageRegionEvent += OnClickMessageRegionEvent;
                    //    panel_msg.Controls.Add(c);
                    if (o.isImage())
                    {
                        string[] file = o.GetFiles();
                        if (file != null)
                        {
                            foreach (string guid in file)
                            {
                                if (guid.Trim() != "")
                                {
                                    Image img = null; //job.Message_Image_Get((int)o.ObjId, guid,0); // плохо. нет иконки
                                    Image imgicon = job.Message_Image_Get((int)o.ObjId, guid, 1);// icon
                                    if (imgicon == null)
                                    {
                                        imgicon  = job.Message_Image_Get((int)o.ObjId, guid, 0); // плохо. нет иконки
                                    }
                                    else
                                    {

                                    }
                                    
                                    if (img != null || null != imgicon)
                                    {
                                        c.files.Add(new Tuple<Image, Image, string>(img, imgicon, guid));
                                    }
                                    else
                                    {
                                       //вообще нет ничего
                                    }
                                   
                                }
                            }
                        }
                    }
               //     if (c_last==null)
                    {
                        c_last = c;
                    }
                    c.Tag = o;
                    if (o.userid == job.GetMyUserId())
                        c.Position = MessagePosition.xrMyMessage;

                    c.user = GetUser(o.userid);
                    //if (c.Top>)
                    {

                        
                    }
                    job.LastMessageAdd = o.ObjId;



                    */
                    //          if (num==1)
                    //              flowLayoutPanel1.ScrollControlIntoView(c);
                    //flowLayoutPanel1.AutoScrollMinSize = new Size(150, 150);

                    //            if (c_last != null)
                    //      flowLayoutPanel1.Controls.Add(c);


                }



                catch (Exception err)
                {
                    E(err);
                }


            }




        }



        private WS_JobInfo.User GetUser(int userid)
        {
            try
            {
                WS_JobInfo.User ret = job.users.Where(s => s.UserId == userid).FirstOrDefault();
                if (ret == null)
                {
                    WS_JobInfo.User user = job.GetUser(userid);

                    if (user != null)
                        job.users.Add(user);
                    return ret;
                }

                return ret;
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }
            return null;

        }

        private void OnMessageShownEvent(object sender)
        {
            try
            {
                /*Фиксируем на сайте факт просмотра*/
                XMessageCtrl c = sender as XMessageCtrl;
                WS_JobInfo.Obj o = c.Tag as WS_JobInfo.Obj;

                Chat chat = GetCurrentChatObj();
                if (chat == null)
                {
                    //?????
                    job.UpdateChats(0);
                    chat = GetCurrentChatObj();
                }

                WS_JobInfo.Obj ochat = GetCurrentTreeChatObj();
                long chatid = ochat.ObjId;

                DebugInfo("MsgShown " + chatid.ToString() + " => " + o.ObjId.ToString());

                if (chat?.statistic?.LastShownObjId < o.ObjId)
                {
                    job.Message_Shown(chatid, o.ObjId);
                    UpdateChatAfterMessageShown(chat);
                }
                else
                {

                }







            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }


        }

        private void UpdateChatAfterMessageShown(Chat chat)
        {
            int chatid = chat.chatId;
            Chat_GetMyStatistic(chatid);// обновить статистику

            if (chat != null && chat.statistic != null)
            {
                UpdateStatusChatInTree(chat);
                ButtonHaveNewMessages_Update(chat.statistic);
                int beforeshow = chat.statistic.LastShownObjId;


                int countnew = chat.statistic.CountNew;
            }
            else
            {
                ButtonHaveNewMessages_Hide();
            }

            /*
            if (false && chat?.statistic?.LastShownObjId < o.ObjId)
            {

                job.Message_Shown(chatid, o.ObjId);
                Chat_GetMyStatistic(chat.chatId);// обновить статистику
                int aftershow = chat.statistic.LastShownObjId;

                int count = job.MessgesList.Where(s => s.ObjId > o.ObjId).Count();
                if (count >= 1)
                {
                    // LoadNextMessage(chat,);

                    if (chat.statistic != null)
                    {
                        var r = job.GetMessages(chat.chatId, chat.statistic, 10);
                        //tbl_ChatUserInfo newstat =  
                        Chat_GetMyStatistic(chat.chatId);
                        ShowJob(job, 0, listBox_msg); //, r
                    }
                }
            }*/
        }

        private void ButtonHaveNewMessages_Update(UserChatInfo statistic)
        {

            if (statistic.CountNew > 0)
            {
                ButtonHaveNewMessages_Show(statistic.CountNew);

            }
            else
            {
                ButtonHaveNewMessages_Hide();
            }
        }

        public static void ExecuteIn(int milliseconds, Action action
            , UserChatInfo statistic
            )
        {
            var timer = new System.Windows.Forms.Timer();
            timer.Tick += (s, e) => { action(); timer.Stop(); };
            timer.Interval = milliseconds;
            timer.Start();
        }

        List<Tuple<TreeNode, string>> treeUpateTimer = new List<Tuple<TreeNode, string>>();

        private void UpdateStatusChatInTree(Chat chat)
        {

            //    ExecuteIn(1000, () =>
            {

                var stopwatch = new Stopwatch();

                stopwatch.Start();
                timer_UpdateTreeNodeAfterChange.Stop();
                //  treeView1.BeginUpdate();
                TreeNode[] res = treeView1.Nodes.Find("chat_" + chat.statistic.ChatId, true);
                //     Chat c = job.Chats.Where(s => s.chatId == chat.statistic.ChatId).FirstOrDefault();
                //     if (c != null)
                {

                    foreach (TreeNode tn in res)
                    {
                        string s = chat.Text;
                        if (chat.statistic?.CountNew > 0)
                        {
                            s += " \t(" + chat.statistic.CountNew.ToString() + ")";
                        }

                        for (int i = treeUpateTimer.Count - 1; i >= 0; i--)
                        {
                            if ((((Tuple<TreeNode, string>)treeUpateTimer[i]).Item1) == tn)
                                if ((((Tuple<TreeNode, string>)treeUpateTimer[i]).Item2) != s)
                                    treeUpateTimer.RemoveAt(i);
                        }
                        /*
                        foreach (Tuple<TreeNode, string> iit in treeUpateTimer)
                        {
                            treeUpateTimer.Remove(iit);
                        }*/

                        //                        treeUpateTimer.Remove(sq =>  (TreeNode)((Tuple<TreeNode, string>)sq).Item1) == tn );  //&& ((Tuple<TreeNode, string>)sq).Item2!=s

                        if (s != tn.Text

                            )
                        {
                            treeUpateTimer.Add(new Tuple<TreeNode, string>(tn, s));
                        }

                        if (treeUpateTimer.Count > 0)
                            timer_UpdateTreeNodeAfterChange.Start();

                        //                  
                    }
                }
                stopwatch.Stop();
                var elapsed_time = stopwatch.ElapsedMilliseconds;
                if (elapsed_time != 0)
                {

                }
            }

            //,statistic
            //);

            ;
            //  treeView1.EndUpdate();
        }

        private void ButtonHaveNewMessages_Hide()
        {
            ButtonHaveNewMessages.Visible = false;
            //          button9.Visible = false;
        }

        private void ButtonHaveNewMessages_Show(int countNew)
        {
            ButtonHaveNewMessages.Visible = true;
            ButtonHaveNewMessages.Text = countNew.ToString();
            button9.Visible = true;
        }

        private void listBox1_DrawItem2(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;
            Obj o = listBox_msg.Items[e.Index] as Obj;
            long chatid = GetCurrentChatId();
            if (o.Tag.links != null && o.Tag.links.Where(s => s.objid_to == chatid).Any())
            {
                e.Graphics.DrawMessage_SourceChat(o.Tag, e.Bounds, e.State);
            }
            else
                e.Graphics.DrawMessage(o.Tag, new Rectangle((int)e.Graphics.VisibleClipBounds.X,
                    (int)e.Bounds.Y,
                    (int)e.Graphics.VisibleClipBounds.Size.Width, (int)e.Graphics.VisibleClipBounds.Size.Height
                    ), e.State, imageList_smile);

            job.Message_Shown(chatid, o.id);
            return;/**/
        }

        private void OnMessageShow(Job job, Obj o)
        {
            job.Job_SetMsgLast(o);// ** хотелось бы последовательные ID, а так будут пропуски
        }

        private void listBox1_MeasureItem(object sender, MeasureItemEventArgs e)
        {

            // Cast the sender object back to ListBox type.

            ListBox theListBox = (ListBox)sender;


            var o = listBox_msg.Items[e.Index] as Obj;

            //temWidth  // e.ItemHeight
            Size s = o.Tag.MeasureDrawObj((int)e.Graphics.VisibleClipBounds.Width, (int)e.Graphics.VisibleClipBounds.Height);
            e.ItemHeight = s.Height + 35;
            e.ItemWidth = s.Width;
            return;
        }

        private void ContentMenu_Devices(Job _job)
        {
            Device_MenuItems.DropDownItems.Clear();
            // https://habr.com/post/145077/

            #region Регистрируем вход

            {
                ToolStripSeparator sub = new ToolStripSeparator();
                Device_MenuItems.DropDownItems.Add(sub);
                sub.Tag = null;

                var MobileAdd = new ToolStripMenuItem("Запросы мобильных устройств");

                Device_MenuItems.DropDownItems.Add(MobileAdd);
                MobileAdd.Tag = null;
            }

            #endregion



        }

        private void sub_ComputerTokenDialog(object sender, EventArgs e)
        {
            ToolStripMenuItem it = sender as ToolStripMenuItem;
            if (it.Checked == true)
            {
                if (MessageBox.Show("Вы уверены что хотить аннулировать Токен для устройства?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Device d = it.Tag as Device;
                    if (d != null)
                    {
                        job.Close();
                        job.UnregisterComputer(d);
                        {
                            it.Checked = false;
                        }
                    }
                }

            }
            else
            {
                MainConnect();

            }
        }

        private void MainConnect()
        {
            try
            {
                string Prif = GetServer_ChatURL();
                Job_Run(Setup.URLServer);
            }
            catch (Exception err)
            {
                E(err);
            }


        }

        private string GetServer_ChatURL()
        {
      //      if (автоToolStripMenuItem.Checked==false)
            {
                string Prif = "ws://localhost:53847/";

                if (jobInfoToolStripMenuItem.Checked)
                    Prif = "ws://jobinfo/xml/";
                //http://jobinfo/xml/xml/GetUserInfo.asmx
                if (lockalToolStripMenuItem.Checked)
                    Prif = "ws://localhost:53847/";
                if (ghpsqlToolStripMenuItem.Checked)
                    Prif = "ws://ghp-sql/xml/";

                Setup.URLServer = Prif;
                return Prif;
            }
            //else             return "";
        }

        private void CallTheService(string url)
        {

            /*WS_JobInfo.TheServiceClient client = new TheService.TheServiceClient();
            client.Endpoint.Address = new System.ServiceModel.EndpointAddress(url);
            var results = client.AMethodFromTheService();
            */
        }
        private void onConnectToJob()
        {
            //job.LoadObjects();
        }

        public void showForm(bool show)
        {
            if (show)
            {
                Show();
                Activate();
                WindowState = FormWindowState.Normal;
            }
            else
            {
                Hide();
                WindowState = FormWindowState.Minimized;
            }
        }

        private void notifyIcon_ji_MouseClick(object sender, MouseEventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            {
                Application_ShowOnDesctop();

            }

            /*
                        if (lastDeactivateValid && Environment.TickCount - lastDeactivateTick < 1000) return;
                        this.Show();
                        this.Activate();
                        */
        }

        private void Application_ShowOnDesctop()
        {
            this.TopMost = true;
            Show();
            Visible = true;
            this.TopMost = false;
            WindowState = FormWindowState.Normal;
            Activate();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)


            {

                this.TopMost = false;
                Hide();


            }
        }

        private void notifyIcon_ji_DoubleClick(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
            this.TopMost = true;
            Show(); this.TopMost = false;
        }

        private void toolStripSplitButton1_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            StripSplitButton_status.Text = e.ClickedItem.Text;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // OnDraw Реаьне

            //ListBox theListBox = (ListBox)sender;
            //if (theListBox.SelectedIndex != -1)
            //{
            //    var o = listBox1.Items[theListBox.SelectedIndex] as Obj;
            //    job.Job_SetMsgLast(o.id);// ** хотелось бы последовательные ID, а так будут пропуски
            //}
        }

        private void context_msg_Opening(object sender, CancelEventArgs e)
        {
            try
            {
                XMessageCtrl m = context_msg.SourceControl as XMessageCtrl;
                //m.MessageObj.links

                //переделать через for
                StripMenuItem_SendToChat.DropDownItems.Clear();
                //StripMenuItem_SendToChat.DropDownItems.AddRange(Objs_Chat.Select(s => new ToolStripMenuItem(s.temp_string.Trim().Substring(0, s.temp_string.Trim().Length > 20 ? 15 : s.temp_string.Trim().Length)) ).ToArray() );

                ToolStripMenuItem_LinkedChats.DropDownItems.Clear();
                WS_JobInfo.Obj o = m.MessageObj as WS_JobInfo.Obj;
                if (o == null)
                    return;
                int[] ids = o.links.Where(s => s.sgLinkTypeId == 18).Select(s => s.objid_to).ToArray();

                var ttttt = Objs_Chat.Where(s => ids.Contains((int)s.ObjId)).ToArray();

                //.temp_string.Trim().Substring(0, s.temp_string.Trim().Length > 20 ? 15 : s.temp_string.Trim().Length

                StripMenuItem_SendToChat.DropDownItems.AddRange(ttttt.Select(s => new ToolStripMenuItem(s.GetText())).ToArray());
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }

        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && (sender as ListBox).SelectedIndex > 0)
            {
                (sender as ListBox).SelectedIndex = (sender as ListBox).IndexFromPoint(e.X, e.Y);

                ListBox theListBox = (ListBox)sender;
                Obj o = listBox_msg.Items[theListBox.SelectedIndex] as Obj;
                if (o.type == MsgType.msg)
                {
                    theListBox.ContextMenuStrip = context_msg;
                }
                else
                if (o.type == MsgType.chat)
                {
                    theListBox.ContextMenuStrip = contextMenu_chat;
                }
                else
                    theListBox.ContextMenuStrip = null;
            }

        }

        private void информацияОСообщенииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XMessageCtrl mc = context_msg.SourceControl as XMessageCtrl;
            FormInfoMsg f = new FormInfoMsg();
            f.Set_FormParam(job, mc);
            f.ShowDialog();
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {

        }

        private void создатьЧатToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Obj o = listBox_msg.SelectedItem as Obj;
                if (o == null) return
                          ;
                FormEnterText f = new FormEnterText();

                f.SetDesktopLocation(Cursor.Position.X - 5, Cursor.Position.Y - 29);
                //f.Top  = Cursor.Position.X;
                //f.Left = Cursor.Position.Y;
                if (f.ShowDialog() == DialogResult.OK)
                {
                    Obj _o = new Obj();
                    _o.type = MsgType.chat;

                    job.Chat_Add(o.id, f.GetSgnTypeId(), f.GetText(), f.GetComment());


                }

                // _o.Add
                //Job j = new Job("");
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }

        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            try
            {
                Chat c = GetCurrentChatObj();
                job.Job_Leave(c.ObjId.ObjId);
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }

        }

        private void button1_Paint(object sender, PaintEventArgs e)
        {//http://qaru.site/questions/530238/form-with-rounded-borders-in-c
            System.Drawing.Drawing2D.GraphicsPath buttonPath =
       new System.Drawing.Drawing2D.GraphicsPath();

            // Set a new rectangle to the same size as the button 
            // ClientRectangle property.
            System.Drawing.Rectangle newRectangle = button1.ClientRectangle;

            // Decrease the size of the rectangle.
            newRectangle.Inflate(-3, -3);

            // Draw the button border.
            e.Graphics.DrawEllipse(System.Drawing.Pens.Black, newRectangle);

            // Increase the size of the rectangle to include the border.
            newRectangle.Inflate(1, 1);

            // Create a circle within the new rectangle.
            buttonPath.AddEllipse(newRectangle);

            // Set the button Region property to the newly created 
            // circle region.
            button1.Region = new System.Drawing.Region(buttonPath);
        }

        private void button2_Paint(object sender, PaintEventArgs e)
        {
            Rectangle Bounds = new Rectangle(0, 0, button2.Width, button2.Height);
            int CornerRadius = 10;
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(Bounds.X, Bounds.Y, CornerRadius, CornerRadius, 180, 90);
            path.AddArc(Bounds.X + Bounds.Width - CornerRadius, Bounds.Y, CornerRadius, CornerRadius, 270, 90);
            path.AddArc(Bounds.X + Bounds.Width - CornerRadius, Bounds.Y + Bounds.Height - CornerRadius, CornerRadius, CornerRadius, 0, 90);
            path.AddArc(Bounds.X, Bounds.Y + Bounds.Height - CornerRadius, CornerRadius, CornerRadius, 90, 90);
            path.CloseAllFigures();
            e.Graphics.FillPath(System.Drawing.Brushes.Yellow, path);
            e.Graphics.DrawPath(System.Drawing.Pens.Black, path);


            button2.Region = new Region(path);

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void Device_MenuItems_Click(object sender, EventArgs e)
        {

        }

        private void получитьСписокПользователейToolStripMenuItem_Click(object sender, EventArgs e)
        {
            job.GetUsers();

        }

        private void lockalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                lockalToolStripMenuItem.Checked = true;
                jobInfoToolStripMenuItem.Checked = false;
                DisconnectToolStripMenuItem.Checked = false;
                ghpsqlToolStripMenuItem.Checked = false;
                MainConnect();
            }
            catch (Exception err)
            {
                E(err);
            }
        }

        private void jobInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lockalToolStripMenuItem.Checked = false;
            jobInfoToolStripMenuItem.Checked = true;
            DisconnectToolStripMenuItem.Checked = false;
            ghpsqlToolStripMenuItem.Checked = false;
            MainConnect();
        }


        private void OnMsgReciveDelegateCall(Job _job, string cmd, long chatid, long msgid)
        {
            DateTime db = DateTime.Now;
            try
            {
                DebugInfo("OnMsgRecive " + cmd + " " + chatid + " " + msgid.ToString());
                //             return;
                XROGi_Class.Chat chat = GetCurrentChatObj();

                mp.Show_Message((int)chatid, (int)msgid);
                //        return;
                if (chat != null)
                {



                    int currentchatid = chat.chatId;
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
                            //     Chat_GetMyStatistic(chat.chatId);
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
                                    /*if (r.Length==1)
                                    if (msgid==r[0].ObjId)
                                        {
                                            flowLayoutPanel1.Controls
                                        }
                                        */
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



                                {

                                    int CurrentLastId = chat.statistic.LastObjId;
                                    /*        if (CurrentLastId <= 0)
                                                CurrentLastId = cui.LastShownObjId;*/
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
                        Chat_GetMyStatistic(chatid, false);
                        TreeNodeUpdateCounter((int)chatid);



                    }
                }
                else
                {
                    Chat_GetMyStatistic(chatid);
                    TreeNodeUpdateCounter((int)chatid);
                }
                DebugInfo("OnMsgRecive time_ms=" + (DateTime.Now - db).Milliseconds.ToString());
            }

            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }
        }
        private void OnMsgRecive(Job _job, string cmd, long chatid, long msgid)
        {
            try
            {
                this.BeginInvoke(OnMsgReciveDelegateMethod, _job, cmd, chatid, msgid);
            }
            catch (Exception err)
            {

            };
        }

        private void TreeNodeUpdateCounter(int chatId)
        {
            try
            {
                up?.Invalidate();

                TreeNode[] res = treeView1.Nodes.Find("chat_" + chatId, true);
                foreach (TreeNode tn in res)
                {
                    Chat c = job.Chats.Where(s => s.chatId == chatId).FirstOrDefault();
                    if (c != null)
                    {
                        string s = c.Text;
                        //            tn.Text = s+ " (" + c.statistic.CountNew.ToString() + ")";
                        string t = "В чате появилось новое сообщение";
                        notifyIcon_ji.BalloonTipText = t;

                        //notifyIcon_ji.BalloonTipTitle = "Hi";
                        notifyIcon_ji.ShowBalloonTip(0, "JI [" + s + "]", t, ToolTipIcon.Info);


                    }

                    //                  
                }
            }
            catch
            (Exception err)
            {

            }
        }

        FormDebug fd = null;

        Queue<LogData> debuginfo = new Queue<LogData>();
        private void DebugInfo(string v)
        {
            LogData ld = null;
            if (b_boss)
            {
                ld = new LogData(DateTime.Now, v);
                debuginfo.Enqueue(ld);
                while (debuginfo.Count > 1000)
                {
                    //LogData ld=
                    debuginfo.Dequeue();
                }
            }
            if (DebugForm.Checked)
            {
                if (fd == null)
                {
                    fd = new FormDebug();
                    while (debuginfo.Count > 0)
                    {

                        LogData _ld =
                        debuginfo.Dequeue();
                        fd.AddMsg(_ld.GetTextInfo());
                    }
                    fd.Show();
                }

                if (fd != null)
                {
                    if (ld != null)
                    {
                        fd.AddMsg(ld.GetTextInfo());
                    }
                    else
                        fd.AddMsg(v);
                }

            }

        }

        private long GetCurrentChatId()
        {
            try
            {
                var t = GetCurrentChatObj();
                if (t != null)
                {
                    return t.ObjId.ObjId;
                }

            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }

            return 0;
        }


        private XROGi_Class.Chat GetCurrentChatObj()
        {
            try
            {
                Chat c1 = job.GetSelectedChat();
                if (c1 != null)
                    return c1;
                if (tabControl1.SelectedTab == tabPage_Svod)
                {
                    if (treeView1.SelectedNode != null)
                    {
                        WS_JobInfo.Obj sel = treeView1.SelectedNode.Tag as WS_JobInfo.Obj;
                        if (sel == null)
                        {
                            Chat c = treeView1.SelectedNode.Tag as Chat;
                            return c;
                        }
                        else
                        {
                            if (sel.ObjId == 0)
                            {
                                if (sel.Guid != "")
                                {

                                }
                                else
                                {

                                }
                                Chat c = job.GetSelectedChat();
                            }
                            return job.Chats.Where(s => s.ObjId.ObjId == sel.ObjId).FirstOrDefault();//.Add(new Chat(chat, stat));
                        }
                        //             job.Chat_AddCorporative(sel,"TExt");
                    }
                }
                /* foreach (ListViewItem r in listView_chats.SelectedItems)
                 {
                     if (listView_chats.SelectedItems != null)
                         switch (r)
                         {
                             case ListViewItem i:

                                 XROGi_Class.Chat O = (r.Tag as XROGi_Class.Chat);

                                 //   job.GetMessages((r.Tag as JobInfo.Obj).id, 0);
                                 return O;
                         }

                 }*/

            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }
            return null;
        }
        private void OnChatListChanged(Job _job)
        {
            try
            {
                DebugInfo("OnChatListChanged ");

                Tree_Load_Chats_All(_job);
            }
            catch (Exception err)
            {

            };
        }

        private void OnTokenReciveMethodCall(Job _job)
        {
            try
            {
                ContentMenu_Devices(job);
                if (_job != null && _job.this_device != null)
                    DebugInfo("Recive Token " + _job.this_device.TokenId.ToString());
                if (debug)
                {
                    Text = _job.this_device.TokenId.ToString();
                    tokenToolStripMenuItem.Text = _job.this_device.TokenId.ToString();
                    serverConnectedToolStripMenuItem.Text = Setup.URLServer;
                    mashineNameToolStripMenuItem.Text = Setup.MachineName;
                    userLoginToolStripMenuItem.Text = Setup.UserLogin;
                }
                try
                {
                    job.UserGetSelf();
                }
                catch (Exception err)
                {

                }


                if (_job.this_device.TokenId != -1)
                {
                    ShowInfoBallon("Новый статус", "Подключен к серверу");
                    toolStripStatusLabel2.Text = "Подключен к серверу";
                    if (up.users == null)
                        job.GetUsers();
                    else
                    {

                    }
                    Tree_Load_Chats_All(_job);
                    int selectedchat = job.Chat_GetSelected();
                    FindAndSelectInTreeChatById(selectedchat);
                    job.Spravka_GetStatus();
                    //         job.GetUsers();
                }

                //       throw new NotImplementedException();
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }
        }

        private void FindAndSelectInTreeChatById(int selectedchat)
        {
            TreeNode[] res = treeView1.Nodes.Find("chat_" + selectedchat, true);
            if (res.Length >= 1)
            {
                treeView1.SelectedNode = res[0];
                treeView1.Focus();//.Refresh();
            }
        }

        private void OnTokenRecive(Job _job)
        {
            try
            {
                this.BeginInvoke(OnTokenReciveMethod, _job);
            }
            catch (Exception err)
            {

            };

        }

        private void OnDiscnnecedJobServer(Job _job, Exception errIn)
        {
            try
            {
                this.BeginInvoke(OnDisConnecedWithErrMethod, _job, errIn);
            }
            catch (Exception err)
            {

            };
        }
        private void OnConnecedToJobServerCall(Job _job)
        {
            try
            {
                DebugInfo("<<<<<<<<<<<<<<<<<<<<<<<<<<< OnConnecedToJobServerCompleat >>>>>>>>>>>>>>>>>>>>>>>>>>>> ");


                ToolStripMenuItem sub = new ToolStripMenuItem(_job.this_device.Name);
                sub.CheckOnClick = false;
                sub.Checked = true;
                //     Device_MenuItems.DropDownItems.Add(sub);
                sub.Tag = _job.this_device;
                sub.Click += sub_ComputerTokenDialog;

                toolStripStatusLabel2.Text = "Подключен к серверу.Идентификация...";
                //                job.ConnectToServer(Environment.MachineName, Environment.UserName);
                //       job.ConnectToServer(Setup.MachineName, "i.malykhina");
                job.ConnectToServer(Setup.MachineName, Setup.UserLogin);
                foreach (string par in job.Setup_Params)
                {
                    if (par == "RunShowPageGroups=false")
                    {
                        label9.Visible = true;
                        treeView1.Enabled = false;
                    }
                    if (par == "RunShowPageGroups=true")
                    {
                        label9.Visible = false;
                        treeView1.Enabled = true;
                    }
                    if (par.StartsWith("MainWEBServer="))
                    {

                    }
                }

                //    RequestChats(job);


            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }
        }
        private void OnConnecedToJobServerCompleat(Job _job)
        {
            try
            {
                //  if (resul == true)
                this.BeginInvoke(OnConnecedToJobServerMethod, _job);
            }
            catch (Exception err)
            {

            };
        }

        private void Tree_Load_Chats_All(Job _job)
        {
            try
            {
                //      listView_chats.Items.Clear();



                Objs_Chat.Clear();
                if (tn_OU != null && tn_OU.Nodes.Count == 0)
                    LoadRootOU(tn_OU);

                if (tn_hash != null && tn_hash.Nodes.Count == 0)
                    LoadHashTree(tn_hash);

                if (tn_user != null && tn_user.Nodes.Count == 0)
                    LoadUsersTree(tn_user);

                Load_Personal_Chats();

                WS_JobInfo.Obj[] objs = job.Chat_GetList_PublicByType(0, new int[] { 7, 9, 10, 30, 32 });
                Chat[] chats = objs.Select(s => new Chat(s, null)).ToArray();
                tn_ChatsType7.Nodes.Clear();
                try
                {
                    Chat_AddToTree_Recursiv(tn_ChatsType7, chats, 0, "unsubscribed_");
                }
                catch (Exception err)
                {
                    E(err);
                }

                //    List<Chat> l = _job.Chats;
                try
                {
                    Chat[] cl = _job.GetChatList_FromCache();
                    if (cl != null)
                    {

                        //TreeNode[] tn = treeView1.Nodes.Find("TreeNode_Personal", true);


                        if (tn_Personal != null)
                        {

                            int Deep = 0;
                            TreeView_Load_Personal_Chats(cl);



                            //TreeNode[] nodes = cl.Where(s=>!s.ObjId.links.Select(ss=>ss.objid_to==s.ObjId.ObjId).Any()).Select(lo => new TreeNode()
                            // {
                            //     ContextMenuStrip = contextMenu_chat,
                            //     Tag = lo,
                            //     ToolTipText = lo.ObjId.GetText(),
                            //     Text = lo.ObjId.GetText() + " \t(" + lo.statistic.CountNew.ToString() + ")"/*+ " id=" + lo.id.ToString()*/,
                            //     Name = "chat_" + lo.ObjId.ObjId.ToString()
                            //     ,
                            //     ImageIndex = GetImageIndexById(lo.ObjId.sgTypeId)
                            //     ,
                            //     SelectedImageIndex = GetImageIndexById(lo.ObjId.sgTypeId)
                            // }
                            // )
                            // .ToArray();// Group = listView_chats.Groups[0]
                            // tn_Personal.Nodes.AddRange(nodes);

                            Chat[] arr = cl.Where(s => s.ObjId.Guid != "").ToArray();
                            foreach (Chat c in arr)
                            {
                                if (c.chatId == 64710)
                                {

                                }
                                TreeNode[] tns = treeView1.Nodes.Find("ou_" + c.ObjId.Guid, true);
                                foreach (TreeNode tn in tns)
                                {
                                    if (tn.Tag != null)
                                        if ((tn.Tag as WS_JobInfo.Obj).ObjId == 0)
                                        {
                                            (tn.Tag as WS_JobInfo.Obj).ObjId = c.ObjId.ObjId;
                                        }
                                }
                            }

                        }
                        tn_Personal.Expand();


                        //   Objs_Chat.AddRange(cl);

                        //        Obj[] ll = GetObj(Objs_Chat);

                        //            ListViewItem[] rrrr = ll.Select(lo => new ListViewItem() { Tag = lo, Text = lo.Tag.GetText()/*+ " id=" + lo.id.ToString()*/, Name = "chat_" + lo.id.ToString(), Group = listView_chats.Groups[0] }).ToArray();
                        //                    listView_chats.Items.AddRange(rrrr);


                        //                    listView_chats.Items.AddRange(_job.Chats.Select(s => s.ObjId).Select(lo => new ListViewItem() { Tag = lo, Text = lo.Tag.GetText()/*+ " id=" + lo.id.ToString()*/, Name = "chat_" + lo.id.ToString(), Group = listView_chats.Groups[0] }).ToArray());
                        //_job.Chats
                        //       ListViewItem[] rrrr2 = cl.Select(lo => new ListViewItem() { Tag = lo, ToolTipText = lo.ObjId.GetText(), Text = lo.ObjId.GetText()/*+ " id=" + lo.id.ToString()*/, Name = "chat_" + lo.ObjId.ObjId.ToString(), Group = listView_chats.Groups[0] }).ToArray();
                        //       listView_chats.Items.AddRange(rrrr2);


                        /*foreach (WS_JobInfo.Obj o in Objs_Chat)
                        {

                            {
                                Obj lo = new Obj() { id = o.ObjId, temp_string = o.Type, type = MsgType.chat };
                                ListViewItem it =  

                            }

                        }
                        */
                    }
                    else
                    {
                        //       listView_chats.Items.Clear();
                    }


                }
                catch (ChatWsFunctionException err) { E(err); }
                catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
                catch (Exception err) { E(err); }

            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }

        }

        private void TreeView_Load_Personal_Chats(Chat[] cl)
        {
            int Deep = 0;
            tn_Personal.Nodes.Clear();
            Chat_AddToTree_Recursiv(tn_Personal, cl, Deep, "chat_");
        }

        private void Load_Personal_Chats()
        {
            try
            {
                job.UpdateChats(0);
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }
        }

        private void Chat_AddToTree_Recursiv(TreeNode tn_root, Chat[] cl, int deep, string prifix)
        {

            if (tn_root == null)
                throw new Exception("Шо?");
            ///      tn_Personal.Nodes.Clear();
            if (deep == 0)
            {
                //         foreach (XROGi_Class.Chat c in cl.Where(s=>!s.ObjId.links.Where(s2=>s2.sgLinkTypeId==18).Any()))
                try
                {




                    TreeNode[] nodes = cl
                        // если нет родителей
                        .Where(s => !s.ObjId.links.Where(s2 => s2.sgLinkTypeId == 18 && s2.objid_to == s.ObjId.ObjId).Any())
                        //                        .Where(s = cl.Select(ccc=>ccc.ObjId.links.Select(ccc2=>ccc2.objid_from).ToArray())   )

                        .Select(lo => new TreeNode()
                        {
                            ContextMenuStrip = contextMenu_chat,
                            Tag = lo,
                            ToolTipText = lo.Text,
                            Text = lo.Text,
                            Name = prifix + lo.ObjId.ObjId.ToString()
                                      ,
                            ImageIndex = GetImageIndexById(lo.ObjId.sgTypeId)
                                      ,
                            SelectedImageIndex = GetImageIndexById(lo.ObjId.sgTypeId)

                        }
                            )
                            .ToArray();// Group = listView_chats.Groups[0]
                    if (nodes.Where(s => s.Tag == null).Any())
                    {

                    }
                    foreach (TreeNode tn in nodes)
                    {
                        try
                        {
                            if (tn_root.Nodes.Find(tn.Name, false).Length == 0)
                            {
                                tn_root.Nodes.Add(tn);
                                Chat_AddToTree_Recursiv(tn, cl, deep + 1, prifix);
                            }
                            else
                            {
                            }
                        }
                        catch (Exception err)
                        {

                        }

                    }
                }
                catch (Exception err)
                {

                }
            }
            else
            {
                //  return;
                //         foreach (XROGi_Class.Chat c in cl.Where(s=>!s.ObjId.links.Where(s2=>s2.sgLinkTypeId==18).Any()))
                try
                {
                    Chat parentchat = tn_root.Tag as Chat;
                    if (parentchat.chatId == 64710)
                    {

                    }
                    TreeNode[] nodes = cl
                        .Where(s => s.ObjId.links.Where(s2 => s2.sgLinkTypeId == 18 && s2.objid_from == parentchat.ObjId.ObjId
                        //links.Where(s2 => s2.sgLinkTypeId == 18 && s2.objid_from == parentchat.ObjId.ObjId
                                        && s.ObjId.ObjId != parentchat.chatId
                                    ).Any())
                        .Select(lo => new TreeNode()
                        {
                            ContextMenuStrip = contextMenu_chat,
                            Tag = lo,
                            ToolTipText = lo.Text,
                            Text = lo.Text + (lo.statistic == null
                                        ? ""
                                        :
                                            lo.statistic?.CountNew > 0
                                            ? " \t(" + lo.statistic?.CountNew.ToString() + ")"
                                            :
                                            ""
                                            ).ToString()
                                            , /*,*/
                            Name = prifix + lo.ObjId.ObjId.ToString()
                                      ,
                            ImageIndex = GetImageIndexById(lo.ObjId.sgTypeId)
                                      ,
                            SelectedImageIndex = GetImageIndexById(lo.ObjId.sgTypeId)

                        }
                            )
                            .ToArray();// Group = listView_chats.Groups[0]
                    if (nodes.Count() > 0)
                    {

                        foreach (TreeNode tn in nodes)
                        {
                            try
                            {
                                if (tn_root.Nodes.Find(tn.Name, false).Length == 0)
                                {
                                    tn_root.Nodes.Add(tn);
                                    Chat_AddToTree_Recursiv(tn, cl, deep + 1, prifix);
                                }
                                else
                                {
                                }
                            }
                            catch (Exception err)
                            {

                            }

                        }


                        /*
                        if (tn_root.Nodes.Find(tn.Name, false).Length == 0)
                        {
                            tn_root.Nodes.AddRange(nodes);
                            foreach (TreeNode tn in nodes)
                            {

                                Chat_AddToTree_Recursiv(tn, cl, deep + 1, prifix);

                            }
                        }*/

                    }

                }
                catch (Exception err)
                {

                }
            }

        }

        private void LoadUsersTree(TreeNode tn_user)
        {
            try
            {
                return;
                job.GetUsers();

                foreach (WS_JobInfo.User user in job.users.OrderBy(s => s.UserName))
                {

                    tn_user.Nodes.Add(new TreeNode() { Name = "user_" + user.UserId, Text = user.UserName, ToolTipText = user.GetPositions(), Tag = user });
                }
                tn_hash.ExpandAll();
            }
            catch (ChatWsFunctionException err)
            {
                E(err);
            }
            catch (ChatDisconnectedException err)
            {
                E(err); Chat_UnSelect();
            }
            catch (Exception err) { E(err); }
        }

        private void LoadHashTree(TreeNode tn_hash)
        {
            try
            {
                view_tbl_HashTag[] h = job.Hash_GetList("#", 0, 0);
                foreach (view_tbl_HashTag tag in h)
                {
                    tn_hash.Nodes.Add(new TreeNode() { Name = "hash_" + tag.HashTag, Text = tag.HashTag.Substring(1), ToolTipText = tag.HashTagComment, Tag = tag, SelectedImageIndex = 9, ImageIndex = 9 });
                }
                tn_hash.ExpandAll();
            }
            catch (ChatWsFunctionException err)
            {
                E(err);
            }
            catch (ChatDisconnectedException err)
            {
                E(err); Chat_UnSelect();
            }
            catch (Exception err) { E(err); }
        }

        private int GetImageIndexById(int sgTypeId)
        {
            /*SgnId	Name
7	Общий чат
8	Приватный чат
9	Доска обьявлений
10	Стол заказов
15	Избранное
30	Корпоративный чат
32	Папка*/
            switch (sgTypeId)
            {
                case 6:
                case 7:
                    {
                        return 1;

                    }
                case 8:
                case 9:
                case 10:
                case 15:
                    {
                        return 6;
                    }
                case 32:
                    {
                        return 2;
                    }
                case 30:
                    {
                        return 8;
                    }
                case 43:
                    {
                        return 14;
                    }
                default:
                    {
                        return 6;

                    }
            }
            //       return 6;
        }

        private Obj[] GetObj(List<WS_JobInfo.Obj> objs_Chat)
        {
            List<Obj> ol = new List<Obj>();
            foreach (WS_JobInfo.Obj o in Objs_Chat)
            {
                if (o.links != null && o.links.Count() > 0)
                {

                }
                {
                    MsgType mt = GetMsgType_ByClassId(o.sgClassId);
                    Obj lo = new Obj() { Tag = o, id = o.ObjId, type = mt };
                    ol.Add(lo);
                }

            }
            return ol.ToArray();
        }
        private Obj[] GetObj(WS_JobInfo.Obj[] objs_Chat)
        {
            List<Obj> ol = new List<Obj>();
            if (objs_Chat != null)
                foreach (WS_JobInfo.Obj o in objs_Chat)
                {
                    if (o.links != null && o.links.Count() > 0)
                    {

                    }

                    {
                        MsgType mt = GetMsgType_ByClassId(o.sgClassId);

                        Obj lo = new Obj() { Tag = o, id = o.ObjId, type = mt };
                        ol.Add(lo);
                    }

                }
            return ol.ToArray();
        }

        private MsgType GetMsgType_ByClassId(int sgClassId)
        {
            var mt = MsgType.chat;
            if (sgClassId == 1)
            {
                mt = MsgType.msg;
            }
            if (sgClassId == 6)
            {
                mt = MsgType.chat;
            }
            return mt;
        }

        private void обновитьТокенToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Text = job.this_device.TokenId.ToString();
        }

        private object _lock = new object();
        private void timer1_Tick(object sender, EventArgs e)

        {
            try
            {
                lock (_lock)
                {
                    try
                    {
                        timer1.Stop();
                        //            if (job!=null && job.this_device!=null)
                        //Text = job.this_device.Token;
                        //         notifyIcon_ji.Visible = false;
                        //         notifyIcon_ji.Visible = true;
                        //      this.Show();
                        if (Setup.bAutoConnect)
                            if (job != null)
                            {
                                if (job.StatusConnect == xConnectStatus.b_Created
                                        ||
                                        job.StatusConnect == xConnectStatus.b_Disconnected
                                        )
                                {
                                    DebugInfo("Timer Reconect begin");
                                    Job_Run("");
                                }
                            }
                    }
                    finally
                    {
                        timer1.Interval = 145000;
                        timer1.Start();
                    }
                }
            }
            catch (Exception err)
            {
                E(err);
            }
        }

        private void ждатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            job.b_ReciveLoop = true;
            job.WaitIncome();
            //await Task.WhenAll(Receive(webSocket), Send(webSocket));
        }

        private void StatusLabel_Main_Click(object sender, EventArgs e)
        {

        }

        private void отправитьТестВсемToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //debug
            job.SendMessage("MSG", "Test " + DateTime.Now.ToString());
        }

        private void AutoConnect_Click(object sender, EventArgs e)
        {
            AutoConnect.Checked = !AutoConnect.Checked;
            Setup.bAutoConnect = AutoConnect.Checked;
        }

        private void запросыТестовыеToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {

        }

        private void listView1_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {

        }

        private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {

        }

        private void новыйToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            try
            {
                var t = sender;
                switch (sender)
                {
                    case ToolStripMenuItem i:
                        {
                            if (i.DropDownItems.Count == 0)
                            {
                                WS_JobInfo.ChatTypes[] ct = job.GetTypeChatList();
                                if (ct != null)
                                {
                                    foreach (WS_JobInfo.ChatTypes c in ct)
                                    {
                                        ToolStripMenuItem i2 = new ToolStripMenuItem() { Text = c.Name, Tag = c };
                                        i2.Click += ChatAddNew;
                                        i.DropDownItems.Add(i2);
                                        //i.DropDownItems.AddRange(ct.OrderBy(s => s.OrderId).Select(s => new ToolStripMenuItem() { Text = s.Name, Tag = s, }).ToArray());
                                    }
                                }
                                else

                                {
                                    //       MessageBox.Show("Сервер не доступен");
                                }
                            }
                            break;
                        }
                    default:
                        break;

                }
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }
        }

        private void ChatAddNew(object sender, EventArgs e)
        {
            try
            {
                switch (sender)
                {
                    case ToolStripMenuItem i:
                        {
                            WS_JobInfo.ChatTypes c = i.Tag as WS_JobInfo.ChatTypes;
                            string DefaultName = "";
                            if (c != null)
                            {
                                switch (c.sgTypeId)

                                {
                                    case 32:
                                        DefaultName = "Новая папка";
                                        break;
                                    case 8:
                                        DefaultName = "Приватный чат";
                                        break;
                                    case 9:
                                        DefaultName = "Доска обьявлений";
                                        break;
                                    case 10:
                                        DefaultName = "Стол заказов";
                                        break;
                                    case 11:
                                        DefaultName = "Избранное";
                                        break;
                                    default:
                                        DefaultName = "Новый чат";
                                        break;
                                }
                                DefaultName += " [" + DateTime.Now.ToString() + "]";


                                if ((treeView1.SelectedNode.Tag as WS_JobInfo.Obj) == null)
                                {
                                    int new_chatid = job.Chat_Add(-1, c.sgTypeId, DefaultName, "Описание тестового чата");
                                    Tree_Load_Chats_All(job);
                                }
                                else
                                {
                                    var chat = GetCurrentChatObj();
                                    if (chat != null)
                                    {
                                        if (MessageBox.Show("Создать " + DefaultName + " в " + chat.Text, "Подтвердите создание", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                            == DialogResult.Yes)
                                        {
                                            job.Chat_Add(chat.ObjId.ObjId, c.sgTypeId, DefaultName, "Описание тестового чата");
                                        }

                                    }
                                    else
                                        job.Chat_Add(-1, c.sgTypeId, DefaultName, "Описание тестового чата");
                                }

                            }
                        }
                        break;
                    default:
                        break;

                }
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }
        }

        private void новыйToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void DeleteSubscribe()
        {

            if (mi == null)
            {

            }
            else
            {
                listBox_msg.MeasureItem -= mi;
                mi = null;

            }
            if (eh == null)
            {

            }
            else
            {
                listBox_msg.DrawItem -= eh;
                eh = null;
            }


        }

        private void Chat_Select(long id)
        {

            {
                if (debug)
                {
                    if (id > 0)
                        toolStripStatusLabel_chat_selectedInfo.Text = "# Выбран чат id=" + id.ToString();
                    else
                    {
                        toolStripStatusLabel_chat_selectedInfo.Text = "";
                    }
                }
                if (id <= 0)
                {
                    MainChatName_Cmd.Text = "";

                    job?.Chat_Selected(0);
                    tn_Personal.Nodes.Clear();
                    tn_OU.Nodes.Clear();
                    Chat_UnSelect();
                    //mp.Chat_UnSelect();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            onSendNewText();
            CorrectTextEditHeight();
        }

        private void onSendNewText()
        {
            try
            {

                string TextOut = GetTextToSend();
                TextOut = TextOut.Replace("\r\n", "<br/>");
                if (TextOut == "")
                {
                    MessageBox.Show("Введите текст для отправки");
                    return;
                }
                if (mp.Mode == XChatMessagePanelMode.EditOneMessage)
                {
                    var r = mp.GetEditMessage();
                    r.newmsg.AddText(TextOut);// "Ответ на соообщение"
                    job.Message_Replay(r.newmsg);
                    ClearTextToSend();
                    mp.SetMode(null, XChatMessagePanelMode.ShowMesages);
                    return;
                }

                string textxml = "<root><text var=\"1\">" + TextOut + "</text></root>";

                var chat = GetCurrentChatObj();
                if (chat != null && chat.chatId > 0)
                {


                    job.Add_Message((int)chat.ObjId.ObjId, textxml);
                    //      job.Chat_UpdateMyStatistic(chat.ObjId.ObjId);
                    ClearTextToSend();
                    if (mp.Mode == XChatMessagePanelMode.EditOneMessage)
                    {
                        mp.SetMode(null, XChatMessagePanelMode.ShowMesages);
                    }
                }
                else
                if (chat == null || chat?.chatId == 0)
                {  //Чат не создан
                    WS_JobInfo.Obj obj_ou = GetCurrentTreeChatObj();
                    if (obj_ou != null)
                    {
                        if (obj_ou.ObjId == 0)
                        {
                            WS_JobInfo.Obj sel = treeView1.SelectedNode.Tag as WS_JobInfo.Obj;
                            string ou_new = job.Chat_AddCorporative(obj_ou, textxml);
                            int id = Convert.ToInt32(ou_new);
                            //       job.Chat_SubscribeUser(id, job.GetMyUser(), xrTypeSubscribe.GuestUserInChat);
                            try
                            {
                                job.Add_Message(id, textxml);
                            }
                            catch (Exception err)
                            {

                            }

                            if (mp.Mode == XChatMessagePanelMode.EditOneMessage)
                            {
                                mp.SetMode(null, XChatMessagePanelMode.ShowMesages);
                            }

                            ClearTextToSend();
                        }
                        else
                        {

                            job.Chat_SubscribeUser(obj_ou.ObjId, job.GetMyUser(), xrTypeSubscribe.GuestUserInChat);
                            job.Add_Message((int)obj_ou.ObjId, textxml);
                            if (mp.Mode == XChatMessagePanelMode.EditOneMessage)
                            {
                                mp.SetMode(null, XChatMessagePanelMode.ShowMesages);
                            }
                            Chat_GetMyStatistic(obj_ou.ObjId);
                            ClearTextToSend();
                        }

                    }
                }
            }
            catch (ChatWsFunctionException err)
            {
                E(err);
            }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }

        }


        private void ClearTextToSend()
        {
            userControl11.tb.Clear();
            return;

        }
        private string GetTextToSend()
        {
            string ggg = userControl11.tb.GetPlainText();
            return ggg;
            return textBox1.Text.Trim();
        }

        private Chat GetCurrentTreeChat()
        {
            Chat chat = job.GetSelectedChat();
            if (chat != null)
            {
                return chat;
            }
            //     return null;
            if (tabControl1.SelectedTab == tabPage_Svod)
            {

                if (treeView1.SelectedNode != null)
                {
                    if ((treeView1.SelectedNode.Tag as Chat) != null)
                    {
                        return (treeView1.SelectedNode.Tag as Chat);
                    }
                    return null; // должно быть то что ниже а не нулл
                                 //     return treeView1.SelectedNode.Tag as WS_JobInfo.Obj;
                }
            }
            return null;
            /*     */
        }
        private WS_JobInfo.Obj GetCurrentTreeChatObj()
        {
            Chat chat = job.GetSelectedChat();
            if (chat != null)
            {
                return chat.ObjId;
            }
            //     return null;
            if (tabControl1.SelectedTab == tabPage_Svod)
            {

                if (treeView1.SelectedNode != null)
                {
                    if ((treeView1.SelectedNode.Tag as Chat) != null)
                    {
                        return (treeView1.SelectedNode.Tag as Chat).ObjId;
                    }
                    return treeView1.SelectedNode.Tag as WS_JobInfo.Obj;
                }
            }
            return null;
            /*     */
        }

        private void отвеитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XMessageCtrl mc = context_msg.SourceControl as XMessageCtrl;

            //1           XMessageCtrl newmsg = new XMessageCtrl();
            //        newmsg.SetCurrntcChat(GetCurrentChatId());
            //1           newmsg.SetCurrntcChat(mc.chatId);

            //1           newmsg.AddParentMsg(mc);
            //1            newmsg.AddText(textBox1.Text);// "Ответ на соообщение"
            mp.SetMode(mc, XChatMessagePanelMode.EditOneMessage);
            textBoxWPF_Focus();
            //1            job.Message_Replay(newmsg);
        }

        private void textBoxWPF_Focus()
        {
            elementHost1.Focus();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            CorrectTextEditHeight();

        }

        private void CorrectTextEditHeight()
        {
            return;
            /*
             bool IsHTMLDataOnClipboard = Clipboard.ContainsData(DataFormats.Html);

             // If there is HTML data on the clipboard, retrieve it.
             string htmlData;
             if (IsHTMLDataOnClipboard)
             {

                 htmlData = Clipboard.GetText(TextDataFormat.Html);

             }
             */
            //     string g = Clipboard.GetText();
            if (GetTextToSend() == string.Empty)
            {
                button_Send.Enabled = false;
            }
            else
                button_Send.Enabled = true;
            string[] lines = GetTextToSend().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            if (lines.Length > 1)
            {
                int needH = lines.Count() * 14 + 14;
                if (needH > 100)
                    needH = 100;
                panel_inText.Height = needH;

                //Уменьшить панель сообщений
                //    panel_inMesages.Height = panel1.Height - panel_users.Height- needH;
            }
            else
                panel_inText.Height = 30;
        }

        private void покинутьЧатToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Chat o = GetCurrentChatObj();
            job.Job_Leave(o.ObjId.ObjId);
        }

        private void отключитьсяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            job.Close();
            lockalToolStripMenuItem.Checked = jobInfoToolStripMenuItem.Checked = false;
            DisconnectToolStripMenuItem.Checked = true;
            //lockalToolStripMenuItem.Checked = false;
        }

        private void переименоватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Chat o = GetCurrentChatObj();

            FormChatInfo f = new FormChatInfo();
            f.ShowChat(job, o);
            f.ShowDialog();

        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
        }

        private void button5_Click(object sender, EventArgs e)
        {
        }

        private void button8_Click(object sender, EventArgs e)
        {
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
        }

        private void button10_Click(object sender, EventArgs e)
        {
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {

            listBox_msg.Update();

            listBox_msg.Refresh();

        }

        private void listView1_DrawItem_1(object sender, DrawListViewItemEventArgs e)
        {

        }

        private void listView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = new ListViewItem(list[e.ItemIndex].ToString());

        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            //            e.Graphics.FillRectangle(Pens.White, e.ClipBounds);
            e.Graphics.FillRectangle(Brushes.White, e.ClipBounds);
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {/*
            if (e.RowIndex < 0)
                return;
            if (e.RowIndex >= job.MessgesList.Count)
                return;
            WS_JobInfo.Obj o = job.MessgesList[e.RowIndex] as WS_JobInfo.Obj;
            long chatid = GetCurrentChat();
            Size s =  o.MeasureDrawObj(e.CellBounds.Width, e.CellBounds.Height);
           // dataGridView1.Rows[e.RowIndex].Height = s.Height;
            e.Graphics.DrawMessage(o, e.CellBounds, e.State, imageList1);
            
            //if (o.Tag.links != null && o.Tag.links.Where(s => s.objid_to == chatid).Any())
            //{
            //    e.Graphics.DrawMessage_SourceChat(o.Tag, e.Bounds, e.State);
            //}
            //else
            //    e.Graphics.DrawMessage(o.Tag, new Rectangle((int)e.Graphics.VisibleClipBounds.X,
            //        (int)e.Bounds.Y,
            //        (int)e.Graphics.VisibleClipBounds.Size.Width, (int)e.Graphics.VisibleClipBounds.Size.Height
            //        ), e.State, imageList1);
            //
            job.Message_Shown(chatid, o.ObjId);
            e.Graphics.DrawRectangle(Pens.Beige, e.CellBounds);
            return;
            */
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void ОбновитьРазмерToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ОбновитьРазмерToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            XMessageCtrl c = sender as XMessageCtrl;





        }

        private void panel4_MouseEnter(object sender, EventArgs e)
        {
            panel_msg.Focus();
        }

        private void panel4_MouseLeave(object sender, EventArgs e)
        {

        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            return;
            TreeNode tn = treeView1.SelectedNode;
            if (tn != null)
            {
                string root = "";

                WS_JobInfo.Obj o = tn.Tag as WS_JobInfo.Obj;
                if (o != null)
                    root = o.Guid;
                WS_JobInfo.Obj[] ous = job.GetOU(root);
                foreach (WS_JobInfo.Obj ou in ous)
                {
                    bool f = false;
                    foreach (TreeNode t in tn.Nodes)
                    {
                        if (t.Text == ou.xml)
                        {
                            f = true;
                            TreeNode tn2 = new TreeNode(ou.xml) { Tag = ou, Text = ou.xml.ToString() };

                            tn.Nodes.Add(tn2);
                            LoadOU(tn2);
                            break;
                        }
                    }

                    if (f == false)//&& tn.Nodes.Count==0
                    {
                        TreeNode tn2 = new TreeNode(ou.xml) { Tag = ou, Text = ou.xml.ToString() };

                        tn.Nodes.Add(tn2);
                        LoadOU(tn2);
                    }


                }
            }
        }

        private void LoadOU(TreeNode tn)
        {
            try
            {
                string root = "";
                WS_JobInfo.Obj o = tn.Tag as WS_JobInfo.Obj;
                if (o != null)
                    root = o.Guid;
                WS_JobInfo.Obj[] ous = job.GetOU(root);



                if (ous != null)
                {
                    ObjOu[] arr = ous.Select(s => new ObjOu(s, o)).ToArray();
                    job.ou_data.AddRange(arr);

                    foreach (WS_JobInfo.Obj ou in ous)
                    {

                        bool f = false;
                        foreach (TreeNode t in tn.Nodes)
                        {
                            if (t.Text == ou.xml)
                            {
                                f = true;
                                //      TreeNode tn2 = new TreeNode(ou.xml) { Tag = ou, Text = ou.xml.ToString() };
                                //       tn.Nodes.Add(tn2);
                            }
                        }
                        // if (f == false)
                        {
                            TreeNode tn2 = new TreeNode(ou.xml)
                            {
                                Tag = ou,
                                Text = ou.xml.ToString(),
                                ImageIndex = GetImageIndexById(ou.sgTypeId),
                                SelectedImageIndex = GetImageIndexById(ou.sgTypeId)
                                ,
                                Name = "ou_" + ou.Guid.ToString()
                            };

                            tn.Nodes.Add(tn2);

                        }

                    }
                }
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }
        }

        private void E(Exception err)
        {
            if (DebugException.Checked)
            {
                System.Diagnostics.Debug.WriteLine(err.Message.ToString());
                //      MessageBox.Show(err.Message.ToString(),"Exception",  MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (debug || b_boss)
                {
                    /*      notifyIcon_ji.BalloonTipText = err.Message.ToString();
                          //notifyIcon_ji.BalloonTipTitle = "Hi";
                          notifyIcon_ji.ShowBalloonTip(1000);*/
                    DebugObject(err);
                    //     DebugObject(err);
                }
            }
        }
        private void E(string Method, Exception err)
        {
            if (DebugException.Checked)
            {
                System.Diagnostics.Debug.WriteLine(err.Message.ToString());
                //      MessageBox.Show(err.Message.ToString(),"Exception",  MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (debug)
                {

                    {
                        notifyIcon_ji.BalloonTipText = Method + Environment.NewLine + err.Message.ToString();
                        //notifyIcon_ji.BalloonTipTitle = "Hi";
                        notifyIcon_ji.ShowBalloonTip(1000);
                    }
                    DebugObject(Method, err);
                    //     DebugObject(err);
                }
            }
        }
        private void DebugObject(object obj)
        {
            //    string printString = "";

            foreach (System.Reflection.PropertyInfo pi in obj.GetType().GetProperties())
            {
                DebugInfo(pi.Name + " : " + pi.GetValue(obj, new object[0]));
                //printString += pi.Name + " : " + pi.GetValue(obj, new object[0]) + "\n";
            }
            //DebugInfo(printString);
            ///MessageBox.Show(printString);
        }
        private void DebugObject(string Method, object obj)
        {
            //    string printString = "";

            foreach (System.Reflection.PropertyInfo pi in obj.GetType().GetProperties())
            {
                DebugInfo(Method + " >\t" + pi.Name + " :\t" + pi.GetValue(obj, new object[0]));
                //printString += pi.Name + " : " + pi.GetValue(obj, new object[0]) + "\n";
            }
            //DebugInfo(printString);
            ///MessageBox.Show(printString);
        }

        private void E(ChatWsFunctionException err)
        {
            if (DebugException.Checked)
            {
                System.Diagnostics.Debug.WriteLine(err.Message.ToString());

                DebugObject(err);
                //       MessageBox.Show(err.Message.ToString(), "ChatWsFunctionException", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void E(ChatDisconnectedException err)
        {
            if (DebugException.Checked)
            {
                System.Diagnostics.Debug.WriteLine(err.Message.ToString());
                DebugObject(err);
                //          MessageBox.Show(err.Message.ToString(), "ChatDisconnectedException", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {

                TreeNode tn = e.Node;
                bool b = false;
                if ((tn.Tag as WS_JobInfo.User) != null)
                {
                    ShowUserInfoParam(tn.Tag as WS_JobInfo.User);
                    //mp.Chat_UnSelect();
                    Chat_UnSelect();
                    Chat_LoadToControlObj(null);
                    b = true;
                }
                if ((tn.Tag as XROGi_Class.Chat) != null)
                {
                    ShowUserInfoParam(null);//на всякий

                    Refresh_Chats(tn);
                    //  return;
                    SelectMessageTab();
                    b = true;
                }
                if (b == false)
                {
                    ShowUserInfoParam(null);//на всякий
                    Refresh_Chats(tn);
                    SelectMessageTab();
                }

            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }
        }


        private void SelectChat(Chat chat)
        {
            try
            {
                Info("Загрузка чата в просмотр");
                bool b_changed = job.Chat_Selected(chat.ObjId.ObjId);
                if (b_changed)
                {
                    Info("Отключиться от предыдущего чата");
                    Chat_UnSelect();

                }
                //           job.Message_GetListIDs(chat);
                Info("Подключиться к чату");
                mp.Chat_Select(ref chat);
                Chat_LoadToControls(chat);
                Info("");
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }
        }

        private void Info(string v)
        {
            JI_Form_Job_OnBackWorkBeginCallInvokeMethod(v);
        }

        private void SelectChat(WS_JobInfo.Obj o)
        {
            //         root = o.Guid;
            bool b_changed = job.Chat_Selected(o.ObjId);
            if (b_changed)
            {

                Chat_UnSelect();

            }
            //           job.Message_GetListIDs(chat);
            //mp.Chat_UnSelect();
            Chat_UnSelect();
            Chat_LoadToControlObj(o);
        }

        private void Refresh_Chats(TreeNode tn)
        {
            if (tn != null)
            {
                //     string root = "";
                //  WS_JobInfo.Obj o = null;
                switch (tn.Tag)
                {
                    case Chat chat:
                        {
                            SelectChat(chat);
                            /*root = chat.ObjId.Guid;
                            bool b_changed = job.Chat_Selected(chat.ObjId.ObjId);
                            if (b_changed)
                            {

                                Chat_DisplayClearOnChatChange();

                            }
                            //           job.Message_GetListIDs(chat);
                            mp.Chat_Select(chat);
                            Chat_LoadToControls(chat.ObjId);*/
                            break;
                        }
                    case WS_JobInfo.Obj o:
                        {
                            SelectChat(o);
                            //         root = o.Guid;
                            /*     bool b_changed = job.Chat_Selected(o.ObjId);
                                 if (b_changed)
                                 {

                                     Chat_DisplayClearOnChatChange();

                                 }
                                 //           job.Message_GetListIDs(chat);
                                 mp.Chat_Select(null);
                                 Chat_LoadToControls(o);*/
                            break;
                        }
                    default:
                        //mp.Chat_UnSelect();
                        Chat_UnSelect();
                        Chat_LoadToControls(null);
                        break;
                }
                /*
           //     Chat chat = tn.Tag as Chat;
                if (chat != null)
                {
                    o = chat.ObjId;
                }
               


                if (o == null)
                    o = tn.Tag as WS_JobInfo.Obj;
                if (o != null)
                {
                    root = o.Guid;
                    bool b_changed = job.Chat_Selected(o.ObjId);
                    if (b_changed)
                    {
                      
                        Chat_DisplayClearOnChatChange();

                    }
                    //           job.Message_GetListIDs(chat);
                    mp.Chat_Select(chat);
                    Chat_LoadToControls(o);
                  
                }*/

            }
        }



        private void Chat_UnSelect()
        {
            return;
            maxH = 0;
            DeleteSubscribe();
            job.MessgesList.Clear();

            panel_users.Controls.Clear();
            MainChatName_Cmd.Text = "";
            MainChatName_Cmd.Visible = false;
            mp.Chat_UnSelect();
        }

        private void Chat_ShowMessagesToControls(WS_JobInfo.Obj o)
        {

            #region LoadMessage
            try
            {
                if (o == null) return;
                if (o.ObjId > 0)
                {
                    long id = o.ObjId;
                    UserChatInfo cui = Chat_GetMyStatistic(id);

                    if (cui != null) // к чату подписан
                    {
                        Chat_Select(id);
                        int CurrentLastId = cui.CountShownEndObjId;
                        if (CurrentLastId <= 0)
                            CurrentLastId = cui.LastShownObjId;
                        ScrollAdd(id, CurrentLastId, 10);

                        /*
                        XMessageCtrl[] ddd = flowLayoutPanel1.Controls.Cast<XMessageCtrl>().ToArray();
                        XMessageCtrl f = ddd.Where(s => s.MessageObj.ObjId == cui.LastShownObjId).FirstOrDefault();
                        if (f != null)
                        {
                            flowLayoutPanel1.ScrollControlIntoView(f);

                        }
                        */
                        //SelectMessage(CurrentLastId);
                    }
                    else
                    {

                    }

                }
                else
                {
                    panel_msg.Controls.Clear();
                    job.Chat_Selected(0);
                }
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }
            #endregion
        }

        private UserChatInfo Chat_GetMyStatistic(long id, bool b_Now = true)
        {
            UserChatInfo cui = job.Chat_GetMyStatistic(id, b_Now);
            DebugInfo("chatid=" + cui.ChatId
                    + "\tnew=(" + cui.CountNew + ")"
                    + "\t=[" + cui.StartShownObjId + ","
                    + "" + cui.LastShownObjId + ","
                    + "" + cui.CountShownEndObjId + ","
                    + "" + cui.LastObjId + "]"
                    + " userid{" + cui.UserId + "}");
            return cui;


        }

        private void ScrollAdd(long id, int currentLastId, int CountScroll)
        {
            WS_JobInfo.Obj[] r1 = GetMessages(id, currentLastId, -CountScroll); //msg_inChat =

            AddToEndList(r1, 0);
            /*      WS_JobInfo.Obj[] r = job.GetMessages(id, currentLastId, 0); //msg_inChat =

                  AddToEndList(r, 0);*/
            WS_JobInfo.Obj[] r4 = GetMessages(id, currentLastId, CountScroll); //msg_inChat =
            if (r4.Length > 0)
            {
                AddToEndList(r4, currentLastId);
            }
            //17         else
            //17            SelectedObjIdInPanel(currentLastId);
        }

        private void SelectMessage(int currentLastId)
        {
        }
        private void Chat_ShowUsersToControls(WS_JobInfo.Obj o)
        {
        }
        private void Chat_ShowUsersToControls(Chat c)
        {
            WS_JobInfo.Obj o = c.ObjId;
            try
            {
                if (o == null) return;
                #region LoadUsers
                int maxH = 0;
                int maxv = 0;
                int constPixelPerLine = 50;
                panel_users.Controls.Clear();

                //Приписанные пользователи
                WS_JobInfo.User[] us;
                WS_JobInfo.User[] us_income;
                //       job.users.Clear();
                if (o.Guid != null)
                {
                    c.users.Clear();
                    us = job.GetOU_Users(o.Guid.ToString());
                    c.users.AddRange(us);
                    if (us != null)
                    {
                        foreach (WS_JobInfo.User u in us)
                        {
                            try
                            {
                                UserClassControl u1 = new UserClassControl();
                                /* if (u.foto != null)
                                 {
                                     MemoryStream ms = new MemoryStream(u.foto);

                                     Image f = ScaleImage(Image.FromStream(ms), 40, 40);
                                     //  f.Save("c:\\temp\\!!_th.jpg", ImageFormat.Jpeg);
                                     u1.User_Foto = f;
                                 }*/
                                u1.User_Foto = ScaleImage(u.GetFoto(), 40, 40);
                                u1.user = u;
                                u1.ContextMenuStrip = contextMenu_user;
                                //    contextMenu_user.Tag = u;
                                u1.Top = maxv;
                                u1.Left = maxH;
                                maxH += 60;
                                if (maxH + 69 > panel_users.Width)
                                {
                                    maxv += constPixelPerLine;
                                    maxH = 0;
                                }
                                u1.Text = u.UserName;
                                u1.Tag = u;
                                u1.Click += OnUserSelected;
                                u1.DoubleClick += ShowUserInfo;

                                toolTip1.SetToolTip(u1, u1.Text + "\r\n" + string.Join("\r\n", u.positions.Select(s => s.Position).ToArray()));

                                panel_users.Controls.Add(u1);
                            }
                            catch (Exception err)
                            {

                            }
                        }
                        //                  job.users.AddRange(us);
                    }
                    #endregion
                }

                if (o.ObjId > 0)
                {
                    us_income = job.GetChat_Users((int)o.ObjId);
                    //    .Where(s => s.positions.Select(ss => ss.Period.dte == null))
                    //        .ToArray();
                    if (us_income != null)
                    {
                        //            job.users.AddRange(us_income);
                        if (maxH != 0)// Новую строку если были до этого показы 
                        {
                            maxv += constPixelPerLine;
                            maxH = 0;
                        }
                        if (us_income != null)
                            foreach (WS_JobInfo.User u in us_income.Where(s => s.UserId != job.GetMyUserId()))
                            {
                                UserClassControl u1 = new UserClassControl();
                                if (u.foto != null)
                                {
                                    MemoryStream ms = new MemoryStream(u.foto);

                                    Image f = ScaleImage(Image.FromStream(ms), 40, 40);
                                    //  f.Save("c:\\temp\\!!_th.jpg", ImageFormat.Jpeg);
                                    u1.User_Foto = f;

                                }
                                u1.user = u;
                                u1.ContextMenuStrip = contextMenu_user;
                                u1.Top = maxv;
                                u1.Left = maxH;
                                u1.DoubleClick += ShowUserInfo;
                                maxH += 60;
                                if (maxH + 69 > panel_users.Width)
                                {
                                    maxv += constPixelPerLine;
                                    maxH = 0;
                                }
                                u1.Text = u.UserName;
                                u1.Tag = u;
                                toolTip1.SetToolTip(u1, u1.Text + "\r\n" + string.Join("\r\n", u.positions.Select(s => s.Position).ToArray()));
                                u1.Click += OnUserSelected;
                                panel_users.Controls.Add(u1);

                            }
                    }
                    {
                        UserClassControl u1 = new UserClassControl();

                        {
                            /*
                            ResourceManager resourceManager = Properties.Resources.ResourceManager;
                            MemoryStream stream = resourceManager.GetMemoryStream("user_add.png");
                            /*
                            ResourceManager rm = new ResourceManager("AppReso/*urces", typeof(Form1).Assembly);
                            Bitmap screen = (Bitmap)Image.FromStream(rm.GetStream("user_add.png"));

        */

                            ;
                            //         Image f = ScaleImage(imageList_foto.Images["useradd"], 40, 40);
                            //  f.Save("c:\\temp\\!!_th.jpg", ImageFormat.Jpeg);


                        }

                        u1.User_Foto = imageList_foto.Images["useradd"];
                        u1.ContextMenuStrip = contextMenu_useradd;

                        u1.DoubleClick += OnUserAddToChat;
                        u1.Click += OnUserAddToChat;
                        u1.Top = maxv;
                        u1.Left = maxH;
                        maxH += 60;
                        if (maxH + 69 > panel_users.Width)
                        {
                            maxv += constPixelPerLine;
                            maxH = 0;
                        }
                        u1.Text = "Добавить пользователя в Чат";
                        u1.Tag = o;
                        toolTip1.SetToolTip(u1, u1.Text);

                        panel_users.Controls.Add(u1);

                    }

                }
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }
        }

        private void ShowUserInfo(object sender, EventArgs e)
        {

            UserClassControl userc = sender as UserClassControl;
            ShowUserInfo(userc);


        }
        private void ShowUserInfo(ChatUserPanel userc)
        {
            if (userc != null)
            {
                WS_JobInfo.User user = userc.GetUser();// userc.Tag as WS_JobInfo.User;
                if (user != null)
                {
                    ShowUserInfoParam(user);
                }
                else
                    ShowUserInfoParam(null);//на всякий
            }
        }
        private void ShowUserInfo(UserClassControl userc)
        {
            if (userc != null)
            {
                WS_JobInfo.User user = userc.user;// userc.Tag as WS_JobInfo.User;
                if (user != null)
                {
                    ShowUserInfoParam(user);
                }
                else
                    ShowUserInfoParam(null);//на всякий
            }
        }

        private void ShowUserInfoParam(WS_JobInfo.User user)
        {
            if (user == null)
            {
                tabPage_userInfo.Tag = user;
                pictureBox1.Image = null;
            }
            else
                try
                {
                    textBox_UserPhones.Text = "";
                    fn_GetUserParametersResult[] p = job.User_GetParameters(user.UserId);
                    if (p != null)
                    {
                        fn_GetUserParametersResult par = p.Where(s => s.sgParamClass == 34 && s.sgParamVid == 39 && s.sgValueType == 41).FirstOrDefault();
                        if (par != null)
                        {

                            textBox_UserPhones.Text = par.ParamValue;
                        }
                    }
                    label_FIO.Text = user.UserName;
                    label7.Text = user.GetPositions();
                    label5.Text = user.GetDepartament();

                    //  pictureBox1.Image = user.GetFoto();

                    Image User_Foto = ScaleImage(user.GetFoto(), 137, 160);
                    //      if (User_Foto != null)
                    {

                        pictureBox1.Image = User_Foto;




                    }


                    Hash_Load(user.UserId);
                    tabPage_userInfo.Tag = user;

                    tabControl2.SelectedTab = tabPage_userInfo;

                }
                catch (ChatWsFunctionException err) { E(err); }
                catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
                catch (Exception err) { E(err); }
        }

        private void Hash_Load(int userId)
        {
            view_tbl_HashTag[] h = job.Hash_GetList("", 0, userId);
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = h;
        }

        private void OnUserAddToChat(object sender, EventArgs e)
        {
            tabControl2.SelectedTab = tabPage_FindUser;
            this.AcceptButton = button_FindUsers;
            textBox2.Focus();
            //MessageBox.Show("Добавление пользователя в чаты");
        }

        private void OnUserSelected(object sender, EventArgs e)
        {
            UserClassControl u = sender as UserClassControl;
            u.b_selected = !u.b_selected;
            u.Refresh();
        }

        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            if (image != null)
            {
                var ratioX = (double)maxWidth / image.Width;
                var ratioY = (double)maxHeight / image.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var newWidth = (int)(image.Width * ratio);
                var newHeight = (int)(image.Height * ratio);

                var newImage = new Bitmap(newWidth, newHeight);

                using (var graphics = Graphics.FromImage(newImage))
                    graphics.DrawImage(image, 0, 0, newWidth, newHeight);

                return newImage;
            }
            return null;
        }

        private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            // return;
            try
            {
                TreeNode tn = e.Node;
                if (tn != null)
                {
                    string root = "";

                    WS_JobInfo.Obj o = tn.Tag as WS_JobInfo.Obj;
                    if (o != null)
                        root = o.Guid;
                    if (tn_OU != null && tn_OU.Nodes.Count == 0)
                        LoadRootOU(tn_OU);
                    if (root == "")
                        return;


                    //        LoadOU(tn2);


                    return;
                    WS_JobInfo.Obj[] ous = job.GetOU(root);
                    if (ous == null)
                    {
                        job.ConnectToServer(Environment.MachineName, Environment.UserName);
                    }
                    else
                        foreach (WS_JobInfo.Obj ou in ous)
                        {
                            bool f = false;
                            foreach (TreeNode t in tn.Nodes)
                            {
                                if (t.Text == ou.xml)
                                {
                                    //           LoadOU(t);
                                    f = true;

                                    break;
                                }
                            }

                            if (f == false)//&& tn.Nodes.Count==0
                            {
                                TreeNode tn2 = new TreeNode(ou.xml) { Tag = ou, Text = ou.xml.ToString() };

                                tn.Nodes.Add(tn2);
                                //        LoadOU(tn2);
                            }


                        }
                }
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }

        }



        //        List<WS_JobInfo.Obj, WS_JobInfo.Obj>> ou_data = new List<WS_JobInfo.Obj, WS_JobInfo.Obj>();
        private void LoadRootOU(TreeNode tn_OU)
        {
            try
            {
                job.LoagAllOU();

                job.GetUsers();


                FilterTreeDataAndShowObject("");
            }
            catch (ChatWsFunctionException err)
            {
                E(err);
            }
            catch (ChatDisconnectedException err)
            {
                E(err); Chat_UnSelect();
            }
            catch (Exception err) { E(err); }
            //tn_user.Collapse();
        }

        private void FilterTreeDataAndShowObject(string filter)
        {
            tn_OU.Nodes.Clear();
            if (job == null || job.ou_data == null)
                return;
            ObjOu oou = job.ou_data.Where(s => s.ou.Deep == 0).ToArray().FirstOrDefault();
            if (oou != null)
            {
                TreeNode tn2 = new TreeNode(oou.ou.xml) { Tag = oou.ou, Text = oou.ou.xml.ToString() };
                tn_OU.Nodes.Add(tn2);

                LoadOUFromJob(tn2, filter, oou.ou);
                tn_OU.Expand();
                tn2.Collapse();//.Expand();
                ShowUserNodes(filter);
            }

        }

        private void ShowUserNodes(string filter)
        {
            tn_user.Nodes.Clear();
            return;
            WS_JobInfo.User[] users = job.users.Where(s => s.UserName.ToLower().Contains(filter.ToLower())).Distinct().OrderBy(s2 => s2.UserName).ToArray();

            foreach (var u in users)
            {
                TreeNode tn2 = new TreeNode(u.UserName)
                {
                    Tag = u
                    ,
                    Text = u.UserName.ToString(),
                    ImageKey = "user",
                    SelectedImageKey = "user"
                    ,
                    ContextMenuStrip = contextMenu_userchat
                };
                tn_user.Nodes.Add(tn2);

            }
            //TreeNode tn2 = new TreeNode(oou.ou.xml) { Tag = oou.ou, Text = oou.ou.xml.ToString() };
            //tn_OU.Nodes.Add(tn2);




        }

        private void ShowOUToNodes(ObjOu oou1)
        {
            tn_OU.Nodes.Clear();
            ObjOu[] oou = job.ou_data.Where(s => s.ou.Parent_Guid == oou1.ou.Guid).ToArray();

            //foreach (WS_JobInfo.Obj ou in ous)
            //{
            //    TreeNode tn2 = new TreeNode(ou.xml)
            //    {
            //        Tag = ou,
            //        Text = ou.xml.ToString(),
            //        ImageIndex = GetImageIndexById(ou.sgTypeId),
            //        SelectedImageIndex = GetImageIndexById(ou.sgTypeId)
            //        ,
            //        Name = "ou_" + ou.Guid.ToString()
            //    };

            //    tn.Nodes.Add(tn2);
            //}

            //foreach (ObjOu q in oou )
            //{
            //    foreach (WS_JobInfo.Obj ou in ous)
            //    {
            //        TreeNode tn2 = new TreeNode(ou.xml)
            //        {
            //            Tag = ou,
            //            Text = ou.xml.ToString(),
            //            ImageIndex = GetImageIndexById(ou.sgTypeId),
            //            SelectedImageIndex = GetImageIndexById(ou.sgTypeId)
            //            ,
            //            Name = "ou_" + ou.Guid.ToString()
            //        };

            //        tn.Nodes.Add(tn2);
            //    }

            //    TreeNode tn2 = new TreeNode(q.ou.xml) { Tag = q.ou, Text = q.ou.xml.ToString() };
            //    tn_OU.Nodes.Add(tn2);
            //    LoadOUFromJob(tn2);
            //     }
        }
        private void LoadOUFromJob(TreeNode tn, string filter, WS_JobInfo.Obj o)
        {
            try
            {
                string root = "";
                //= tn.Tag as WS_JobInfo.Obj;
                if (o != null)
                    root = o.Guid;
                WS_JobInfo.Obj[] ous = job.ou_data.Where(s => s.ou != null && s.ou.Parent_Guid == root).Select(s => s.ou).ToArray();



                if (ous != null && ous.Length > 0)
                {
                    ObjOu[] arr = ous.Select(s => new ObjOu(s, o)).ToArray();
                    //       job.ou_data.AddRange(arr);

                    foreach (WS_JobInfo.Obj ou in ous)
                    {
                        if (isDeepSubObject(ou, filter) || ou.xml.Contains(filter))
                        {
                            //        if (ou.xml.Contains(filter))
                            {

                                TreeNode tn2 = new TreeNode(ou.xml)
                                {
                                    Tag = ou,
                                    Text = ou.xml.ToString(),
                                    ImageIndex = GetImageIndexById(ou.sgTypeId),
                                    SelectedImageIndex = GetImageIndexById(ou.sgTypeId)
                                    ,
                                    Name = "ou_" + ou.Guid.ToString()
                                };

                                tn.Nodes.Add(tn2);
                                LoadOUFromJob(tn2, filter, ou);
                            }

                            //else
                            //{
                            //    LoadOUFromJob(tn, filter, ou);
                            //}
                        }

                    }
                }
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }
        }

        private bool isDeepSubObject(WS_JobInfo.Obj ou_in, string filter)
        {
            bool b_ret = false;
            WS_JobInfo.Obj[] ous = job.ou_data.Where(s => s.ou != null && s.ou.Parent_Guid == ou_in.Guid).Select(s => s.ou).ToArray();
            foreach (WS_JobInfo.Obj ou in ous)
            {
                if (ou.xml.Contains(filter))
                {
                    return true;
                }
                else
                {
                    if (isDeepSubObject(ou, filter))
                    {
                        b_ret = true;
                        return true;
                    }
                }
            }
            return b_ret;
        }

        private void LoadRootOU_Old(TreeNode tn_OU)
        {
            try
            {
                tn_OU.Nodes.Clear();

                WS_JobInfo.Obj[] ous = job.GetOU("");
                ObjOu[] arr = ous.Select(s => new ObjOu(s, null)).ToArray();
                job.ou_data.AddRange(arr);


                if (ous != null)
                    foreach (WS_JobInfo.Obj ou in ous)
                    {
                        bool f = false;
                        foreach (TreeNode t in tn_OU.Nodes)
                        {
                            if (t.Text == ou.xml)
                            {
                                LoadOU(t);
                                f = true;
                                break;
                            }
                        }

                        if (f == false)//&& tn.Nodes.Count==0
                        {
                            TreeNode tn2 = new TreeNode(ou.xml) { Tag = ou, Text = ou.xml.ToString() };
                            tn_OU.Nodes.Add(tn2);
                            LoadOU(tn2);
                        }
                        /* */
                    }
                tn_OU.Expand();
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (tabControl1.SelectedTab==tabPage_Svod)
            //{
            //    if (treeView1.Nodes.Count==0)
            //    {
            //            string root = "";
            //            WS_JobInfo.Obj[] ous = job.GetOU(root);
            //            foreach (WS_JobInfo.Obj ou in ous)
            //            {
            //                    TreeNode tn2 = new TreeNode(ou.xml) { Tag = ou, Text = ou.xml.ToString() };
            //                    treeView1.Nodes.Add(tn2);
            //                    LoadOU(tn2);
            //            }
            //    }
            //}
        }

        private void userClassControl1_ValueChanged(object sender, int Value)
        {

        }


        UserClassControl user_Selected = null;
        private void contextMenu_user_Opening(object sender, CancelEventArgs e)
        {
            user_Selected = null;

            ContextMenuStrip menuSubmitted = sender as ContextMenuStrip;
            if (menuSubmitted != null)
            {
                Control sourceControl = menuSubmitted.SourceControl;
                if (sourceControl != null)
                {
                    user_Selected = sourceControl as UserClassControl;
                }
            }
        }

        private void StripMenuItem_SendToChat_Click(object sender, EventArgs e)
        {
            //StripMenuItem it = sender as Conte StripMenuItem;
        }



        private void button_Down_ShowNewMsg_Click(object sender, EventArgs e)
        {

        }

        private void button_Up_ShowOldersMsg_Click(object sender, EventArgs e)
        {
            //tbl_ChatUserInfo c =  job.Chat_GetMyStatistic()
        }

        private void messageChatControl1_OnMessageShownEvent(object sender)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            //      FlowLayoutPanel ft = sender as FlowLayoutPanel;
            //        e.Graphics.DrawCircle(new Pen(Color.Red, 1), 15, 15, 10);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool LockWindowUpdate(IntPtr hWnd);


        private void MessageLoadLast()
        {// добавить сообщеря вниз списка
            try
            {
                Chat chat = GetCurrentChatObj();
                //     for (int i = 0; i < flowLayoutPanel1.Controls.Count; i++)


                if (chat.statistic == null)
                    Chat_GetMyStatistic(chat.chatId);
                if (chat.statistic != null)
                //if (chat.statistic.LastObjId)
                {

                    var r = GetMessages(chat.chatId, (int)chat.statistic.LastObjId, -10);
                    AddToEndList(r, (int)chat.statistic.LastObjId);
                }
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }
        }






        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void roundButton1_Click(object sender, EventArgs e)
        {
            MessageLoadLast();
        }

        private void ButtonHaveNewMessages_Click(object sender, EventArgs e)
        {

        }

        private void обновитьВсеЧатыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Chat_Select(0); //
                Tree_Load_Chats_All(job);
                FilterTreeDataAndShowObject(textBox_filter.Text);
            }
            catch (Exception err)
            {

            }
        }

        private void отладкаОшибокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DebugException.Checked = !DebugException.Checked;
        }

        private void iusmirnovToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            SelectClientName();
        }

        private void button_FindUsers_Click(object sender, EventArgs e)
        {
            FindUserFromText(textBox2.Text);

        }

        private void FindUserFromText(string text)
        {
            try
            {

                WS_JobInfo.User[] ct = job.User_FindFull(text);
                if (ct != null)
                {
                    dataGridView_Users.AutoGenerateColumns = false;
                    dataGridView_Users.DataSource = ct;
                }


            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }
        }

        private void dataGridView_Users_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView_Users.Rows)
            {
                WS_JobInfo.User u = row.DataBoundItem as WS_JobInfo.User;
                if (u.foto != null)
                {
                    row.Cells[1].Value = ScaleImage(u.GetFoto(), 40, 40);
                    row.Height = 45;
                }
                else
                    row.Height = 25;

                row.Cells[2].Value = u.UserName;
                if (u.positions != null)
                    row.Cells[3].Value = u.GetPositions();// String.Join("\r\n", u.positions.Select(s => s.Position).ToArray());

                row.Cells[4].Value = "99-9999";

            }
        }

        int button3_mode = 0;/*0  - добавить пользователя  1  - Закрыть ввод*/
        private void button3_Click_1(object sender, EventArgs e)
        {
            int t = 0;
            if (button3_mode == 0)
            {
                List<WS_JobInfo.User> add = new List<WS_JobInfo.User>();
                foreach (DataGridViewRow row in dataGridView_Users.Rows)
                {
                    DataGridViewCheckBoxCell cc = (row.Cells[0] as DataGridViewCheckBoxCell);
                    if (Convert.ToBoolean(cc.Value) == true)
                    {
                        add.Add(row.DataBoundItem as WS_JobInfo.User);
                    }
                }
                if (add.Count == 0)
                {
                    foreach (DataGridViewRow row in dataGridView_Users.SelectedRows)
                    {
                        if (row.DataBoundItem != null)

                            add.Add(row.DataBoundItem as WS_JobInfo.User);
                    }
                }
                foreach (WS_JobInfo.User user in add.ToArray())
                {
                    try
                    {
                        var chat = this.GetCurrentTreeChatObj();
                        //        int TypeSubscribe = 1; //значение перекрыто внутренними классом
                        //GetCurrentTreeChatObj()
                        job.Chat_SubscribeUser(chat.ObjId, user, xrTypeSubscribe.GuestUserInChat);
                        t++;
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("Не удалось добавить пользователя [" + user.UserName + "] в чат. Добавить может любой состоящий в группе.");
                    }
                }
                if (t > 0)
                    if (MessageBox.Show("Добавлено участников (" + t.ToString() + ")\r\nЗакрыть окно добавления участников", "Статус", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        BackToChatTable();
                    }
                //button3.Text = "Завершить добавление";
            }

        }

        bool firstTime_textBox2_Find = true;
        private void textBox2_MouseEnter(object sender, EventArgs e)
        {
            if (firstTime_textBox2_Find)
            {
                firstTime_textBox2_Find = false;
                textBox2.Clear();
            }
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            BackToChatTable();

        }

        private void BackToChatTable()
        {
            tabControl2.SelectedTab = tabPage_Msg;
            mp.Focus();
            Refresh_SelectedChat();
        }

        private void Refresh_SelectedChat()
        {
            var o = GetCurrentTreeChat();
            Chat_LoadToControls(o);
        }

        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.None)
            {
                if (e.Delta != 0)
                {

                }
            }


            ///          flowLayoutPanel1.AutoScrollPosition = new Point(e.X, e.Y);
        }

        private void button9_Click_2(object sender, EventArgs e)
        {
            var t = GetCurrentTreeChatObj();
            var stat = Chat_GetMyStatistic(t.ObjId);

            if (stat != null)

            {

                var r = GetMessages(t.ObjId, (int)stat.LastObjId, -30);
                AddToEndList(r, (int)stat.LastObjId);
                button9.Visible = false;
            }
        }
        WS_XROGi ws;
        private void button14_Click(object sender, EventArgs e)
        {
            if (ws == null)
            {
                ws = new WS_XROGi();
                ws.OnConneced += TestConnected;
                ws.OnIncomeMessage += OnIncomeMessage;
                Task<bool> bb = ws.ConnectAsync("", true);

            }
        }

        private void OnIncomeMessage(string Message)
        {
            this.BeginInvoke(OnDebugInfo, Message);

        }

        private void TestConnected()
        {
            string f = "<cmd name=\"gettoken\" xmlns=\"http://localhost/xrogi\">\r\n  <user xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" name=\"iu.smirnov\" xmlns=\"http://localhost/xrogi\">\r\n    <device name=\"PARK-IT-PC444\" devicetype=\"Microsoft Windows NT 6.2.9200.0&#x9;Win32NT\" TokenId=\"-1\" Token_Counter=\"0\">\r\n      <period dtb=\"0001-01-01T00:00:00\" dte=\"0001-01-01T00:00:00\" dtc=\"2019-02-06T07:38:43.7205576+03:00\" dtd=\"0001-01-01T00:00:00\" />\r\n    </device>\r\n  </user>\r\n</cmd>";
            ws.Send(f);
            //        throw new NotImplementedException();
        }



        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {

        }

        private void исключитьИзЧатаToolStripMenuItem_Click(object sender, EventArgs e)
        {

            WS_JobInfo.User user = user_Selected.Tag as WS_JobInfo.User;
            if (user != null)
            {
                try
                {

                    var chat = this.GetCurrentTreeChat();
                    int TypeSubscribe = 1; //значение перекрыто внутренними классом
                    job.Chat_UnSubscribeUser(chat.ObjId, chat.ObjId.ObjId, user, TypeSubscribe);
                    Chat_LoadToControls(chat);
                }
                catch (Exception err)
                {
                    MessageBox.Show("Не удалось добавить пользователя [" + user.UserName + "] в чат. Добавить может любой состоящий в группе.");
                }
            }



            /*
        ContextMenu menuSubmitted = sender as ContextMenu;
        if (menuSubmitted != null)
        {
            Control sourceControl = menuSubmitted.SourceControl;
        }
        */
            /*
            ContextMenuStrip menuSubmitted = sender as ContextMenuStrip;
            if (menuSubmitted != null)
            {
                Control sourceControl = menuSubmitted.SourceControl;
                if (sourceControl != null)
                {
                    UserClassControl u = sourceControl as UserClassControl;
                }
            }*/
        }

        private void button15_Click(object sender, EventArgs e)
        {
            view_tbl_HashTag[] arr = dataGridView1.DataSource as view_tbl_HashTag[];
            List<view_tbl_HashTag> l = new List<view_tbl_HashTag>();
            l.AddRange(arr);
            l.Add(new view_tbl_HashTag());

            dataGridView1.DataSource = l.ToArray();
            button_HashSave.Visible = true;
            button_HashRemove.Visible = true;
        }

        private void button16_Click(object sender, EventArgs e)
        {

        }

        private void button17_Click(object sender, EventArgs e)
        {
            int uid = 0;
            view_tbl_HashTag[] arr = dataGridView1.DataSource as view_tbl_HashTag[];
            foreach (view_tbl_HashTag h in arr)
            {
                if (h.UserId.HasValue)
                {
                    uid = h.UserId.Value;
                }
                //         if (h.HashTagId==0)
                {
                    job.Hash_Update(h);
                }
            }
            //button_HashSave.Visible = false;

            Hash_Load(uid);
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                FindUserFromText(textBox2.Text);
            }
        }

        private void button15_Click_1(object sender, EventArgs e)
        {
            try
            {
                WS_JobInfo.Obj obj_ou = GetCurrentTreeChatObj();
                if (obj_ou != null)
                {


                    // string FileName = @"d:\tmp\2.jpg";
                    if (openFileDialog_Image.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            Image image1 = Image.FromFile(openFileDialog_Image.FileName);
                            //Image image1 = Image.FromFile(FileName);
                            //             Image image2 =  ScaleImage(image1, 100, 100);

                            //   image1.ty

                            job.Add_Image(obj_ou.ObjId, image1, "Изображение");

                            //      job.Chat_UpdateMyStatistic(chat.ObjId.ObjId);

                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("Не могу зарузить файл" + err.InnerException.ToString());
                        }
                    }
                }



            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); Chat_UnSelect(); }
            catch (Exception err) { E(err); }

        }

        private void treeView1_Leave(object sender, EventArgs e)
        {
            if ((sender as TreeView).SelectedNode != null)
                (sender as System.Windows.Forms.TreeView).SelectedNode.BackColor = Color.LightGray;// (sender as TreeView).ForeColor;//Color.Red; //your highlight color

        }

        private void treeView1_Enter(object sender, EventArgs e)
        {
            if ((sender as TreeView).SelectedNode != null)
                (sender as TreeView).SelectedNode.BackColor = (sender as TreeView).BackColor;
        }

        private void информацияОПользователеToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {

            }
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                //     this.Close();
                if (tabControl2.SelectedTab == tabPage2)
                {
                    SelectMessageTab();
                }
                if (tabControl2.SelectedTab == tabPage_userInfo)
                {
                    SelectMessageTab();
                }
                if (tabControl2.SelectedTab == tabPage_Msg)
                {
                    if (mp.Mode == XChatMessagePanelMode.EditOneMessage)
                    {
                        mp.SetMode(null, XChatMessagePanelMode.ShowMesages);
                    }
                }

                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        private void SelectMessageTab()
        {
            /*
            if (мояToolStripMenuItem.Checked==true)
            {
                tabControl2.SelectedTab = tabPage3;
                mp.Focus();
            }
            else*/
            tabControl2.SelectedTab = tabPage_Msg;
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox_image_MouseDown(object sender, MouseEventArgs e)
        {
            //        if (e.Button== MouseButtons.Right)
            {
                SelectMessageTab();
            }
        }

        onEditBoxChanged ebc;
        private void textBox3_Enter(object sender, EventArgs e)
        {
            ebc = new onEditBoxChanged(sender as TextBox);
            ebc.SetChangedUser(tabPage_userInfo.Tag as WS_JobInfo.User);
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            try
            {
                if (ebc != null)
                {
                    if (ebc.isChanged())
                    {
                        WS_JobInfo.User user =
                        ebc.GetChangedUser();
                        string newValue = ebc.GetNewValue();
                        int IdentityParamToServerChanged = 0;
                        job.SetUserParameter(user, UserDopParameter.parPhone, IdentityParamToServerChanged, newValue);
                    }
                }
            }
            catch (ChatWsFunctionException err) { E(err); }
            catch (ChatDisconnectedException err) { E(err); }
            catch (Exception err) { E(err); }
        }

        private void ButtonHaveNewMessages_Paint(object sender, PaintEventArgs e)
        {/*
            var s = e.Graphics.MeasureString(ButtonHaveNewMessages.Text, new Font("Arial", 12));
            
            GraphicsPath r = e.Graphics.PathRoundedRectangleFill(0, 0, (int)s.Height+5, (int)s.Width+5, 2);
            ButtonHaveNewMessages.Region = new System.Drawing.Region(e.Graphics.PathRoundedRectangleFill(e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height, 5));
            e.Graphics.FillPath(new SolidBrush(Color.White) , r);
            
            e.Graphics.DrawString(ButtonHaveNewMessages.Text, new Font("Arial", 12), new SolidBrush(Color.Black),0,0);
            */
        }

        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {/*
            e.DrawDefault = true;
            return;*/
            if (!e.Node.IsVisible)
            {
                return;
            }

            bool bLoaded = false;
            if (e.State == TreeNodeStates.Selected)
            {

            }
            if (e.Bounds.Location.X >= 0 && e.Bounds.Location.Y >= 0)
            {
                if (e.Node.Tag != null)
                {
                }
                if ((e.State & TreeNodeStates.Selected) != 0)
                {

                    if ((e.Node.Tag as Chat)?.ObjId.ObjId == 158)
                    {
                        if (e.State == 0)
                        {
                            //     return;
                        }
                        else
                        {

                            if (e.State == TreeNodeStates.Selected)
                            {

                            }
                        }
                    }
                    //...
                    // code determining whether data has been loaded is done here
                    // setting bLoaded true or false
                    //...
                }
                else
                {
                    //      e.DrawDefault = true;
                    //     return;
                }

                Font useFont = null;
                Brush useBrush = null;

                if (bLoaded)
                {
                    useFont = e.Node.TreeView.Font;
                    useBrush = SystemBrushes.WindowText;
                }
                else
                {
                    useFont = treeView1.Font;// m_grayItallicFont;
                    useBrush = SystemBrushes.GrayText;
                }
                XROGi_Class.Chat c = e.Node.Tag as XROGi_Class.Chat;
                string text = e.Node.Text;
                if (c?.statistic?.CountNew > 0)
                {
                    if (!text.Contains(")"))
                        text += (c.statistic == null ? "" : " \t(" + c.statistic?.CountNew.ToString() + ")").ToString();
                }
                if (text != e.Node.Text)
                {
                    e.Node.Text = text;
                }
                if (text.Contains(")"))
                {
                    useBrush = Brushes.Blue;// SystemBrushes.;
                    useFont = new Font(e.Node.TreeView.Font.Name, e.Node.TreeView.Font.Size, FontStyle.Bold, Font.Unit);
                }
                e.Graphics.DrawString(text, useFont, useBrush, e.Bounds.Location);
                /*        


         TreeNodeStates treeState = e.State;
         Font treeFont = e.Node.NodeFont ?? e.Node.TreeView.Font;



         XROGi_Class.Chat c = e.Node.Tag as XROGi_Class.Chat;
         if (c?.statistic.CountNew > 0)
         {
             treeFont =  new Font(treeFont, FontStyle.Bold);  
             //useFont = new Font(e.Node.TreeView.Font.Name, e.Node.TreeView.Font.Size, FontStyle.Bold, Font.Unit);
         }


         // Colors.
         Color foreColor = e.Node.ForeColor;
         string strDeselectedColor = @"#6B6E77", strSelectedColor = @"#94C7FC";
         Color selectedColor = System.Drawing.ColorTranslator.FromHtml(strSelectedColor);
         Color deselectedColor = System.Drawing.ColorTranslator.FromHtml(strDeselectedColor);

         // New brush.
         SolidBrush selectedTreeBrush = new SolidBrush(selectedColor);
         SolidBrush deselectedTreeBrush = new SolidBrush(deselectedColor);

         // Set default font color.
         if (foreColor == Color.Empty)
             foreColor = e.Node.TreeView.ForeColor;

         // Draw bounding box and fill.
         if (e.Node == e.Node.TreeView.SelectedNode)
         {
             // Use appropriate brush depending on if the tree has focus.
             if (this.Focused)
             {
                 foreColor = SystemColors.HighlightText;
                 e.Graphics.FillRectangle(selectedTreeBrush, e.Bounds);
                 ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds, foreColor, SystemColors.Highlight);
                 TextRenderer.DrawText(e.Graphics, e.Node.Text, treeFont, e.Bounds,
                                              foreColor, TextFormatFlags.GlyphOverhangPadding);
             }
             else
             {
                 Size s = TextRenderer.MeasureText(e.Graphics, e.Node.Text, treeFont);
                 Rectangle rec = new Rectangle(e.Bounds.X,e.Bounds.Y , s.Width, s.Height);
                 foreColor = SystemColors.HighlightText;
                 e.Graphics.FillRectangle(deselectedTreeBrush, rec); // e.Bounds
                 ControlPaint.DrawFocusRectangle(e.Graphics, rec, foreColor, SystemColors.Highlight);
                 TextRenderer.DrawText(e.Graphics, e.Node.Text, treeFont, rec,
                                              foreColor, TextFormatFlags.GlyphOverhangPadding);
             }
         }
         else
         {
             if ((e.State & TreeNodeStates.Hot) == TreeNodeStates.Hot)
             {
                 e.Graphics.FillRectangle(SystemBrushes.Window, e.Bounds);
                 TextRenderer.DrawText(e.Graphics, e.Node.Text, treeView1.Font, e.Bounds,
                                              System.Drawing.Color.Black, TextFormatFlags.GlyphOverhangPadding);
             }
             else
             {
                 e.Graphics.FillRectangle(SystemBrushes.Window, e.Bounds);
                 TextRenderer.DrawText(e.Graphics, e.Node.Text, treeFont, e.Bounds,
                                              foreColor, TextFormatFlags.GlyphOverhangPadding);
             }
         }

*/
            }
        }

        private void CreatePrivatChat_Click(object sender, EventArgs e)
        {


            try
            {
                var t = sender as ToolStripMenuItem;
                WS_JobInfo.User u = GetSelectedContentUser(contextMenu_userchat);
                OpenPersonalChat(u.UserId, u.UserName);
            }
            catch (Exception err)
            {
                E(err);
            }
        }

        private WS_JobInfo.User GetSelectedContentUser(ContextMenuStrip contextMenu_userchat)
        {
            ChatUserPanel cup = (contextMenu_userchat.SourceControl) as ChatUserPanel;
            WS_JobInfo.User u;
            if (cup != null)
                u = cup.GetUser();
            else
            {

                cup = (contextMenu_userchat.Tag) as ChatUserPanel;
                if (cup != null) u = cup.GetUser();
                else
                {
                    Control tu = (contextMenu_userchat.SourceControl) as Control;
                    u = (tu as TreeView).SelectedNode.Tag as WS_JobInfo.User;
                }
            }
            return u;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode NewNode;

            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
            {
                Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
                TreeNode DestinationNode = ((TreeView)sender).GetNodeAt(pt);
                XROGi_Class.Chat c = DestinationNode.Tag as XROGi_Class.Chat;
                if (DestinationNode == tn_Personal)
                {

                }
                if (c != null)
                {

                    NewNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
                    //if (DestinationNode.TreeView != NewNode.TreeView)
                    if (DestinationNode != NewNode)
                    {
                        if (DestinationNode.Name.StartsWith("unsubscribed_"))
                        {
                            job.Chat_CreateMainLink((int)c.ObjId.ObjId, (int)(NewNode.Tag as XROGi_Class.Chat)?.ObjId.ObjId);
                        }
                        else
                        if (DestinationNode.Name.StartsWith("chat_"))
                        {
                            job.Chat_CreateLink((int)c.ObjId.ObjId, (int)(NewNode.Tag as XROGi_Class.Chat)?.ObjId.ObjId);
                        }
                        DestinationNode.Nodes.Add((TreeNode)NewNode.Clone());
                        DestinationNode.Expand();
                        //Remove Original Node
                        NewNode.Remove();
                    }
                }
                else
                {

                }

            }
        }

        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
            {
                Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
                TreeNode DestinationNode = ((TreeView)sender).GetNodeAt(pt);
                if (DestinationNode == null) e.Effect = DragDropEffects.None;
                else
                {
                    XROGi_Class.Chat c = DestinationNode.Tag as XROGi_Class.Chat;
                    if (c != null)
                    {
                        e.Effect = DragDropEffects.Move;
                    }
                    else
                        e.Effect = DragDropEffects.None;
                }
            }
        }

        private void treeView1_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
            {
                Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
                TreeNode DestinationNode = ((TreeView)sender).GetNodeAt(pt);
                if (DestinationNode == null) e.Effect = DragDropEffects.None;
                else
                {
                    XROGi_Class.Chat c = DestinationNode.Tag as XROGi_Class.Chat;
                    if (c != null)
                    {
                        e.Effect = DragDropEffects.Move;
                    }
                    else
                        e.Effect = DragDropEffects.None;
                }
            }
        }

        private void timer_UpdateTreeNodeAfterChange_Tick(object sender, EventArgs e)
        {
            bool b_changed = false;
            try
            {
                timer_UpdateTreeNodeAfterChange.Enabled = false;
                if (treeUpateTimer.Count > 0)
                    lock (treeUpateTimer)
                    {
                        treeView1.BeginUpdate();
                        foreach (Tuple<TreeNode, string> v in treeUpateTimer)
                        {

                            if (v.Item1 != null)
                                if (v.Item1.Text != v.Item2.ToString())
                                {
                                    v.Item1.Text = v.Item2.ToString();
                                    b_changed = true;
                                }

                        }
                        treeUpateTimer.Clear();
                        treeView1.EndUpdate();
                    }
                if (b_changed)
                {
                    treeView1.Refresh();
                }
            }
            catch (Exception err)
            {
                E(err);
            }
        }

        private void доступВсемToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Chat o = GetCurrentChatObj();
                if (ToolStripMenuItemSetPublic.Checked == false)
                {
                    if (MessageBox.Show("Вы уверены, что хотите предоставить доступ к этому чату всем?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        job.Chat_SetPublic(o, true);
                    }
                }
                else
                {
                    if (MessageBox.Show("Вы уверены, что хотите удалить общий доступ к этому чату?\r\n Все кто находится в чате на текущий момент продолжат им пользоваться.", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        job.Chat_SetPublic(o, false);
                    }
                }
            }
            catch (Exception err)
            {
                E(err);
            }
        }

        internal WS_JobInfo.Obj[] GetMessages(long ParentChatId, WS_JobInfo.UserChatInfo cui, int CountDelta)
        {
            //        Cursor c = Cursor.Current;
            try
            {
                //          Cursor.Current = Cursors.AppStarting;

                WS_JobInfo.Obj[] ret = job.GetMessages(ParentChatId, cui, CountDelta);
                DebugInfo("GetMessages_2\tParentChatId=" + cui.LastObjId.ToString()
              + "\tCountDelta=(" + CountDelta.ToString() + ") result=" + ret.Length.ToString()
              );
                return ret;
            }
            finally
            {
                //            Cursor.Current = c;
            }
        }

        internal WS_JobInfo.Obj[] GetMessages(long ParentChatId, int Get_AfterMesageId, int CountDelta)
        {
            Cursor c = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.AppStarting;

                WS_JobInfo.Obj[] ret = job.GetMessages(ParentChatId, Get_AfterMesageId, CountDelta);
                DebugInfo("GetMessages_1\tParentChatId=" + Get_AfterMesageId.ToString()
               + "\tCountDelta=(" + CountDelta.ToString() + ") result=" + ret.Length.ToString()
               );
                return ret;
            }
            finally
            {
                Cursor.Current = c;
            }
        }

        private void textBox1_MouseEnter(object sender, EventArgs e)
        {
            textBoxWPF_Focus();
        }

        private void mp_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                if (elementHost1.Focused == false)
                    mp.Focus();
            }
            catch (Exception)
            {

            }
        }

        private void mp_MouseLeave(object sender, EventArgs e)
        {

        }

        private void panel_users_Paint(object sender, PaintEventArgs e)
        {

        }

        private void mp_Click(object sender, EventArgs e)
        {

        }

        private void panel_inMesages_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox_filter_TextChanged(object sender, EventArgs e)
        {
            /*    if (toolStripTextBox1.Text == "" 
               || (toolStripTextBox1.Text != "" && toolStripTextBox1.Text.Length>=3)
               )*/
            {

            }
        }

        private void textBox_filter_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBox_filter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FilterTreeDataAndShowObject(textBox_filter.Text);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //                      splitContainer1.Panel1.Enabled = !splitContainer1.Panel1.Enabled;
            panel_users.Visible = checkBox1.Checked;
            splitContainer1.Panel1Collapsed = !checkBox1.Checked;
            if (!checkBox1.Checked)
            {
                splitContainer1.Panel1.Hide();
            }

            else
                splitContainer1.Panel1.Show();

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Chat c = job.GetSelectedChat();
            c.SetMode_FileFilter(checkBox2.Checked);
        }

        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XMessageCtrl mc = context_msg.SourceControl as XMessageCtrl;
            string data = mc.MessageObj.GetText();
            Clipboard.SetText(
            data
            );
            notifyIcon_ji.ShowBalloonTip(0, "JI", "Сообщение скопировано в буфер обмена", ToolTipIcon.Info);

        }

        private void DeleteMesageToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void Ping_Server_Tick(object sender, EventArgs e)
        {
            try
            {
                if (job != null)
                    job.PingWinSocket();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message.ToString());
            }
            try
            {
                if (SendTestMessage.Checked == true)
                {
                    SendTestMessageMethod();
                }
            }
            catch (Exception err)
            {

            }

        }

        int i_Message = 0;


        private void SendTestMessageMethod()
        {
            i_Message++;
            string textxml = "<root><text var=\"1\">" + i_Message.ToString() + "</text></root>";

            int chatid = 33276;
            //  if (chat != null && chat.chatId > 0)
            {


                job.Add_Message(chatid, textxml);
                //      job.Chat_UpdateMyStatistic(chat.ObjId.ObjId);
                //       textBox1.Text = "";

            }
        }

        private void ТестТрансформацииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string ret = XParse.ParseMessageToHtml("<root><text ver=\"1.0\">test</text></root>");
        }

        private void ShowStatusConnect_Click(object sender, EventArgs e)
        {

        }

        private void DataGridView_Users_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Button17_Click_2(object sender, EventArgs e)
        {
            textBox3.Text = "";
            FilterUser();
        }

        private void TextBox3_TextChanged(object sender, EventArgs e)
        {

            FilterUser();
        }

        private void Info_Click(object sender, EventArgs e)
        {


            try
            {
                var t = sender as ToolStripMenuItem;

                ChatUserPanel cup = (contextMenu_userchat.SourceControl) as ChatUserPanel;
                WS_JobInfo.User u;
                if (cup != null)
                    u = cup.GetUser();
                else
                {

                    cup = (contextMenu_userchat.Tag) as ChatUserPanel;
                    if (cup != null) u = cup.GetUser();
                    else
                    {
                        Control tu = (contextMenu_userchat.SourceControl) as Control;
                        u = (tu as TreeView).SelectedNode.Tag as WS_JobInfo.User;
                    }
                }
                int[] users = new int[] { u.UserId };
                // 8 - private

                //    UserClassControl userc = sender as UserClassControl;
                ShowUserInfo(cup);

                //    int c = Convert.ToInt32(strChatId);

                //   job.Chat_SubscribeUser(GetCurrentTreeChatObj(), newChatId, u, 1);
            }
            catch (Exception err)
            {
                E(err);
            }
        }

        private void Up_OnUserSelected(ChatUserPanel u, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (e.Clicks == 1)
                    {
                        try
                        {
                            if (u.personal_chatid != null)
                            {

                            }
                            else
                            {
                                //  if (job.Chats.Where(s=>s.))
                                int? chatid = u.GetUser().PersonalChatId;
                                if (chatid.HasValue)
                                {
                                    Chat c = job.Chats.Where(s => s.chatId == chatid).FirstOrDefault();
                                    if (c != null)
                                        SelectChat(c);
                                    else
                                    {
                                        ShowUserInfo(u);//
                                    }

                                }
                                else
                                {
                                    Chat c = job.FindPrivateChat(u.UserId);
                                    if (c != null)
                                    {
                                        SelectChat(c);
                                        BackToChatTable();
                                        panel_users.Visible = false;
                                        splitContainer1.Panel1Collapsed = true;
                                        splitContainer1.Panel1.Hide();

                                    }
                                    else
                                        ShowUserInfo(u);
                                }
                            }
                        }
                        catch (Exception err)
                        {

                        }
                    }
                    if (e.Clicks == 2)
                        ShowUserInfo(u);
                }
            }
            catch (Exception err)
            {

            }

        }

        private void ЗакрытьПриватныйЧатToolStripMenuItem_Click(object sender, EventArgs e)
        {


            try
            {
                var t = sender as ToolStripMenuItem;
                WS_JobInfo.User u = GetSelectedContentUser(contextMenu_userchat);
                int[] users = new int[] { u.UserId };
                Chat c = GetCurrentChatObj();
                if (c.ObjId.sgTypeId == 8) //"Приватный чат"
                {
                    job.Job_Leave(c.chatId);
                }
            }
            catch (Exception err)
            {
                E(err);
            }
        }

        private void TabPage_userInfo_Click(object sender, EventArgs e)
        {

        }

        private void Button18_Click(object sender, EventArgs e)
        {
            try
            {
                var t = sender as ToolStripMenuItem;

                WS_JobInfo.User u = null;//up.User_GetSelectedFirstOrDefault();
                u = tabPage_userInfo.Tag as WS_JobInfo.User;
                if (u != null)
                {

                    // int[] users = new int[] { u.UserId };
                    //int[] users = new int[] { u.UserId };
                    OpenPersonalChat(u.UserId, u.UserName);


                }
            }
            catch (Exception err)
            {
                E(err);
            }
        }

        private void OpenPersonalChat(int UserId, string UserName)
        {
            int[] users = new int[] { UserId };
            /*

              Chat c = job.FindPrivateChat(u.UserId);
              if (c == null)
              {
                  // 8 - private
                  int newChatId5 = job.Chat_CreateAndSubscribe(-1, 8, UserName, "Приватный чат для общения 2х человек", users);
                  Chat c2 = job.Chats.Where(s => s.chatId == newChatId5).FirstOrDefault();

              }
              else
              {
                  SelectChat(c);
                  BackToChatTable();
              }
              */
            /**/

            int newChatId = 0;
            int sgTypeId = 8;
            AutoSelect_ChatId = 0;
            if (job.Chats.Where(s => s.ObjId.sgTypeId == sgTypeId
                                    && s.ObjId.UsersInChat.Where(s2 => s2 == UserId).Any()
                                ).Any() == false)
            {
                newChatId = job.Chat_CreateAndSubscribe(-1, sgTypeId, UserName, "Приватный чат для общения 2х человек", users);
                AutoSelect_ChatId = newChatId;
            }
            else
            {
                newChatId = (int)job.Chats.Where(s => s.ObjId.sgTypeId == sgTypeId
                                  && s.ObjId.UsersInChat.Where(s2 => s2 == UserId).Any()).FirstOrDefault().ObjId.ObjId;
            }

            Chat c = job.Chats.Where(s => s.chatId == newChatId).FirstOrDefault();
            if (c == null)
            { //чат только создали, ждём уведомления от WS

            }
            else
            {
                SelectChat(c);
                BackToChatTable();
            }

        }

        private void Button19_Click(object sender, EventArgs e)
        {
            try
            {
                var t = sender as ToolStripMenuItem;

                WS_JobInfo.User u = null;//up.User_GetSelectedFirstOrDefault();
                u = tabPage_userInfo.Tag as WS_JobInfo.User;
                if (u != null)
                {
                    //   ChatUserPanel cup = (contextMenu_userchat.SourceControl) as ChatUserPanel;
                    //    WS_JobInfo.User u;
                    //     if (cup != null)
                    //         u = cup.GetUser();
                    //else
                    /*
                                    {

                                        cup = (contextMenu_userchat.Tag) as ChatUserPanel;
                                        if (cup != null) u = cup.GetUser();
                                        else
                                        {
                                            Control tu = (contextMenu_userchat.SourceControl) as Control;
                                            u = (tu as TreeView).SelectedNode.Tag as WS_JobInfo.User;
                                        }
                                    }*/
                    int[] users = new int[] { u.UserId };
                    Chat c = job.FindPrivateChat(u.UserId);
                    //    Chat c = GetCurrentChatObj();
                    if (c.ObjId.sgTypeId == 8
                        //                        && c.ObjId.p
                        ) //"Приватный чат"
                    {
                        job.Job_Leave(c.chatId);
                    }
                }
            }
            catch (Exception err)
            {
                E(err);
            }
        }

        private void JI_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                job?.Close();
            }
        }

        private void CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            BackToChatTable();
        }

        private void CheckBox4_CheckedChanged(object sender, EventArgs e)
        {
            BackToChatTable();
        }

        private void ДобавитьВВыбранныйЧатToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var t = sender as ToolStripMenuItem;
                WS_JobInfo.User u = GetSelectedContentUser(contextMenu_userchat);
                int[] users = new int[] { u.UserId };
                Chat c = GetCurrentChatObj();
                if (c == null)
                {
                    MessageBox.Show("Чат не выбран. Выбранный чат отображается вверу, в меню, зелёным цветом");

                }
                else
                {
                    if (c.ObjId.sgTypeId != 8) //"Приватный чат"
                    {
                        job.Chat_SubscribeUser(c.chatId, u, xrTypeSubscribe.GuestUserInChat);
                        //job.Job_Leave(c.chatId);
                    }
                    else
                        MessageBox.Show("Нельзя добавлять третьего человека в приватный чат, т.к. предыдущая переписка может стать доступна.");
                }

            }
            catch (Exception err)
            {
                E(err);
            }
        }

        private void DebugForm_Click(object sender, EventArgs e)
        {

        }

        private void ПерерисоватьОкнаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mp.Invalidate();
            up.Invalidate();
        }

        private void Mp_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {

                mp.Focus();
            }
            catch (Exception)
            {

            }
        }

        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (linkLabel1 != sender)
                linkLabel1.LinkVisited = false;
            if (linkLabel2 != sender)
                linkLabel2.LinkVisited = false;
            if (linkLabel3 != sender)
                linkLabel3.LinkVisited = false;
            (sender as LinkLabel).LinkVisited = true;

            if (textBox3.Text.Trim() != "")
            {
                try
                {
                    ToolTip tt = new ToolTip();
                    tt.IsBalloon = true;
                    tt.InitialDelay = 0;
                    tt.AutomaticDelay = 1000;
                    tt.AutoPopDelay = 1000;
                    // tt.ShowAlways = true;
                    string msg = "Задана фильтрующая строка, показаны не все результаты.";
                    //       tt.SetToolTip(textBox3, "Задана фильтрующая строка, показаны не все результаты.");
                    tt.Show(msg, textBox3, 50, -40, 1000);
                }
                catch (Exception err)
                {

                }
            }



            //)
            //var t = chats.Where(s => s.users.Where(s => s.UserId != 4)).Select(s2 => s2.UserId).ToArray());
            FilterUser();
        }

        private void FilterUser()
        {

            LinkLabel linkLabel = null;
            if (linkLabel1.LinkVisited) linkLabel = linkLabel1;
            if (linkLabel2.LinkVisited) linkLabel = linkLabel2;
            if (linkLabel3.LinkVisited) linkLabel = linkLabel3;
            if (linkLabel == null)
            {
                //up.SetFilter(textBox3.Text);
                up.SetFilterStatus(textBox3.Text, xEnumUserFiler.xFilterAll, null);
            }
            if (linkLabel1 == linkLabel)
                up.SetFilterStatus(textBox3.Text, xEnumUserFiler.xFilterAll, null);
            if (linkLabel2 == linkLabel)
            {
                Chat[] chats = job.Get_PrivateChats();
                int[] users = chats.Select(s => s.ObjId.UsersInChat.Where(s1 => s1 != job.GetMyUserId()).FirstOrDefault())
                    //                .Distinct()
                    .ToArray();
                up.SetFilterStatus(textBox3.Text, xEnumUserFiler.xFilterSubscribe, users);
            }
            if (linkLabel3 == linkLabel)
            {

                up.SetFilterStatus(textBox3.Text, xEnumUserFiler.xFilterOnline, null);
            }
        }

        private void Wheel_inverse_Click(object sender, EventArgs e)
        {
            Setup.Mouse_Wheel_bInverse = Wheel_inverse.Checked;
        }

        private void BotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectClientName();
        }

        private void SelectClientName()
        {

            if (iusmirnovToolStripMenuItem.Checked)
            {
                botToolStripMenuItem.Checked = false;
                defaultToolStripMenuItem.Checked = false;
                Setup.UserLogin = iusmirnovToolStripMenuItem.Text;
            }
            if (botToolStripMenuItem.Checked)
            {
                iusmirnovToolStripMenuItem.Checked = false;
                defaultToolStripMenuItem.Checked = false;
                Setup.UserLogin = botToolStripMenuItem.Text;
            }
            if (defaultToolStripMenuItem.Checked)
            {
                botToolStripMenuItem.Checked = false;
                iusmirnovToolStripMenuItem.Checked = false;
                Setup.UserLogin = defaultToolStripMenuItem.Text;
            }
            MainConnect();
        }

        private void DefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectClientName();
        }

        private void IusmirnovToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectClientName();
        }

        private void ElementHost1_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }

        private void Button12_Click_1(object sender, EventArgs e)
        {
            //userControl11.tb.Text = "111111111";
            //     userControl11.Tag = 5;
            //     string ggg =userControl11.tb.GetPlainText();



        }
        void ctr_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {   //https://stackoverflow.com/questions/4350862/wpf-events-in-winforms
            /*UPD: e.KeyboardDevice.Modifiers (e is System.Windows.Input.KeyEventArgs) stores info about Ctrl, Alt, etc.*/

            if (e.Key == System.Windows.Input.Key.Enter)
            {
                onSendNewText();
                CorrectTextEditHeight();
            }
            elementHost1.Focus();            /* your custom handling for key-presses */
        }

        private void TabPage3_Click(object sender, EventArgs e)
        {

        }

        private void CheckBox5_CheckedChanged(object sender, EventArgs e)
        {
            BackToChatTable();
        }

        private void Button13_Click_1(object sender, EventArgs e)
        {
            tabControl2.SelectedTab = tabPage3;
        }

        private void ElementHost1_ChildChanged_1(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }

        private void GhpsqlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lockalToolStripMenuItem.Checked = false;
            jobInfoToolStripMenuItem.Checked = false;
            DisconnectToolStripMenuItem.Checked = false;
            ghpsqlToolStripMenuItem.Checked = true;
            MainConnect();
        }

        private void ElementHost3_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }

        private void Button20_Click(object sender, EventArgs e)
        {
            Form1 ss = new Form1();
            ss.ShowDialog();


            X_WPF_Msg m = new X_WPF_Msg("Смирнов");
            //    m.Time = "11-55";
            //     x_WPF_MsgList1.Msglist.Items.Add(m);


            /*
            msglist.Items.Add(m);
            msglist.Items.Add(new X_WPF_Msg("Иванов") { });
            msglist.Items.Add(new X_WPF_Msg("Кулаков") { });
            msglist.Items.Add(new X_WPF_Msg("Смирнов") { });
            msglist.Items.Add(new X_WPF_Msg("Иванов") { });
            msglist.Items.Add(new X_WPF_Msg("Кулаков") { });
            */
        }

        private void ElementHost1_ChildChanged_2(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }

        private void TabPage1_Click(object sender, EventArgs e)
        {

        }

        private void Button4_Click_1(object sender, EventArgs e)
        {
            WS_JobInfo.User dddd = job.GetMyUser();


            // WPF UserControl.
           /*  ElementHost host = new ElementHost();
           // host.Dock = DockStyle.Fill;
            XWPF_User userControl1 = new XWPF_User();
            userControl1.DataContext = dddd;
            host.Child = userControl1;
            panel3.Controls.Add(host);
              */
        

            var ctr = (elementHost4.Child as XWPF_User);
            if (ctr == null)
                return;
            var ttttt = new Test_Class();
            ctr.DataContext = ttttt;
            
        }

        private void ДобавитьЧерезQRСсылкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
        }

        private void ДобавитьЧерезQRСсылкуToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            int userid = job.GetMyUserId();
            if (userid >= 0)
            {
                FormQRRequest f = new FormQRRequest();
                f.SetUsedId(userid);
                f.ShowDialog();
            }
            else

                MessageBox.Show("Вначале надо соединиться с сервером.");
        }

        private void SetupMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ОтправитьЛогОшибокРазработчикуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int ff = 0;
            ExchangeSendEmail es = new ExchangeSendEmail();

            string s = "";
            while (debuginfo.Count > 0)
            {
                LogData _ld =
                debuginfo.Dequeue();
                s+= ff++.ToString()+".\t" + _ld.GetTextInfo().Replace("\r\n", "<BR/>").Replace("<", "/") + "<BR/>";

            }
            es.SendMail("JI.LogMessage", s, "iu.sminov@ghp.lc");
        }
    }

}

