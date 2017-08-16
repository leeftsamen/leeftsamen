// <copyright file="IndexViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Marketplace
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;

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
        }

        public List<MarketplaceItemCategory> Categories { get; internal set; }

        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        public List<SelectListItem> CategoriesForSelect { get; set; }

        public IEnumerable<HelpIcon> HelpIcons { get; set; }

        public Dictionary<int, MarketplaceItem> Items { get; internal set; }
    }
}