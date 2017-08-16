// <copyright file="StatsService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data;
    using LeeftSamen.Portal.Data.Models;

    public class StatsService : IStatsService
    {
        private readonly IApplicationDbContext databaseContext;

        public StatsService(IApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<Stat> GenerateStatEntryAsync()
        {
            var stat = this.databaseContext.Stats.Create();
            stat.ActivitiesAttendees = await this.GetActivitiesAttendeesAsync().ConfigureAwait(false);
            stat.ActivitiesTotal = await this.GetActivitiesTotalAsync().ConfigureAwait(false);
            stat.CircleJobs = await this.GetCircleJobsAsync().ConfigureAwait(false);
            stat.CircleMembers = await this.GetCircleMembersAsycn().ConfigureAwait(false);
            stat.CircleMessages = await this.GetCircleMessagesAsync().ConfigureAwait(false);
            stat.CirclesPrivate = await this.GetCirclesPrivateAsync().ConfigureAwait(false);
            stat.CirclesPublic = await this.GetCirclesPublicAsync().ConfigureAwait(false);
            stat.CirclesTotal = await this.GetCirclesTotalAsync().ConfigureAwait(false);
            stat.DateTime = DateTime.Now;
            stat.MarketplaceItemsOffered = await this.GetMarketplaceItemsOfferedAsync().ConfigureAwait(false);
            stat.MarketplaceItemsTotal = await this.GetMarketplaceItemsTotalAsync().ConfigureAwait(false);
            stat.MarketplaceItemsAsked = await this.GetMarketplaceItemsAskedAsync().ConfigureAwait(false);
            stat.NeighborhoodAssociationMessages =
                await this.GetNeighborhoodAssociationMessagesAsync().ConfigureAwait(false);
            stat.NeighborhoodMessagesTotal = await this.GetNeighborhoodMessagesTotalAsync().ConfigureAwait(false);
            stat.NeighborhoodNeighborMessages = await this.GetNeighborhoodNeighborMessagesAsync().ConfigureAwait(false);
            stat.NeighborhoodOrganizationMessages =
                await this.GetNeighborhoodOrganizationMessagesAsync().ConfigureAwait(false);
            stat.OrganisationsTotal = await this.GetOrganisationsTotalAsync().ConfigureAwait(false);
            stat.OrganisationVolunteers = await this.GetOrganisationVolunteersAsync().ConfigureAwait(false);
            stat.OrganisationAssociations = await this.GetOrganisationAssociationsAsync().ConfigureAwait(false);
            stat.OrganisationProfessionals = await this.GetOrganisationProfessionalsAsync().ConfigureAwait(false);
            stat.UsersActiveDays = await this.GetUsersActiveDaysAsync().ConfigureAwait(false);
            stat.UsersActiveHours = await this.GetUsersActiveHoursAsync().ConfigureAwait(false);
            stat.UsersAvgNeighborhoodRadius = await this.GetUsersAvgNeighborhoodRadiusAsync().ConfigureAwait(false);
            stat.UsersTotal = await this.GetUsersTotalAsync().ConfigureAwait(false);
            this.databaseContext.Stats.Add(stat);

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            return stat;
        }

        public async Task<int> GetActivitiesAttendeesAsync()
        {
            return await this.databaseContext.ActivityAttendances.CountAsync().ConfigureAwait(false);
        }

        public async Task<int> GetActivitiesTotalAsync()
        {
            return await this.databaseContext.Activities.CountAsync().ConfigureAwait(false);
        }

        public async Task<int> GetCircleJobsAsync()
        {
            return await this.databaseContext.Jobs.CountAsync().ConfigureAwait(false);
        }

        public async Task<int> GetCircleMembersAsycn()
        {
            return await this.databaseContext.CircleMemberships.CountAsync().ConfigureAwait(false);
        }

        public async Task<int> GetCircleMessagesAsync()
        {
            return await this.databaseContext.Circles.Select(c => c.Messages).CountAsync().ConfigureAwait(false);
        }

        public async Task<int> GetCirclesPrivateAsync()
        {
            return await this.databaseContext.Circles.Where(c => c.IsPrivate).CountAsync().ConfigureAwait(false);
        }

        public async Task<int> GetCirclesPublicAsync()
        {
            return await this.databaseContext.Circles.Where(c => !c.IsPrivate).CountAsync().ConfigureAwait(false);
        }

        public async Task<int> GetCirclesTotalAsync()
        {
            return await this.databaseContext.Circles.CountAsync().ConfigureAwait(false);
        }

        public async Task<int> GetMarketplaceItemsOfferedAsync()
        {
            return
                await
                this.databaseContext.MarketplaceItems.Where(m => m.Type == MarketplaceItem.MarketplaceItemType.Offered)
                    .CountAsync()
                    .ConfigureAwait(false);
        }

        public async Task<int> GetMarketplaceItemsTotalAsync()
        {
            return await this.databaseContext.MarketplaceItems.CountAsync().ConfigureAwait(false);
        }

        public async Task<int> GetMarketplaceItemsAskedAsync()
        {
            return
                await
                this.databaseContext.MarketplaceItems.Where(m => m.Type == MarketplaceItem.MarketplaceItemType.Asked)
                    .CountAsync()
                    .ConfigureAwait(false);
        }

        public async Task<List<MarketplaceItemsPerCategory>> GetMarketplaceItemsPerCategoryAsync()
        {
            var categories =
                await
                this.databaseContext.MarketplaceItems.Where(m => m.Type == MarketplaceItem.MarketplaceItemType.Asked)
                    .GroupBy(m => m.Category.Name)
                    .Select(
                        mg =>
                        new MarketplaceItemsPerCategory
                            {
                                Category = mg.Key,
                                Count = mg.Count(),
                                Type = MarketplaceItem.MarketplaceItemType.Asked
                            })
                    .ToListAsync()
                    .ConfigureAwait(false);
            categories.AddRange(
                await
                this.databaseContext.MarketplaceItems.Where(m => m.Type == MarketplaceItem.MarketplaceItemType.Offered)
                    .GroupBy(m => m.Category.Name)
                    .Select(
                        mg =>
                        new MarketplaceItemsPerCategory
                            {
                                Category = mg.Key,
                                Count = mg.Count(),
                                Type = MarketplaceItem.MarketplaceItemType.Offered
                            })
                    .ToListAsync()
                    .ConfigureAwait(false));
            return categories;
        }

        public async Task<int> GetNeighborhoodAssociationMessagesAsync()
        {
            return
                await
                this.databaseContext.NeighborhoodMessages.Where(
                    m =>
                    m.OrganizationMembership.Organization.OrganizationType.Type == OrganizationType.Types.Association)
                    .CountAsync()
                    .ConfigureAwait(false);
        }

        public async Task<int> GetNeighborhoodMessagesTotalAsync()
        {
            return await this.databaseContext.NeighborhoodMessages.CountAsync().ConfigureAwait(false);
        }

        public async Task<int> GetNeighborhoodNeighborMessagesAsync()
        {
            return
                await
                this.databaseContext.NeighborhoodMessages.Where(m => m.OrganizationMembership == null)
                    .CountAsync()
                    .ConfigureAwait(false);
        }

        public async Task<int> GetNeighborhoodOrganizationMessagesAsync()
        {
            return
                await
                this.databaseContext.NeighborhoodMessages.Where(
                    m =>
                    m.OrganizationMembership.Organization.OrganizationType.Type == OrganizationType.Types.Professional
                    || m.OrganizationMembership.Organization.OrganizationType.Type == OrganizationType.Types.Volunteer)
                    .CountAsync()
                    .ConfigureAwait(false);
        }

        public async Task<int> GetOrganisationsTotalAsync()
        {
            return await this.databaseContext.Organizations.CountAsync().ConfigureAwait(false);
        }

        public async Task<int> GetOrganisationVolunteersAsync()
        {
            return
                await
                this.databaseContext.Organizations.Where(
                    o => o.OrganizationType.Type == OrganizationType.Types.Volunteer).CountAsync().ConfigureAwait(false);
        }

        public async Task<int> GetOrganisationAssociationsAsync()
        {
            return
                await
                this.databaseContext.Organizations.Where(
                    o => o.OrganizationType.Type == OrganizationType.Types.Association)
                    .CountAsync()
                    .ConfigureAwait(false);
        }

        public async Task<int> GetOrganisationProfessionalsAsync()
        {
            return
                await
                this.databaseContext.Organizations.Where(
                    o => o.OrganizationType.Type == OrganizationType.Types.Professional)
                    .CountAsync()
                    .ConfigureAwait(false);
        }

        public async Task<List<Stat>> GetStatsAsync(DateTime? dateFrom, DateTime? dateTo)
        {
            var from = dateFrom ?? DateTime.MinValue;
            var to = dateTo ?? DateTime.MaxValue;
            return
                await
                this.databaseContext.Stats.Where(s => s.DateTime >= from && s.DateTime <= to)
                    .OrderByDescending(s => s.DateTime)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<Stat> GetLastStatAsync()
        {
            return
                await
                this.databaseContext.Stats.OrderByDescending(s => s.DateTime)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
        }

        public async Task<int> GetUsersActiveDaysAsync()
        {
            var days = DateTime.Now.AddDays(-7);
            return await this.databaseContext.Users.Where(u => u.LastSeen > days).CountAsync().ConfigureAwait(false);
        }

        public async Task<int> GetUsersActiveHourAsync()
        {
            var hour = DateTime.Now.AddHours(-1);
            return await this.databaseContext.Users.Where(u => u.LastSeen > hour).CountAsync().ConfigureAwait(false);
        }

        public async Task<int> GetUsersActiveHoursAsync()
        {
            var hours = DateTime.Now.AddHours(-24);
            return await this.databaseContext.Users.Where(u => u.LastSeen > hours).CountAsync().ConfigureAwait(false);
        }

        public async Task<int> GetUsersAvgNeighborhoodRadiusAsync()
        {
            return (int)await this.databaseContext.Users.AverageAsync(u => u.NeighborhoodRadius).ConfigureAwait(false);
        }

        public async Task<List<UsersPerCity>> GetUsersPerCityAsync()
        {
            return
                await
                this.databaseContext.Users.GroupBy(u => u.City)
                    .Select(ug => new UsersPerCity { City = ug.Key, Count = ug.Count() })
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<int> GetUsersTotalAsync()
        {
            return await this.databaseContext.Users.CountAsync().ConfigureAwait(false);
        }
    }
}