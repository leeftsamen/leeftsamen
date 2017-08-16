// <copyright file="CircleActivityInvitation.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Data.Models
{
    public class CircleActivityInvitation
    {
        public virtual string AcceptToken { get; set; }

        public virtual CircleActivity CircleActivity { get; set; }

        public virtual int CircleActivityInvitationId { get; set; }

        public virtual string Email { get; set; }

        public virtual DateTime InvitationDateTime { get; set; }

        public virtual User InvitedBy { get; set; }

        public virtual User User { get; set; }
    }
}
