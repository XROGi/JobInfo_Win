using System;
using System.Collections.Generic;
using System.Text;

namespace Ji.ClassSR
{
    public class ObjStatus
    {
        public int ObjStatusId { get; set; }
        public int ObjId { get; set; }
        public int PeriodId { get; set; }
        public int UserId { get; set; }
        public int sgMsgStatusId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public DateTime dtb { get; set; }
        public DateTime ? dte { get; set; }
        public int sgTypeId { get; set; }
    }
}
