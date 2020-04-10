using System;
using System.Collections.Generic;
using System.Text;

namespace Ji.ClassSR
{
    public partial class ViewPpsStep2Results
    {
        public int IdStep2 { get; set; }
        public string Result { get; set; }
        public DateTime DateReq { get; set; }
        public DateTime? DateIn { get; set; }
        public DateTime? DateOut { get; set; }
        public string ParkingNumber { get; set; }
        public string TiketNumber { get; set; }
    }
}
