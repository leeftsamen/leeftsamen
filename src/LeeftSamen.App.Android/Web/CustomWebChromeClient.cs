// <copyright file="CustomWebChromeClient.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.App.Android.Web
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using global::Android.App;

    using global::Android.Util;

    using global::Android.Webkit;

    using Java.Interop;

    using LeeftSamen.App.Android.Activities;

    internal class CustomWebChromeClient : WebChromeClient
    {
        private readonly MainActivity activity;

        public CustomWebChromeClient(MainActivity activity)
        {
            if (activity == null)
            {
                throw new ArgumentNullException("activity");
            }

            this.activity = activity;
        }

        public override bool OnConsoleMessage(ConsoleMessage consoleMessage)
        {
            Log.Debug(
                "LeeftSamen",
                "{0} -- From line {1} of {2}",
                consoleMessage.Message(),
                consoleMessage.LineNumber(),
                consoleMessage.SourceId());

            return true;
        }

        public override bool OnJsAlert(WebView view, string url, string message, JsResult result)
        {
            new AlertDialog.Builder(view.Context).SetTitle("LeeftSamen")
                .SetMessage(message)
                .SetPositiveButton("OK", (sender, args) => { })
                .SetCancelable(false)
                .Create()
                .Show();

            result.Cancel();

            return true;
        }

        // For Android 5.0+
        public override bool OnShowFileChooser(
            WebView webView,
            IValueCallback filePathCallback,
            FileChooserParams fileChooserParams)
        {
            this.CallOpenFileChooserCallback(filePathCallback);

            return true;
        }

        // For Android 3.0+
        // ReSharper disable once InconsistentNaming
        [Export]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Reviewed. Suppression is OK here.")]
        public void openFileChooser(IValueCallback uploadMsg, Java.Lang.String acceptType)
        {
            this.CallOpenFileChooserCallback(uploadMsg);
        }

        // For Android 4.1+
        // ReSharper disable once InconsistentNaming
        [Export]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Reviewed. Suppression is OK here.")]
        public void openFileChooser(IValueCallback uploadMsg, Java.Lang.String acceptType, Java.Lang.String capture)
        {
            this.CallOpenFileChooserCallback(uploadMsg);
        }

        private void CallOpenFileChooserCallback(IValueCallback filePathCallback)
        {
            this.activity.OnOpenFileChooserCallback(filePathCallback);
        }
    }
}