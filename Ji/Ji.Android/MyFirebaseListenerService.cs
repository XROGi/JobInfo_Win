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
    [Service, IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class XROGiFirebaseListenerService : FirebaseMessagingService
    {
        public override void OnMessageReceived(RemoteMessage message)
        {
            try
            {
                base.OnMessageReceived(message);

                var notification = message.GetNotification();
                if (notification != null)
                {
                    var title = notification.Title;
                    var body = notification.Body;

                    SendNotification(title, body);
                }
                else
                {

                }
            }catch (Exception err)
            {

            }
        }

        [Obsolete]
        private void SendNotification(string title, string body)
        {
            try
            {
                var intent = new Intent(this, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.ClearTop);
                // intent.PutExtra("launchArguments", "stuff");

                var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

                var defaultSoundUri = RingtoneManager.GetDefaultUri(RingtoneType.Notification);

                var notificationBuilder =
                    new NotificationCompat.Builder(this)
                        .SetSmallIcon(Resource.Drawable.chat48)
                        .SetContentTitle(title)
                        .SetContentText(body)
                        .SetAutoCancel(true)
                        .SetSound(defaultSoundUri)
                        .SetContentIntent(pendingIntent);

                var notificationManager = NotificationManager.FromContext(this);
                notificationManager.Notify(0, notificationBuilder.Build());
            }
            catch (Exception err)
            {

            }
}
    }
}