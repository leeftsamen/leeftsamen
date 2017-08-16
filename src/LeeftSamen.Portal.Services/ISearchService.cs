// <copyright file="ISearchService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services
{
    using System.Collections.Generic;
    using System.Data.Entity.Spatial;
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data.Models;

    public interface ISearchService
    {
        Task<List<SearchResult>> SearchActivitiesAsync(string query, DbGeography position, int radius);

        Task<List<SearchResult>> SearchAsync(string query, DbGeography position, int radius);

        Task<List<SearchResult>> SearchCirclesAsync(string query, DbGeography position, int radius);

        Task<List<SearchResult>> SearchMarketplaceAsync(string query, DbGeography position, int radius);

        Task<List<SearchResult>> SearchNeighborhoodMessagesAsync(string query, DbGeography position, int radius);
    }
}