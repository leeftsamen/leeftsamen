// <copyright file="MarketplaceItemCategory.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class MarketplaceItemCategory
    {
        public virtual int CategoryId { get; set; }

        public virtual string Name { get; set; }

        public virtual string Title { get; set; }

        public virtual string Description { get; set; }

        public CategoryAlias Alias { get; set; }

        public int SortOrder { get; set; }

        /// <summary>
        /// An alias representing a category. This way we can code businesslogic based on it.
        /// </summary>
        public enum CategoryAlias
        {
            // NOTICE: Changing a Enum value below will cause the related items to be handled as being category 'Stuff' in code business logic
            HelpNeighborhood,
            Meals,
            Stuff,
            Borrowing
        }
    }
}