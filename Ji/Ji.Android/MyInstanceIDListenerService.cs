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
using Firebase.Iid;

namespace Ji.Droid
{
[Service, IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    [Obsolete]
    public class XROGiFirebaseInstanceIdService : FirebaseInstanceIdService
    {
        [Obsolete]
        public override void OnTokenRefresh()
        {
            try
            {//eADPcr6hrbw:APA91bFrjju5QuCP9p_slqaWMbeYd1Y2_FCDYWGCvzdSOWNXCzecYabi8b5cly52tRN2TaY4_-SHeubHNGQkJzz6u9VbgaX5dY-AErzE9p4cHyOGYQNzeN-qO7I8ZycEk7C3VqV7xNm7
                var token = FirebaseInstanceId.Instance.Token;
               if (App.ddd!=null)
                App.ddd.Register_FB(token.ToString());
                // todo
                // 1. check if token has changed
                // 2. if changed communicate it to backend
            }catch (Exception err)
            {

            }
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        [Obsolete]
        public override void OnStart(Intent intent, int startId)
        {
            base.OnStart(intent, startId);
        }
    }
}
 