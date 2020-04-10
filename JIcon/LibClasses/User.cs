using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace JobInfo
{
    [XmlRoot("user", Namespace = "http://localhost/xrogi", IsNullable = false)]
    public class User
    {

        internal bool isValid;
        [XmlAttribute(AttributeName = "name")]
        public string userName;

        public Device device;

        public User()
        {
            this.userName = "no set user ib constructor";
        }

        public User(string userName)
        {
            this.userName = userName;
        }

        internal void SetDevice(Device d)
        {
            device = d;
        }


        internal XDocument GetXml()
        {
            return  XDocument.Parse(toXml());
        }

         
        internal string toXml()
        { var xml = "";
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
            }catch (Exception err)
            {
                
            }
            return xml;
        }
    }
}