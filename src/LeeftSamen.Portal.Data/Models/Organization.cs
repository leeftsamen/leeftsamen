// <copyright file="Organization.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class Organization
    {
        public virtual ICollection<District> ActiveInDistricts { get; set; }

        public virtual string Address { get; set; }

        public virtual string OpeningHours { get; set; }

        public virtual string City { get; set; }

        public virtual string Description { get; set; }

        public virtual string Email { get; set; }

        public virtual ICollection<OrganizationInvitation> Invitations { get; set; }

        public virtual ICollection<OrganizationService> Services { get; set; }

        public virtual ICollection<OrganizationProduct> Products { get; set; }

        public virtual ICollection<ActionParticipant> Actions { get; set; }

        public virtual ICollection<ActionVote> Votes { get; set; }

        public virtual bool IsRequestPending { get; set; }

        public virtual bool? Hidden { get; set; }

        public virtual Media Logo { get; set; }

        public virtual int? LogoId { get; set; }

        public virtual ICollection<OrganizationMembership> Members { get; set; }

        public virtual ICollection<OrganizationTheme> Themes { get; set; }

        public virtual string Name { get; set; }

        public virtual int OrganizationId { get; set; }

        public virtual OrganizationType OrganizationType { get; set; }

        public virtual string Phone { get; set; }

        public virtual string PostalCode { get; set; }

        public virtual DateTime? RequestDateTime { get; set; }

        public virtual User RequestedBy { get; set; }

        public virtual string Website { get; set; }
    }
}