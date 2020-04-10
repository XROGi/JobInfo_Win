using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobInfo
{
    static public class Setup
    {
        //  string _URLServer;
        internal static void StartInit()
        {
            bAutoConnect = true;
        }
        internal static bool bAutoConnect;

        public static string URLServer;// { get { return _URLServer; };  set { _URLServer = value; }; }
        public static string MachineName;// { get; internal set; }
        public static string UserLogin;// { get; internal set; }
        internal static bool Mouse_Wheel_bInverse;
    }
}
