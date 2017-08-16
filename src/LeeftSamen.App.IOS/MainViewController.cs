// <copyright file="MainViewController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>
namespace LeeftSamen.App.IOS
{
    using System;
    using System.Linq;
    using System.Net;

    using SystemConfiguration;

    using CoreGraphics;

    using Foundation;

    using UIKit;

    internal class MainViewController : UIViewController
    {
        private const string BaseUrl = "https://app.leeftsamen.nl/";

        private const string LastUrlKey = "LastUrlKey";

        private UIWebView webView;

        public MainViewController()
        {
            // set default User-Agent
            var userAgent = string.Format(
                "LeeftSamen.App/{0} (iOS)",
                NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"]);
            var dictionary = NSDictionary.FromObjectAndKey(FromObject(userAgent), FromObject("UserAgent"));
            NSUserDefaults.StandardUserDefaults.RegisterDefaults(dictionary);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.View.BackgroundColor = UIColor.White;

            this.webView = new UIWebView { ShouldStartLoad = (view, request, type) =>
                               {
                                   if (this.ShouldOpenInWebView(request))
                                   {
                                       if (request.HttpMethod.ToLower() != "get")
                                       {
                                           return true;
                                       }

                                       if (request.Url.ToString().ToLower().Contains("iospush"))
                                       {
                                           return true;
                                       }

                                       var handler = new NotificationHandler();
                                       if (handler.DevicePushToken == null || !this.ShouldAddPushToken(request))
                                       {
                                           return true;
                                       }

                                       string anchor = string.Empty;
                                       var url = request.Url.ToString().ToLower();
                                       if (url.Contains("#"))
                                       {
                                           var splitted = url.Split('#');
                                           url = splitted[0];
                                           anchor = "#" + splitted[1];
                                       }

                                       if (url.Contains("?"))
                                       {
                                           url += "&iospush=" + handler.DevicePushToken;
                                       }
                                       else
                                       {
                                           url += "?iospush=" + handler.DevicePushToken;
                                       }

                                       url += anchor;
                                       this.webView.LoadRequest(new NSUrlRequest(new NSUrl(url)));
                                       return false;
                                   }

                                   UIApplication.SharedApplication.OpenUrl(request.Url);
                                   return false;
                               } };

            this.webView.LoadRequest(new NSUrlRequest(this.GetSavedUrl()));
            this.Add(this.webView);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (this.HasInternetConnection())
            {
                return;
            }

            var alert = new UIAlertView()
                            {
                                Title = "Fout",
                                Message = "Voor deze app heb je een internetverbinding nodig"
                            };
            alert.AddButton("OK");
            alert.Show();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            this.SaveUrl();
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            this.webView.Frame = new CGRect(0, 0, this.View.Frame.Width, this.View.Frame.Height);
        }

        public void OpenUrl(string url)
        {
            if (url.Contains('#'))
            {
                var splitted = url.Split('#');
                this.webView.LoadRequest(
                    new NSUrlRequest(
                        new NSUrl("#" + splitted[1], new NSUrl(BaseUrl + url))));
            }
            else
            {
                this.webView.LoadRequest(
                    new NSUrlRequest(
                        new NSUrl(BaseUrl + url)));
            }
        }

        private NSUrl GetSavedUrl()
        {
            var url = NSUserDefaults.StandardUserDefaults.StringForKey(LastUrlKey);
            if (string.IsNullOrWhiteSpace(url))
            {
                url = BaseUrl;
            }

            var anchor = string.Empty;
            if (url.Contains("#"))
            {
                anchor = url.Split('#')[1];
            }

            string queryString;
            try
            {
                queryString = new Uri(url).Query.Substring(1);
                queryString = "?" + string.Join("&", queryString.Split('&').Where(q => !q.StartsWith("iospush")));
            }
            catch
            {
                queryString = string.Empty;
            }

            var handler = new NotificationHandler();
            if (handler.DevicePushToken != null)
            {
                if (string.IsNullOrWhiteSpace(queryString) || queryString == "?")
                {
                    queryString = "?iospush=" + handler.DevicePushToken;
                }
                else
                {
                    queryString += "&iospush=" + handler.DevicePushToken;
                }
            }

            try
            {
                return new NSUrl(BaseUrl + new Uri(url).AbsolutePath + queryString + anchor);
            }
            catch
            {
                return new NSUrl(BaseUrl + queryString + anchor);
            }
        }

        private bool HasInternetConnection()
        {
            var networkReachability = new NetworkReachability(new IPAddress(new byte[] { 8, 8, 8, 8 }));
            NetworkReachabilityFlags flags;
            return networkReachability.TryGetFlags(out flags);
        }

        private void SaveUrl()
        {
            if (this.webView == null)
            {
                return;
            }

            var url = string.Empty;
            if (this.webView.Request != null)
                url = this.webView.Request.MainDocumentURL.AbsoluteString;
            NSUserDefaults.StandardUserDefaults.SetString(url, LastUrlKey);
        }

        private bool ShouldOpenInWebView(NSUrlRequest request)
        {
            return request.MainDocumentURL.ToString().ToLower().Contains("app.leeftsamen.nl");
        }

        private bool ShouldAddPushToken(NSUrlRequest request)
        {
            return request.Url.ToString().ToLower().Contains("app.leeftsamen.nl");
        }
    }
}