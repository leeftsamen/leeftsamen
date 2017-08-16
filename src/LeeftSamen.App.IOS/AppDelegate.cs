// <copyright file="AppDelegate.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;

namespace LeeftSamen.App.IOS
{
    using Foundation;

    using UIKit;

    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        private UIWindow window;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // this app hides the statusbar
            // check info.plist for the settings:
            // <key>UIStatusBarHidden</key>
            // <true/>
            // <key>UIViewControllerBasedStatusBarAppearance</key>
            // <false/>
            this.window = new UIWindow(UIScreen.MainScreen.Bounds) { RootViewController = new MainViewController() };
            this.window.MakeKeyAndVisible();

            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                                   UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                                   new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            else
            {
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(
                    UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound);
            }

            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;

            return true;
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            try
            {
                UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;

                if (application.ApplicationState == UIApplicationState.Active)
                {
                    var aps = userInfo.ObjectForKey(new NSString("aps")) as NSDictionary;
                    if (aps != null && aps.ContainsKey(new NSString("alert")))
                    {
                        UIApplication.SharedApplication.ScheduleLocalNotification(
                            new UILocalNotification
                                {
                                    AlertBody = aps[new NSString("alert")] as NSString,
                                    UserInfo = userInfo
                                });
                    }

                    return;
                }

                this.OpenUrl(userInfo);
            }
            catch
            {
                // Do nothing
            }
        }

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            // show an alert
            var okayAlertController = UIAlertController.Create(notification.AlertAction, notification.AlertBody, UIAlertControllerStyle.Alert);
            okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            okayAlertController.AddAction(
                UIAlertAction.Create(
                    "Bekijk",
                    UIAlertActionStyle.Default,
                    action =>
                        {
                            this.OpenUrl(notification.UserInfo);
                        }));
            this.window.RootViewController.PresentViewController(okayAlertController, true, null);
        }

        public override void DidRegisterUserNotificationSettings(UIApplication application, UIUserNotificationSettings notificationSettings)
        {
            Debug.WriteLine("Registering user notification settings");
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            Debug.WriteLine("Failed to register for remote notification");
        }

        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            Debug.WriteLine("Received a remote notification");
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            Debug.WriteLine("Registered for remote notifications and got device token");

            var handler = new NotificationHandler
            {
                DevicePushToken =
                    deviceToken.ToString()
                        .Replace("<", string.Empty)
                        .Replace(">", string.Empty)
                        .Replace(" ", string.Empty)
            };
        }

        private void OpenUrl(NSDictionary userInfo)
        {
            var mainController = this.window.RootViewController as MainViewController;
            mainController?.OpenUrl(userInfo[new NSString("url")] as NSString);
        }
    }
}