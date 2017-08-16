// <copyright file="IndexViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Activities
{
    using System;
    using System.Collections.Generic;

    using LeeftSamen.Portal.Data.Models;

    public class IndexViewModel
    {
        public List<ActivityViewModel> Activities { get; set; }

        public IEnumerable<HelpIcon> HelpIcons { get; set; }

        public int? ShownInCircle { get; set; }

        public int? Age { get; set; }

        public class ActivityViewModel
        {
            public int ActivityId { get; set; }

            public bool AllDay { get; set; }

            public bool? Attending { get; set; }

            public string Location { get; set; }

            public DateTime StartDateTime { get; set; }

            public string Title { get; set; }

            public OrganizationMembership OrganizationMembership { get; set; }

            public bool AllAges { get; set; }

            public int? AgeFrom { get; set; }

            public int? AgeTo { get; set; }
        }
    }
}