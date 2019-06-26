using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JobInfo.XROGi_Extensions
{
    static class XROGi_Exception
    {
        static public void XROGi(this Exception o,string method)
        {
            if (Environment.UserName != "iu.smirnov")
            {
                MessageBox.Show(method +"\r\n"+o.Message.ToString(), "XROGi_Exception");
            }
        }
    }
}
