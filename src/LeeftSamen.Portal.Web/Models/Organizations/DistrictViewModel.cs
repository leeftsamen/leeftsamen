// <copyright file="DistrictViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Organizations
{
    /// <summary>
    /// The district view model.
    /// </summary>
    public class DistrictViewModel
    {
        /// <summary>
        /// Gets or sets the district id.
        /// </summary>
        public int DistrictId { get; set; }

        /// <summary>
        /// Gets or sets the district name.
        /// </summary>
        public string DistrictName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether selected.
        /// </summary>
        public bool Selected { get; set; }
    }
}