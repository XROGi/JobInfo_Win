using Ji.Models;
using Ji.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Xamarin.Forms;

namespace Ji.ViewModels
{
    public class JobViewModel : INotifyPropertyChanged
    {
        
        public bool bBusy { get; set; }
        public bool b_JobNative { get; set; }
        public bool b_JobSvod { get; set; }
        
        public ObjMsg Msg { get; set; }
        public  ObjMsg NewMsg { get; set; }
        public ObservableCollection<UserChat> ListUser { get; set; }
        public DateTime DateEnd { get; set; }
        public JobViewModel(ObjMsg _Msg )
        {
            try
            {
                Msg = _Msg;
                NewMsg = new ObjMsg();
                NewMsg.sgTypeId = 58;
                ListUser = new ObservableCollection<UserChat>();
                DateEnd = DateTime.Now.AddDays(1).Date;
                b_JobNative = true; 
                b_JobSvod = true;
            }
            catch (Exception err)
            {
            }
        }


        public JobClassType JobClassTypeId { get; set; }
        public string ClassType
        {
            get
            {
                ;
                switch (JobClassTypeId)
                {
                    case (JobClassType.OneTime): return "Разовая сегодня";
                    case (JobClassType.AddToPlan): return "Добавить в план работ";
                    case (JobClassType.PeriodicalJob): return "Периодическая задача";
                    case (JobClassType.Plan): return "Плановая работа";
                    case (JobClassType.Speed): return "Внеплановая срочная";
                    case (JobClassType.VIP): return "Срочная VIP(+Контроль)";
                    default: return "Разовая сегодня";
                }

                return "Неизвестный тип";
            }
            set
            {
                switch (value)
                {
                    case "Разовая сегодня": JobClassTypeId = JobClassType.OneTime; break;
                    case "Добавить в план работ": JobClassTypeId = JobClassType.AddToPlan; break;
                    case "Периодическая задача": JobClassTypeId = JobClassType.PeriodicalJob; break;
                    case "Плановая работа": JobClassTypeId = JobClassType.Plan; break;
                    case "Внеплановая срочная": JobClassTypeId = JobClassType.Speed; break;
                    case "Срочная VIP(+Контроль)": JobClassTypeId = JobClassType.VIP; break;
                    default:
                        {
                            JobClassTypeId = JobClassType.OneTime; break;
                        }
                }
            }
        }


        public Command UserAdd
        {
            get
            {
                return new Command(async () =>
                {
                    GroupChat _newGroup = new GroupChat(NewMsg);
                    
                    var employeesServices = new UsersAddViewPage(_newGroup);
                    (Application.Current as App).MainPage = new NavigationPage(employeesServices); // or other navigation methods
                });
            }
        }
        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal int[] GetUserList()
        {
            List<int> ret = ListUser.Select(s => s.UserId).ToList();
            if (App.ddd!=null && App.ddd.UserId>0)
                ret.Add(App.ddd.UserId);
            return ret.ToArray();
        }

        internal void SetUserList(UserChat[] selected_UserChat)
        {
            foreach (UserChat d1 in selected_UserChat)
            {
                ListUser.Add(d1);

            }
            OnPropertyChanged(nameof(ListUser));
        }
        #endregion
    }
}