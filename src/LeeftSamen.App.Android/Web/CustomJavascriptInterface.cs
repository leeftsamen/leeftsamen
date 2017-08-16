// <copyright file="CustomJavascriptInterface.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using Android.App;
using Android.Content;
using Android.Webkit;
using Java.Interop;

namespace LeeftSamen.App.Android.Web
{
    internal class CustomJavascriptInterface : Java.Lang.Object
    {
        Activity activity;

        public CustomJavascriptInterface(Activity activity)
        {
            this.activity = activity;
        }

        [Export]
        [JavascriptInterface]
        public void CloseApplication()
        {
            this.activity.Finish();
        }

        [Export]
        [JavascriptInterface]
        public string GetIosCode()
        {
            if (!string.IsNullOrEmpty(RegistrationIntentService.Token))
            {
                //RegistrationIntentService.DeliverToken = false;
                return RegistrationIntentService.Token;
            }

            return string.Empty;
        }
    }
}