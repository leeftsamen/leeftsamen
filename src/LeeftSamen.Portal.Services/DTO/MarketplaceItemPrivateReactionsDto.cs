// <copyright file="MarketplaceItemPrivateReactionsDto.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services.DTO
{
    using System.Collections.Generic;

    using LeeftSamen.Portal.Data.Models;

    public class MarketplaceItemPrivateReactionsDto
    {
        public MarketplaceItem MarketplaceItem { get; set; }

        public List<MarketplaceItemReaction> Reactions { get; set; }
    }
}