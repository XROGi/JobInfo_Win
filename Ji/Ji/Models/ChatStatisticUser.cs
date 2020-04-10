using Ji.ClassSR;
using System;

namespace Ji.Models
{
    public partial class ChatStatisticUser
    {
        public ChatStatisticUser()
        {
        }
        public ChatStatisticUser(ViewUserStatistic v)
        {
            if (v == null)
                return;
            ChatId = v.ChatId;
            UserId = v.UserId;
            StartShownObjId = v.StartShownObjId;
            LastObjId = v.LastObjId;
            CountNew = v.CountNew;
            LastShownObjId = v.LastShownObjId;
            if (v.LastObjPage.HasValue)
                PageLastObj = v.LastObjPage.Value;
            if (v.LastShownPage.HasValue)
                PageLastShownObj = v.LastShownPage.Value;
            LastObjIdDateCreate = v.LastObjDate;
                
        }

        private int chatUserInfoIdField;

        private int chatIdField;

        private int userIdField;

        private int startShownObjIdField;

        private int lastShownObjIdField;

        private int lastObjIdField;

        private int countNewField;

        private int countShownEndObjIdField;
        //private DateTime LastObjIdDateCreate;
        public DateTime LastObjIdDateCreate { get; set; }

        public System.DateTime dt_Statistic;
        private ViewUserStatistic vus;

       

        /// <remarks/>
        public int ChatUserInfoId
        {
            get
            {
                return this.chatUserInfoIdField;
            }
            set
            {
                this.chatUserInfoIdField = value;
            }
        }

        /// <remarks/>
        public int ChatId
        {
            get
            {
                return this.chatIdField;
            }
            set
            {
                this.chatIdField = value;
            }
        }

        /// <remarks/>
        public int UserId
        {
            get
            {
                return this.userIdField;
            }
            set
            {
                this.userIdField = value;
            }
        }

        /// <remarks/>
        public int StartShownObjId
        {
            get
            {
                return this.startShownObjIdField;
            }
            set
            {
                this.startShownObjIdField = value;
            }
        }

        /// <remarks/>
        public int LastShownObjId
        {
            get
            {
                return this.lastShownObjIdField;
            }
            set
            {
                this.lastShownObjIdField = value;
            }
        }

        /// <remarks/>
        public int LastObjId
        {
            get
            {
                return this.lastObjIdField;
            }
            set
            {
                this.lastObjIdField = value;
            }
        }

        /// <remarks/>
        public int CountNew
        {
            get
            {
                return this.countNewField;
            }
            set
            {
                this.countNewField = value;
            }
        }

        /// <remarks/>
        public int CountShownEndObjId
        {
            get
            {
                return this.countShownEndObjIdField;
            }
            set
            {
                this.countShownEndObjIdField = value;
            }
        }

        public int PageLastObj { get; set; }
        public int PageLastShownObj { get; set; }
    }
}
