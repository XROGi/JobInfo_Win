using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Ji.Droid.XROGiClasses;
using Ji.Services;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(XToast))]

namespace Ji.Droid.XROGiClasses
{
   // http://qaru.site/questions/334427/toast-equivalent-on-xamarin-forms
    public class XToast : IXROGiToast
    {
        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            try
            {
                if (MainThread.IsMainThread)
                {
                    MyMainThreadCode(message);
                }
                else
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        try
                        {
                            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
                        }catch (Exception errr)
                        {

                        }
                    });
                    
                }


                //Activity.RunOnUiThread(() => {
                //    //details.Text = OStateUserID.name + "\n" + OStateUserID.population;

                //});

                //
            }
            catch (Exception err)
            {

            }
        }

        

        private void MyMainThreadCode(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }

   
    }
}