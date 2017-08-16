// <copyright file="NotificationHandler.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using Foundation;
using System.Diagnostics;

namespace LeeftSamen.App.IOS
{
    class NotificationHandler
    {
        private string KeyPushDeviceToken = "PushNotificationToken";

        public string DevicePushToken
        {
            get
            {
                return NSUserDefaults.StandardUserDefaults.StringForKey(this.KeyPushDeviceToken);
            }

            set
            {
                Debug.WriteLine("iOS Push Token: " + value);
                // Don't update the token if it's null or if it's still the same value
                if (string.IsNullOrWhiteSpace(value) || value.Equals(this.DevicePushToken))
                {
                    return;
                }

                NSUserDefaults.StandardUserDefaults.SetString(value, this.KeyPushDeviceToken);
            }
        }
    }
}
