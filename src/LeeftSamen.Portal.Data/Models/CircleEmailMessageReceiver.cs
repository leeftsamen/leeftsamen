// <copyright file="CircleEmailMessageReceiver.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Data.Models
{
    public class CircleEmailMessageReceiver
    {
        public virtual int ReceiverId { get; set; }

        public virtual bool HasRemovedMessage { get; set; }

        public virtual User Receiver { get; set; }

        public virtual CircleEmailGroup EmailGroup { get; set; }

        public virtual int EmailMessageId { get; set; }
    }
}
