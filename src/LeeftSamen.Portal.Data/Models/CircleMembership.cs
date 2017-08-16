// <copyright file="CircleMembership.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;

    public class CircleMembership
    {
        public virtual Circle Circle { get; set; }

        public virtual int CircleMembershipId { get; set; }

        public virtual bool IsAdministrator { get; set; }

        public virtual DateTime MemberSinceDateTime { get; set; }

        public virtual User User { get; set; }

        public virtual bool ReceiveEmails { get; set; }

        public string Profile { get; set; }
    }
}