//using Android.Support.V7.Widget;
using Ji.ClassSR;
using Ji.Services;
using Ji.Views;
using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Services;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using Xamarin.Forms;

namespace Ji.Models
{
    
    
    public class ObjMsg : INotifyPropertyChanged
    {

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        int taps = 0;
        Command tapCommand;
        public ObjMsg()
        {
            // configure the TapCommand with a method
            tapCommand = new Command(OnTapped);
            SelectedItemMsg = new Command(async () => await ExecuteLoadItemsCommand());
            TapImageCommand = new Command(OpenPopup);
            JobCompleatCommand = new Command((arg) => {

                Console.WriteLine(arg);

            });
        }

        private async void OpenPopup(object obj)
        {
            Debug.WriteLine("Opening popup ");
            MessagePopupMenuView popupProperties = new MessagePopupMenuView();
            var scaleAnimation = new ScaleAnimation
            {
                PositionIn = MoveAnimationOptions.Right,
                PositionOut = MoveAnimationOptions.Left
            };

            popupProperties.Animation = scaleAnimation;

            await PopupNavigation.Instance.PushAsync(popupProperties);

            //     void ShowPopup_Clicked(object sender, EventArgs e) => Popup?.ShowPopup(sender as View);
            //      string result = await DisplayActionSheet("", "", null, "", "", "");
            //PopupMenu menu = new PopupMenu(this, showPopupMenu);
            //    var action = await DisplayActionSheet("ActionSheet: Send to?", "Cancel", null, "Email", "Twitter", "Facebook");
            //     Debug.WriteLine("Action: " + action);
        }

        async private Task ExecuteLoadItemsCommand()
        {

        }
        public Command TapImageCommand { get; private set; }

        public Command SelectedItemMsg { get; set; }


        public ICommand JobCompleatCommand { get; set; }
        public ICommand JobBeginCommand { get; set; }

        public ICommand JobCancelCommand { get; set; }

        public bool isJobWait
        {
            get
            {
                /*if (JobStatusId == 63)
                    return true;
                else
                    return false;
                */
                if (b_isMessgeJobTemplate.HasValue == false)
                    return false;
                else
                {
                    if (JobStatusId == 63)
                        return false;
                    else
                         if (JobStatusId == 64)
                            return false;
                    return true;
                }
                return true;
            }
        }
        public bool isJobInWork { 
            get
            {
                if (b_isMessgeJobTemplate.HasValue == false) return false;
                else

                {
                    if (JobStatusId == 63)
                        return true;
                    else
                        return false;
                }
                return false ;
            }
        }

        private System.Threading.Timer timer1sec;

        Command getInform;
        bool ? b_isMessgeJobTemplate;
        internal bool isMessgeJobTemplate()
        {
            if (b_isMessgeJobTemplate.HasValue)
            {
                return b_isMessgeJobTemplate.Value;
            }
            int? isJobReplyObkId = links.Where(s => s.sgLinkTypeId == 19 && s.objid_to == ObjId).FirstOrDefault()?.objid_from;
            if (isJobReplyObkId.HasValue)
            {
                b_isMessgeJobTemplate = true;
                //links.Where(s => s.sgLinkTypeId == 19 && s.objid_to == ObjId).FirstOrDefault()?.
                Xamarin.Forms.Device.StartTimer(TimeSpan.FromMilliseconds(1000 ), TimerElapsed);
          
           /*     timer1sec = new System.Threading.Timer(_ => {

                    Job_GetTimerEnd = DateTime.Now.ToShortTimeString();
                    //callback();
                });
                StartTimer(TimeSpan.FromMilliseconds(1000));
                StopTimer();
                */
                return true;
            }
            return false;
        }

        
        private bool TimerElapsed()
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                //Job_GetTimerEnd = DateTime.Now.ToLongTimeString();

                if (Status != null)
                {
                    var t = Status.Where(s => s.sgTypeId == 61).OrderByDescending(s=>s.dtb).FirstOrDefault();
                    if (t != null)
                    {
                        TimeSpan ts;
                        string elapsedTime;
                        if (t.sgMsgStatusId != 65)
                        {
                            ts = DateTime.Now - t.dtb;
                            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
                            //ts.Milliseconds / 10); ; ; ; ;
                            Job_GetTimerEnd = elapsedTime;
                        }
                        else
                        {
                            Job_GetTimerEnd = "";
                        }
                        ts = DateTime.Now - Status.Last().dtb;
                        elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
                        Job_GetTimeAll = elapsedTime;


                    }
                }
                else

                {
                    Job_GetTimeAll = Job_GetTimerEnd = "";;
                }
                //  TimeSpan ts = stopWatch.Elapsed;

