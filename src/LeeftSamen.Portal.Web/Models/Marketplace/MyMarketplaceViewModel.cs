// <copyright file="MyMarketplaceViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Marketplace
{
    using System.Collections.Generic;

    using LeeftSamen.Portal.Data.Models;

    /// <summary>
    /// The my items view model.
    /// </summary>
    public class MyMarketplaceViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyMarketplaceViewModel"/> class.
        /// </summary>
        public MyMarketplaceViewModel()
        {
            this.MarketplaceItems = new List<MarketplaceItem>();
        }

        /// <summary>
        /// Gets or sets the marketplace items.
        /// </summary>
        public List<MarketplaceItem> MarketplaceItems { get; set; }
    }
}