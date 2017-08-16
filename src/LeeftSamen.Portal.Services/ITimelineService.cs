// <copyright file="ITimelineService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data.Models;

    public interface ITimelineService
    {
        Task<List<Activity>> GetActivitiesAsync(User user, DateTime since, int limit = 10);

        Task<List<ActivityAttendance>> GetAttendancesAsync(User user, DateTime since, int limit = 10);

        Task<List<Circle>> GetPublicCirclesAsync(User user, DateTime since, int limit = 10);

        Task<List<Job>> GetJobsAsync(User user, DateTime since, int limit = 10);

        Task<List<Job>> GetTimeLineJobsAsync(User user, DateTime since, int limit = 10);

        Task<List<MarketplaceItem>> GetMarketplaceItemsAsync(User user, DateTime since, MarketplaceItemCategory.CategoryAlias? type, int limit = 10);

        Task<List<MarketplaceItemReaction>> GetMarketplaceReactionsAsync(User user, DateTime since, int limit = 10);

        Task<List<NeighborhoodMessage>> GetNeighborhoodMessagesAsync(User user, DateTime since, int limit = 10);
    }
}