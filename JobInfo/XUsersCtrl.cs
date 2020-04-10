using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JobInfo.WS_JobInfo;
using JobInfo.XROGi_Class;
using JobInfo.XROGi_Extensions;

namespace JobInfo
{
    public class XUsersCtrl : System.Windows.Forms.Panel
    {
        public const int constTimeSec_CacheUserOnline = 90;

        public XHashDataClass Data;
        public delegate void OnUserListUpdateDelegate(XUsersCtrl sender);
        public event OnUserListUpdateDelegate OnUserListUpdate = delegate { };





        List<WS_JobInfo.UserStatus> OnlineUserInfo = new List<UserStatus>();
        String FilterFIO="";
        xEnumUserFiler FilterDopType;
        int step = 0;

        public List<WS_JobInfo.User> users = new List<WS_JobInfo.User>();
        public List<WS_JobInfo.User> users_Filtered = new List<WS_JobInfo.User>();

        List<ChatUserPanel> ShownUsersControls = new List<ChatUserPanel>();
        List<int> SelectedUserIds = new List<int>();

        public ContextMenuStrip UserContextMenuStrip { get; internal set; }

        public delegate void OnUserSelectedDelegate(ChatUserPanel u , MouseEventArgs e);
        public event OnUserSelectedDelegate OnUserSelected = delegate { };

        public delegate void onUser_NeedFotoDelegate(XUsersCtrl sender, ChatUserPanel user);
        public event onUser_NeedFotoDelegate onUser_NeedFoto = delegate { };

        public delegate void onMessages_GetCountNotReadDelegate(XUsersCtrl sender, ChatUserPanel user);
        public event onMessages_GetCountNotReadDelegate OnMessages_GetCountNotRead = delegate { };

        public delegate void onUser_NeedUserStatusInfoDelegate(XUsersCtrl sender, int[] users);
        public event onUser_NeedUserStatusInfoDelegate onUser_NeedUserStatusInfo = delegate { };

        public delegate void OnDebugClassEventDelegate(object sender, string FunctionName, string param_values);
        public event OnDebugClassEventDelegate OnDebugClassEvent = delegate { };

