using Android.Content.PM;
using Ji.Models;
using Xamarin.Forms;

namespace Ji.Droid
{
    public class AndroidApplicationInfo : IMyApplicationInfo
    {
        public void closeApplication()
        {
            //  Android.OS.Process.KillProcess(Android.OS.Process.MyPid()); Quit(); ;
            // App.Quit(); ;
      //      if (Device.OS == TargetPlatform.Android)
            {
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            }
        }

        public string Get_clientname()
        {
            var context = Android.App.Application.Context;
            var VersionNumber = context.PackageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.MetaData).VersionName;
            var BuildNumber = context.PackageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.MetaData).VersionCode.ToString();
            var NamePack = context.PackageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.MetaData).PackageName;

            return NamePack  + " "+VersionNumber + "." + BuildNumber;
            //android: versionCode = "33" android: versionName = "1.0"


            //IOS
            /*var VersionNumber = NSBundle.MainBundle.InfoDictionary.ValueForKey(new NSString("CFBundleShortVersionString")).ToString();  
var BuildNumber = NSBundle.MainBundle.InfoDictionary.ValueForKey(new NSString("CFBundleVersion")).ToString();  */
        }
        public string Get_clientvers()
        {
            var context = Android.App.Application.Context;
            var VersionNumber = context.PackageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.MetaData).VersionName;
            var BuildNumber = context.PackageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.MetaData).VersionCode.ToString();
            var NamePack = context.PackageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.MetaData).PackageName;

            return  VersionNumber + "." + BuildNumber;
            //android: versionCode = "33" android: versionName = "1.0"


            //IOS
            /*var VersionNumber = NSBundle.MainBundle.InfoDictionary.ValueForKey(new NSString("CFBundleShortVersionString")).ToString();  
var BuildNumber = NSBundle.MainBundle.InfoDictionary.ValueForKey(new NSString("CFBundleVersion")).ToString();  */
        }
        //    [Android.Runtime.Register("myPid", "()I", "")]
        public string Get_MyPid()
        {
            string f = Android.OS.Process.MyPid().ToString();
            return f;
        }

        public string Get_AndroidMAC()
        {
            string mac = Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
            return mac;
        }
    }
}