using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Ji.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";// + Device.Android;
  //          client.
           OpenWebCommand = new Command(() => Launcher.OpenAsync(new Uri("https://www.svod-int.ru")));

            try
            {
                Test();
            }
            catch (Exception)
            {

                
            }
         
        }

        private async void Test()
        {
            //ServiceReference1.ChatLevelServiceClient c = new ServiceReference1.ChatLevelServiceClient();
            //string a = await c.AboutAsync().ConfigureAwait(false);
        }

        public ICommand OpenWebCommand { get; }
    }
}