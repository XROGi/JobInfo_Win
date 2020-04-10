using Newtonsoft.Json;
using System;

namespace Push_FireBase
{
    internal class FB_Setup
    {
        public string PostURL { get; internal set; }
        public string ServerKeyHeader
        {
            get
            {
                return string.Format("key={0}", ServerKey);
            }
            set
            {

            }
        }
        public string ServerKey
        {
            get;set;
        }
        public string SenderId { get; internal set; }
        public string Priority { get; internal set; }
        public string Sound { get; internal set; }
        public string PostContent { get; internal set; }
        public string SenderIdHeader { get { return string.Format("id={0}", SenderId); }set { } }

        internal string GetJson(string to, string title, string body)
        {
            var fbsetup = this;
            //cloudmessage page see!!!!!!!!!!!!
            var serverKey = string.Format("key={0}", fbsetup.ServerKeyHeader);
            var senderId = string.Format("id={0}", fbsetup.SenderId);

            var data = new
            {
                to, // Recipient device token
                notification = new { title, body, sound = fbsetup.Sound }
               ,
                priority = fbsetup.Priority
               ,
                content_available = true
            };

            // Using Newtonsoft.Json
            string jsonBody = JsonConvert.SerializeObject(data);
            return jsonBody;
        }
    }
}