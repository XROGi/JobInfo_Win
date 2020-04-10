using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Firebase.Messaging;

namespace Ji.Droid
{
    //[Service]
    //[IntentFilter(new[] { "com.google.firemase.MESSAGING_EVENT" })]

    //class JiFirebaseMessagimgService: FirebaseMessagingService
    //{
    //    [Obsolete]
    //    public override void OnMessageReceived(RemoteMessage p0)
    //    {
    //        base.OnMessageReceived(p0);

    //        SendNotification(p0.GetNotification().Body);
    //    }

    //    [Obsolete]
    //    private void SendNotification(string body)
    //    {
    //        var intent = new Intent(this, typeof(MainActivity));
    //        intent.AddFlags(ActivityFlags.ClearTop);
    //        var pendingintent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);
    //        var defSoundURL =RingtoneManager.GetDefaultUri(RingtoneType.Notification);
    //        var notificationbuilder = new NotificationCompat.Builder(this)
    //            .SetSmallIcon(Resource.Drawable.chat48)
    //            .SetContentTitle("JI")
    //            .SetContentText(body)
    //            .SetAutoCancel(true)
    //            .SetSound(defSoundURL)
    //            .SetContentIntent(pendingintent);
    //        var notifManager = NotificationManager.FromContext(this);
    //        notifManager.Notify(0, notificationbuilder.Build());

    //    }
    //}

    
}