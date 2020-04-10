using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Ji.Models
{

    [Table("ChatPages")]
    public    class ChatPages
    {
        [PrimaryKey, AutoIncrement, Column("_ChatPageId")]
        public int ChatPageId { get; set; }
        public int ChatId { get; set; }
        public int PageNumber { get; set; }
        public DateTime ? DateDownload { get; set; }
        
    }
}
