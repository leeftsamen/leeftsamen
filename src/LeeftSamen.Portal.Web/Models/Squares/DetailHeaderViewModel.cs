// <copyright file="DetailHeaderViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Squares
{
    using System.Collections.Generic;

    using LeeftSamen.Portal.Data.Models;

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
        /// Gets or sets the square id.
        /// </summary>
        public int SquareId { get; set; }

        public bool IsUserAdmin { get; set; }

        /// <summary>
        /// Gets or sets the cover color.
        /// </summary>
        public int CoverColor { get; set; }

        /// <summary>
        /// Gets or sets the cover image id.
        /// </summary>
        public int? CoverImageId { get; set; }

        public IEnumerable<HelpIcon> HelpIcons { get; set; }

        /// <summary>
        /// Gets or sets the menu items.
        /// </summary>
        public List<MenuItemModel> MenuItems { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the profile image id.
        /// </summary>
        public int? ProfileImageId { get; set; }
    }
}