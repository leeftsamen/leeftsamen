// <copyright file="StatsController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Web.Helpers;

namespace LeeftSamen.Portal.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using LeeftSamen.Portal.Services;
    using LeeftSamen.Portal.Web.Models.Stats;
    using LeeftSamen.Portal.Web.Utils;

    [Authorize(Roles = "StatsViewer,Admin")]
    public class StatsController : BaseController
    {
        private readonly IStatsService statsService;

        private readonly IUserService userService;

        public StatsController(ICurrentUserInformation currentUserInformation, IStatsService statsService, IUserService userService)
            : base(currentUserInformation)
        {
            this.statsService = statsService;
            this.userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var lastStat = await this.statsService.GetLastStatAsync();

            // TODO: make this a cronjob
            if (lastStat == null || lastStat.DateTime < DateTime.Now.AddDays(-1))
            {
                lastStat = await this.statsService.GenerateStatEntryAsync();
            }

            var model = new IndexViewModel
                            {
                                StatDate = lastStat.DateTime,
                                UsersActiveDays = lastStat.UsersActiveDays,
                                UsersActiveHour = await this.statsService.GetUsersActiveHourAsync(),
                                UsersActiveHours = lastStat.UsersActiveHours,
                                UsersAvgNeighborhoodRadius = lastStat.UsersAvgNeighborhoodRadius,
                                UsersTotal = lastStat.UsersTotal,
                                UsersPerCity = await this.statsService.GetUsersPerCityAsync(),
                                NeighborhoodMessagesTotal = lastStat.NeighborhoodMessagesTotal,
                                NeighborhoodAssociationMessages = lastStat.NeighborhoodAssociationMessages,
                                NeighborhoodNeighborMessages = lastStat.NeighborhoodNeighborMessages,
                                NeighborhoodOrganizationMessages = lastStat.NeighborhoodOrganizationMessages,
                                OrganisationAssociations = lastStat.OrganisationAssociations,
                                OrganisationProfessionals = lastStat.OrganisationProfessionals,
                                OrganisationsTotal = lastStat.OrganisationsTotal,
                                OrganisationVolunteers = lastStat.OrganisationVolunteers,
                                CircleJobs = lastStat.CircleJobs,
                                CircleMembers = lastStat.CircleMembers,
                                CircleMessages = lastStat.CircleMessages,
                                CirclesPrivate = lastStat.CirclesPrivate,
                                CirclesPublic = lastStat.CirclesPublic,
                                CirclesTotal = lastStat.CirclesTotal,
                                ActivitiesAttendees = lastStat.ActivitiesAttendees,
                                ActivitiesTotal = lastStat.ActivitiesTotal,
                                MarketplaceItemsAsked = lastStat.MarketplaceItemsAsked,
                                MarketplaceItemsOffered = lastStat.MarketplaceItemsOffered,
                                MarketplaceItemsTotal = lastStat.MarketplaceItemsTotal,
                                MarketplaceItemsPerCategory = await this.statsService.GetMarketplaceItemsPerCategoryAsync()
                            };
            return this.View(model);
        }
    }
}