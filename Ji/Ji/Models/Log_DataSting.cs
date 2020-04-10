using System;
using System.Collections.Generic;
using System.Text;

namespace Ji.Models
{
    public class Log_DataSting
    {
        public DateTime DateMsg = DateTime.Now;

        public String Text { get; set; }
        public override string ToString()
        {
            return $"{DateMsg.ToLongTimeString()} {Text}";

        }
    }
}
