// <copyright file="MarketplaceItem.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using Common.InterfaceText;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity.Spatial;

    public class MarketplaceItem
    {
        public const int ImageCount = 5;

        public enum MarketplaceItemType
        {
            /// <summary>
            /// The offered.
            /// </summary>
            [Display(ResourceType = typeof(Label), Name = "Offered")]
            Offered,

            /// <summary>
            /// The asked.
            /// </summary>
            [Display(ResourceType = typeof(Label), Name = "Asked")]
            Asked
        }

        public enum MarketplacePaymentOption
        {
            [Display(ResourceType = typeof(Label), Name = "PaymentOptionFree")]
            Free,
            [Display(ResourceType = typeof(Label), Name = "PaymentOptionAuction")]
            Auction
        }

        public enum MarketplaceCurrency
        {
            [Display(ResourceType = typeof(Label), Name = "PaymentCurrencyEuro")]
            Euro,
            [Display(ResourceType = typeof(Label), Name = "PaymentCurrencyZuiderling")]
            Zuiderling
        }

        public virtual MarketplaceItemCategory Category { get; set; }

        public virtual DateTime CreationDateTime { get; set; }

        public virtual string Description { get; set; }

        public bool PreferenceDelivery { get; set; }

        public bool PreferencePickup { get; set; }

        public bool PreferenceMail { get; set; }

        public bool PreferenceOnline { get; set; }

        public virtual Media Image1 { get; set; }

        public virtual int? Image1Id { get; set; }

        public virtual Media Image2 { get; set; }

        public virtual int? Image2Id { get; set; }

        public virtual Media Image3 { get; set; }

        public virtual int? Image3Id { get; set; }

        public virtual Media Image4 { get; set; }

        public virtual int? Image4Id { get; set; }

        public virtual Media Image5 { get; set; }

        public virtual int? Image5Id { get; set; }

        public virtual int MarketplaceItemId { get; set; }

        public virtual OrganizationMembership OrganizationMembership { get; set; }

        public virtual int? OrganizationMembershipId { get; set; }

        public virtual User Owner { get; set; }

        public virtual ICollection<MarketplaceItemReaction> Reactions { get; set; }

        public virtual string Title { get; set; }

        public virtual MarketplaceCurrency Currency { get; set; }

        public virtual decimal? Price { get; set; }

        public virtual MarketplaceItemType Type { get; set; }

        public virtual bool AllowSharing { get; set; }

        public virtual double? Distance { get; set; }

        public virtual MarketplacePaymentOption? PaymentOption { get; set; }

        [DefaultValue(true)]
        public virtual bool IsPublic { get; set; }

        public virtual int? ShowInCircleId { get; set; }

        public bool? ShowLocation { get; set; }

        public virtual DbGeography Position { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public bool HasAnyImage()
        {
            for (var i = 1; i <= ImageCount; i++)
            {
                if (this.HasImage(i))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasImage(int index)
        {
            return this.ImageId(index).HasValue;
        }

        public int? ImageId(int index)
        {
            switch (index)
            {
                case 1:
                    return this.Image1Id;
                case 2:
                    return this.Image2Id;
                case 3:
                    return this.Image3Id;
                case 4:
                    return this.Image4Id;
                case 5:
                    return this.Image5Id;
            }

            return null;
        }
    }
}