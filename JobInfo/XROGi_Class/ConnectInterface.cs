using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobInfo.XROGi_Class
{
    public class ConnectInterface
    {
        public int Id;
        public string Server_Name;
        private string server_SOAP;
        private string server_WS;

        public string Server_SOAP { get => "http://"+ Server_Name+ server_SOAP; set => server_SOAP = value; }
        public string Server_WS { get => "ws://" + Server_Name +  server_WS; set => server_WS = value; }
    }
}
