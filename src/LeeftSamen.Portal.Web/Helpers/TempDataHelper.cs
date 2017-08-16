// <copyright file="TempDataHelper.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Helpers
{
    using System.Web.Mvc;

    /// <summary>
    /// The temp data helper.
    /// </summary>
    public static class TempDataHelper
    {
        /// <summary>
        /// The notification type.
        /// </summary>
        public enum NotificationType
        {
            /// <summary>
            /// The success.
            /// </summary>
            Success = 1,

            /// <summary>
            /// The danger.
            /// </summary>
            Danger = 2,

            /// <summary>
            /// The warning.
            /// </summary>
            Warning = 3,

            /// <summary>
            /// The info.
            /// </summary>
            Info = 4,
        }

        /// <summary>
        /// The create notification.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="TempDataDictionary"/>.
        /// </returns>
        public static TempDataDictionary CreateNotification(NotificationType type, string message)
        {
            return new TempDataDictionary { { "NotificationType", (int)type }, { "NotificationMessage", message } };
        }
    }
}