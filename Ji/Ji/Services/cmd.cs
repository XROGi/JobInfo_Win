using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Ji.Services
{
    [XmlRoot("cmd", Namespace = "http://localhost/xrogi", IsNullable = false)]

    public class Cmd
    {
        /* IIS  var ToketReq = user_elem.Attribute("name").Value; // ID - вместо логина ищем токен 
                                                var dev = user_elem.Element(ns + "device");
                                                var dev_name = dev.Attribute("name").Value;
                                                var dev_type = dev.Attribute("devicetype").Value;
                                                var OSVersion = dev.Attribute("OSVersion").Value; 
                                                var pid = cmd.Attribute("pid").Value;
                                                var clientname = cmd.Attribute("clientname").Value;
*/
        [XmlAttribute(AttributeName = "name")]
        public string name; // "gettoken"  "cleartoken":
        [XmlAttribute(AttributeName = "vers")]
        public string Vers; // "gettoken"  "cleartoken":
        [XmlAttribute(AttributeName = "clientname")]
        public string clientname;
        [XmlAttribute(AttributeName = "clientvers")]
        public string clientvers;
        [XmlAttribute(AttributeName = "pid")]
        public string pid;
        [XmlAttribute(AttributeName = "TokenId")]
        public string TokenId;
     //   [XmlAttribute(AttributeName = "user")]
        public User user;
        public Device device;


        internal XDocument GetXml()
        {
            return XDocument.Parse(toXml());
        }


        internal string toXml()
        {
            var xml = "";
            try
            {


                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = false,
                    Encoding = Encoding.UTF8
                };


                XmlSerializer xsSubmit = new XmlSerializer(typeof(Cmd));

                //using (MemoryStream memoryStream = new MemoryStream()) https://stackoverflow.com/questions/4928323/xml-serialization-encoding
                //using (var sww = new StringWriter())
                using (var memoryStream = new MemoryStream())
                {
                    using (XmlWriter writer = XmlWriter.Create(memoryStream, xmlWriterSettings))
                    {
                        xsSubmit.Serialize(writer, this);
                        memoryStream.Position = 0;
                    }
                    using (var sr = new StreamReader(memoryStream))
                    {
                        if (sr.EndOfStream == false)
                        {
                            xml = sr.ReadToEnd();

                        }
                    }


                }
                return xml;
            }
            catch (Exception err)
            {

            }
            return xml;
        }

    }

}
