// <copyright file="OverviewViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Marketplace
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    using LeeftSamen.Portal.Data.Models;
    using System.ComponentModel.DataAnnotations;
    using Common.InterfaceText;
    using System;/// <summary>
                 /// The index view model.
                 /// </summary>
    public class OverviewViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OverviewViewModel"/> class.
        /// </summary>
        public OverviewViewModel()
        {
            this.MarketplaceItems = new List<MarketplaceItem>();
        }

        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        public List<SelectListItem> Categories { get; set; }

        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        public List<SelectListItem> OrderByOptions { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        public int? Category { get; set; }

        /// <summary>
        /// Gets or sets the marketplace items.
        /// </summary>
        public List<MarketplaceItem> MarketplaceItems { get; set; }

        /// <summary>
        /// Gets or sets the selected type.
        /// </summary>
        public MarketplaceItem.MarketplaceItemType? SelectedType { get; set; }

        public IEnumerable<HelpIcon> HelpIcons { get; set; }

        public bool ListView { get; set; }

        public string SearchQuery { get; set; }

        public string CurrentCategory { get; set; }

        public MarketplaceItem.MarketplaceItemType? Type { get; set; }

        public int Take { get; internal set; }

        public int? CircleId { get; set; }

        public string OverviewText { get; set; }

        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "TitleIsRequired")]
        public string ItemTitle { get; set; }

        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "DescriptionIsRequired")]
        public string ItemDescription { get; set; }

        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "DateIsRequired")]
        public DateTime? ExpirationDate { get; set; }
    }
}