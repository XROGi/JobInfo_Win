using Ji.ClassSR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ji.ViewModels
{
    class DebugConnectLodViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ConnectionLog> Items { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DebugConnectLodViewModel()
        {
            App.ddd.OnLogConnecttionAdd += Ddd_OnLogConnecttionAdd;
            Items = new ObservableCollection<ConnectionLog>();
      
        }

        private void Ddd_OnLogConnecttionAdd(ConnectionLog log)
        {
            Items.Add(log);// OnLogConnecttionAdd
            OnPropertyChanged(nameof(Items));
        }
    }
}
