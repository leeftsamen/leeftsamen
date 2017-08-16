// <copyright file="RegistrationIntentService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

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
using Android.Gms.Gcm.Iid;
using Android.Gms.Gcm;

namespace LeeftSamen.App.Android
{
    [Service(Exported = false)]
    class RegistrationIntentService : IntentService
    {
        public static string Token { get; set; }
        //public static bool DeliverToken { get; set; }

        static object locker = new object();

        public RegistrationIntentService() : base("RegistrationIntentService") { }

        protected override void OnHandleIntent(Intent intent)
        {
            try
            {
                lock (locker)
                {
                    var instanceID = InstanceID.GetInstance(this);
                    var gcm = GoogleCloudMessaging.GetInstance(this.ApplicationContext);
                    var token = gcm.Register("xxxxxxxxxxx");

                    //Subscribe(token);
                    Token = token;
                    //DeliverToken = true;
                }
            }
            catch (Exception e)
            {
                return;
            }
        }

        //void Subscribe(string token)
        //{
        //    var pubSub = GcmPubSub.GetInstance(this);
        //    pubSub.Subscribe(token, "/topics/global", null);
        //}
    }
}