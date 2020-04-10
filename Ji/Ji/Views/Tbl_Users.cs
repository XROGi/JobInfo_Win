using SQLite;

namespace Ji.Droid
{
    
        [Table("Tbl_Users")]
        public class Tbl_Users
        {
            [PrimaryKey, AutoIncrement, Column("_UserId")]
            public int UserId { get; set; }

            [MaxLength(255)]
            public string UserName { get; set; }

            [MaxLength(255)]
            public string Server_WS { get; set; }

            [MaxLength(255)]
            public string TokenReqId { get; set; }

        [MaxLength(255)]
        public string UrlImage { get; set; }

        [MaxLength(255)]
        public string Phones { get; set; }
 
        public byte[] Bytes { get; set; }

        }
   
}