// <copyright file="IndexViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Stats
{
    using System;
    using System.Collections.Generic;

    using LeeftSamen.Portal.Data.Models;

    public class IndexViewModel
    {
        public int UsersActiveDays { get; set; }

        public int UsersActiveHour { get; set; }

        public int UsersActiveHours { get; set; }

        public int UsersTotal { get; set; }

        public int UsersAvgNeighborhoodRadius { get; set; }

        public List<UsersPerCity> UsersPerCity { get; set; }

        public int NeighborhoodMessagesTotal { get; set; }

        public int NeighborhoodNeighborMessages { get; set; }

        public int NeighborhoodAssociationMessages { get; set; }

        public int NeighborhoodOrganizationMessages { get; set; }

        public int OrganisationsTotal { get; set; }

        public int OrganisationProfessionals { get; set; }

        public int OrganisationAssociations { get; set; }

        public int OrganisationVolunteers { get; set; }

        public int CirclesTotal { get; set; }

        public int CirclesPublic { get; set; }

        public int CirclesPrivate { get; set; }

        public int CircleMembers { get; set; }

        public int CircleMessages { get; set; }

        public int CircleJobs { get; set; }

        public int MarketplaceItemsTotal { get; set; }

        public int MarketplaceItemsOffered { get; set; }

        public int MarketplaceItemsAsked { get; set; }

        public int ActivitiesTotal { get; set; }

        public int ActivitiesAttendees { get; set; }

        public List<MarketplaceItemsPerCategory> MarketplaceItemsPerCategory { get; set; }

        public DateTime StatDate { get; set; }
    }
}