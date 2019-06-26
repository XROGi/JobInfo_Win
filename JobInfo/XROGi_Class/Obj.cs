using JobInfo.XROGi_Extensions;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace JobInfo
{
    internal class Obj
    { 
        
        /// <summary>
        ///  Создатель сообщения
        /// </summary>
        internal User user;

        public WS_JobInfo.Obj Tag;
        public long    id   { get; internal set; }
  //      public string guid { get; internal set; }
        public MsgType type { get; internal set; }
//        public string temp_string { get { if (temp_string == "") return "Не названный элемент"; else return temp_string; } internal set { temp_string = value; } }
     //   public string temp_string { get; internal set; }

        internal bool isShown()
        {
            return false;
        }


    }
    public enum MsgType
    {
        msg,
        chat,
        file,
        voice,
        gps,
        image,
    }

}