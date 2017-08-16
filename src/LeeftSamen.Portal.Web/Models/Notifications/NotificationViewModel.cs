// <copyright file="NotificationViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Notifications
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// The new notification view model.
    /// </summary>
    public class NotificationViewModel
    {
        /// <summary>
        /// The url.
        /// </summary>
        private string url;

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the notification id.
        /// </summary>
        public int NotificationId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether read.
        /// </summary>
        public bool Read { get; set; }

        public DateTime CreationDateTime { get; set; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        public string Url
        {
            get
            {
                if (this.Read)
                {
                    return this.url;
                }

                var helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                return helper.Action("RedirectAndSetRead", "Notifications", new { id = this.NotificationId });
            }

            set
            {
                this.url = value;
            }
        }
    }
}