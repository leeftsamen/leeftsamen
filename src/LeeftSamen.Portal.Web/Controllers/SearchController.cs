// <copyright file="SearchController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Antlr.Runtime.Misc;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.Services;
    using LeeftSamen.Portal.Web.Models.Search;
    using LeeftSamen.Portal.Web.Utils;

    using Microsoft.Ajax.Utilities;
    using System.Configuration;

    public class SearchController : BaseController
    {
        private readonly ISearchService searchService;

        public SearchController(ICurrentUserInformation currentUserInformation, ISearchService searchService)
            : base(currentUserInformation)
        {
            this.searchService = searchService;
        }

        [HttpGet]
        public async Task<ActionResult> Index(string query)
        {
            var model = new IndexViewModel { Query = query };
            if (string.IsNullOrWhiteSpace(query) || query.Length <= 2)
            {
                return this.View(model);
            }

            var results = await this.searchService.SearchAsync(query, this.CurrentUserPosition, this.CurrentUserNeighborhoodRadius);
            foreach (var result in results.OrderByDescending(r => r.Score))
            {
                var category = string.Empty;
                var url = string.Empty;
                switch (result.Category)
                {
                    case SearchResult.Categories.Activities:
                        category = Title.Activities;
                        url = this.Url.RouteUrl(
                            "DefaultDetail",
                            new { controller = "Activities", action = "Detail", id = result.Identifier });
                        break;
                    case SearchResult.Categories.Circles:
                        category = Title.Circles;
                        url = this.Url.RouteUrl(
                            "DefaultDetail",
                            new { controller = "Circles", action = "Detail", id = result.Identifier });
                        break;
                    case SearchResult.Categories.Marketplace:
                        category = Title.Marketplace;
                        url = this.Url.RouteUrl(
                            "DefaultDetail",
                            new { controller = "Marketplace", action = "Detail", id = result.Identifier });
                        break;
                    case SearchResult.Categories.NeighborhoodMessages:
                        category = Title.NeighborhoodMessages;
                        var messageType = "NeighborMessages";
                        if (result.ExtraData.Value == "AssociationMessages")
                        {
                            messageType = "AssociationMessages";
                        }

                        if (result.ExtraData.Value == "OrganizationMessages")
                        {
                            messageType = "OrganizationMessages";
                        }

                        url = this.Url.RouteUrl(
                            "NeighborhoodMessage",
                            new { controller = "NeighborhoodMessages", messageType, messageId = result.Identifier, action = "MessageDetail" });
                        break;
                }

                model.Results.Add(
                    new IndexViewModel.Result { Label = result.Label, Category = category, Url = url });
            }

            model.UniqueCategories = model.Results.OrderBy(r => r.Category).DistinctBy(r => r.Category).Select(r => r.Category).ToArray();

            return this.View(string.Format("Index{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty),  model);
        }
    }
}