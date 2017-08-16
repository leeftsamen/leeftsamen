// <copyright file="OrganizationInvitation.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;

    public class OrganizationInvitation
    {
        public virtual string AcceptToken { get; set; }

        public virtual string Email { get; set; }

        public virtual DateTime InvitationDateTime { get; set; }

        public virtual User InvitedBy { get; set; }

        public virtual Organization Organization { get; set; }

        public virtual int OrganizationInvitationId { get; set; }

        public virtual User User { get; set; }
    }
}