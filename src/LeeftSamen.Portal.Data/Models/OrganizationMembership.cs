// <copyright file="OrganizationMembership.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;

    public class OrganizationMembership
    {
        public virtual bool IsAdministrator { get; set; }

        public virtual DateTime MemberSinceDateTime { get; set; }

        public virtual Organization Organization { get; set; }

        public virtual int OrganizationMembershipId { get; set; }

        public virtual User User { get; set; }
    }
}