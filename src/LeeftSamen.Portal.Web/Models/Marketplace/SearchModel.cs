// <copyright file="SearchModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Marketplace
{
    public class SearchModel
    {
        public int Skip { get; set; }

        public bool ListView { get; set; }

        public int? Category { get; set; }

        public string Query { get; set; }

        public MarketplaceItem.MarketplaceItemType? Type { get; set; }

        public Services.MarketplaceService.ItemOrderByOption OrderBy { get; set; }

        public int Take { get { return 15; } }

        public int? CircleId { get; set; }
    }
}