// <copyright file="ItemPostModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Marketplace
{
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using System.Web.Mvc;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Data.Models;
    using System.Collections.Generic;
    using System;

    public class ItemPostModel
    {
        [Display(ResourceType = typeof(Label), Name = "Category")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "CategoryIsRequired")]
        public int? CategoryCategoryId { get; set; }

        [Display(ResourceType = typeof(Label), Name = "Description")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "DescriptionIsRequired")]
        [AllowHtml]
        //[MaxLength(4000, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Error))]
        public string Description { get; set; }

        public HttpPostedFileBase Image1 { get; set; }

        public HttpPostedFileBase Image2 { get; set; }

        public HttpPostedFileBase Image3 { get; set; }

        public HttpPostedFileBase Image4 { get; set; }

        public HttpPostedFileBase Image5 { get; set; }

        [Display(ResourceType = typeof(Label), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "TitleIsRequired")]
        [MaxLength(40, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Error))]
        public string Title { get; set; }

        [Display(ResourceType = typeof(Label), Name = "PreferenceDelivery")]
        public bool PreferenceDelivery { get; set; }

        [Display(ResourceType = typeof(Label), Name = "PreferencePickup")]
        public bool PreferencePickup { get; set; }

        [Display(ResourceType = typeof(Label), Name = "PreferenceMail")]
        public bool PreferenceMail { get; set; }

        [Display(ResourceType = typeof(Label), Name = "PreferenceOnline")]
        public bool PreferenceOnline { get; set; }

        public MarketplaceItem.MarketplaceCurrency Currency { get; set; }

        [Display(ResourceType = typeof(Label), Name = "Price")]
        [Range(1, int.MaxValue, ErrorMessageResourceName = "RangeNumericPositive", ErrorMessageResourceType = typeof(Error))]
        public decimal? Price { get; set; }

        [Display(ResourceType = typeof(Label), Name = "TypeOfMarketplaceItem")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "MarketplaceItemTypeRequired")]
        public MarketplaceItem.MarketplaceItemType Type { get; set; }

        [Display(ResourceType = typeof(Label), Name = "ShareDescription")]
        public bool AllowSharing { get; set; }

        [Display(ResourceType = typeof(Label), Name = "IsPublic")]
        public bool IsPublic { get; set; }

        public int? ShowInCircleId { get; set; }

        private bool? _showInCircle { get; set; }

        public DateTime? ExpirationDate { get; set; }

        [Display(ResourceType = typeof(Label), Name = "ShowInCircle")]
        public bool ShowInCircle
        {
            get
            {
                if (!this._showInCircle.HasValue)
                {
                    this._showInCircle = this.ShowInCircleId.HasValue;
                }

                return this._showInCircle.Value;
            }

            set
            {
                this._showInCircle = value;
            }
        }

        public MarketplaceItem.MarketplacePaymentOption? PaymentOption { get; set; }

        [Display(ResourceType = typeof(Label), Name = "ShowlocationMarketplaceItem")]
        public bool ShowLocation { get; set; }
    }
}