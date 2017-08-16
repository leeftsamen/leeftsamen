// <copyright file="ActivitiesViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Organizations
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The members view model.
    /// </summary>
    public class ActivitiesViewModel
    {
        /// <summary>
        /// Gets or sets the ActivitiesModel.
        /// </summary>
        public Activities.IndexViewModel ActivitiesModel   { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the organization id.
        /// </summary>
        public int OrganizationId { get; set; }
    }
}