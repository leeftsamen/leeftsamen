// <copyright file="MarketplaceService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System.Web.Configuration;

namespace LeeftSamen.Portal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Data;
    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.Services.DTO;
    using EmailTemplates.Models;
    using Data.Enums;

    public class MarketplaceService : IMarketplaceService
    {
        private readonly IApplicationDbContext databaseContext;

        private readonly ILinkGenerator linkGenerator;

        private readonly IMailerService mailerService;

        private readonly IMediaService mediaService;

        private readonly INotificationService notificationService;

        public MarketplaceService(
            IApplicationDbContext databaseContext,
            IMailerService mailerService,
            IMediaService mediaService,
            ILinkGenerator linkGenerator,
            INotificationService notificationService)
        {
            this.databaseContext = databaseContext;
            this.mailerService = mailerService;
            this.mediaService = mediaService;
            this.linkGenerator = linkGenerator;
            this.notificationService = notificationService;
        }

        public enum ItemOrderByOption
        {
            Date,

            Distance,

            Price,

            PriceDesc
        }

        public async Task<MarketplaceItem> CreateMarketplaceItemAsync(
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
            string portalUrl)
        {
            var item = this.databaseContext.MarketplaceItems.Create();
            item.Title = title;
            item.Description = description;
            item.Currency = currency;
            item.Price = price;
            item.PaymentOption = paymentOption;
            item.ShowInCircleId = showInCircleId;
            item.Type = type;
            item.Owner = owner;
            item.Position = owner.Position;
            item.OrganizationMembership = organizationMembership;
            item.CreationDateTime = DateTime.Now;
            item.Category = category;
            item.AllowSharing = allowSharing;
            item.IsPublic = isPublic;
            item.PreferenceDelivery = preferenceDelivery;
            item.PreferenceMail = preferenceMail;
            item.PreferenceOnline = preferenceOnline;
            item.PreferencePickup = preferencePickup;
            item.ShowLocation = showlocation;
            item.ExpirationDate = expirationDate;

            if (image1 != null)
            {
                item.Image1 = this.mediaService.CreateMedia(image1);
            }

            if (image2 != null)
            {
                item.Image2 = this.mediaService.CreateMedia(image2);
            }

            if (image3 != null)
            {
                item.Image3 = this.mediaService.CreateMedia(image3);
            }

            if (image4 != null)
            {
                item.Image4 = this.mediaService.CreateMedia(image4);
            }

            if (image5 != null)
            {
                item.Image5 = this.mediaService.CreateMedia(image5);
            }

            this.databaseContext.MarketplaceItems.Add(item);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            return item;
        }

        public async Task<MarketplaceItemReaction> CreateReactionAsync(
            MarketplaceItem marketplaceItem,
            string text,
            User creator,
            OrganizationMembership organizationMembership,
            string portalUrl,
            MarketplaceItemReaction parentReaction = null)
        {
            var reaction = this.databaseContext.MarketplaceItemReactions.Create();
            reaction.MarketplaceItem = marketplaceItem;
            reaction.Text = text;
            reaction.Creator = creator;
            reaction.OrganizationMembership = organizationMembership;
            reaction.Parent = parentReaction;
            reaction.CreationDateTime = DateTime.Now;
            this.databaseContext.MarketplaceItemReactions.Add(reaction);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            User emailToUser = null;
            if (marketplaceItem.Owner.Id != reaction.Creator.Id)
            {
                emailToUser = marketplaceItem.Owner;
            }
            else if (parentReaction != null && marketplaceItem.Owner.Id == reaction.Creator.Id)
            {
                emailToUser =
                    await
                    this.databaseContext.MarketplaceItemReactions.Where(
                        r => r.ParentId == parentReaction.ReactionId || r.ReactionId == parentReaction.ReactionId)
                        .Select(r => r.Creator)
                        .Where(c => c.Id != marketplaceItem.Owner.Id)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(false);
            }

            if (emailToUser == null)
            {
                return reaction;
            }

            if (emailToUser.ReceivesMarketplaceMail)
            {
                var reactionUrl = this.linkGenerator.GenerateMarketplaceReactionLink(
                    marketplaceItem.MarketplaceItemId,
                    reaction.ReactionId);

                var model = new EmailTemplates.Models.MarketplaceReactionModel
                                {
                                    Subject =
                                        string.Format(
                                            Subject.MarketplaceReaction,
                                            marketplaceItem.Title),
                                    PortalUrl = portalUrl,
                                    MarketplaceItemTitle =
                                        marketplaceItem.Title,
                                    ReceiverName = emailToUser.Name,
                                    SenderName = creator.Name,
                                    ReactionUrl = reactionUrl
                                };

                await this.mailerService.SendAsync(model, emailToUser).ConfigureAwait(false);

                var message = string.Format(
                    Common.InterfaceText.Notification.MarketplaceReaction,
                    creator.Name,
                    marketplaceItem.Title);
                await
                    this.notificationService.CreateNotificationForUserAsync(emailToUser, message, reactionUrl, SettingName.MarketplaceReaction)
                        .ConfigureAwait(false);
            }

            return reaction;
        }

        public async Task<List<MarketplaceItemCategory>> GetAllMarketplaceItemCategoriesAsync()
        {
            return
                await
                this.databaseContext.MarketplaceItemCategories.OrderBy(c => c.SortOrder)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<List<MarketplaceItem>> GetAllOwnerMarketplaceItemsAsync(
            string userId,
            OrganizationMembership organizationMembership)
        {
            if (organizationMembership == null)
            {
                return
                    await
                    this.databaseContext.MarketplaceItems.Where(
                        i => i.Owner.Id == userId && i.OrganizationMembershipId == null)
                        .ToListAsync()
                        .ConfigureAwait(false);
            }

            return
                await
                this.databaseContext.MarketplaceItems.Where(
                    i =>
                    i.Owner.Id == userId
                    && i.OrganizationMembership.OrganizationMembershipId
                    == organizationMembership.OrganizationMembershipId).ToListAsync().ConfigureAwait(false);
        }

        public async Task<MarketplaceItem> GetLatestOwnerMarketplaceItemsAsync(string userId)
        {
            return await this.databaseContext.MarketplaceItems.Where(
                i => i.Owner.Id == userId && i.OrganizationMembershipId == null)
                .OrderByDescending(i => i.CreationDateTime)
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<Dictionary<int, MarketplaceItem>> GetLatestMarketplaceItemsAsync(
            DbGeography position,
            int radius)
        {
            return await this.databaseContext.MarketplaceItems

                             // Search distance
                             .Where(i => i.IsPublic && i.Position.Distance(position) <= radius)

                             // Group by category
                             .GroupBy(i => i.Category)

                             // Get latest item per category
                             .Select(
                                 i =>
                                 new
                                     {
                                         category = i.Key.CategoryId,
                                         item = i.OrderByDescending(o => o.CreationDateTime).FirstOrDefault()
                                     })

                             // Transform to dictionary
                             .ToDictionaryAsync(i => i.category, i => i.item).ConfigureAwait(false);
        }

        public async Task<MarketplaceItem> GetMarketplaceItemByIdAsync(int? itemId)
        {
            return
                await
                this.databaseContext.MarketplaceItems.Include(i => i.Category)
                    .FirstOrDefaultAsync(i => i.MarketplaceItemId == itemId)
                    .ConfigureAwait(false);
        }

        public async Task<MarketplaceItem> GetMarketplaceItemByIdAsync(int? itemId, DbGeography position, int radius)
        {
            return
                await
                this.databaseContext.MarketplaceItems.Include(i => i.Category)
                    .FirstOrDefaultAsync(
                        i => i.MarketplaceItemId == itemId && i.Position.Distance(position) <= radius)
                    .ConfigureAwait(false);
        }

        public async Task<MarketplaceItemCategory> GetMarketplaceItemCategoryByIdAsync(int categoryId)
        {
            return
                await
                this.databaseContext.MarketplaceItemCategories.FirstOrDefaultAsync(c => c.CategoryId == categoryId)
                    .ConfigureAwait(false);
        }

        public async Task<MarketplaceItemCategory> GetMarketplaceItemCategoryByAliasAsync(int aliasId)
        {
            return
                await
                this.databaseContext.MarketplaceItemCategories.FirstOrDefaultAsync(c => (int)c.Alias == aliasId)
                    .ConfigureAwait(false);
        }

        public async Task<Media> GetMarketplaceItemImageAsync(int? itemId, int? mediaId, int? index)
        {
            // TODO: Refactor to just a list of Media instead of 5 properties and get rid of the index parameter
            Expression<Func<MarketplaceItem, bool>> predicate = null;
            Expression<Func<MarketplaceItem, Media>> selector = null;

            switch (index)
            {
                case 1:
                    predicate = i => i.Image1Id == mediaId;
                    selector = i => i.Image1;
                    break;
                case 2:
                    predicate = i => i.Image2Id == mediaId;
                    selector = i => i.Image2;
                    break;
                case 3:
                    predicate = i => i.Image3Id == mediaId;
                    selector = i => i.Image3;
                    break;
                case 4:
                    predicate = i => i.Image4Id == mediaId;
                    selector = i => i.Image4;
                    break;
                case 5:
                    predicate = i => i.Image5Id == mediaId;
                    selector = i => i.Image5;
                    break;
            }

            if (predicate == null)
            {
                return null;
            }

            return
                await
                this.databaseContext.MarketplaceItems.Where(i => i.MarketplaceItemId == itemId)
                    .Where(predicate)
                    .Select(selector)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
        }

        public async Task<MarketplaceItemReaction> GetMarketplaceItemReactionByIdAsync(
            int marketplaceItemId,
            int reactionId)
        {
            return
                await
                this.databaseContext.MarketplaceItemReactions.FirstOrDefaultAsync(
                    r => r.MarketplaceItem.MarketplaceItemId == marketplaceItemId && r.ReactionId == reactionId)
                    .ConfigureAwait(false);
        }

        public async Task<List<MarketplaceItem>> GetMarketplaceItemsAsync(
            DbGeography position,
            int radius,
            MarketplaceItem.MarketplaceItemType? type,
            int? categoryId,
            int? circleId,
            string query,
            ItemOrderByOption orderby,
            int skip = 0,
            int take = int.MaxValue)
        {
            // Search circle OR distance
            var itemQuery = circleId.HasValue ? this.databaseContext.MarketplaceItems.Where(i => i.ShowInCircleId == circleId) :
                this.databaseContext.MarketplaceItems.Where(i => i.IsPublic && i.Position.Distance(position) <= radius);

            itemQuery = itemQuery.Where(i => !i.ExpirationDate.HasValue || i.ExpirationDate > DateTime.Now);

            // Search category
            if (categoryId.HasValue)
            {
                itemQuery = itemQuery.Where(i => i.Category.CategoryId == categoryId);
            }

            // Search type
            if (type.HasValue)
            {
                itemQuery = itemQuery.Where(i => i.Type == type);
            }

            // Search query
            if (!string.IsNullOrEmpty(query))
            {
                itemQuery = itemQuery.Where(i => i.Title.Contains(query) || i.Description.Contains(query));
            }

            switch (orderby)
            {
                case ItemOrderByOption.Date:
                    itemQuery = itemQuery.OrderByDescending(i => i.CreationDateTime);
                    break;
                case ItemOrderByOption.Distance:
                    itemQuery = itemQuery.OrderBy(i => i.Position.Distance(position));
                    break;
                case ItemOrderByOption.Price:
                    itemQuery = itemQuery.OrderBy(i => i.Price);
                    break;
                case ItemOrderByOption.PriceDesc:
                    itemQuery = itemQuery.OrderByDescending(i => i.Price);
                    break;
            }

            var result = await itemQuery.Skip(skip).Take(take).ToListAsync().ConfigureAwait(false);

            result.ForEach(i => i.Distance = Math.Round((double)(i.Position.Distance(position) / 1000), 1));

            return result;
        }

        public async Task<List<MarketplaceItem>> GetPublicMarketplaceItemsAsync(DbGeography position, int radius, int limit)
        {
            return
                await this.databaseContext.MarketplaceItems.Where(i => i.IsPublic && i.Position.Distance(position) <= radius).Take(limit).ToListAsync().ConfigureAwait(false);
        }

        public async Task<MarketplaceItemPrivateReactionsDto> GetMarketplaceItemWithPrivateReactionsByIdAsync(
            int? itemId,
            string viewerId,
            DbGeography position,
            int radius,
            OrganizationMembership viewerOrganizationMembership)
        {
            var result = new MarketplaceItemPrivateReactionsDto
                             {
                                 MarketplaceItem =
                                     await
                                     this.GetMarketplaceItemByIdAsync(
                                         itemId).ConfigureAwait(false),
                                 Reactions =
                                     viewerOrganizationMembership == null
                                         ? await
                                           this.databaseContext
                                               .MarketplaceItemReactions.Where(
                                                   r =>
                                                   r.MarketplaceItem
                                                       .MarketplaceItemId == itemId
                                                   && ((viewerId
                                                        == r.MarketplaceItem.Owner.Id)
                                                       || (r.Creator.Id
                                                           == r.MarketplaceItem.Owner
                                                                  .Id
                                                           || r.Creator.Id
                                                           == viewerId)))
                                               .ToListAsync()
                                               .ConfigureAwait(false)
                                         : await
                                           this.databaseContext
                                               .MarketplaceItemReactions.Where(
                                                   r =>
                                                   r.MarketplaceItem
                                                       .MarketplaceItemId == itemId
                                                   && ((viewerId
                                                        == r.MarketplaceItem.Owner.Id
                                                        && viewerOrganizationMembership
                                                               .OrganizationMembershipId
                                                        == r.MarketplaceItem
                                                               .OrganizationMembershipId)
                                                       || ((r.Creator.Id
                                                            == r.MarketplaceItem
                                                                   .Owner.Id
                                                            && r
                                                                   .OrganizationMembershipId
                                                            == r.MarketplaceItem
                                                                   .OrganizationMembershipId)
                                                           || (r.Creator.Id
                                                               == viewerId
                                                               && r
                                                                      .OrganizationMembershipId
                                                               == viewerOrganizationMembership
                                                                      .OrganizationMembershipId))))
                                               .ToListAsync()
                                               .ConfigureAwait(false)
                             };
            if (result.MarketplaceItem != null && viewerId != null && position != null)
            {
                result.MarketplaceItem.Distance =
                    Math.Round((double)(result.MarketplaceItem.Position.Distance(position) / 1000), 1);
            }

            return result;
        }

        public async Task RemoveMarketplaceItemAsync(MarketplaceItem item)
        {
            if (item.Image1 != null)
            {
                this.databaseContext.Media.Remove(item.Image1);
            }

            if (item.Image2 != null)
            {
                this.databaseContext.Media.Remove(item.Image2);
            }

            if (item.Image3 != null)
            {
                this.databaseContext.Media.Remove(item.Image3);
            }

            if (item.Image4 != null)
            {
                this.databaseContext.Media.Remove(item.Image4);
            }

            if (item.Image5 != null)
            {
                this.databaseContext.Media.Remove(item.Image5);
            }
            this.databaseContext.MarketplaceItems.Remove(item);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<MarketplaceItem> UpdateMarketplaceItemAsync(
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
            bool showLocation,
            DateTime? expirationDate)
        {
            item.Title = title;
            item.Description = description;
            item.Type = type;
            item.Category = category;
            item.AllowSharing = allowSharing;
            item.IsPublic = isPublic;
            item.Currency = currency;
            item.Price = price;
            item.PaymentOption = paymentOption;
            item.ShowInCircleId = showInCircleId;
            item.PreferenceDelivery = preferenceDelivery;
            item.PreferenceMail = preferenceMail;
            item.PreferenceOnline = preferenceOnline;
            item.PreferencePickup = preferencePickup;
            item.ShowLocation = showLocation;
            item.ExpirationDate = expirationDate;

            if (image1 != null)
            {
                if (item.Image1 != null)
                {
                    this.databaseContext.Media.Remove(item.Image1);
                }

                item.Image1 = this.mediaService.CreateMedia(image1);
            }

            if (image2 != null)
            {
                if (item.Image2 != null)
                {
                    this.databaseContext.Media.Remove(item.Image2);
                }

                item.Image2 = this.mediaService.CreateMedia(image2);
            }

            if (image3 != null)
            {
                if (item.Image3 != null)
                {
                    this.databaseContext.Media.Remove(item.Image3);
                }

                item.Image3 = this.mediaService.CreateMedia(image3);
            }

            if (image4 != null)
            {
                if (item.Image4 != null)
                {
                    this.databaseContext.Media.Remove(item.Image4);
                }

                item.Image4 = this.mediaService.CreateMedia(image4);
            }

            if (image5 != null)
            {
                if (item.Image5 != null)
                {
                    this.databaseContext.Media.Remove(item.Image5);
                }

                item.Image5 = this.mediaService.CreateMedia(image5);
            }

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            return item;
        }

        public async Task<MarketplaceItemTransaction> CreateMarketplaceItemTransactionAsync(MarketplaceItem marketplaceItem,
            User sender,
            User receiver,
            string portalUrl)
        {
            var item = this.databaseContext.MarketplaceItemTransactions.Create();
            item.Amount = marketplaceItem.Price ?? 0;
            item.Currency = MarketplaceItem.MarketplaceCurrency.Zuiderling;
            item.MarketplaceItem = marketplaceItem;
            item.Sender = sender;
            item.SenderZuiderlingAccount = sender.ZuiderlingAccount;
            item.Receiver = receiver;
            item.ReceiverZuiderlingAccount = receiver.ZuiderlingAccount;

            this.databaseContext.MarketplaceItemTransactions.Add(item);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            var marketplaceItemUrl = this.linkGenerator.GenerateMarketplaceItemLink(marketplaceItem.MarketplaceItemId);
            var model = new EmailTemplates.Models.MarketplaceTransactionModel
            {
                Subject =
                    string.Format(
                        Subject.MarketplaceTransaction,
                        marketplaceItem.Title),
                PortalUrl = portalUrl,
                MarketplaceItemTitle =
                    marketplaceItem.Title,
                ReceiverName = receiver.Name,
                SenderName = sender.Name,
                MarketplaceItemUrl = marketplaceItemUrl
            };

            await this.mailerService.SendAsync(model, receiver).ConfigureAwait(false);
            await this.mailerService.SendAsync(model, WebConfigurationManager.AppSettings["EmailPortalAdmin"]).ConfigureAwait(false);

            var message = string.Format(
                Common.InterfaceText.Notification.MarketplaceTransaction,
                sender.Name,
                marketplaceItem.Title);
            //await
            //    this.notificationService.CreateNotificationForUserAsync(receiver, message, marketplaceItemUrl, SettingName.MarketplaceTransaction)
            //        .ConfigureAwait(false);

            return item;
        }

        public async Task<MarketplaceItemTransaction> GetMarketplaceItemTransactionForMarketplaceItemAsync(int marketplaceItemId, string senderId, string receiverId)
        {
            return
                await
                this.databaseContext.MarketplaceItemTransactions
                    .FirstOrDefaultAsync(i => i.MarketplaceItem.MarketplaceItemId == marketplaceItemId && i.Sender.Id == senderId && i.Receiver.Id == receiverId)
                    .ConfigureAwait(false);
        }
    }
}