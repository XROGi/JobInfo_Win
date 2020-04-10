using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ji.ClassSR
{
    public class SRMessageLog : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public SRMessageLog()
        {
            TimeCreate = DateTime.Now.ToString("hh:MM:ss");
        }
        public String TimeCreate { get; set; }
        public String FunctionName { get; set; } 
        public String FunctionParam { get; set; }
    }
}
