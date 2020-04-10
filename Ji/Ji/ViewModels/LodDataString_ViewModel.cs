using Ji.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Ji.ViewModels
{
    public class LodDataString_ViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Log_DataSting> Items_Log { get; set; }

        public Command LoadItemsCommand { get; set; }



        public LodDataString_ViewModel()
        {
            Items_Log = new ObservableCollection<Log_DataSting>(); 
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            Task.Run(async () => { await ExecuteLoadItemsCommand(); });
        }

        async private Task ExecuteLoadItemsCommand()
        {


           
            Log_DataSting result;
            while (App.ddd.log_income.TryDequeue(out result))
            {
                Items_Log.Add(result);
            }


            //App.ddd.log_income.
            //.Add(new Log_DataSting() { Text = "!!!@@1" });
            //Items_Log.Add(new Log_DataSting() { Text = "!!!@@2" });
            //Items_Log.Add(new Log_DataSting() { Text = "!!!@@3" });
            return;
        }
       
        public event PropertyChangedEventHandler PropertyChanged;
    }
}