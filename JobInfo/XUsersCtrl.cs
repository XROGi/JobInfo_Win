using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JobInfo.WS_JobInfo;
using JobInfo.XROGi_Class;

namespace JobInfo
{
    public class XUsersCtrl : System.Windows.Forms.Panel
    {
        public delegate void OnUserListUpdateDelegate(XUsersCtrl sender);
        public event OnUserListUpdateDelegate OnUserListUpdate = delegate { };
        List<WS_JobInfo.UserStatus> OnlineUserInfo = new List<UserStatus>();
        String FilterFIO="";
        int step = 0;

        public List<WS_JobInfo.User> users = new List<WS_JobInfo.User>();
        List<ChatUserPanel> ShownUsersControls = new List<ChatUserPanel>();
        List<int> SelectedUserIds = new List<int>();

        public ContextMenuStrip UserContextMenuStrip { get; internal set; }

        public delegate void OnUserSelectedDelegate(ChatUserPanel u , MouseEventArgs e);
        public event OnUserSelectedDelegate OnUserSelected = delegate { };

        public delegate void onUser_NeedFotoDelegate(XUsersCtrl sender, ChatUserPanel user);
        public event onUser_NeedFotoDelegate onUser_NeedFoto = delegate { };

        public delegate void onUser_NeedUserStatusInfoDelegate(XUsersCtrl sender, int[] users);
        public event onUser_NeedUserStatusInfoDelegate onUser_NeedUserStatusInfo = delegate { };

        internal void SetUsers(WS_JobInfo.User[] r)
        {
            Close();
            users.AddRange(r.OrderBy(s=>s.UserName));
            Refresh();
            OnUserListUpdate(this);
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
                foreach (WS_JobInfo.User u in users.Where(s => s.UserName.ToLower().Contains(Filter)))
                {
                    _st++;
                    if (_st < step) continue;

                    ChatUserPanel c = new ChatUserPanel(u);
                    {
                        c.Top = t;
                        c.Width = Width;
                        c.Height = 75;
                        c.ContextMenuStrip = UserContextMenuStrip;
                        c.onUser_NeedFoto += onUser_NeedFotoNow;
                        
                        if (SelectedUserIds.Where(s => s == u.UserId).Any())
                        {
                            c.Selected = true;
                        }
                        else c.Selected = false;

                        ShownUsersControls.Add(c);
                        //          this.Controls.Add(c);
                    };
                    bool b_online = false;
                    
                    foreach (WS_JobInfo.UserStatus us in OnlineUserInfo)
                    {
                        if (us.UserId == u.UserId)
                        {
                            b_online = true;
                        }
                    }
                    c.PaintUserV1(e.Graphics, b_online);
                    //    this.Container.Add(c);
                    //          c.Refresh();
                    if (t <= Height)
                        t += c.Height;
                    else
                        break;
                    //if ()
                }
            }catch (Exception err)
            {

            }
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
            base.OnMouseWheel(e);
            if (e.Delta != 0)
            {
                //          this.ScrollControlIntoView.mi.
                step -= (e.Delta/120);
                if (step < 0)
                    step = 0;
                Refresh();
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

        private void RequestOnlineData()
        {
            int[] users = ShownUsersControls.Select(s => s.UserId).ToArray();
            onUser_NeedUserStatusInfo(this,users);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons. Left)
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
                            OnUserSelected(u,e);
                           

                            Invalidate();
                          //    Refresh();

                        }
                        catch (Exception err)
                        {

                        }
                    }
                   
                }
            }
                if (e.Button== MouseButtons.Right)
            {
          Point      startPoint = this.PointToScreen(e.Location);
          
                foreach (ChatUserPanel u in ShownUsersControls)
                {
                    if (u.Bounds .Contains(e.Location))
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
            
            
        }

        internal void SetFilter(string v)
        {
            FilterFIO = v;
            step = 0;
            Refresh();
        }

        internal void Close()
        {
            users?.Clear();
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
              
               
            }

        }
    }
}
