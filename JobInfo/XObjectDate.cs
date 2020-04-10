using System;

namespace JobInfo
{
    internal class XObjectDate
    {
        public int userid;
        public  DateTime DateUpdate;

        public XObjectDate(int userid, DateTime now)
        {
            this.userid = userid;
            this.DateUpdate = now;
        }
    }
}