                // Format and display the TimeSpan value.
                //string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                //    ts.Hours, ts.Minutes, ts.Seconds,
                //    ts.Milliseconds / 10);


            });
            return true;// to keep timer reccurring
            //return false to stop timer
        }
        void StartTimer(TimeSpan time)
        {
            timer1sec.Change(TimeSpan.FromMilliseconds(1000), TimeSpan.Zero);
        }
        void StopTimer()
        {
            timer1sec.Change(TimeSpan.Zero, TimeSpan.Zero);
        }

        string _strJobState;
        public string strJobState { get { return _strJobState; } }


        string _strJobWorker;
        public string strJobWorker { get { return _strJobWorker; } }

        

        public Command GetInform
        {
            get
            {
                return getInform;
            }
            set
            {
                getInform = value;
            }
        }

        public Command TapCommand
        {
            get {
                return tapCommand;
            }
            set
            {
                tapCommand = value;
            }
        }
        void OnTapped(object s)
        {
            taps++;
            Debug.WriteLine("parameter: " + s);
        }


        static int[] status = new int[] { 12, 14 };

        public int ObjId { get; set; }
        public int ParentObjId { get; set; }

        public string Data { get; set; }

        /*        public string FIO { get; set; }
                public string Skill { get; set; }
                public string OU { get; set; }

                private bool b_ImageDefault = false;
                */


        public int sgClassId { get; set; }
        public int sgTypeId { get; set; }
        public int sgGruupId { get; set; }
        public string Type { get; set; }
        public string Guid { get; set; }
        public string Parent_Guid { get; set; }
        public int Deep { get; set; }
        public Ji.Droid.Period period { get; set; }

        int _ShownState;
        /// <summary>
        /// Статус показа /досьавки сообщеения
        /// 0- создано, не отправлено
        /// 10- поставлено в очередь на отправку
        /// 20- доставлено на сервер
        /// 30- доставлено уведомлением к клиенту
        /// 40-получено но не просмотрено
        /// 50-просмотрено
        /// 60-скрыто
        /// 70-удалено
        /// </summary>
        /// 
        public int ShownState

        {
            get
            {
                return _ShownState;

            }
            set
            {
                if (status.Contains(value))
                {
                    if (_ShownState != value)
                    {
                        _ShownState = value;

                    }
                }
                else
                {
                    _ShownState = 12;

                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(Message_GetSendState));
            }


        }


        public bool b_isStatusLoaded
        {
            get
            { 
                return Status != null; 
            } 
        }

        int JobStatusId { get; set; }

        ObjStatus[] _Status;
        public ObjStatus[] Status
        {
            get { return _Status; }
            set {
                _Status = value;
                ObjStatus state = _Status.Where(s => s.sgTypeId == 61).FirstOrDefault();
                if (state == null)
                {
                    _strJobState = "Выбор исполнителя";
                    _strJobWorker = "";

                }
                else
                {
                    _strJobState = state.Name;
                    _strJobWorker = state.UserName;
                    JobStatusId = state.sgMsgStatusId;
                }
                OnPropertyChanged(nameof(b_isStatusLoaded));
                OnPropertyChanged(nameof(strJobState));
                OnPropertyChanged(nameof(strJobWorker));

                OnPropertyChanged(nameof(isJobInWork));
                OnPropertyChanged(nameof(isJobWait));


            }
        }


        public DateTime DateCreate { get; set; }
        //    public Period period;

        string _source_xml;
        public string xml
        {
            get { return _source_xml; }
            set { _source_xml = value; Data = _source_xml; }
        }

        //    public LinksObj[] links;
        public int userid { get; set; }
        string _userCreater;
        public string userCreater
        {
            get { return _userCreater; } set { _userCreater = value; }
        }

        internal bool IsMyMessage()
        {
            try
            {

                return userid == App.ddd.UserId;
            } catch (Exception err)
            {

            }
            return false;
        }

        //  enum MsgContentTypeEnum { MessgeText, MessgeImage, MessgeInfo, MessgeGPS };
        internal MsgContentTypeEnum GetTypeMsg()
        {
            if (Data != null)
            {
                int start = Data.ToLower().IndexOf("fileid=");
                if (start > 0)
                    return MsgContentTypeEnum.MessgeImage;
            }
            //  if (ObjId % 5 == 0)
            return MsgContentTypeEnum.MessgeText;
            //  //   if (ObjId % 2 == 0)
            //         return MsgContentTypeEnum.MessgeInfo;
            //    // if (ObjId % 3 == 0)
            ////     if (ObjId % 4 == 0)
            //         return MsgContentTypeEnum.MessgeGPS;

            //     return MsgContentTypeEnum.MessgeText;
        }

        public int[] UsersInChat { get; set; }
        public int MessageNum { get; set; }
        public int PageNum { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ImageURL
        {
            get => App.ddd.SOAP_SoapServer() + "/xml/GetUserImage.ashx?id=" + userid + "&tiket=" + App.ddd.connectInterface.TokenSeanceId;
        }
        public string MessageImage_KeyName

        {
            get
            {
                /*<root>
      <img ver="1.0" fileid="a226bdfc-7606-4998-b924-795a8bd00dee" />
      <text ver="1.0">Изображение</text>
    </root>
    */
                try
                {
                    //"http://194.190.100.194/1.png"
                    string fi = "fileid=";
                    int start = Data.IndexOf(fi);
                    if (start > 0)
                    {
                        int end = Data.IndexOf("\"", start + fi.Length + 1);
                        string s = Data.Substring(start + fi.Length + 1, end - (start + fi.Length + 1));
                        return "GetFileData.ashx?id=" + s;
                    }
                    else

                    {
                        return "";

                    }
                }
                catch (Exception err)
                {
                    return "";
                }
                return "";
            }
        }
        public string MessageImage_GetImageURL

        {
            get
            {
                /*<root>
      <img ver="1.0" fileid="a226bdfc-7606-4998-b924-795a8bd00dee" />
      <text ver="1.0">Изображение</text>
    </root>
    */
                try
                {
                    //"http://194.190.100.194/1.png"
                    string fi = "fileid=";
                    int start = Data.IndexOf(fi);
                    if (start > 0)
                    {
                        int end = Data.IndexOf("\"", start + fi.Length + 1);
                        string s = Data.Substring(start + fi.Length + 1, end - (start + fi.Length + 1));
                        string url = App.ddd.SOAP_SoapServer() + "/xml/GetFileData.ashx?" + "tiket=" + "3545647155565&nmessage=" + App.ddd.connectInterface.TokenSeance_Counter + "&id=" + s;
                        return url;// "http://194.190.100.194/xml/GetFileData.ashx?" + "tiket=" + App.ddd.connectInterface.TokenSeanceId + "&nmessage=" + App.ddd.connectInterface.TokenSeance_Counter + "&id=" + s;
                    }
                    else

                    {
                        return "";

                    }
                } catch (Exception err)
                {
                    return "";
                }
                return "";
            }
        }
        public string Message_GetTime
        {
            get
            {
                if (period != null && period.dtb.HasValue)
                {
                    return period.dtb.Value.ToString("HH:mm");
                }
                else
                    return "__:__";
            }

        }


        string _Job_GetTimerEnd;
        public string Job_GetTimerEnd { set
            {
                if (_Job_GetTimerEnd != value)
                {
                    _Job_GetTimerEnd = value;
                    OnPropertyChanged();
                }
            }
            get
            {
                return _Job_GetTimerEnd;
            }
        }
        string _Job_GetTimeAll;
        public string Job_GetTimeAll
        {
            set
            {
                if (_Job_GetTimeAll != value)
                {
                    _Job_GetTimeAll = value;
                    OnPropertyChanged();
                }
            }
            get
            {
                return _Job_GetTimeAll;
            }
        }
        public string Message_GetSendState
        {
            get
            {
                if (_ShownState == 0) return @"send0.jpg";
                if (_ShownState == 12) return @"send1.png";
                if (_ShownState == 14) return @"send3.png";
                return _ShownState.ToString();
            }

        }
        public string DebugInfo
        {
            get
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    return PageNum.ToString() + " " + ObjId.ToString();
                }
                return String.Empty;
            }

        }


        public String Text
        {
            get
            {
                return MessageImage_GetImageText;
            }
            set
            {
                xml = "<root><text>" + value + "</text></root>";
            }
        }
        public String Description
        {
            get;set;
            
        }

        

        public string MessageImage_GetImageText

        {
            get
            {
                /*<root>
      <img ver="1.0" fileid="a226bdfc-7606-4998-b924-795a8bd00dee" />
      <text ver="1.0">Изображение</text>
    </root>
    */
                if (Data != null)
                {
                    try
                    {
                        string text = "";
                        XmlDocument d = new XmlDocument();
                        d.LoadXml(Data);
                        XmlNodeList n = d.GetElementsByTagName("text");
                        if (n != null)
                        {

                            foreach (XmlNode r in n)
                            {
                                text += r.InnerText;
                                text += " ";
                                //    Console.WriteLine(r.InnerText);
                            }
                        }
                        return text.Trim();
                    } catch (Exception err)
                    {
                        return String.Empty;
                    }
                }
                return String.Empty;

            }
        }

        public GroupChat Chat { get; internal set; }
        public LinksObj [] links { get; set; }
    }
}
