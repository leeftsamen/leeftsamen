// <copyright file="MarketplaceItemsPerCategory.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    public class MarketplaceItemsPerCategory
    {
        public string Category { get; set; }

        public int Count { get; set; }

        public MarketplaceItem.MarketplaceItemType Type { get; set; }
    }
}