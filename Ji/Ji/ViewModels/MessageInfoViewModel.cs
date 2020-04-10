using Ji.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ji.ViewModels
{
    public class MessageInfoViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
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

  

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        bool isBusy;
        ObjMsg msg;
        public ObservableCollection<ObjMsg> ChatsHistory
        {
            get;
            set;
        }


        public MessageInfoViewModel(ObjMsg _msg)
        {
            isBusy = false;
            msg = _msg;
            Task.Run(async () => { await ExecuteLoadItemsCommand(); });

        }

        async private Task ExecuteLoadItemsCommand()
        {
            try
            {
                isBusy = true;

            }
            finally
            {
                isBusy = false;
            }
        }
    }
}
