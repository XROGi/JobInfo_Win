using JobInfo.XROGi_Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobInfo
{
    public class MessageLive
    {
        public int              id;
        public XMessageCtrl     msg ;
        public WS_JobInfo.Obj   obj;
        public bool             b_show;
        public DateTime DateRequest_obj;
        public DateTime DateResponse_obj;


    }
}
