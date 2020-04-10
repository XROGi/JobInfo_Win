using System;
using System.Collections.Generic;
using System.Text;

namespace Ji.Models
{
    public class ObjJob
    {
        public ObjMsg JobChat { get; set; }
        public List<ObjMsg> Msgs { get;  }
        public DateTime DateFinish { get { return JobPeriod.dte.Value; } set { JobPeriod.dte = value; } }
        public Ji.Droid.Period JobPeriod { get; }
        public string JobClass { get; set; }
        public bool isJobNative { get; set; }
        public bool isJobSvod { get; set; }
        public List<int> UsersIn { get; }

        public MsgObjType JobType { get; set; }
 //       public JobClassType jobClassType { get; set; }

        public ObjJob()
        {
            isJobNative = false;
            isJobSvod = false;
            JobClass = "";
            Msgs = new List<ObjMsg>();
            JobChat = null;
            UsersIn = new List<int>();
            JobPeriod = new Droid.Period();
           
        }

        internal void SetUsersIn(int[] users)
        {
            UsersIn.AddRange(users);
        }
    }
}
