// <copyright file="TimelineService.cs" company="LeeftSamen B.V.">
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

    public class TimelineService : ITimelineService
    {
        private readonly IApplicationDbContext databaseContext;

        public TimelineService(IApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<List<Activity>> GetActivitiesAsync(User user, DateTime since, int limit = 10)
        {
            return
                await
                this.databaseContext.Activities.Include(a => a.Creator)
                    .Include(a => a.Attendees)
                    .Where(
                        a =>
                        a.CreationDate > since && a.Position.Distance(user.Position) <= user.NeighborhoodRadius
                        && a.Creator.Id != user.Id && a.Circle == null)
                    .OrderByDescending(a => a.CreationDate)
                    .Take(limit)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<List<ActivityAttendance>> GetAttendancesAsync(User user, DateTime since, int limit = 10)
        {
            return
                await
                this.databaseContext.ActivityAttendances.Include(a => a.Activity)
                    .Include(a => a.User)
                    .Where(
                        a =>
                        a.ModificationDate.Value > since && a.Attending
                        && a.Activity.Position.Distance(user.Position) <= user.NeighborhoodRadius
                        && a.User.Id != user.Id
                        && (a.Activity.Attendees.Any(aa => aa.User.Id == user.Id) || a.Activity.Creator.Id == user.Id))
                    .OrderByDescending(a => a.ModificationDate.Value)
                    .Take(limit)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<List<Circle>> GetPublicCirclesAsync(User user, DateTime since, int limit = 10)
        {
            return
                await
                this.databaseContext.Circles.Where(
                    a =>
                    a.CreationDateTime > since
                    && a.Position.Distance(user.Position) <= user.NeighborhoodRadius
                    && a.Creator.Id != user.Id
                    && !a.IsPrivate)
                    .OrderByDescending(a => a.CreationDateTime)
                    .Take(limit)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<List<Job>> GetJobsAsync(User user, DateTime since, int limit = 10)
        {
            return await this.databaseContext.Jobs.Where(j => j.Circle.Members.Any(m => m.User.Id == user.Id) && (
                !j.IsOnlyVisibleToSelectedMembers
                || j.VisibleToMembers.Any(m => m.User.Id == user.Id)) && j.DueDateTime >= since).ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<Job>> GetTimeLineJobsAsync(User user, DateTime since, int limit = 10)
        {
            return
                await
                this.databaseContext.Jobs.Include(j => j.Circle)
                    .Include(j => j.Creator)
                    .Where(
                        j =>
                        j.Circle.Members.Any(m => m.User.Id == user.Id)
                        && (!j.IsOnlyVisibleToSelectedMembers || j.VisibleToMembers.Any(m => m.User.Id == user.Id))
                        && j.CreationDateTime >= since)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<List<MarketplaceItem>> GetMarketplaceItemsAsync(User user, DateTime since, MarketplaceItemCategory.CategoryAlias? alias, int limit = 10)
        {
            return
                await
                this.databaseContext.MarketplaceItems.Include(i => i.Category)
                    .Include(i => i.Owner)
                    .Where(
                        m =>
                        m.CreationDateTime > since && m.Position.Distance(user.Position) <= user.NeighborhoodRadius
                        && m.Owner.Id != user.Id && !m.ShowInCircleId.HasValue
                        && (!alias.HasValue || alias.Value == m.Category.Alias))
                    .OrderByDescending(m => m.CreationDateTime)
                    .Take(limit)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<List<MarketplaceItemReaction>> GetMarketplaceReactionsAsync(User user, DateTime since, int limit = 10)
        {
            return
                await
                this.databaseContext.MarketplaceItemReactions.Include(i => i.Creator)
                    .Include(i => i.MarketplaceItem)
                    .Include(i => i.MarketplaceItem.Category)
                    .Where(
                        r =>
                        r.CreationDateTime > since && r.MarketplaceItem.Owner.Id == user.Id && r.Creator.Id != user.Id)
                    .OrderByDescending(r => r.CreationDateTime)
                    .Take(limit)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<List<NeighborhoodMessage>> GetNeighborhoodMessagesAsync(User user, DateTime since, int limit = 10)
        {
            return
                await
                this.databaseContext.NeighborhoodMessages.Include(m => m.Creator)
                    .Include(m => m.OrganizationMembership)
                    .Include(m => m.OrganizationMembership.Organization)
                    .Where(
                        m =>
                        m.CreationDateTime > since && m.Creator.Id != user.Id
                        && m.Position.Distance(user.Position) <= user.NeighborhoodRadius)
                    .OrderByDescending(m => m.CreationDateTime)
                    .Take(limit)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }
    }
}