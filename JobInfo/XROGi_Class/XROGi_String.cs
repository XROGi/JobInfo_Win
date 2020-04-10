using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobInfo.XROGi_Class
{
    public static class XROGi_String
    {
        public  static string  FIO_GetFamiliaIO(string FIO_FULL)
        {
            try
            {
                string[] fio = FIO_FULL.Trim().Split();
                if (fio.Length == 3)
                {
                    return String.Format("{0} {1}.{2}.", fio[0], fio[1][0], fio[2][0]);
                }
                else return FIO_FULL;
            }catch (Exception err)
            {

            }
            return "FIO_GetFamiliaIO Error";
        }
    }
}
