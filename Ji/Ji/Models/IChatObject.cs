using Ji.Droid;
using System.ComponentModel;

namespace Ji.Models
{
    enum XTypeGroup { xtPersonalGroup, xt }
    interface IChatObject 
    {
        string Text { get; set; }
        string Description { get; set; }
        int ObjId { get; set; }
        MsgObjType TypeId { get; set; }
        Period period { get; set; }

    }
}