        internal void SetUsers(WS_JobInfo.User[] r)
        {
            Close();
            //r[0].vers
            users.Clear();
            users.AddRange(r.OrderBy(s => s.UserName));
            Data.SetUsers(r);
            SetFilterStatus("" ,FilterDopType, null);
                
            Refresh();
            OnUserListUpdate(this);
        }
        public XUsersCtrl()
        {
            Data = new XHashDataClass();
            FilterDopType = xEnumUserFiler.xFilterAll;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);
                int t = 0;
                int _st = 0;
                String Filter = GetFilter().ToLower();
                ShownUsersControls.Clear();
                /*if (users_Filtered.Count==0)
                {
                    SetFilter(FilterFIO);
                }*/
                if (users_Filtered!=null)
                foreach (WS_JobInfo.User u in users_Filtered)
                {
                    _st++;
                    if (_st < step) continue;
                    if (step>0)
                        {
                            e.Graphics.FillCircle(Brushes.Gray, Width / 2, 3, 3);
                            e.Graphics.FillCircle(Brushes.Gray, Width / 2 - 10, 3, 3);
                            e.Graphics.FillCircle(Brushes.Gray, Width / 2 + 10, 3, 3);
                        }
                        ChatUserPanel c = new ChatUserPanel(u);
                    {
                        c.live = Data.Live.Where(s => s.user.UserId == (int)u.UserId).FirstOrDefault();
                        c.Top = t;
                        c.Width = Width;
                        c.Height = 75;
                        c.ContextMenuStrip = UserContextMenuStrip;
                            c.onUser_NeedFoto += onUser_NeedFotoNow;
                            c.OnMessages_GetCountNotRead += C_OnMessages_GetCountNotRead    ;  ;

                            if (SelectedUserIds.Where(s => s == u.UserId).Any())
                        {
                            c.Selected = true;
                        }
                        else c.Selected = false;
                            
                        Data.User_Shown(c);
                        ShownUsersControls.Add(c);
                            
                        //          this.Controls.Add(c);
                    };
                    bool b_online = false;


                    b_online = OnlineUserInfo.Where(s => s.UserId == u.UserId ).Any();
                    /*
                    foreach (WS_JobInfo.UserStatus us in OnlineUserInfo)
                    {
                        if (us.UserId == u.UserId)
                        {
                            b_online = true;
                        }
                    }*/
                    c.PaintUserV1(e.Graphics, b_online);
                    if (t <= Height)
                        t += c.Height;
                    else
                        break;
                   
                }
                RequestOnlineData();
            }
            catch (Exception err)
            {

            }
        }

        private void C_OnMessages_GetCountNotRead(ChatUserPanel sender)
        {
            OnMessages_GetCountNotRead(this, sender);
        }

        private void onUser_NeedFotoNow(ChatUserPanel sender)
        {
            onUser_NeedFoto(this, sender);
        }

        private string GetFilter()
        {
            return FilterFIO;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            OnDebugClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name), e.Delta.ToString());

            base.OnMouseWheel(e);
            if (e.Delta != 0)
            {
                //          this.ScrollControlIntoView.mi.
                int Delta = -(e.Delta / 120);
                if (Setup.Mouse_Wheel_bInverse)
                {
                    Delta = -Delta;
                }
                if (users_Filtered.Count >= step)
                    step += Delta;

                if (step >= users_Filtered.Count)
                    step = users_Filtered.Count;
                 if (step < 0)
                    step = 0;
                Refresh();
            }
           
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            OnDebugClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name),e.KeyCode.ToString());
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Down)
            {
                step += 1;
                if (step < 0)
                    step = 0;
                Invalidate();
            }
            if (e.KeyCode == Keys.Up)
            {
                step -= 1;
                if (step < 0)
                    step = 0;
                Invalidate();
            }
            if (e.KeyCode == Keys.PageDown)
            {
                step += 10;
                if (step < 0)
                    step = 0;
                Invalidate();
            }
            if (e.KeyCode == Keys.PageUp)
            {
                step -= 10;
                if (step < 0)
                    step = 0;
                Invalidate();
            }
            if (e.KeyCode == Keys.Home)
            {
                step = 0;
                Invalidate();
            }
            if (e.KeyCode == Keys.End)
            {
                step = users_Filtered.Count-1;
                Invalidate();
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            this.Focus();
    //        if (OnlineUserInfo.Count==0)
            {
                RequestOnlineData();
            }
        }

        
        List<XObjectDate> Queue_UserOnline = new List<XObjectDate>();

        
        private void RequestOnlineData()
        {
            //List<int> users = ShownUsersControls.Select(s => s.UserId).ToList();
            int[] users = Data.Shown_GetList();
            List<XObjectDate> newUsers = new List<XObjectDate>();
            FlushOnlineQueue();
            foreach (int userid in users)
            {
                if (isUserIdIn_Queue_UserOnline(userid)==false)
                {
                    XObjectDate o = new XObjectDate(userid, DateTime.Now);
                    Queue_UserOnline.Add(o);
                    newUsers.Add(o);
                }
            }
            if (newUsers.Count > 0)
            {
                onUser_NeedUserStatusInfo(this, newUsers.Select(s => s.userid).ToArray());
                OnDebugClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name), newUsers.Count.ToString());
                newUsers.Clear();
                newUsers = null;
            }
        }
        private void All_RequestOnlineStatus()
        {
            //List<int> users = ShownUsersControls.Select(s => s.UserId).ToList();
            int[] users = Data.AllUser_GetList();
            List<XObjectDate> newUsers = new List<XObjectDate>();
            FlushOnlineQueue();
            foreach (int userid in users)
            {
                if (isUserIdIn_Queue_UserOnline(userid) == false)
                {
                    XObjectDate o = new XObjectDate(userid, DateTime.Now);
                    Queue_UserOnline.Add(o);
                    newUsers.Add(o);
                }
            }
            if (newUsers.Count > 0)
            {
                onUser_NeedUserStatusInfo(this, newUsers.Select(s => s.userid).ToArray());
                OnDebugClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name), newUsers.Count.ToString());
                newUsers.Clear();
                newUsers = null;
            }
        }

        private bool isUserIdIn_Queue_UserOnline(int userid)
        {
            foreach (XObjectDate o  in Queue_UserOnline)
            {
                if (o.userid==userid)
                {
                    return true;
                }
            }
            return false;
        }

        private void FlushOnlineQueue()
        {
            while (true)
            {
                if (Queue_UserOnline.Count <= 0) break;
                XObjectDate o = Queue_UserOnline[0];
                if ((DateTime.Now - o.DateUpdate).TotalSeconds > constTimeSec_CacheUserOnline)
                { // старые элементы удалим
                    Queue_UserOnline.RemoveAt(0);
                }
                else
                    break;
            }

        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            try
            {
                base.OnMouseDown(e);
                OnDebugClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name), e.Button.ToString());
                if (e.Button == MouseButtons.Left)
                {
                    foreach (ChatUserPanel u in ShownUsersControls)
                    {
                        if (u.Bounds.Contains(e.Location))
                        {
                            try
                            {
                                string f = u.GetUser().UserName;
                                SelectedUserIds.Clear();
                                SelectedUserIds.Add(u.UserId);
                                OnUserSelected(u, e);


                                Invalidate();
                                //    Refresh();

                            }
                            catch (Exception err)
                            {

                            }
                        }

                    }
                }
                if (e.Button == MouseButtons.Right)
                {
                    Point startPoint = this.PointToScreen(e.Location);

                    foreach (ChatUserPanel u in ShownUsersControls)
                    {
                        if (u.Bounds.Contains(e.Location))
                        {
                            try
                            {
                                string f = u.GetUser().UserName;
                                SelectedUserIds.Clear();
                                SelectedUserIds.Add(u.UserId);

                                UserContextMenuStrip.Tag = u;
                                UserContextMenuStrip.Show(Cursor.Position);
                                Invalidate();
                                //UserContextMenuStrip.Show(u, u.RectangleToScreen(e.Location));
                            }
                            catch (Exception err)
                            {

                            }
                        }
                        //Rectangle controlRectangle;
                        //for (int i = 0; i < Controls.Count; i++)
                        //{
                        //    controlRectangle = Controls[i].RectangleToScreen
                        //  (Controls[i].ClientRectangle);
                        //    if (controlRectangle.IntersectsWith(theRectangle))
                        //    {
                        //        Controls[i].BackColor = Color.BurlyWood;
                        //    }
                        //}

                        //  if (u.Location.)
                    }
                }
            }catch (Exception err)
            {

            }
            
        }

 
        internal void Close()
        {
           
      //20190630      users?.Clear();
            ShownUsersControls?.Clear();
            SelectedUserIds?.Clear();
        }

        internal WS_JobInfo.User User_GetSelectedFirstOrDefault()
        {
            if (SelectedUserIds.Count != 1)
            {
                return null;

            }
            return users.Where(s => s.UserId == SelectedUserIds[0]).FirstOrDefault();

        }

        internal void AddStatus(UserStatus[] ee)
        {
            if (ee.Count() > 0)
            {

                //int[] ret = ee.Except(s=>s. OnlineUserInfo).ToArray();
                //if (ret.Length > 0)
                    OnlineUserInfo.AddRange(ee);

                Data.SetStatusOnline(ee);
               
            }

        }

        internal void SetFilterStatus(string text, xEnumUserFiler xFilterAll, int[] _users)
        {
            try
            {
                if (text ==null)
                {

                }
                Data.SetFilter(text.ToLower());
                FilterFIO = text.ToLower();
                step = 0;

                FilterDopType = xFilterAll;
                if (xFilterAll == xEnumUserFiler.xFilterAll)
                {
                    step = 0;
                    users_Filtered = users
                        .Where(s => s.UserName != null && s.UserName.ToLower().Contains(FilterFIO)).ToList();
                    OnDebugClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name), FilterFIO);
                }
                if (xFilterAll == xEnumUserFiler.xFilterOnline)
                {
                    step = 0;
                    All_RequestOnlineStatus();
                    int[] users2 = null; 
                    try
                    {
                    

                        var tttt = Data.Live.Where(s => s.b_online == true).ToArray();
                        int[] ddd = tttt.Select(s => s.user.UserId).ToArray();

                        users2 = ddd;

                        users2 = Data.Online_GetList();
                    }
                    catch (Exception err)
                    {

                    }
                    users_Filtered = users
                        .Where(s => s.UserName != null && s.UserName.ToLower().Contains(FilterFIO))
                        .Where(s => users2.Contains(s.UserId)).ToList();
                    OnDebugClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name), FilterFIO);
                }
                if (xFilterAll == xEnumUserFiler.xFilterSubscribe)
                {
                    step = 0;

                    users_Filtered = users
                        .Where(s => s.UserName != null && s.UserName.ToLower().Contains(FilterFIO))
                        .Where(s => _users!=null && _users.Contains(s.UserId)).ToList();
                    //          users_Filtered = users.Where(s => s.UserId  && s.PersonalChatId.HasValue).ToList();
                    OnDebugClassEvent(this, string.Format("{0}.{1}", MethodBase.GetCurrentMethod().DeclaringType.FullName, MethodBase.GetCurrentMethod().Name), FilterFIO);
                }
            }catch (Exception err)
            {
                
            }
            Refresh();
        }

        internal void UpdateStatisticChat(Chat chat, UserChatInfo obj)
        {
            UserLive ul= Data.Live.Where(s => s.PublicChatId == chat.chatId).FirstOrDefault();
            ul.statistic = obj;
            Invalidate();
        }
    }
}
