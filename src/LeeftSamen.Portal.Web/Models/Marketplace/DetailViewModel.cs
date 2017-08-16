// <copyright file="DetailViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Marketplace
{
    using System.Collections.Generic;

    using LeeftSamen.Portal.Data.Models;

    /// <summary>
    /// The detail view model.
    /// </summary>
    public class DetailViewModel
    {
        public enum TransactionStatus
        {
            NoZuiderlingAccount,
            NotPaid,
            Paid,
            MarketplaceItemOwner
        }

        public bool CurrentUserCanVIew { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether current user can edit.
        /// </summary>
        public bool CurrentUserCanEdit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether current user can make a transaction.
        /// </summary>
        public TransactionStatus UserTransactionStatus { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether current user can place base reaction.
        /// </summary>
        public bool CurrentUserCanPlaceBaseReaction { get; set; }

        /// <summary>
        /// Gets or sets the marketplace item.
        /// </summary>
        public MarketplaceItem MarketplaceItem { get; set; }

        /// <summary>
        /// Gets or sets the new reaction.
        /// </summary>
        public string NewReaction { get; set; }

        /// <summary>
        /// Gets or sets the new reaction parent id.
        /// </summary>
        public int? NewReactionParentId { get; set; }

        public bool AllowSharing { get; set; }

        public bool AllowZuiderling { get; set; }

        /// <summary>
        /// Gets or sets the reactions.
        /// </summary>
        public List<MarketplaceItemReaction> Reactions { get; set; }

        public int? ShownInCircle { get; set; }
    }
}