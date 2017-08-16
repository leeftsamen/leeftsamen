// <copyright file="MyInstanceIDListenerService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using Android.App;
using Android.Content;
using Android.Gms.Gcm.Iid;

namespace LeeftSamen.App.Android
{
    [Service(Exported = false), IntentFilter(new[] { "com.google.android.gms.iid.InstanceID" })]
    class MyInstanceIDListenerService : InstanceIDListenerService
    {
        public override void OnTokenRefresh()
        {
            var intent = new Intent(this, typeof(RegistrationIntentService));
            this.StartService(intent);
        }
    }
}