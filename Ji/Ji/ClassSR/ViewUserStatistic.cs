using System;
using System.Collections.Generic;
using System.Text;

namespace Ji.ClassSR
{
    public class ViewUserStatistic
    {
        public int ChatUserInfoId { get; set; }
        public int ChatId { get; set; }
        public int UserId { get; set; }
        public int StartShownObjId { get; set; }
        public int LastShownObjId { get; set; }
        public int LastObjId { get; set; }
        public int CountNew { get; set; }
        public DateTime? Db { get; set; }
        public DateTime? De { get; set; }
        public int? CurrentShownEndObjId { get; set; }
        public int? LastObjPage { get; set; }
        public int? LastShownPage { get; set; }
        public DateTime LastObjDate { get; set; }
        public DateTime? VersLastChange { get; set; }
    }
}
