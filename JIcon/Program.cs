using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace JIcon
{
    static class Program
    {
        static Mutex mutex;
     //   [DllImport("Kernel32.dll")]
 //       public static extern Boolean IsWindowsServer();

        static bool IsSingleInstance()

        {
            try
            {
                //Проверяем на наличие мутекса в системе
                Mutex.OpenExisting("JobInfo_Icon");
            }
            catch
            {
                //Если получили исключение значит такого мутекса нет, и его нужно создать
                bool isNew=false;
                 mutex = new Mutex(true, "JobInfo_Icon", out isNew);
                return isNew;
            }
            //Если исключения не было, то процесс с таким мутексом уже запущен
            return false;
        }

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                bool isNew = false;
    //            using (mutex = new Mutex(true, "JobInfo_Icon", out isNew))
                {
   //                 if (IsWindowsServer()==false)
                    {
                        if (IsSingleInstance())
                        {
                            Application.EnableVisualStyles();
                            Application.SetCompatibleTextRenderingDefault(false);
                            Application.Run(new Form1());

                        }
                        else
                        {
                            MessageBox.Show("Уже запущено", "JobInfo_Icon");
                        }
                    }
              
                }
            }
            catch //(Exception err)
            {

            }
            if (mutex != null)
            {
                mutex.ReleaseMutex();
                mutex = null;
            }

        }
    }
}
