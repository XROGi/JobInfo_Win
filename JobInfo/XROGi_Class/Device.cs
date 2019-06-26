using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace JobInfo
{
    [XmlRoot("Device", Namespace = "http://localhost/xrogi", IsNullable = false)]
    public class Device
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name;
        [XmlAttribute(AttributeName = "devicetype")]
        public string DeviceType;
        [XmlAttribute(AttributeName = "enabled")]
        public string Enabled;

        /*[XmlAttribute(AttributeName = "devicetoken")]
        public string Token;
        */
        [XmlAttribute(AttributeName = "TokenId")]
        public long TokenId;

        long _Token_Counter;

        [XmlAttribute(AttributeName = "Token_Counter")]
        public long Token_Counter { get { _Token_Counter++; return _Token_Counter; } set { _Token_Counter = 0; } }
        public Period period;

        
        public Device()
        {
      //      this.machineName = "";
            Name = "Device";
            Token_Counter = 0;
        }

        public Device(string machineName)
        {
            Name = machineName;
            Update(machineName);
            TokenId = -1;
            _Token_Counter = 0;
            period = new Period();
            toXml();
        }
        public bool Token_isOk()
        {
            if (TokenId == 0)
            {
                return false;
            }

            if (TokenId == -1)
                return false;
            return true;
        }
        public void Update(string machineName)
        {
            if (machineName== Environment.MachineName)
            {
                //https://stackoverflow.com/questions/2819934/detect-windows-version-in-net
                OperatingSystem os = Environment.OSVersion;
                bool vers = Environment.Is64BitOperatingSystem;
                DeviceType = os.VersionString+"\t"+os.Platform;

                if (Environment.UserDomainName!="")
                { 
                }
                // ComputerInfo().TotalPhysicalMemory;
            }
        }

        internal XDocument GetXml()
        {
            return XDocument.Parse(toXml());
        }
        public string toXml()
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(Device));
       
            var xml = "";

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, this);
                    xml = sww.ToString(); // Your XML
                }
            }
            return xml;
        }

        internal void Token_Set(long v)
        {

            //TokenId = v.ToString();
            TokenId = v;
        }

        internal void Token_Clear()
        {
            //Token = "!!??!!" ;
            TokenId = -1;
        }
        // public string Name { get { return this.machineName; } internal set { this.machineName = value; } }
        // public string Token { get { return this.machineToken; } internal set { this.machineToken = value; } }
    }
}