// <copyright file="MyGcmListenerService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using Android.App;
using Android.Content;
using Android.OS;
using Android.Gms.Gcm;
using Android.Util;
using LeeftSamen.App.Android.Activities;
using Android.Media;

namespace LeeftSamen.App.Android
{
    [Service(Exported = false), IntentFilter(new[] { "com.google.android.c2dm.intent.RECEIVE" })]
    public class MyGcmListenerService : GcmListenerService
    {
        private static int i;

        public override void OnMessageReceived(string from, Bundle data)
        {
            var alert = data.GetString("alert");
            var url = data.GetString("url");
            this.SendNotification(alert, url);
        }

        void SendNotification(string alert, string url)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            intent.PutExtra("url", url);
            var pendingIntent = PendingIntent.GetActivity(this, i, intent, 0);
            i++;

            var notificationBuilder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.Icon)
                .SetContentTitle(alert)
                .SetAutoCancel(true)
                .SetStyle(new Notification.BigTextStyle().BigText(alert))
                .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
                //.SetGroup("LeeftSamen")
                .SetContentIntent(pendingIntent);

            var notificationManager = (NotificationManager)this.GetSystemService(NotificationService);
            notificationManager.Notify(i, notificationBuilder.Build());
        }
    }
}