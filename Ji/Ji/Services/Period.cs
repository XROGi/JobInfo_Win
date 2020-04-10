using System;
using System.Xml.Serialization;

namespace Ji.Droid
{
    public class Period
    {
        //    public string id;
        [XmlAttribute]
        public DateTime ? dtb { get; set; }
        [XmlAttribute]
        public DateTime ? dte { get; set; }
        [XmlAttribute]
        public DateTime ? dtc { get; set; }
        [XmlAttribute]
        public DateTime ? dtd { get; set; }
        public Period()
        {
            dtc = DateTime.Now;
        }

        [XmlAttribute(AttributeName = "id")]
        public int PeriodId;
        
        public bool isFinish()
        {
            if(dte<=DateTime.Now)
                return true;
            return false;
        }
        public bool FinishNow()
        {
            dte = DateTime.Now;
            return false;
        }
    }
}