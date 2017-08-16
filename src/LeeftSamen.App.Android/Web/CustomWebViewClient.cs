// <copyright file="CustomWebViewClient.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using Android.Net;

namespace LeeftSamen.App.Android.Web
{
    using System;
    using System.Linq;

    using global::Android.Content;

    using global::Android.Net.Http;

    using global::Android.Webkit;

    using Uri = global::Android.Net.Uri;

    internal class CustomWebViewClient : WebViewClient
    {
        private readonly Context context;

        public CustomWebViewClient(Context context)
        {
            this.context = context;
        }

        public event EventHandler ResourceLoaded;

        public event EventHandler UrlLoaded;

        public override void OnLoadResource(WebView view, string url)
        {
            if (url.StartsWith("tel:"))
            {
                var tel = new Intent(Intent.ActionCall, Uri.Parse(url));
                tel.AddFlags(ActivityFlags.NewTask);

                this.context.StartActivity(tel);
                return;
            }

            base.OnLoadResource(view, url);
            this.OnResourceLoaded(EventArgs.Empty);
        }

        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);

            this.OnUrlLoaded(EventArgs.Empty);
        }

        public override void OnReceivedSslError(WebView view, SslErrorHandler handler, SslError error)
        {
            base.OnReceivedSslError(view, handler, error);
        }

        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            if (url == null)
            {
                return false;
            }

            if (url.StartsWith("mailto:"))
            {
                MailTo mt = MailTo.Parse(url);
                Intent i = new Intent(Intent.ActionSend);

                i.PutExtra(Intent.ExtraEmail, new[] { mt.To });
                i.PutExtra(Intent.ExtraSubject, mt.Subject);
                i.PutExtra(Intent.ExtraCc, mt.Cc);
                i.PutExtra(Intent.ExtraText, mt.Body);

                i.SetType("message/rfc822");
                i.AddFlags(ActivityFlags.NewTask);

                this.context.StartActivity(i);

                view.Reload();

                return true;
            }

            // If the URL is not an app URL, open it in a browser instead.
            if (!url.StartsWith(Constants.AppUrl))
            {
                var browserIntent = new Intent(Intent.ActionView, Uri.Parse(url));
                browserIntent.AddFlags(ActivityFlags.NewTask);

                this.context.StartActivity(browserIntent);

                view.Reload();

                return true;
            }

            return false;
        }

        protected virtual void OnResourceLoaded(EventArgs e)
        {
            if (this.ResourceLoaded != null)
            {
                this.ResourceLoaded(this, e);
            }
        }

        protected virtual void OnUrlLoaded(EventArgs e)
        {
            if (this.UrlLoaded != null)
            {
                this.UrlLoaded(this, e);
            }
        }
    }
}