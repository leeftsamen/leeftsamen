// <copyright file="PushNotification.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Data.Enums;
using System;

namespace LeeftSamen.Portal.Data.Models
{
    public class PushNotification
    {
        /// <summary>
        /// Gets or sets the alert.
        /// </summary>
        public virtual string Alert { get; set; }

        /// <summary>
        /// Gets or sets the badge.
        /// </summary>
        public virtual int Badge { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        public virtual DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is processed.
        /// </summary>
        public virtual bool IsProcessed { get; set; }

        /// <summary>
        /// Gets or sets the processed date.
        /// </summary>
        public virtual DateTime? ProcessedDate { get; set; }

        /// <summary>
        /// Gets or sets the push notification id.
        /// </summary>
        public virtual int PushNotificationId { get; set; }

        /// <summary>
        /// Gets or sets the type of model.
        /// </summary>
        public virtual ModelType Type { get; set; }

        /// <summary>
        /// Gets or sets the id of model.
        /// </summary>
        public virtual int? TypeId { get; set; }
    }
}
