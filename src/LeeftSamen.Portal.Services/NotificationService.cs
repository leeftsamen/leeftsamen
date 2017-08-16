// <copyright file="NotificationService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    using PushSharp;
    using PushSharp.Android;
    using PushSharp.Apple;
    using PushSharp.Core;

    using LeeftSamen.Portal.Data;
    using LeeftSamen.Portal.Data.Models;
    using System.Diagnostics;
    using Data.Enums;
    using System.Text.RegularExpressions;

    public class NotificationService : INotificationService
    {
        private static readonly log4net.ILog log =
    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IApplicationDbContext databaseContext;
        private readonly IUserService userService;
        private readonly ISettingService settingService;

        public NotificationService(IApplicationDbContext databaseContext, IUserService userService, ISettingService settingService)
        {
            this.databaseContext = databaseContext;
            this.userService = userService;
            this.settingService = settingService;
        }

        public async Task CreateNotificationForUserAsync(User user, string message, string url, SettingName notificationType)
        {
            await this.CreateNotificationForUsersAsync(new List<User> { user }, message, url, notificationType).ConfigureAwait(false);
        }

        public async Task CreateNotificationForUsersAsync(IEnumerable<User> users, string message, string url, SettingName notificationType)
        {
            foreach (var user in users)
            {
                var devices = await this.userService.GetActiveUserDevices(user).ConfigureAwait(false);

                var website = await this.settingService.GetUserSettingByUserAndSetting(user.Id, "notification-website", notificationType.ToString()).ConfigureAwait(false);

                if (website == null || website.Value == "true")
                {
                    var notification = this.databaseContext.Notifications.Create();
                    notification.ForUser = user;
                    notification.Message = message;
                    notification.CreationDateTime = DateTime.Now;
                    notification.Url = url;
                    this.databaseContext.Notifications.Add(notification);
                    await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
                }

                var mobile = await this.settingService.GetUserSettingByUserAndSetting(user.Id, "notification-mobile", notificationType.ToString()).ConfigureAwait(false);

                // Intesive non-critical operation, let's not block method continuation -> no await operator.

                var unreadCount = (await this.GetLatestNotificationsForUserAsync(user)).Count(n => !n.Read);

                if (mobile == null || mobile.Value == "true")
                {
                    var task = Task.Run(() => SendPushNotification(devices, url, Regex.Replace(message, "<.*?>", string.Empty), unreadCount));
                }
            }
        }

        private static void SendPushNotification(List<UserDevice> devices, string url, string message, int unReadNotificationCount)
        {
            var broker = new PushBroker();
            broker.OnNotificationSent += NotificationSent;
            broker.OnChannelException += ChannelException;
            broker.OnServiceException += ServiceException;
            broker.OnNotificationFailed += NotificationFailed;
            broker.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
            broker.OnDeviceSubscriptionChanged += DeviceSubscriptionChanged;
            broker.OnChannelCreated += ChannelCreated;
            broker.OnChannelDestroyed += ChannelDestroyed;

            if (url.Contains("//"))
            {
                url = url.Substring(url.Replace("//", "").IndexOf('/') + 2);
            }
            else
            {
                if (url.Contains("/"))
                    url = url.Substring(url.IndexOf('/'));
            }

            try
            {
                broker.RegisterAppleService(new ApplePushChannelSettings(true, AppDomain.CurrentDomain.BaseDirectory + "bin\\Certificates\\leeftsamen_push_prod.p12", "xxxxxxxxxxxxxxxxxx", true));
            }
            catch (Exception e)
            {
            }

            broker.RegisterGcmService(
                new GcmPushChannelSettings("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"));

            foreach (var device in devices)
            {
                switch (device.DeviceType)
                {
                    case DeviceType.iOS:
                        broker.QueueNotification(
                            new AppleNotification().ForDeviceToken(device.Token)
                                .WithAlert(message)
                                .WithBadge(unReadNotificationCount)
                                .WithCustomItem("content-available", 1)
                                .WithCustomItem("url", url));
                        break;

                    case DeviceType.Android:
                        var apayload = string.Format(@"{{""alert"":""{0}"",""url"":""{1}""}}", message, url);
                        var notificationGCM = new GcmNotification().ForDeviceRegistrationId(device.Token).WithJson(apayload);

                        broker.QueueNotification(notificationGCM);
                        break;
                }
            }

            log.Debug("queueNotifications");
            broker.StopAllServices();
        }

        public async Task<List<Data.Models.Notification>> SetAllNotificationsReadAsync(User user)
        {
            var notifications = await this.databaseContext.Notifications.Where(n => n.ForUserId == user.Id && n.Read == false).OrderByDescending(n => n.CreationDateTime)
                    .Take(10)
                    .ToListAsync()
                    .ConfigureAwait(false);

            foreach (var notification in notifications)
            {
                notification.Read = true;
            }

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            return notifications;
        }

        public async Task<List<Data.Models.Notification>> GetLatestNotificationsForUserAsync(User user, int maxCount = 5)
        {
            return
                await
                this.databaseContext.Notifications.Where(n => n.ForUserId == user.Id)
                    .OrderByDescending(n => n.CreationDateTime)
                    .Take(maxCount)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<Data.Models.Notification> GetNewNotificationForUserAsync(User user)
        {
            return
                await
                this.databaseContext.Notifications.Where(n => n.ForUserId == user.Id && !n.Shown)
                    .OrderBy(n => n.CreationDateTime)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
        }

        public async Task<int> GetUnreadCountForUserAsync(User user)
        {
            return
                await
                this.databaseContext.Notifications.Where(n => n.ForUserId == user.Id && !n.Read)
                    .CountAsync()
                    .ConfigureAwait(false);
        }

        public async Task<Data.Models.Notification> GetNotificationByIdAsync(int notificationId)
        {
            return
                await
                this.databaseContext.Notifications.Where(n => n.NotificationId == notificationId)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
        }

        public async Task<Data.Models.Notification> SetNotificationReadAsync(Data.Models.Notification notification)
        {
            notification.Read = true;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            return notification;
        }

        public async Task<Data.Models.Notification> SetNotificationShownAsync(Data.Models.Notification notification)
        {
            notification.Shown = true;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            return notification;
        }

        private static void DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, INotification notification)
        {
            Debug.WriteLine("DeviceSubscriptionChanged");
        }

        private static void NotificationSent(object sender, INotification notification)
        {
            Debug.WriteLine("NotificationSent");
        }

        private static void NotificationFailed(object sender, INotification notification, Exception notificationFailureException)
        {
            Debug.WriteLine("NotificationFailed");
        }

        private static void ChannelException(object sender, IPushChannel channel, Exception exception)
        {
            Debug.WriteLine("ChannelException");
        }

        private static void ServiceException(object sender, Exception exception)
        {
            Debug.WriteLine("ServiceException");
        }

        private static void DeviceSubscriptionExpired(object sender, string expiredDeviceSubscriptionId, DateTime timestamp, INotification notification)
        {
            Debug.WriteLine("DeviceSubscriptionExpired");
        }

        private static void ChannelDestroyed(object sender)
        {
            Debug.WriteLine("ChannelDestroyed");
        }

        private static void ChannelCreated(object sender, IPushChannel pushChannel)
        {
            Debug.WriteLine("ChannelCreated");
        }
    }
}