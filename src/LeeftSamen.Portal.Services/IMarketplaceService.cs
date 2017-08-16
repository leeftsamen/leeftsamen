// <copyright file="IMarketplaceService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services
{
    using System.Collections.Generic;
    using System.Data.Entity.Spatial;
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.Services.DTO;
    using System;

    public interface IMarketplaceService
    {
        Task<MarketplaceItem> CreateMarketplaceItemAsync(
            string title,
            string description,
            MarketplaceItem.MarketplaceCurrency currency,
            decimal? price,
            MarketplaceItem.MarketplacePaymentOption? paymentOption,
            int? showInCircleId,
            MarketplaceItem.MarketplaceItemType type,
            User owner,
            OrganizationMembership organizationMembership,
            MarketplaceItemCategory category,
            ImageDto image1,
            ImageDto image2,
            ImageDto image3,
            ImageDto image4,
            ImageDto image5,
            bool allowSharing,
            bool isPublic,
            bool preferenceDelivery,
            bool preferenceMail,
            bool preferenceOnline,
            bool preferencePickup,
            bool showlocation,
            DateTime? expirationDate,
            string portalUrl);

        Task<MarketplaceItemReaction> CreateReactionAsync(
            MarketplaceItem marketplaceItem,
            string text,
            User creator,
            OrganizationMembership organizationMembership,
            string portalUrl,
            MarketplaceItemReaction parentReaction = null);

        Task<List<MarketplaceItemCategory>> GetAllMarketplaceItemCategoriesAsync();

        Task<List<MarketplaceItem>> GetAllOwnerMarketplaceItemsAsync(
            string userId,
            OrganizationMembership organizationMembership);

        Task<MarketplaceItem> GetLatestOwnerMarketplaceItemsAsync(string userId);

        Task<Dictionary<int, MarketplaceItem>> GetLatestMarketplaceItemsAsync(DbGeography position, int radius);

        Task<MarketplaceItem> GetMarketplaceItemByIdAsync(int? itemId);

        Task<MarketplaceItem> GetMarketplaceItemByIdAsync(int? itemId, DbGeography position, int radius);

        Task<MarketplaceItemCategory> GetMarketplaceItemCategoryByIdAsync(int categoryId);

        Task<MarketplaceItemCategory> GetMarketplaceItemCategoryByAliasAsync(int aliasId);

        Task<Media> GetMarketplaceItemImageAsync(int? itemId, int? mediaId, int? index);

        Task<MarketplaceItemReaction> GetMarketplaceItemReactionByIdAsync(int marketplaceItemId, int reactionId);

        Task<List<MarketplaceItem>> GetMarketplaceItemsAsync(
            DbGeography position,
            int radius,
            MarketplaceItem.MarketplaceItemType? type,
            int? categoryId,
            int? circleId,
            string query,
            MarketplaceService.ItemOrderByOption orderby,
            int skip = 0,
            int take = int.MaxValue);

        Task<List<MarketplaceItem>> GetPublicMarketplaceItemsAsync(DbGeography position, int radius, int limit);

        Task<MarketplaceItemPrivateReactionsDto> GetMarketplaceItemWithPrivateReactionsByIdAsync(
            int? itemId,
            string viewerId,
            DbGeography position,
            int radius,
            OrganizationMembership viewerOrganizationMembership);

        Task RemoveMarketplaceItemAsync(MarketplaceItem item);

        Task<MarketplaceItem> UpdateMarketplaceItemAsync(
            MarketplaceItem item,
            string title,
            string description,
            MarketplaceItem.MarketplaceCurrency currency,
            decimal? price,
            MarketplaceItem.MarketplacePaymentOption? paymentOption,
            int? showInCircleId,
            MarketplaceItem.MarketplaceItemType type,
            MarketplaceItemCategory category,
            ImageDto image1,
            ImageDto image2,
            ImageDto image3,
            ImageDto image4,
            ImageDto image5,
            bool allowSharing,
            bool isPublic,
            bool preferenceDelivery,
            bool preferenceMail,
            bool preferenceOnline,
            bool preferencePickup,
            bool showlocation,
            DateTime? expirationDate);

        Task<MarketplaceItemTransaction> CreateMarketplaceItemTransactionAsync(
            MarketplaceItem marketplaceItem,
            User sender,
            User receiver,
            string portalUrl);

        Task<MarketplaceItemTransaction> GetMarketplaceItemTransactionForMarketplaceItemAsync(
            int marketplaceItemId,
            string senderId,
            string receiverId);
    }
}