using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ji.ClassSR
{
    public class ConnectionLog : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public DateTime DateCreate { get; set; }

        DateTime _DateBeginConnect;
        DateTime _DateConnected;
        DateTime _DateErrorConnected;
        DateTime _DateDisConnected;
        DateTime _DateBeginReConnect;
        DateTime _DateReConnected;
        string _event;

        public DateTime DateBeginConnect { get { return _DateBeginConnect; } set { _DateBeginConnect = value; OnPropertyChanged(); } }
        public DateTime DateConnected { get { return _DateConnected; } set { _DateConnected = value; OnPropertyChanged(); } }
        public DateTime DateErrorConnected { get { return _DateErrorConnected; } set { _DateErrorConnected = value; OnPropertyChanged(); } }
        public DateTime DateDisConnected { get { return _DateDisConnected; } set { _DateDisConnected = value; OnPropertyChanged(); } }
        public DateTime DateBeginReConnect { get { return _DateBeginReConnect; } set { _DateBeginReConnect = value; OnPropertyChanged(); } }
        public DateTime DateReConnected { get { return _DateReConnected; } set { _DateReConnected = value; OnPropertyChanged(); } }
        public int nReConnected { get; set; }

        
        public String  Event { get { return _event; } set { _event  = value; OnPropertyChanged(); } }


        public Exception ErrorException { get; set; }
        public ConnectionLog()
        {
            DateCreate = DateTime.Now; nReConnected = 0;
            Event = "Создано";
        }
    }
}
