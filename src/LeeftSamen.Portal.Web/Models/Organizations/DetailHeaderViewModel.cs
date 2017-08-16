// <copyright file="DetailHeaderViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Organizations
{
    using System.Collections.Generic;

    /// <summary>
    /// The detail header view model.
    /// </summary>
    public class DetailHeaderViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetailHeaderViewModel"/> class.
        /// </summary>
        public DetailHeaderViewModel()
        {
            this.MenuItems = new List<MenuItemModel>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether current user is member.
        /// </summary>
        public bool CurrentUserIsMember { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether current user is organization administrator.
        /// </summary>
        public bool CurrentUserIsOrganizationAdministrator { get; set; }

        /// <summary>
        /// Gets or sets the logo id.
        /// </summary>
        public int? LogoId { get; set; }

        /// <summary>
        /// Gets or sets the menu items.
        /// </summary>
        public List<MenuItemModel> MenuItems { get; set; }

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