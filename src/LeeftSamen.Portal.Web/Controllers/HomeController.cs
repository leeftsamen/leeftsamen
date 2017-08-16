// <copyright file="HomeController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Web.Models.Register;

namespace LeeftSamen.Portal.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.Services;
    using LeeftSamen.Portal.Web.Models.Home;
    using LeeftSamen.Portal.Web.Utils;
    using Data.Enums;
    /// <summary>
    /// The home controller.
    /// </summary>
    public class HomeController : BaseController
    {
        private readonly ITimelineService timelineService;
        private readonly IActionService actionService;

        public HomeController(ICurrentUserInformation currentUserInformation, ITimelineService timelineService, IActionService actionService)
            : base(currentUserInformation)
        {
            this.timelineService = timelineService;
            this.actionService = actionService;
        }

        private enum ShowAtHome
        {
            All,

            Jobs,

            NeighborhoodMessages,

            Activities,

            Marketplace
        }

        [HttpGet]
        public async Task<ActionResult> Activities()
        {
            return await this.IndexAuthenticated(ShowAtHome.Activities);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            if (this.Request.IsAuthenticated)
            {
                 return await this.IndexAuthenticated(ShowAtHome.All);
            }

            this.ViewBag.BackgroundPoster = true;

            return this.View(new IndexRegisterAccountViewModel());
        }

        [HttpGet]
        public async Task<ActionResult> Jobs()
        {
            return await this.IndexAuthenticated(ShowAtHome.Jobs);
        }

        [HttpGet]
        public async Task<ActionResult> Marketplace()
        {
            return await this.IndexAuthenticated(ShowAtHome.Marketplace);
        }

        [HttpGet]
        public async Task<ActionResult> NeighborhoodMessages()
        {
            return await this.IndexAuthenticated(ShowAtHome.NeighborhoodMessages);
        }

        [HttpGet]
        public ActionResult PrivacyPolicy()
        {
            return this.View();
        }

        [HttpGet]
        public ActionResult TermsAndConditions()
        {
            return this.View();
        }

        private async Task<ActionResult> IndexAuthenticated(ShowAtHome show)
        {
            var timeLine = new List<IndexAuthenticatedViewModel.TimelineItem>();

            var since = DateTime.Now.AddMonths(-1);

            if (System.Web.HttpContext.Current.Session["registeriosafterlogon"] != null)
            {
                this.CurrentUserInformation.AddDevice(System.Web.HttpContext.Current.Session["registeriosafterlogon"].ToString(), DeviceType.iOS);
            }

            if (System.Web.Configuration.WebConfigurationManager.AppSettings["Company"] == "leeftsamen" && (show == ShowAtHome.All || show == ShowAtHome.NeighborhoodMessages))
            {
                var messages = await this.timelineService.GetNeighborhoodMessagesAsync(this.CurrentUser, since);
                foreach (var message in messages)
                {
                    var messageType = "NeighborMessages";
                    if (message.OrganizationMembershipId.HasValue
                        && message.OrganizationMembership.Organization.OrganizationType.Type
                        == OrganizationType.Types.Association)
                    {
                        messageType = "AssociationMessages";
                    }
                    else if (message.OrganizationMembershipId.HasValue)
                    {
                        messageType = "OrganizationMessages";
                    }

                    var timelineItem = new IndexAuthenticatedViewModel.NeighborhoodMessageTimelineItem
                    {
                        Category = Title.NeighborhoodMessages,
                        CategoryClass = "fa fa-newspaper-o",
                        Title = message.Title,
                        Description = message.IntroductionText,
                        Date = message.CreationDateTime,
                        ImageId = message.Image1Id,
                        MessageId = message.MessageId,
                                Url =
                                    this.Url.RouteUrl(
                                        "NeighborhoodMessage",
                                        new
                                        {
                                            controller = "NeighborhoodMessages",
                                            messageType,
                                            messageId = message.MessageId,
                                            action = "MessageDetail"
                                        }),
                        Action =
                                    string.Format(
                                        Common.InterfaceText.Notification
                                    .NeighborhoodMessageAdded,
                                        message.Creator.Name,
                                        message.Title),
                        UserProfileImageId =
                                    message.OrganizationMembershipId.HasValue
                                        ? null
                                        : message.Creator.ProfileImageId,
                        UserId = message.OrganizationMembershipId.HasValue
                                        ? null
                                        : message.Creator.Id
                    };

                    var messageType2 = NeighborhoodMessage.MessageTypes.NeighborMessages;
                    if (message.OrganizationMembershipId.HasValue)
                    {
                        switch (message.OrganizationMembership.Organization.OrganizationType.Type)
                        {
                            case OrganizationType.Types.Professional:
                            case OrganizationType.Types.Volunteer:
                                messageType2 = NeighborhoodMessage.MessageTypes.OrganizationMessages;
                                break;
                            case OrganizationType.Types.Association:
                                messageType2 = NeighborhoodMessage.MessageTypes.AssociationMessages;
                                break;
                        }
                    }

                    timelineItem.MessageType = messageType2;

                    timeLine.Add(timelineItem);
                }
            }

            if (show == ShowAtHome.All || show == ShowAtHome.Jobs)
            {
                var jobs = await this.timelineService.GetTimeLineJobsAsync(this.CurrentUser, since);
                timeLine.AddRange(
                    jobs.Select(
                        j =>
                        new IndexAuthenticatedViewModel.JobTimelineItem
                            {
                                Category = Title.Circles,
                                CategoryClass = "fa fa-dot-circle-o",
                                Action =
                                    string.Format(
                                        Common.InterfaceText.Notification
                                    .JobAdded,
                                        j.Creator.Name,
                                        j.Title,
                                        j.Circle.Name),
                                Date = j.CreationDateTime,
                                Url =
                                    this.Url.RouteUrl(
                                        "CircleSubResources",
                                        new
                                            {
                                                action = UrlParameter.Optional,
                                                circleId = j.Circle.CircleId
                                            }),
                                UserProfileImageId = j.Creator.ProfileImageId,
                                UserId = j.Creator.Id,
                                CircleId = j.Circle.CircleId,
                                JobId = j.JobId,
                                Assigned = j.Assignee != null
                            }));
            }

            if (show == ShowAtHome.All || show == ShowAtHome.Marketplace)
            {
                var items = await this.timelineService.GetMarketplaceItemsAsync(this.CurrentUser, since, null);
                timeLine.AddRange(
                    items.Select(
                        i =>
                        new IndexAuthenticatedViewModel.MarketplaceTimelineItem
                            {
                                Category = GetTitle(i.Category.Alias),
                                CategoryClass = "fa fa-exchange",
                                Action =
                                    string.Format(
                                        Common.InterfaceText.Notification
                                    .MarketplaceItemAdded,
                                        i.Owner.Name,
                                        i.Title),
                                Date = i.CreationDateTime,
                                MarketplaceItemId = i.MarketplaceItemId,
                                ImageId = i.Image1Id,
                                Url =
                                    this.Url.RouteUrl(
                                        "DefaultDetail",
                                        new
                                            {
                                                controller = "Marketplace",
                                                action = "Detail",
                                                id = i.MarketplaceItemId
                                            }),
                                UserProfileImageId =
                                    i.OrganizationMembershipId.HasValue
                                        ? null
                                        : i.Owner.ProfileImageId,
                                        UserId = i.OrganizationMembershipId.HasValue
                                        ? null
                                        : i.Owner.Id
                            }));

                var reactions = await this.timelineService.GetMarketplaceReactionsAsync(this.CurrentUser, since);
                timeLine.AddRange(
                    reactions.Select(
                        r =>
                        new IndexAuthenticatedViewModel.TimelineItem
                            {
                                Category = GetTitle(r.MarketplaceItem.Category.Alias),
                                CategoryClass = "fa fa-exchange",
                                Action =
                                    string.Format(
                                        Common.InterfaceText.Notification
                                    .MarketplaceReaction,
                                        r.Creator.Name,
                                        r.MarketplaceItem.Title),
                                Date = r.CreationDateTime,
                                Url =
                                    this.Url.RouteUrl(
                                        "DefaultDetail",
                                        new
                                            {
                                                controller = "Marketplace",
                                                action = "Detail",
                                                id =
                                        r.MarketplaceItem.MarketplaceItemId
                                            })
                                    + "#reaction-" + r.ReactionId,
                                UserProfileImageId =
                                    r.OrganizationMembershipId.HasValue
                                        ? null
                                        : r.Creator.ProfileImageId,
                                        UserId = r.OrganizationMembershipId.HasValue
                                        ? null
                                        : r.Creator.Id
                            }));
            }

            if (show == ShowAtHome.All || show == ShowAtHome.Activities)
            {
                var activities = await this.timelineService.GetActivitiesAsync(this.CurrentUser, since);
                timeLine.AddRange(
                    activities.Select(
                        a =>
                        new IndexAuthenticatedViewModel.ActivityTimelineItem
                            {
                                Category = Title.Activities,
                                CategoryClass = "fa fa-calendar",
                                Action =
                                    string.Format(
                                        Common.InterfaceText.Notification
                                    .ActivityAdded,
                                        a.Creator.Name,
                                        a.Title),
                                Date = a.CreationDate,
                                ActivityId = a.ActivityId,
                                Attending = a.Attendees.Any(at => at.User.Id == this.CurrentUser.Id),
                                Url =
                                    this.Url.RouteUrl(
                                        "DefaultDetail",
                                        new
                                            {
                                                controller = "Activities",
                                                action = "Detail",
                                                id = a.ActivityId
                                            }),
                                UserProfileImageId =
                                    a.OrganizationMembershipId.HasValue
                                        ? null
                                        : a.Creator.ProfileImageId,
                                        UserId = a.OrganizationMembershipId.HasValue
                                        ? null
                                        : a.Creator.Id
                            }));

                var attendances = await this.timelineService.GetAttendancesAsync(this.CurrentUser, since);
                timeLine.AddRange(
                    attendances.Select(
                        a =>
                        new IndexAuthenticatedViewModel.TimelineItem
                            {
                                Category = Title.Activities,
                                CategoryClass = "fa fa-calendar",
                                Action =
                                    string.Format(
                                        Common.InterfaceText.Notification
                                    .ActivityAttending,
                                        a.User.Name,
                                        a.Activity.Title),
                                Date =
                                    a.ModificationDate.GetValueOrDefault(
                                        DateTime.Now),
                                Url =
                                    this.Url.RouteUrl(
                                        "DefaultDetail",
                                        new
                                            {
                                                controller = "Activities",
                                                action = "Detail",
                                                id = a.Activity.ActivityId
                                            }),
                                UserProfileImageId = a.User.ProfileImageId,
                                UserId = a.User.Id
                            }));
            }

            //var actions = await this.actionService.GetActionsAsync(this.CurrentUser);
            var actionItems =this.CurrentUserInformation.UserActions.Select( //actions.Select(
                a =>
                    new IndexAuthenticatedViewModel.ActionItem
                    {
                        ActionId = a.ActionId,
                        Name = a.Name,
                        Title = a.HomeTitle,
                        Text = a.HomeText
                    }
                ).ToList();

            var model = new IndexAuthenticatedViewModel
                            {
                                CurrentUserName = this.CurrentUser.Name,
                                UserId = this.CurrentUser.Id,
                                UserProfileImageId = this.CurrentUser.ProfileImageId,
                                Timeline = timeLine,
                                Actions = actionItems
                            };

            return this.View(string.Format("IndexAuthenticated{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        private static string GetTitle(MarketplaceItemCategory.CategoryAlias category)
        {
            switch (category)
            {
                case MarketplaceItemCategory.CategoryAlias.Borrowing:
                    return Title.StuffToBorrow;
                case MarketplaceItemCategory.CategoryAlias.HelpNeighborhood:
                    return Title.NeighbourHelp;
                case MarketplaceItemCategory.CategoryAlias.Meals:
                    return Title.Meals;
                case MarketplaceItemCategory.CategoryAlias.Stuff:
                    return Title.StuffForSale;
            }

            return Title.Marketplace;
        }
    }
}