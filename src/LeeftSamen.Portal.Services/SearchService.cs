// <copyright file="SearchService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data;
    using LeeftSamen.Portal.Data.Models;

    public class SearchService : ISearchService
    {
        private const int DescriptionScore = 1;

        private const int TitleScore = 2;
        private readonly IApplicationDbContext databaseContext;

        public SearchService(IApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<List<SearchResult>> SearchActivitiesAsync(string query, DbGeography position, int radius)
        {
            var result = new List<SearchResult>();

            var activities =
                await
                this.databaseContext.Activities.Where(i => (i.Title.Contains(query) || i.Description.Contains(query)) && i.Position.Distance(position) <= radius)
                    .ToListAsync()
                    .ConfigureAwait(false);
            foreach (var activity in activities)
            {
                var score = activity.Title.Contains(query) ? TitleScore : 0;
                if (activity.Description.Contains(query))
                {
                    score += DescriptionScore;
                }

                result.Add(
                    new SearchResult
                        {
                            Category = SearchResult.Categories.Activities,
                            Label = activity.Title,
                            Identifier = activity.ActivityId,
                            Score = score
                        });
            }

            return result;
        }

        public async Task<List<SearchResult>> SearchAsync(string query, DbGeography position, int radius)
        {
            var result = new List<SearchResult>();
            result.AddRange(await this.SearchActivitiesAsync(query, position, radius).ConfigureAwait(false));
            result.AddRange(await this.SearchCirclesAsync(query, position, radius).ConfigureAwait(false));
            result.AddRange(await this.SearchMarketplaceAsync(query, position, radius).ConfigureAwait(false));
            result.AddRange(await this.SearchNeighborhoodMessagesAsync(query, position, radius).ConfigureAwait(false));

            return result;
        }

        public async Task<List<SearchResult>> SearchCirclesAsync(string query, DbGeography position, int radius)
        {
            var result = new List<SearchResult>();

            var circles =
                await
                this.databaseContext.Circles.Where(
                    i => (i.Name.Contains(query) || i.Description.Contains(query)) && !i.IsPrivate && i.Position.Distance(position) <= radius)
                    .ToListAsync()
                    .ConfigureAwait(false);
            foreach (var circle in circles)
            {
                var score = circle.Name.Contains(query) ? TitleScore : 0;
                if (circle.Description.Contains(query))
                {
                    score += DescriptionScore;
                }

                result.Add(
                    new SearchResult
                        {
                            Category = SearchResult.Categories.Circles,
                            Label = circle.Name,
                            Identifier = circle.CircleId,
                            Score = score
                        });
            }

            return result;
        }

        public async Task<List<SearchResult>> SearchMarketplaceAsync(string query, DbGeography position, int radius)
        {
            var result = new List<SearchResult>();

            var marketplaceItems =
                await
                this.databaseContext.MarketplaceItems.Where(
                    i => (i.Title.Contains(query) || i.Description.Contains(query)) && i.Position.Distance(position) <= radius)
                    .ToListAsync()
                    .ConfigureAwait(false);
            foreach (var item in marketplaceItems)
            {
                var score = item.Title.Contains(query) ? TitleScore : 0;
                if (item.Description.Contains(query))
                {
                    score += DescriptionScore;
                }

                result.Add(
                    new SearchResult
                    {
                        Category = SearchResult.Categories.Marketplace,
                        Label = item.Title,
                        Identifier = item.MarketplaceItemId,
                        Score = score
                    });
            }

            return result;
        }

        public async Task<List<SearchResult>> SearchNeighborhoodMessagesAsync(string query, DbGeography position, int radius)
        {
            var result = new List<SearchResult>();

            var neighborhoodMessages =
                await
                this.databaseContext.NeighborhoodMessages.Where(
                    i => (i.Title.Contains(query) || i.IntroductionText.Contains(query) || i.FullText.Contains(query)) && i.Position.Distance(position) <= radius)
                    .ToListAsync()
                    .ConfigureAwait(false);
            foreach (var message in neighborhoodMessages)
            {
                var score = message.Title.Contains(query) ? TitleScore : 0;
                if (!string.IsNullOrWhiteSpace(message.IntroductionText) && message.IntroductionText.Contains(query))
                {
                    score += DescriptionScore;
                }

                if (!string.IsNullOrWhiteSpace(message.FullText) && message.FullText.Contains(query))
                {
                    score += DescriptionScore;
                }

                var messageType = NeighborhoodMessage.MessageTypes.NeighborMessages;
                if (message.OrganizationMembershipId.HasValue)
                {
                    switch (message.OrganizationMembership.Organization.OrganizationType.Type)
                    {
                        case OrganizationType.Types.Association:
                            messageType = NeighborhoodMessage.MessageTypes.AssociationMessages;
                        break;
                        default:
                            messageType = NeighborhoodMessage.MessageTypes.OrganizationMessages;
                            break;
                    }
                }

                result.Add(
                    new SearchResult
                    {
                        Category = SearchResult.Categories.NeighborhoodMessages,
                        Label = message.Title,
                        Identifier = message.MessageId,
                        Score = score,
                        ExtraData = new KeyValuePair<string, string>("MessageType", messageType.ToString())
                    });
            }

            return result;
        }
    }
}