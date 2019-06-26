using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobInfo
{
    

    public class ChatDisconnectedException : Exception
    {
        public ChatDisconnectedException()
        {
        }

        public ChatDisconnectedException(string message)
            : base(message)
        {
        }

        public ChatDisconnectedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    static internal class MyExt
    {
        static public string GetPositions(this WS_JobInfo.User u)
        {
            if (u.positions == null) return "нет данных";
            return String.Join("\r\n", u.positions.Select(s => s.Position).ToArray());
        }
        static public string GetDepartament(this WS_JobInfo.User u)
        {
            //if (u.positions == null) return "нет данных";
            return "";
        }
        static public Image GetFoto(this WS_JobInfo.User u)
        {
            try
            {
                if (u.foto != null)
                    using (MemoryStream ms = new MemoryStream(u.foto))
                    {
                        //Image f = ScaleImage(Image.FromStream(ms), 40, 40);
                        Image f = Image.FromStream(ms);
                        return f;

                    }
            }catch (Exception err)
            {

            }
            return null;
        }
    }
    public class ChatWsFunctionException : Exception
    {
        public ChatWsFunctionException()
        {
        }

        public ChatWsFunctionException(string message)
            : base(message)
        {
        }
        public ChatWsFunctionException( Exception err, string TypeFunction , string functionName )
           : base(functionName, err)
        {
        }
        public ChatWsFunctionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
