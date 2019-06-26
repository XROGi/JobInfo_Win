using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JobInfo.XROGi_Class
{
    class XROGi_RTF
    {
      static  public  string SetRtf( string RTF)
        {

            string[] l = RTF.Split('{','}');
            foreach (string str in l )
            {

            }

            return RTF;
          //  rtb.Rtf = document;
            /*
            var documentBytes = Encoding.UTF8.GetBytes(document);
            using (var reader = new MemoryStream(documentBytes))
            {
                reader.Position = 0;
                rtb.SelectAll(); 
               
                //rtb.rt.Load(reader, DataFormats.Rtf);
            }
        */
        }
    }
}
