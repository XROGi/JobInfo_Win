using Android.App;
using Android.Webkit;
using Android.Widget;
using System;

namespace Ji.Droid
{//https://stackoverflow.com/questions/27984800/xamarin-webviewclient-onloadresource-onpagefinished
    public class HelloWebViewClient : WebViewClient
    {
        public Activity mActivity;
        public HelloWebViewClient(Activity mActivity)
        {
            this.mActivity = mActivity;
        }

        [Obsolete]
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            view.LoadUrl(url);
            Toast.MakeText(mActivity, "Toast Message",
                                 ToastLength.Long).Show();
            return true;
        }
    }
}