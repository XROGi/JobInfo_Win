//using System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Ji.Models
{
    public class SetupAppParam : INotifyPropertyChanged
    {
        public bool b_ModeProgrammer { get; set; }
        public bool isProgrammMode()
        {
            return b_ModeProgrammer;
        }

        public SetupAppParam()
        {
            //SaveParams = new Command(RunSaveParams());
            SaveParams  = new Command(async () => await SaveParamsNow(),()=>!b_BusySave);
            b_BusySave = false;
            b_ShowExceptionText = false;
            bBusySave = false;
           
        }
        bool bBusySave;
        bool b_BusySave
        {
            get { return bBusySave; }
            set
            {
                bBusySave = value;
                OnPropertyChanged();
                SaveParams.ChangeCanExecute();
            }
        }
        private async Task SaveParamsNow()
        {
            b_BusySave = true;

            if (Application.Current.Properties.ContainsKey("Setup_b_ShowExceptionText"))
                Application.Current.Properties["Setup_b_ShowExceptionText"] = b_ShowExceptionText;
            else
                Application.Current.Properties.Add("Setup_b_ShowExceptionText", b_ShowExceptionText);

            if (Application.Current.Properties.ContainsKey("Setup_b_ShowPushMessage"))
                Application.Current.Properties["Setup_b_ShowPushMessage"] = b_ShowPushMessage;
            else
                Application.Current.Properties.Add("Setup_b_ShowPushMessage", b_ShowPushMessage);


            if (Application.Current.Properties.ContainsKey("Setup_b_ModeProgrammer"))
                Application.Current.Properties["Setup_b_ModeProgrammer"] = b_ModeProgrammer;
            else
                Application.Current.Properties.Add("Setup_b_ModeProgrammer", b_ModeProgrammer);


            // Xamarin.Forms.Application.Current.Properties["Setup_b_ShowExceptionText"] = b_ShowExceptionText;
      //      Xamarin.Forms.Application.Current.Properties["Setup_b_ShowPushMessage"] = b_ShowPushMessage;
      //      Xamarin.Forms.Application.Current.Properties["Setup_b_ModeProgrammer"] = b_ModeProgrammer;

            await Application.Current.SavePropertiesAsync().ConfigureAwait(true);
       
            //await Task.Delay    (4000);
            //     await Application.Current.MainPage.DisplayAlert("Save", "Compleat", "OK");
            b_BusySave = false; 
            
        }
        public async Task LoadParams()
        {
            try
            {
                try
                {
                    b_BusySave = true;
                    
                  //  b_ModeProgrammer = false;
                    b_ShowExceptionText = false;
                    b_ShowPushMessage = true;
                    b_ModeProgrammer = System.Diagnostics.Debugger.IsAttached;

                    if (Application.Current.Properties.Count > 0)
                    {
                        if (Application.Current.Properties.ContainsKey("Setup_b_ShowExceptionText"))
                        {
                            b_ShowExceptionText = Convert.ToBoolean(Xamarin.Forms.Application.Current.Properties["Setup_b_ShowExceptionText"].ToString());
                        }

                        if (Application.Current.Properties.ContainsKey("Setup_b_ShowPushMessage"))
                        {
                            b_ShowPushMessage = Convert.ToBoolean(Xamarin.Forms.Application.Current.Properties["Setup_b_ShowPushMessage"]);
                        }


                        if (Application.Current.Properties.ContainsKey("Setup_b_ModeProgrammer"))
                        {
                            b_ModeProgrammer = Convert.ToBoolean(Xamarin.Forms.Application.Current.Properties["Setup_b_ModeProgrammer"]);
                        }

                    }
                    else
                    {
                        await SaveParamsNow().ConfigureAwait(false);
                    }


                    //    await Xamarin.Forms.Application.Current.SavePropertiesAsync();
                }
                catch (Exception err)
                {
                    App.Log(err);
                }
            }
            finally
            {
                //await Task.Delay    (4000);
                //     await Application.Current.MainPage.DisplayAlert("Save", "Compleat", "OK");
                b_BusySave = false;
            }

        }

        public Command SaveParams { get; }

        bool bShowExceptionText;
        public bool b_ShowExceptionText
        {
            get { return bShowExceptionText; }
            set { SetProperty(ref bShowExceptionText, value); }
           
        }

        bool bShowPushMessage;

        public bool b_ShowPushMessage 
        {
            get { return bShowPushMessage; }
            set { SetProperty(ref bShowPushMessage, value); }
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
        #endregion
    }

}