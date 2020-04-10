using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobInfo.WS_JobInfo;

namespace JobInfo
{
    public class UserLive
    {
        public      WS_JobInfo.User         user;
        public      bool                    b_online;
        public      DateTime                Date_LastOnline;
        public      bool                    b_selected;
        public      bool                    b_shown ;
        public      UserStatus              OnlineStatus;
        
        public int PublicChatId;

        public UserChatInfo statistic { get; internal set; }

        internal void SetOnline(UserStatus e)
        {
            if (e.UserId == user.UserId)
            {
                OnlineStatus = e;
                b_online = true;
            }
            
        }
    }
}
