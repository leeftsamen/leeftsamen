// <copyright file="IndexViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Organizations
{
    using System.Collections.Generic;

    using LeeftSamen.Portal.Data.Models;

    /// <summary>
    /// The index view model.
    /// </summary>
    public class IndexViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexViewModel"/> class.
        /// </summary>
        public IndexViewModel()
        {
            this.Organizations = new List<OrganizationViewModel>();
            this.OrganizationThemes = new List<OrganizationThemeViewModel>();
        }

        public IEnumerable<HelpIcon> HelpIcons { get; set; }

        public List<OrganizationThemeViewModel> OrganizationThemes { get; set; }

        /// <summary>
        /// Gets or sets the organizations.
        /// </summary>
        public List<OrganizationViewModel> Organizations { get; set; }

        /// <summary>
        /// The organization view model.
        /// </summary>
        public class OrganizationViewModel
        {
            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the logo id.
            /// </summary>
            public int? LogoId { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the organization id.
            /// </summary>
            public int OrganizationId { get; set; }

            public string OrganizationTypeName { get; set; }
        }
    }
}