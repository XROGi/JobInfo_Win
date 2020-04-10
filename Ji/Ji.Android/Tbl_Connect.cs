using SQLite;

namespace Ji.Droid
{
    [Table("Tbl_Connect")]
    class Tbl_Connect
    {
        [PrimaryKey, AutoIncrement, Column("ConnectId")]
        public int ConnectId { get; set; }

        [MaxLength(255)]
        public string Server_SOAP { get; set; }

        [MaxLength(255)]
        public string Server_WS { get; set; }

        [MaxLength(255)]
        public string TokenReqId { get; set; }

    }
}