using System;

namespace Ji.Services
{

   
    public class ConnectInterface
    {
        public int Id;
        public string Server_Name;//"http://jobinfo.svodint-ru/ji/xml/";
        private string server_SOAP;
        private string server_WS;
        private const string WS_aspx = "ChatHandler.ashx";
        private const string SOAP_aspx = "GetUserInfo.asmx";
        private string tokenReqId;
        private string tokenSeanceId;
        public string Server_SOAP
        {
            get 
            {
                if (string.IsNullOrEmpty(Server_Name) ==false)
                {
                    int pos = Server_Name.IndexOf("://");
                    if (pos > 0)
                    {
                        pos += 3;

                        string f = this.Server_Name.Substring(pos);  //"http://jobinfo.svodint-ru/ji/xml/";

                        server_SOAP = "http://" + f + SOAP_aspx;
                        string host_Server_Name3 = f.Substring(0, f.IndexOf("/"));
                        //
                        return server_SOAP;
                    }
                }
                else

                {
                    return "";
                }
                return "";

                //"http://" + Server_Name + server_SOAP;
            }
            set => server_SOAP = value;
        }
        public string Server_WS
        {
            get
            {
                if (string.IsNullOrEmpty(Server_Name)==false)
                {
                    if (string.IsNullOrEmpty(server_WS))
                    {
                        int pos = Server_Name.IndexOf("://");
                        if (pos > 0)
                        {
                            pos += 3;

                            string f = this.Server_Name.Substring(pos);  //"http://jobinfo.svodint-ru/ji/xml/";

                            server_WS = "ws://" + f + WS_aspx;

                            //
                            return server_WS;
                        }
                    }
                    else
                    {
                        return server_WS;
                    }
                }
                return "";

                //"http://" + Server_Name + server_SOAP;
            }
            set => server_WS = value;
        }
        public string TokenReqId { get => tokenReqId; set => tokenReqId = value; }
        public string TokenSeanceId { get => tokenSeanceId; set { tokenSeanceId = value; _Token_Counter = 1; } }

        long _Token_Counter;
        public long TokenSeance_Counter { get { _Token_Counter++; return _Token_Counter; } set { _Token_Counter = 2; } }

        public string Fb_Token { get; internal set; }

        public bool isSetup()
        {
          if (String.IsNullOrEmpty(Server_SOAP))
            {
                return false;
            }
            return true;
        }
    }
}
