// <copyright file="MarketplaceItemReaction.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;

    public class MarketplaceItemReaction
    {
        public virtual DateTime CreationDateTime { get; set; }

        public virtual User Creator { get; set; }

        public virtual MarketplaceItem MarketplaceItem { get; set; }

        public virtual OrganizationMembership OrganizationMembership { get; set; }

        public virtual int? OrganizationMembershipId { get; set; }

        public virtual MarketplaceItemReaction Parent { get; set; }

        public virtual int? ParentId { get; set; }

        public virtual int ReactionId { get; set; }

        public virtual string Text { get; set; }
    }
}