using Ji.Droid;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Ji.Models
{

    /*
     SgnId	Name
7	Общий чат
8	Приватный чат
9	Доска обьявлений
10	"Стол заказов"
15	Избранное
30	Корпоративный чат
32	Папка
43	Блокнот
58	Задача
*/
    public enum MsgObjType { PublicChat=7, PrivateChatd=8
                            , BoardInfo = 9
                            , StolZakazov = 10
                            , Favorite = 15
                            , CorporativeChat = 30
                            , Folder = 32
                            , Notebook = 43
                            , Job = 58

    }; 
    public enum JobClassType
    {
        OneTime =68,
        AddToPlan=69,
        PeriodicalJob = 70,
        Plan=71,
        Speed=72,
        VIP=73
// 68	Разовая сегодня
//69	Добавить в план работ
//70	Периодическая задача
//71	Плановая работа
//72	Внеплановая срочная
//73	Срочная VIP(+Контроль)

    };
    public class GroupChat : IChatObject, INotifyPropertyChanged
    {
        string _Text;
        string _TextParsed = null;
        string _Description;
        int[] _UserList;


        public delegate void OnReciveStatisticUpdateDelegete(ChatStatisticUser stat);
        public event OnReciveStatisticUpdateDelegete OnReciveStatisticUpdate;

        public void ReciveStatisticUpdate(ChatStatisticUser stat)
        {
            OnReciveStatisticUpdate?.Invoke(stat);
        }
        public GroupChat()
        {
        }
            public GroupChat(Obj   chat )
        {
           // GroupChat gchat = new GroupChat
            {
                Text = chat.xml;
                ObjId = chat.ObjId;
                period = chat.period;
                TypeId = (MsgObjType)chat.sgTypeId;
                UserList = chat.UsersInChat;
                Description = "sds";
                if (chat.Statistic != null)
                    Statistic = new ChatStatisticUser(chat.Statistic);
                //else
                //    Statistic = null;
            };
        }
        public GroupChat(ObjMsg obj)
        {
            if (obj != null)
            {
                Text = obj.Text;
                TypeId = (MsgObjType) obj.sgTypeId;
                // Text = s.xml,
                ObjId = obj.ObjId;
                if (obj.period  != null)
                    period = new Period() { dtb = obj.period.dtb, dte = obj.period.dte, dtc = obj.period.dtc, dtd = obj.period.dtd };
                CountNewMessage = "";
            }
        }


        public int[] UserList
        {
            set { Users_SetRange(value); }
            get { return _UserList; }
        }
    
        public string Text
        {
            get { return GetText(); }
            set { _Text = value.Trim(); }
        }

        string _nCount;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        ChatStatisticUser _Statistic;
        public ChatStatisticUser Statistic 
        {
            get 
            { 
                return _Statistic;
            }
            set
            {
                
                _Statistic = value;
                CountNewMessage = _Statistic.CountNew.ToString();
                ReciveStatisticUpdate(_Statistic);
                OnPropertyChanged();
               
            }
        }
        public string CountNewMessage
        {
            get { return GetCountNew(); }
            set
            {
                if (value != null)
                {
                    if (value != _nCount)
                    {
                        _nCount = value.Trim();
                        OnPropertyChanged(nameof(CountNewMessage));
                    }
                }
            }
        }
        



        public string GetCountNew()
        {
            return _nCount;
        }
            private string GetText()
        {
            try
            {
                if (_TextParsed != null)
                    return _TextParsed;

                if (!(_Text.Trim().StartsWith("<")))
                {
                    return _Text;
                }

                TextReader tr = new StringReader(_Text);
                XDocument xDocument = XDocument.Load(tr);

                //     var xDocument = new XDocument(o.xml); ;
                /*       string out_text = xDocument.ToString();
                       if (!out_text.StartsWith("<root>"))
                       {
                           out_text = "<root>" + out_text + "</root>";
                   //        return out_text;
                       }
                       */
                //      return out_text;

                var varText = from x in xDocument.XPathSelectElements("//text") select x.Value;
                if (varText.Any())
                {

                    string txt = varText.First().ToString();
                    /*if (!txt.StartsWith("<root>"))
                    {
                        txt = "<root>" + txt + "</root>";
                    }*/
                    _TextParsed = txt;
                    return txt;
                }
                return "???-41-???";
                //else
                //{
                //    var varSmile = from x in xDocument.XPathSelectElements("//smile") select x.Value;
                //    if (varSmile.Any())
                //    {
                //        return varSmile.First().ToString();
                //    }

                //}
            }
            catch (Exception err)
            {
                _TextParsed = "Ошибка представления текста[" + _Text + "]";
                return _TextParsed;
            }
            return "Тип не определён[" + _Text + "]";
        }

        public int ObjId        { get; set ; }
        public MsgObjType TypeId       { get; set ; }
        public JobClassType JobClassTypeId { get; set; }

        /*
         7	Общий чат
8	Приватный чат
9	Доска обьявлений
10	"Стол заказов"
15	Избранное
30	Корпоративный чат
32	Папка
43	Блокнот
             */
        public string Type
        {
            get
            {

                switch (TypeId)
                {
                    case (MsgObjType.PublicChat):     return "Общий чат";
                    case (MsgObjType.PrivateChatd):       return "Приватный чат";
                    case (MsgObjType.BoardInfo):       return "Доска обьявлений";
                    case (MsgObjType.StolZakazov):       return "Стол заказов";
                    case (MsgObjType.Favorite):      return "Избранное";
                    case (MsgObjType.CorporativeChat):      return "Корпоративный чат";
                    case (MsgObjType.Folder):      return "Папка";
                    case (MsgObjType.Notebook): return "Блокнот";
                    case (MsgObjType.Job): return "Задача";
                    default: return "Неизвестный тип";
                }

                return "Неизвестный тип";
            }
        }
        public string ClassType
        {
            get
            {
                ;
                switch (JobClassTypeId)
                {
                    case (JobClassType.OneTime      ): return "Разовая сегодня";
                    case (JobClassType.AddToPlan    ): return "Добавить в план работ";
                    case (JobClassType.PeriodicalJob): return "Периодическая задача";
                    case (JobClassType.Plan         ): return "Плановая работа";
                    case (JobClassType.Speed        ): return "Внеплановая срочная";
                    case (JobClassType.VIP          ): return "Срочная VIP(+Контроль)";
                    default: return "Разовая сегодня";
                }

                return "Неизвестный тип";
            }
            set
            {
                switch (value)
                {
                    case "Разовая сегодня": JobClassTypeId = JobClassType.OneTime; break;
                    case "Добавить в план работ":   JobClassTypeId = JobClassType.AddToPlan; break;
                    case "Периодическая задача":    JobClassTypeId = JobClassType.PeriodicalJob; break;
                    case "Плановая работа":         JobClassTypeId = JobClassType.Plan; break;
                    case "Внеплановая срочная":     JobClassTypeId = JobClassType.Speed; break;
                    case "Срочная VIP(+Контроль)":  JobClassTypeId = JobClassType.VIP; break;
                default:
                        {
                            JobClassTypeId = JobClassType.OneTime; break;
                        }
                }
            }
        }
        

        public Period period   { get; set ; }
        public string Description { get => _Description; set => _Description = value; }

        internal void Users_SetRange(int[] _users)
        {
           if (UserList == null)
            {
                _UserList = _users;
            }
           else
            {
                _UserList = null;
                _UserList = _users;
            }
        }

        internal int[] GetUsers()
        {
            return UserList;
        }

        //System.Uri _ImageUrl;
        //public System.Uri ImageUrl
        //{
        //    set {
        //        _ImageUrl = value;
        //    }

        //    get { return new Uri("chat48.png"); }

        //}

        string _ImageUrl;
        public string ImageUrl
        {
            set
            {
                _ImageUrl = value;
            }

            //            get { return "chat48.png"; }
            get
            {


                if (TypeId == MsgObjType.PrivateChatd)
                    return "http://194.190.100.194/xml/GetImgChat.ashx?id=" + ObjId.ToString() + "&uid=" + App.ddd.SR.UserId.ToString();
                if (TypeId == MsgObjType.Job)
                    return "setup.png";

                return "IconGroup512x512.png";
            }

            // 
        }
    }
}
