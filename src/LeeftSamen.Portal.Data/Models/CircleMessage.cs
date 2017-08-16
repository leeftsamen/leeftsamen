// <copyright file="CircleMessage.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class CircleMessage
    {
        public virtual Circle Circle { get; set; }

        public virtual DateTime CreationDateTime { get; set; }

        public virtual User Creator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is private at the time of creation.
        /// </summary>
        public virtual bool IsPrivate { get; set; }

        public virtual bool IsHidden { get; set; }

        public virtual int MessageId { get; set; }

        public virtual string MessageText { get; set; }

        public virtual ICollection<CircleMessageReaction> Reactions { get; set; }

        public virtual Media Attachment { get; set; }

        public virtual int? AttachmentId { get; set; }
    }
}