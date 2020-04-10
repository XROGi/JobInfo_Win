using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ji.Models
{
    public class ChatPagesController
    {
        int DeltaNeedReload = 10;

        public delegate void OnChat_ShowMsgInListDelegete(int chat_objId, int PageNum , ObjMsg[] msg);
        public event OnChat_ShowMsgInListDelegete OnChat_ShowMsgInList;

        public delegate void OnChat_PageCompleatDelegete(int chat_objId, int PageNum);
        public event OnChat_PageCompleatDelegete OnChat_PageCompleat;


        int ChatId;
        int endPageNum;
        ChatPages[] pages;//= App.ddd.DB_MessageGetPages(chat.ObjId);
        SortedList<int, int> PagesListInt = null;

        public ChatPagesController(int chatid)
        {
            try
            {
                ChatId = chatid;
                PagesListInt = new SortedList<int, int>();
                App.ddd.OnNewPageRecive += OnNewPageRecive;

                pages = App.ddd.DB_MessageGetPages(ChatId);
                if (pages != null)
                    foreach (ChatPages page in pages)
                    {
                        PagesListInt.Add(page.PageNumber, page.PageNumber);
                    }
            }
            catch (Exception err)
            {
            }
        }
        public void Close()
        {
            if (App.ddd!=null)
                   App.ddd.OnNewPageRecive -= OnNewPageRecive;
        }


        public void Chat_GetPage(int PageNum)
        {
            try
            {
                if (isPageLoad(PageNum))
                {
                    //1. Выводим кэшированное
                    ObjMsg[] msgs;
                    msgs = App.ddd.DB_MessageGetPage(ChatId, PageNum); ;

                    if (OnChat_ShowMsgInList != null)
                        OnChat_ShowMsgInList(ChatId, PageNum, msgs);
                    int maxObjId = msgs.Max(s => s.ObjId);
                    if (PageNum == PageNum_MaxReq_Db())
                    {
                        ObjMsg[] finishmsg = App.ddd.Message_GetPage(ChatId, PageNum);

                        //2. Выводим свежее из последноего что могло прийти

                        App.ddd.DB_MessageAddNewInPage(ChatId, PageNum, finishmsg.Where(s => s.ObjId > maxObjId).ToArray());
                        if (OnChat_ShowMsgInList != null)
                            OnChat_ShowMsgInList(ChatId, PageNum, finishmsg.Where(s => s.ObjId > maxObjId).ToArray());
                    }
                    
                    //смысл не понятен
                    if (OnChat_PageCompleat != null)
                        OnChat_PageCompleat(ChatId, PageNum);
                }
                else
                {
                    ObjMsg[] msgs;
                    msgs = 
                        App.ddd.Message_GetPage(ChatId, PageNum); ;
                  
                   
                    //смысл не понятен
                    //if (OnChat_PageCompleat != null)
                    //OnChat_PageCompleat(ChatId, PageNum);

                }
            }
            catch (Exception err)
            {
            }
        }

        private void OnNewPageRecive(ObjMsg[] msgs)
        {
            if (msgs != null && msgs.Length > 0)
            {
                App.ddd.DB_MessageSavePage(ChatId, msgs[0].PageNum, msgs);
                if (OnChat_ShowMsgInList != null)
                    OnChat_ShowMsgInList(ChatId, msgs[0].PageNum, msgs);
            }
        }

        public void SetMaxPageNum(int EndPageNum)
        {
            endPageNum = EndPageNum;
        }

        /// <summary>
        /// Последняя скаченная страница (возможно не полная)
        /// </summary>
        /// <returns></returns>
        public int PageNum_MaxReq_Db()
        {
            try
            {
                return PagesListInt.Keys.Max();
            }
            catch (Exception err)
            {

            }
            return 0;
        }


        public bool isPageLoad(int npage)
        {
            try
            {
                if (PagesListInt.ContainsKey(npage))
                {
                    if (isEndofList(npage) == true)
                    {
                        return false;
                    }
                    else
                        return true;

                }
                else
                    return false;
            }
            catch (Exception err)
            {
                
            }
            return false;
        }

        private bool isEndofList(int npage)
        {
            try
            {
                if (npage == PageNum_MaxReq_Db())
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception err)
            {

            }
            return false;
        }
    }
}
