// <copyright file="IndexViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Notifications
{
    using System.Collections.Generic;

    using LeeftSamen.Portal.Data.Models;

    /// <summary>
    /// The index view model.
    /// </summary>
    public class IndexViewModel
    {
        /// <summary>
        /// Gets or sets the notifications.
        /// </summary>
        public List<NotificationViewModel> Notifications { get; set; }
    }
}