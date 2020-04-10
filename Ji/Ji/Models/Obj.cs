using Ji.ClassSR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ji.Models
{

    public class ResultObject 
{ 
    public Obj[] objs { get; set; }
}

public class Obj
    {
         public int ObjId { get; set; }
        public int MessageNum { get; set; }
        public int PageNum { get; set; }
        public int ShownState { get; set; }
        public int sgTypeId { get; set; }
        
        public int sgGruupId { get; set; }
 
        public int MsgId { get; set; }
        public int sgClassId { get; set; }

        public string Type { get; set; }
        public string Guid { get; set; }
        public string Parent_Guid { get; set; }
        public int Deep { get; set; }
        public Droid.Period period { get; set; }
        public string xml { get; set; }
        public string Description { get; set; }
        //   public LinksObj[] links;
        public int userid { get; set; }
        public string userCreater { get; set; }
        public int[] UsersInChat { get; set; }
        public int Vers { get; set; }
        public ViewUserStatistic  Statistic { get; set; }
    }
}
