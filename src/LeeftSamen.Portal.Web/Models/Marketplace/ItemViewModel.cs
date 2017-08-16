// <copyright file="ItemViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Marketplace
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    using LeeftSamen.Portal.Data.Models;

    public class ItemViewModel : ItemPostModel
    {
        public List<MarketplaceItemCategory> CategoriesList { get; set; }

        public List<SelectListItem> Categories { get; set; }

        public MarketplaceItemCategory Category { get; set; }

        public DateTime CreationDateTime { get; set; }

        public int? Image1Id { get; set; }

        public int? Image2Id { get; set; }

        public int? Image3Id { get; set; }

        public int? Image4Id { get; set; }

        public int? Image5Id { get; set; }

        public int? MarketplaceItemId { get; set; }

        public User Owner { get; set; }

        public bool AllowZuiderling { get; set; }

        public List<SelectListItem> CirleSelectListItems { get; set; }

        public int? CircleId { get; set; }

        public bool SocialVersion { get; set; }

        public static ItemViewModel FromPostModel(ItemPostModel model)
        {
            return new ItemViewModel
            {
                PreferenceDelivery = model.PreferenceDelivery,
                PreferenceMail = model.PreferenceMail,
                PreferenceOnline = model.PreferenceOnline,
                PreferencePickup = model.PreferencePickup,
                Price = model.Price,
                PaymentOption = model.PaymentOption,
                Title = model.Title,
                Description = model.Description,
                Type = model.Type,
                CategoryCategoryId = model.CategoryCategoryId,
                AllowSharing = model.AllowSharing,
                IsPublic = model.IsPublic,
                ShowInCircle = model.ShowInCircle,
                ShowInCircleId = model.ShowInCircleId,
                ShowLocation = model.ShowLocation
            };
        }
    }
}