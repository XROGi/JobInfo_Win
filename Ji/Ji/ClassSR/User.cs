using System;
using System.Collections.Generic;
using System.Text;

namespace Ji.ClassSR
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserLogin { get; set; }
        public string SID { get; set; }
        public byte[] foto;
        public UserPosition[] positions { get; set; }
        public int? PersonalChatId { get; set; }
        public string[] Params { get; set; }
        public bool isFavorite { get; set; }
       
    }
}
