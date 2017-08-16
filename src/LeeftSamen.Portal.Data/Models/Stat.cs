// <copyright file="Stat.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;

    public class Stat
    {
        public virtual int ActivitiesAttendees { get; set; }

        public virtual int ActivitiesTotal { get; set; }

        public virtual int CircleJobs { get; set; }

        public virtual int CircleMembers { get; set; }

        public virtual int CircleMessages { get; set; }

        public virtual int CirclesPrivate { get; set; }

        public virtual int CirclesPublic { get; set; }

        public virtual int CirclesTotal { get; set; }

        public virtual DateTime DateTime { get; set; }

        public virtual int MarketplaceItemsOffered { get; set; }

        public virtual int MarketplaceItemsTotal { get; set; }

        public virtual int MarketplaceItemsAsked { get; set; }

        public virtual int NeighborhoodAssociationMessages { get; set; }

        public virtual int NeighborhoodMessagesTotal { get; set; }

        public virtual int NeighborhoodNeighborMessages { get; set; }

        public virtual int NeighborhoodOrganizationMessages { get; set; }

        public virtual int OrganisationsTotal { get; set; }

        public virtual int OrganisationVolunteers { get; set; }

        public virtual int OrganisationAssociations { get; set; }

        public virtual int OrganisationProfessionals { get; set; }

        public virtual int StatId { get; set; }

        public virtual int UsersActiveDays { get; set; }

        public virtual int UsersActiveHours { get; set; }

        public virtual int UsersAvgNeighborhoodRadius { get; set; }

        public virtual int UsersTotal { get; set; }
    }
}