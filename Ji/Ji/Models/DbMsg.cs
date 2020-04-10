using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Ji.Models
{
    [Table("DbMsg")]
    class DbMsg
    {
        [PrimaryKey, AutoIncrement, Column("_DbMsgId")]
        public int DbMsgId { get; set; }

        public int ChatId { get; set; }
        public int PageNum { get; set; }
        public int ObjId { get; set; }
        
        [MaxLength(255)]
        public string Text { get; set; }
        
        public DateTime DateCreate { get; set; }
        
        public int userid { get; set; }
        public int ShownState { get; set; }
     //   [MaxLength(255)]
    //    public string userCreater { get; set; }
        /*
         
               public int ObjId { get; set; }

        public string Data { get; set; }

       


        public int sgClassId;
        public int sgTypeId;
        public int sgGruupId;
        public string Type;
        public string Guid;
        public string Parent_Guid;
        public int Deep;

        public int ShownState { get; set; }
     
        //    public Period period;
        public string xml
        {
            get;
            set;
        }

        //    public LinksObj[] links;
        public int userid;
        string _userCreater;
        public string userCreater
        {
            get { return _userCreater; }
            set { _userCreater = value; }
        }

         */

    }
}
