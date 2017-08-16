// <copyright file="SendWeeklyActivitiesEmailReportJob.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.BackgroundService.Jobs
{
    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.EmailTemplates.Models;
    using LeeftSamen.Portal.Services;
    using log4net;
    using Quartz;
    using System;
    using System.Configuration;
    using System.Globalization;

    [DisallowConcurrentExecution]
    public class SendWeeklyActivitiesEmailReportJob : JobBase
    {
        private readonly ILog logger;

        private readonly IMailerService mailerService;

        private readonly ITimelineService timelineService;

        private readonly IUserService userService;

        public SendWeeklyActivitiesEmailReportJob(
            ILog logger,
            IUserService userService,
            ITimelineService timelineService,
            IMailerService mailerService)
            : base(logger)
        {
            this.logger = logger;
            this.userService = userService;
            this.timelineService = timelineService;
            this.mailerService = mailerService;
        }

        public override async void Execute(IJobExecutionContext context)
        {
            var culture = new CultureInfo(ConfigurationManager.AppSettings["DefaultCulture"]);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            this.logger.Info("SendWeeklyActivitiesEmailReportJob.Execute");

            var period = DateTime.Now.AddDays(-7);
            var users = await this.userService.GetAllUsersAsync();

            this.logger.Info(string.Format("Found {0} users", users.Count));

            foreach (var user in users)
            {
                this.logger.Info(string.Format("Newsletter.Processing user {0}", user.Email));

                if (!user.ReceivesWeekMail)
                {
                    this.logger.Info("Newsletter.User doesn't want email. Continue...");
                    continue;
                }

                var model = new WeeklyActivitiesModel();

                model.Subject = Subject.SendWeeklyActivities;
                model.Recipient = user;
                model.PortalUrl = this.PortalUrl;

                model.Messages = await this.timelineService.GetNeighborhoodMessagesAsync(user, period, 2);
                model.Activities = await this.timelineService.GetActivitiesAsync(user, period, 5);
                model.Circles = await this.timelineService.GetPublicCirclesAsync(user, period, 3);
                model.ForSale = await this.timelineService.GetMarketplaceItemsAsync(user, period, Portal.Data.Models.MarketplaceItemCategory.CategoryAlias.Stuff, 2);
                model.ToBorrow = await this.timelineService.GetMarketplaceItemsAsync(user, period, Portal.Data.Models.MarketplaceItemCategory.CategoryAlias.Borrowing, 2);
                model.Meals = await this.timelineService.GetMarketplaceItemsAsync(user, period, Portal.Data.Models.MarketplaceItemCategory.CategoryAlias.Meals, 2);
                model.NeighborHelp = await this.timelineService.GetMarketplaceItemsAsync(user, period, Portal.Data.Models.MarketplaceItemCategory.CategoryAlias.HelpNeighborhood, 2);

                if (model.Messages.Count == 0 && model.Activities.Count == 0 && model.ForSale.Count == 0 && model.ToBorrow.Count == 0 && model.Meals.Count == 0 && model.NeighborHelp.Count == 0 && model.Circles.Count == 0)
                {
                    this.logger.Info(string.Format("No data found for {0}. Continue...", user.Email));
                    continue;
                }

                this.logger.Info(
                    string.Format(
                        "Sending {0} messages, {1} activities, {2} items and {3} circles to {4}. Success...",
                        model.Messages.Count,
                        model.Activities.Count,
                         model.ForSale.Count + model.ToBorrow.Count + model.Meals.Count + model.NeighborHelp.Count,
                         model.Circles.Count,
                        user.Email));
                try
                {
                    await this.mailerService.SendAsync(model, user.Email).ConfigureAwait(false);
                }
                catch
                {

                }
            }

            this.logger.Info("SendWeeklyActivitiesEmailReportJob.Finished");
        }
    }
}