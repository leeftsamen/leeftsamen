// <copyright file="CircleEmailMessage.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Data.Models
{
    public class CircleEmailMessage
    {
        public virtual string Text { get; set; }

        public virtual DateTime CreationDateTime { get; set; }

        public virtual CircleEmailGroup Group { get; set; }

        public virtual int MessageId { get; set; }

        public virtual User Creator { get; set; }

        public virtual ICollection<CircleEmailMessageReceiver> Recipients { get; set; }

        public virtual string Subject { get; set; }

        public virtual int? ParentMessageId { get; set; }

        public virtual List<CircleEmailMessage> Replies { get; set; }

        public virtual int CircleId { get; set; }

        public virtual bool CreatorHasRemovedMessage { get; set; }
    }
}
