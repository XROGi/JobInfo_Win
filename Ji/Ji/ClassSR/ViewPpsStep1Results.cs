using System;
using System.Collections.Generic;
using System.Text;

namespace Ji.ClassSR
{
    public partial class ViewPpsStep1Results
    {
        public int IdStep1 { get; set; }
        public string Result { get; set; }
        public DateTime DateReq { get; set; }
        public DateTime? DateIn { get; set; }
        public DateTime? DateOut { get; set; }
        public string OutCarType { get; set; }
        public string OutCarNumber { get; set; }
        public string OutWho { get; set; }
        public string OutReason { get; set; }
        public string ParkingNumber { get; set; }
    }
}
