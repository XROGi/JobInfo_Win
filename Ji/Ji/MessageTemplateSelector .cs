using Ji.Models;
using System.Linq;
using Xamarin.Forms;

namespace Ji
{
   
     public class MessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MessgeTextTemplateMy { get; set; }
        public DataTemplate MessgeTextTemplate { get; set; }
        public DataTemplate MessgeTextTemplateChat { get; set; }
        public DataTemplate MessgeImageTemplate { get; set; }
        public DataTemplate MessgeImageTemplateMy { get; set; }
        public DataTemplate MessgeInfoTemplate { get; set; }
        public DataTemplate MessgeGPSTemplate { get; set; }
        public DataTemplate JiModelsObjMsg { get; set; }
        public DataTemplate MessgeJobTemplate { get; set; }
        public DataTemplate MessgeMainJobTemplate { get; set; }

         

        //public DataTemplate NormalTweetTemplate { get; set; }

        //public DataTemplate PromotedTweetTemplate { get; set; }

        //public DataTemplate RetweetTemplate { get; set; }

        public MessageTemplateSelector()
        {
            MessgeTextTemplateMy = new DataTemplate(typeof(ObjMsg));
            MessgeTextTemplate = new DataTemplate(typeof(ObjMsg));
            MessgeImageTemplate = new DataTemplate(typeof(ObjMsg));
            MessgeImageTemplateMy = new DataTemplate(typeof(ObjMsg));
            MessgeInfoTemplate = new DataTemplate(typeof(ObjMsg));
            MessgeGPSTemplate = new DataTemplate(typeof(ObjMsg));
            JiModelsObjMsg = new DataTemplate(typeof(ObjMsg));
            MessgeTextTemplateChat = new DataTemplate(typeof(ObjMsg));
            MessgeJobTemplate = new DataTemplate(typeof(ObjMsg));
            MessgeMainJobTemplate = new DataTemplate(typeof(ObjMsg));
        }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is ObjMsg msg)
            {
              //  return MessgeTextTemplate; ;
          //      ObjMsg msg = item as ObjMsg;

            //    if (msg != null)
                {
                    bool b_MyMessage = msg.IsMyMessage();
                    if (msg.links != null)
                    {
                        int? isMainReplyObkId = msg.links.Where(s => s.sgLinkTypeId == 19 && s.objid_from==msg.ObjId).FirstOrDefault()?.objid_from;
                        if (isMainReplyObkId.HasValue)
                        {
                            return MessgeMainJobTemplate;
                        }
                        if (msg.isMessgeJobTemplate()==true)
                        {
                            return MessgeJobTemplate;
                        }
                        

                    }
                    MsgContentTypeEnum t = msg.GetTypeMsg();
                    if (b_MyMessage == false)
                    {
                        switch (t)
                        {
                            case MsgContentTypeEnum.MessgeText:
                                {
                                    //return MessgeTextTemplate;
                                    if (msg.Chat != null)
                                    {
                                        if (msg.Chat.TypeId ==  MsgObjType.PublicChat  || msg.Chat.TypeId == MsgObjType.Job)
                                        {
                                            return MessgeTextTemplateChat;
                                        }
                                        if (msg.Chat.TypeId == MsgObjType.PrivateChatd)
                                        {
                                            return MessgeTextTemplate;
                                        }
                                        //  string data = msg.Chat.Type;
                                    }
                                    return MessgeTextTemplateChat;
                                }
                            case MsgContentTypeEnum.MessgeImage:
                                return MessgeImageTemplate;
                            case MsgContentTypeEnum.MessgeInfo:
                                return MessgeInfoTemplate;
                            case MsgContentTypeEnum.MessgeGPS:
                                return MessgeInfoTemplate;
                            default:
                                return MessgeTextTemplate;
                        }
                    }
                    else
                    {
                     
                        switch (t)
                        {
                            case MsgContentTypeEnum.MessgeText:
                                return MessgeTextTemplateMy;
                            case MsgContentTypeEnum.MessgeImage:
                             //       return MessgeTextTemplateMy;
                            return MessgeImageTemplateMy;
                            default:
                                return MessgeTextTemplateMy;
                        }
                    }
                    if (t == MsgContentTypeEnum.MessgeText)
                    {

                    }
                    return MessgeTextTemplate;
                }
               
              
            }
            else

                {
                return MessgeTextTemplate;
            }
             

            //if (item is PromotedTweet)
            //    return MessgeImageTemplate;

            //if (item is Retweet)
            //    return MessgeInfoTemplate;

            return null;
        }
    }
    enum MsgContentTypeEnum { MessgeText, MessgeImage, MessgeInfo, MessgeGPS , MessgeJob };
}
