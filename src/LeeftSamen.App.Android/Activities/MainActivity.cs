// <copyright file="MainActivity.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.App.Android.Activities
{
    using System;

    using global::Android.App;

    using global::Android.Content;

    using global::Android.Content.PM;

    using global::Android.Net;

    using global::Android.OS;

    using global::Android.Util;

    using global::Android.Views;

    using global::Android.Webkit;

    using Java.Lang;

    using LeeftSamen.App.Android.Web;

    using Uri = global::Android.Net.Uri;
    using global::Android.Gms.Common;
    using System.Threading.Tasks;
    [Activity(MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    internal class MainActivity : Activity
    {
        private const string CurrentUrlKey = "CurrentUrl";

        private const int FileChooserResultcode = 1;

        private const string StorageKey = "LeeftSamen";

        private const string Tag = "MainActivity";
        private WebView appWebView;

        private int loadStarted;

        private WebView splashScreenWebView;

        private IValueCallback uploadFileCallback;

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (this.CloseIfDisconnected())
            {
                return true;
            }

            if (this.appWebView == null)
            {
                return base.OnKeyDown(keyCode, e);
            }

            if (keyCode != Keycode.Back)
            {
                return base.OnKeyDown(keyCode, e);
            }

            if (this.appWebView.Url != null && this.appWebView.Url == Constants.AppUrl)
            {
                // close app when on homescreen
                this.Finish();
            }
            else
            {
                this.appWebView.LoadUrl("javascript:history.go(-1);");
            }

            return true;
        }

        public override bool OnKeyUp(Keycode keyCode, KeyEvent e)
        {
            if (this.appWebView == null)
            {
                return base.OnKeyUp(keyCode, e);
            }

            if (keyCode == Keycode.Menu)
            {
                this.appWebView.LoadUrl("javascript:$('.navbar-toggle').click();");
            }

            return true;
        }

        public void OnOpenFileChooserCallback(IValueCallback callback)
        {
            if (this.uploadFileCallback != null)
            {
                this.uploadFileCallback.OnReceiveValue(null);
            }

            this.uploadFileCallback = callback;

            var chooseImageIntent = new Intent(Intent.ActionGetContent);
            chooseImageIntent.AddCategory(Intent.CategoryOpenable);
            chooseImageIntent.SetType("image/*");
            this.StartActivityForResult(Intent.CreateChooser(chooseImageIntent, "File Chooser"), FileChooserResultcode);
        }

        public void StoreUrl()
        {
            if (this.appWebView == null)
            {
                return;
            }

            var url = this.appWebView.Url;
            if (url == "about:blank")
            {
                url = Constants.AppUrl;
            }

            var preferencesEditor = this.GetSharedPreferences(StorageKey, FileCreationMode.Private).Edit();
            preferencesEditor.PutString(CurrentUrlKey, url);
            preferencesEditor.Commit();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            Uri result = null;
            if (data != null)
            {
                result = data.Data;
            }

            if (requestCode == FileChooserResultcode && this.uploadFileCallback != null)
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    // Lollipop onOpenFileChooser expects an array of URI's
                    this.uploadFileCallback.OnReceiveValue(result == null ? null : new[] { result });
                }
                else
                {
                    this.uploadFileCallback.OnReceiveValue(result);
                }

                this.uploadFileCallback = null;
                return;
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            this.SetContentView(Resource.Layout.Main);

            CookieSyncManager.CreateInstance(this.ApplicationContext);
            CookieManager.Instance.SetAcceptCookie(true);

            this.splashScreenWebView = this.FindViewById<WebView>(Resource.Id.LocalWebView);
            InitializeWebView(this.splashScreenWebView);

            this.appWebView = this.FindViewById<WebView>(Resource.Id.AppWebView);
            InitializeWebView(this.appWebView);

            var webViewClient = new CustomWebViewClient(this.Application);
            webViewClient.ResourceLoaded += (sender, args) => this.CloseIfDisconnected();
            webViewClient.UrlLoaded += (sender, args) =>
                {
                    if (this.splashScreenWebView.Visibility != ViewStates.Gone)
                    {
                        // Add a small delay before showing the appWebView
                        var diff = DateTime.Now.Millisecond - this.loadStarted;
                        if (diff >= 0 && diff < 2000)
                        {
                            Thread.Sleep(2000 - diff);
                            CookieSyncManager.Instance.Sync();
                        }
                    }

                    this.splashScreenWebView.Visibility = ViewStates.Gone;
                    this.appWebView.Visibility = ViewStates.Visible;
                };
            this.appWebView.SetWebViewClient(webViewClient);
            this.appWebView.SetWebChromeClient(new CustomWebChromeClient(this));

            this.appWebView.Settings.JavaScriptEnabled = true;
            this.appWebView.Settings.DomStorageEnabled = true;
            this.appWebView.Settings.LoadWithOverviewMode = true;
            this.appWebView.Settings.UserAgentString = string.Format(
                "LeeftSamen.App/{0} ({1} {2}; Android {3})",
                this.BaseContext.PackageManager.GetPackageInfo(this.BaseContext.PackageName, 0).VersionName,
                Build.Manufacturer,
                Build.Model,
                Build.VERSION.Release);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat && Build.VERSION.Release != "4.4.3"
                && Build.VERSION.Release != "4.4.4" && Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
            {
                this.appWebView.AddJavascriptInterface(new CustomJavascriptInterface(this), "LeeftSamenAndroidUnsupportedFileUpload");
            }

            this.appWebView.AddJavascriptInterface(new CustomJavascriptInterface(this), "JavaScriptHandler");

            // show the flash/load screen
            this.splashScreenWebView.LoadUrl("file:///android_asset/splashscreen.html");

            var url = this.GetSharedPreferences(StorageKey, FileCreationMode.Private).GetString(CurrentUrlKey, null);
            if (url != null && url.StartsWith(Constants.AppUrl))
            {
                Log.Info(Tag, @"Restoring from previous url: {0}", url);
            }
            else
            {
                url = Constants.AppUrl;
            }

            var intentUrl = this.Intent.GetStringExtra("url");
            if (!string.IsNullOrEmpty(intentUrl))
            {
                url = Constants.AppUrl + intentUrl;
            }

            this.loadStarted = DateTime.Now.Millisecond;
            this.appWebView.LoadUrl(url);
            if (url.Contains("#"))
            {
                //this.appWebView.LoadUrl(url);
                var splitted = url.Split('#');
                var task = Task.Run(
                    delegate
                    {
                        this.appWebView.LoadUrl("javascript:scrollAnchor(" + splitted[1] + ");");
                    });
            }

            if (this.IsPlayServicesAvailable())
            {
                var intent = new Intent(this, typeof(RegistrationIntentService));
                this.StartService(intent);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();

            CookieSyncManager.Instance.StopSync();

            this.StoreUrl();
        }

        protected override void OnResume()
        {
            base.OnResume();

            CookieSyncManager.Instance.StartSync();
        }

        private static void InitializeWebView(WebView webView)
        {
            // disallow zooming/panning
            webView.Settings.BuiltInZoomControls = false;
            webView.Settings.SetSupportZoom(false);

            // scrollbar settings
            webView.ScrollBarStyle = ScrollbarStyles.OutsideOverlay;
            webView.ScrollbarFadingEnabled = false;
        }

        private bool CloseIfDisconnected(
            string message = "U heeft een werkende internetverbinding nodig om deze app te kunnen gebruiken.")
        {
            var activeConnection = ((ConnectivityManager)this.GetSystemService(ConnectivityService)).ActiveNetworkInfo;
            if (activeConnection != null && activeConnection.IsConnected)
            {
                return false;
            }

            if (this.appWebView != null)
            {
                this.appWebView.LoadUrl("about:blank");
            }

            var alertDialog = new AlertDialog.Builder(this).Create();
            alertDialog.SetTitle("Geen internetverbinding");
            alertDialog.SetMessage(message);
            alertDialog.SetButton("OK", (sender, args) => this.Finish());
            alertDialog.Show();

            return true;
        }

        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void LoadUrl(string url)
        {
            this.appWebView.LoadUrl(url);
        }
    }
}