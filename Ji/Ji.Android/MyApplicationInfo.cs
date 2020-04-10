using Android.Content.PM;
using Ji.Models;

namespace Ji.Droid
{
    public class MyApplicationInfo : IMyApplicationInfo
    {
        public void closeApplication()
        {
            ;
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

        //    [Android.Runtime.Register("myPid", "()I", "")]
        public string Get_MyPid()
        {
            string f = Android.OS.Process.MyPid().ToString();
            return f;
        }
    }
}