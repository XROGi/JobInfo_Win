using System;
using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace JobInfo
{
    static internal class XParse
    {
        public static string StreamToString(Stream stream)
        {
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        [Obsolete]
        internal static string ParseMessageToHtml(string xml)
        {
            try
            {
                string input = xml;// "<?xml version=\"1.0\"?> ...";

                //!!! Isolation https://stackoverflow.com/questions/10363174/use-local-images-in-webbrowser-control
           
                //string URL = "file://" + Windows.Storage.ApplicationData.Current.LocalFolder.Path + "\\folder\\filename.png";

                string output;

                XslCompiledTransform xslt = new XslCompiledTransform();
                string tr = StreamToString(Assembly.GetExecutingAssembly().GetManifestResourceStream("JobInfo.msg2html.xslt"));

                using (Stream strm = Assembly.GetExecutingAssembly().GetManifestResourceStream("JobInfo.msg2html.xslt"))
                using (XmlReader reader = XmlReader.Create(strm))
                {
                    string d = reader.ReadInnerXml();
                    XslTransform transform = new XslTransform();
                    //     transform.Load(reader);
                    xslt.Load(reader);

                    using (StringReader sReader = new StringReader(input))
                    using (XmlReader xReader = XmlReader.Create(sReader))
                    using (StringWriter sWriter = new StringWriter())
                    using (XmlWriter xWriter = XmlWriter.Create(sWriter, xslt.OutputSettings))
                    {

                        {

                            /*    */
                            // use the XslTransform object
                            //xslt.Load("transform.xsl");
                            //    xslt.OutputSettings.OutputMethod = XmlOutputMethod.Html;
                            xslt.Transform(xReader, xWriter);
                            string fff = xReader.ToString();
                            output = sWriter.ToString();
                        }




                    }
                }

                if (output.Contains("^_^"))
                {
                }
                string our_ret    = WpfEmoticons.Controls.EmoticonsHelper.ReplaceEmoticToHTMLLink(output);
                if (our_ret != output)
                {
                    output = our_ret;
                }
                    string name = Environment.CurrentDirectory + @"\images\emoticons\" + "01.png";
            //    output = output.Replace("^_^", "<img src=\"" + name + "\"/>");
            ///    output = output.Replace(":)", "<img src=\"" + name + "\"/>");

                return output;
            }catch (Exception err)
            {

            }
            return null;
            /*
            XmlDocument target = new XmlDocument(input.CreateNavigator().NameTable);
            using (XmlWriter writer = target.CreateNavigator().AppendChild())
            {
                transform.Transform(input, writer);
            }


            var myXslTrans = new XslCompiledTransform();
            myXslTrans.Load("stylesheet.xsl");

            XmlReader xmlReader = XmlReader.Create(new StringReader(xml));

           TextWriter myWriter = new TextWriter("result.html", null);

            myXslTrans.Transform(xmlReader, myWriter);
            

    */
        }
    }
}