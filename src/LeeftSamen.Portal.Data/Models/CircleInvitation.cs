// <copyright file="CircleInvitation.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;

    public class CircleInvitation
    {
        public virtual string AcceptToken { get; set; }

        public virtual Circle Circle { get; set; }

        public virtual int CircleInvitationId { get; set; }

        public virtual string Email { get; set; }

        public virtual DateTime InvitationDateTime { get; set; }

        public virtual DateTime ExpireDate { get; set; }

        public virtual User InvitedBy { get; set; }

        public virtual User User { get; set; }
    }
}