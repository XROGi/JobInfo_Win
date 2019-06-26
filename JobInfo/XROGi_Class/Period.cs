using System;
using System.Xml.Serialization;

namespace JobInfo
{
    [XmlRoot("period", Namespace = "http://localhost/xrogi", IsNullable = false)]

    public class Period
    {   /*
        <attribute name = "id" type="string" use="required" />
    <attribute name = "dtb" type="dateTime" use="required" />
    <attribute name = "dte" type="dateTime" />
    <attribute name = "dtc" type="dateTime"  />
    <attribute name = "dtd" type="duration" />
    */
        [XmlAttribute(AttributeName = "id")]
        public string id;
        [XmlAttribute]
        public DateTime dtb;
        [XmlAttribute]
        public DateTime dte;
        [XmlAttribute]
        public DateTime dtc;
        [XmlAttribute]
        public DateTime dtd;
        public Period()
        {
            dtc = DateTime.Now;
        }
    }
}