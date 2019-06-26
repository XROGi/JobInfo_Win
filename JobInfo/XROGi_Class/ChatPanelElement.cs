using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobInfo.XROGi_Class
{
    public enum ChatPanelElementType { btnEndMessage, btnLastShown, btnScrollPoint, pnlMessageTextRegion }
    public class ChatPanelElement
    {
        public ChatPanelElementType ElementType;
        object Tag;
        public ChatPanelElement(ChatPanelElementType et)
        {
            ElementType = et;
        }
        public ChatPanelElement(ChatPanelElementType et,object _Tag)
        {
            ElementType = et;
            Tag = _Tag;
        }
        public int  GetTagInt ()
        {
            if (ElementType == ChatPanelElementType.btnScrollPoint)
            {
                return (int)Tag;
            }
            return -1;
        }
        public XMessageCtrl Get_Tag_MessageChatControl()
        {
            if (ElementType == ChatPanelElementType.pnlMessageTextRegion)
            {
                return Tag as XMessageCtrl;
            }
            throw new Exception("Тип 776");
        }
    }
}
