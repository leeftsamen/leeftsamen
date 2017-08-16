// <copyright file="Notification.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;

    public class Notification
    {
        public virtual DateTime CreationDateTime { get; set; }

        public virtual User ForUser { get; set; }

        public virtual string ForUserId { get; set; }

        public virtual string Message { get; set; }

        public virtual int NotificationId { get; set; }

        public virtual bool Read { get; set; }

        public virtual bool Shown { get; set; }

        public virtual string Url { get; set; }
    }
}