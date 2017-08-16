// <copyright file="IStatsService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data.Models;

    public interface IStatsService
    {
        Task<int> GetActivitiesAttendeesAsync();

        Task<int> GetActivitiesTotalAsync();

        Task<int> GetCircleJobsAsync();

        Task<int> GetCircleMembersAsycn();

        Task<int> GetCircleMessagesAsync();

        Task<int> GetCirclesPrivateAsync();

        Task<int> GetCirclesPublicAsync();

        Task<int> GetCirclesTotalAsync();

        Task<int> GetMarketplaceItemsOfferedAsync();

        Task<int> GetMarketplaceItemsTotalAsync();

        Task<int> GetMarketplaceItemsAskedAsync();

        Task<List<MarketplaceItemsPerCategory>> GetMarketplaceItemsPerCategoryAsync();

        Task<int> GetNeighborhoodAssociationMessagesAsync();

        Task<int> GetNeighborhoodMessagesTotalAsync();

        Task<int> GetNeighborhoodNeighborMessagesAsync();

        Task<int> GetNeighborhoodOrganizationMessagesAsync();

        Task<int> GetOrganisationVolunteersAsync();

        Task<int> GetOrganisationsTotalAsync();

        Task<int> GetOrganisationAssociationsAsync();

        Task<int> GetOrganisationProfessionalsAsync();

        Task<int> GetUsersActiveDaysAsync();

        Task<int> GetUsersActiveHourAsync();

        Task<int> GetUsersActiveHoursAsync();

        Task<int> GetUsersAvgNeighborhoodRadiusAsync();

        Task<List<UsersPerCity>> GetUsersPerCityAsync();

        Task<int> GetUsersTotalAsync();

        Task<Stat> GenerateStatEntryAsync();

        Task<List<Stat>> GetStatsAsync(DateTime? dateFrom, DateTime? dateTo);

        Task<Stat> GetLastStatAsync();
    }
}