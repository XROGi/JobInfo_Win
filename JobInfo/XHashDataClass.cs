using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobInfo.WS_JobInfo;
using JobInfo.XROGi_Class;

namespace JobInfo
{
    public class XHashDataClass
    {
        List<UserLive> live = new List<UserLive>();
        
        string FilterString = "";

        internal List<UserLive> Live { get => live; set => live = value; }

        public XHashDataClass ()
        {
            Live = new List<UserLive>();
        }

        internal void SetUsers(WS_JobInfo.User[] r)
        {
            Live.AddRange(r.Select(s=> new UserLive() {  user = s, b_online  = false, b_selected = false }));
        }

        internal void SetFilter(string filterFIO)
        {
            FilterString = filterFIO;
        }

        internal void SetStatusOnline(UserStatus[] ee)
        {
          foreach (UserStatus e in ee)
            {
                UserLive l = Live.Where(s => s.user.UserId == e.UserId).FirstOrDefault();
                l?.SetOnline(e);

            }
        }

        internal void User_Shown(ChatUserPanel c)
        {
            //ChatUserPanel
            UserLive ul = Live.Where(s => s.user.UserId == c.UserId).FirstOrDefault();//
            if (ul!=null)
                ul.b_shown = true;
        }

        internal int[] Selected_Get()
        {
            return Live.Where(s => s.b_selected == true).Select(s => s.user.UserId).ToArray();
        }
        internal int[] Shown_GetList()
        {
            return Live.Where(s => s.b_shown == true).Select(s => s.user.UserId).ToArray();
        }

        internal int [] Online_GetList()
        {
            var tttt = Live.Where(s => s.b_online == true).ToArray();
            int[] ddd = tttt.Select(s => s.user.UserId).ToArray();
            if (ddd.Count() == 0)
                return null; 
            return ddd;// Live.Where(s => s.b_online == true).Select(s => s.user.UserId).ToArray();
        }

        internal int[] AllUser_GetList()
        {
            return Live
                            //.Where(s => s.b_online == true)
                            .Select(s => s.user.UserId)
                            .Distinct()
                            .ToArray();
        }
    }
}
