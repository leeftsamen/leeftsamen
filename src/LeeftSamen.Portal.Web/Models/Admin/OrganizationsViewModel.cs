// <copyright file="OrganizationsViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Admin
{
    using System;
    using System.Collections.Generic;

    public class OrganizationsViewModel
    {
        public List<Organization> Organizations { get; set; }

        public class Organization
        {
            public string City { get; set; }

            public bool IsRequestPending { get; set; }

            public string Name { get; set; }

            public int OrganizationId { get; set; }

            public DateTime? RequestDateTime { get; set; }

            public string Email { get; set; }

            public string Phone { get; set; }

            public string Website { get; set; }
        }
    }
}