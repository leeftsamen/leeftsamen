// <copyright file="INotificationService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data.Models;
    using Data.Enums;

    public interface INotificationService
    {
        Task CreateNotificationForUserAsync(User user, string message, string url, SettingName notificationType);

        Task CreateNotificationForUsersAsync(IEnumerable<User> users, string message, string url, SettingName notificationType);

        Task<List<Notification>> SetAllNotificationsReadAsync(User user);

        Task<List<Notification>> GetLatestNotificationsForUserAsync(User user, int maxCount = 5);

        Task<Notification> GetNewNotificationForUserAsync(User user);

        Task<Notification> GetNotificationByIdAsync(int notificationId);

        Task<Notification> SetNotificationReadAsync(Notification notification);

        Task<Notification> SetNotificationShownAsync(Notification notification);
    }
}