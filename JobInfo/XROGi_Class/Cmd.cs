using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace JobInfo.XROGi_Class
{
    [XmlRoot("user", Namespace = "http://localhost/xrogi", IsNullable = false)]
    public class Cmd
    {
        [XmlAttribute(AttributeName = "name")]
        string name; // "gettoken"  "cleartoken":
        [XmlAttribute(AttributeName = "clientname")]
        string clientname;
        [XmlAttribute(AttributeName = "pid")]
        string pid;
        [XmlAttribute(AttributeName = "TokenId")]
        string TokenId;
        [XmlAttribute(AttributeName = "user")]
        User user;
        internal void SetUser(User u)
        {
            user = u;
        }

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


                XmlSerializer xsSubmit = new XmlSerializer(typeof(User));

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
