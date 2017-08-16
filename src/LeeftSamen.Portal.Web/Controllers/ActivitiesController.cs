// <copyright file="ActivitiesController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using AutoMapper;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.Services;
    using LeeftSamen.Portal.Web.Attributes;
    using LeeftSamen.Portal.Web.Models.Activities;
    using LeeftSamen.Portal.Web.Models.Circles;
    using LeeftSamen.Portal.Web.Utils;

    using Newtonsoft.Json;

    using DetailViewModel = LeeftSamen.Portal.Web.Models.Activities.DetailViewModel;
    using IndexViewModel = LeeftSamen.Portal.Web.Models.Activities.IndexViewModel;
    using Helpers;

    [CurrentUserMustBeInActiveCity]
    public class ActivitiesController : BaseController
    {
        private readonly IActivityService activityService;

        private readonly ICircleService circleService;

        private readonly IHelpIconService helpIconService;

        private readonly IUserService userService;

        private readonly ISharedService sharedService;

        private readonly INotificationService notificationService;

        private readonly ILinkGenerator linkGenerator;

        public ActivitiesController(
            ICurrentUserInformation currentUserInformation,
            IActivityService activityService,
            ICircleService circleService,
            IHelpIconService helpIconService,
            ISharedService sharedService,
            IUserService userService,
            INotificationService notificationService,
            ILinkGenerator linkGenerator)
            : base(currentUserInformation)
        {
            this.activityService = activityService;
            this.circleService = circleService;
            this.helpIconService = helpIconService;
            this.sharedService = sharedService;
            this.userService = userService;
            this.notificationService = notificationService;
            this.linkGenerator = linkGenerator;
        }

        public static IndexViewModel GetActivitiesOverviewModel(
            IEnumerable<IActivity> activities,
            IEnumerable<HelpIcon> helpIcons,
            User CurrentUser,
            int? circleId)
        {
            var model = new IndexViewModel
            {
                Activities = activities.Select(
                                    a =>
                                        {
                                            var activity = Mapper.Map<IndexViewModel.ActivityViewModel>(a);
                                            var attendance = a.Attendees.FirstOrDefault(at => at.User.Id == CurrentUser.Id);
                                            activity.Attending = attendance != null ? attendance.Attending : (bool?)null;
                                            activity.StartDateTime = activity.StartDateTime.Date < DateTime.Now.Date
                                                                         ? DateTime.MinValue
                                                                         : activity.StartDateTime;
                                            return activity;
                                        }).ToList(),
                HelpIcons = helpIcons,
                ShownInCircle = circleId
            };
            return model;
        }

        [HttpGet]
        public async Task<ActionResult> AcceptInvitation(int? id, string code)
        {
            var invitation = await this.activityService.GetInvitationByActivityIdAcceptTokenAsync(id, code);
            if (invitation == null)
            {
                this.NotifyUserDanger(Alert.ActivityInvitationExpired);
            }
            else
            {
                if (invitation.User == null)
                {
                    invitation.User = await this.userService.FindByEmailAsync(invitation.Email);
                }

                if (invitation.InvitedBy.Id == this.CurrentUser.Id)
                {
                    this.NotifyUserDanger(Alert.ActivityInvitationExpired, invitation.Activity.Title);
                }
                else
                {
                    this.NotifyUserSuccess(Alert.ActivityAttending, invitation.Activity.Title);
                    await this.activityService.AcceptInvitationAsync(invitation, this.CurrentUser);
                }
            }

            return this.RedirectToAction("Detail", "Activities", new { id });
        }

        [ChildActionOnly]
        public ActionResult ActivitiesOverview(IndexViewModel model)
        {
            return this.PartialView(string.Format("ActivitiesOverview{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
            //return this.PartialView("ActivitiesOverview", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Attendance(int? id, int? circleId, bool attending, bool redirectToDetailPage = false)
        {
            if (id != null)
            {
                var activity = await this.activityService.GetActivityByIdAsync(id.Value);
                if (activity == null)
                {
                    return this.HttpNotFound();
                }

                await this.activityService.SetAttendanceAsync(activity, this.CurrentUser, attending);
                string message;
                if (attending)
                {
                    message = string.Format(Common.InterfaceText.Notification.MobileActivityAttending, this.CurrentUser.Name, activity.Title);
                }
                else
                {
                    message = string.Format(Common.InterfaceText.Notification.MobileActivityNotAttending, this.CurrentUser.Name, activity.Title);
                }

                this.NotifyUserSuccess(attending ? Alert.ActivityAttending : Alert.ActivityNotAttending, activity.Title);
            }

            if (redirectToDetailPage)
            {
                return this.RedirectToAction("Detail", "Activities", new { id, circleId });
            }

            return this.RedirectToAction("Index", "Activities", new { circleId });
        }

        [HttpGet]
        public async Task<ActionResult> Attendance(int? id)
        {
            var activity = await this.activityService.GetActivityByIdForPositionAsync(id, this.CurrentUserPosition, this.CurrentUserNeighborhoodRadius);
            if (activity == null)
            {
                return this.HttpNotFound();
            }

            return this.RedirectToAction("Index", "Activities");
        }

        [HttpGet]
        public ActionResult Create(int? circleId)
        {
            ActionResult result;
            if (!this.CurrentUserHasCirclePermissions(circleId, out result))
            {
                return result;
            }

            return this.View(
                string.Format("Edit{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty),
                new PostViewModel
                {
                    StartDateTime = DateTime.Now,
                    EndDateTime = DateTime.Now.AddHours(1),
                    AllowSharing = true,
                    CircleId = circleId
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PostViewModel postModel, int? circleId)
        {
            ActionResult result;
            if (!this.CurrentUserHasCirclePermissions(circleId, out result))
            {
                return result;
            }

            postModel.StartDateTime =
                postModel.StartDateTime.AddHours(postModel.StartDateHour).AddMinutes(postModel.StartDateMinute);
            postModel.EndDateTime =
                postModel.EndDateTime.AddHours(postModel.EndDateHour).AddMinutes(postModel.EndDateMinute);

            if (postModel.EndDateTime < postModel.StartDateTime)
            {
                this.ModelState.AddModelError("EndDateTime", Error.ActivityEndDateBeforeStartDate);
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(string.Format("Edit{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), postModel);
            }

            var activity =
                await
                this.activityService.InsertAsync(
                    postModel.Title,
                    postModel.Description,
                    postModel.StartDateTime,
                    postModel.EndDateTime,
                    postModel.Location,
                    postModel.AllDay,
                    this.CurrentUser,
                    postModel.AllowSharing,
                    postModel.AllAges,
                    postModel.AgeFrom ?? 0,
                    postModel.AgeTo ?? 0,
                    postModel.Recurring,
                    postModel.Recurring == Activity.Recurrance.No ? null : postModel.RecurringEnd,
                    this.CurrentUserInformation.OrganizationMembership,
                    await this.circleService.GetCircleByIdAsync(circleId));

            this.NotifyUserSuccess(Alert.ActivityCreated);

            if (circleId.HasValue)
            {
                return
                    this.Redirect(
                        this.Url.RouteUrl(
                            "CircleActivitiesDetail",
                            new { controller = "Activities", action = "Detail", id = activity.ActivityId, circleId }));
            }
            else
            {
                return
                    this.Redirect(
                        this.Url.RouteUrl(
                            "DefaultDetail",
                            new { controller = "Activities", action = "Detail", id = activity.ActivityId }));
            }
        }

        [HttpGet]
        public ActionResult CreateBulk()
        {
            return this.View(string.Format("CreateBulk{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateBulk(CreateBulkPostModel model)
        {
            var created = 0;
            if (model.Activities != null)
            {
                var activityDates =
                    JsonConvert.DeserializeObject<List<CreateBulkPostModel.ActivityDate>>(model.Activities);
                foreach (var date in activityDates)
                {
                    var dateTime = DateTime.Parse(string.Join("-", date.Date.Split('-').Reverse()));
                    foreach (var activity in date.Activities)
                    {
                        var startDate =
                            dateTime.AddHours(int.Parse(activity.StartTimeH)).AddMinutes(int.Parse(activity.StartTimeM));
                        var endDate =
                            dateTime.AddHours(int.Parse(activity.EndTimeH)).AddMinutes(int.Parse(activity.EndTimeM));

                        if (
                            await
                            this.activityService.InsertAsync(
                                activity.Title,
                                string.Format("<p>{0}</p>", activity.Description),
                                startDate,
                                endDate,
                                activity.Location,
                                activity.AllDay,
                                this.CurrentUser,
                                model.Shareable,
                                true,
                                0,
                                0,
                                Activity.Recurrance.No,
                                null,
                                this.CurrentUserInformation.OrganizationMembership) != null)
                        {
                            created++;
                        }
                    }
                }
            }

            this.NotifyUserSuccess(string.Format(Alert.ActivitiesCreated, created));

            return
                this.Redirect(this.Url.RouteUrl("Default", new { controller = "Activities", action = "MyActivities" }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateReaction(int activityId, int? circleId, string newReaction)
        {
            var activity = await this.activityService.GetActivityByIdAsync(activityId);

            if (activity == null)
            {
                return this.HttpNotFound();
            }

            ActionResult result;
            if (!this.CurrentUserHasCirclePermissions(circleId, out result))
            {
                return result;
            }

            if (this.ModelState.IsValid && !string.IsNullOrEmpty(newReaction) && !newReaction.StartsWith(" "))
            {
                await
                    this.activityService.CreateReactionAsync(
                        activity,
                        this.CurrentUser,
                        this.CurrentUserInformation.OrganizationMembership,
                        newReaction);

                if (this.CurrentUser.Id != activity.Creator.Id)
                {
                    await this.notificationService.CreateNotificationForUserAsync(
                        activity.Creator,
                        string.Format(Common.InterfaceText.Notification.NeighborhoodMessageReaction, this.CurrentUser.Name, activity.Title),
                        this.linkGenerator.GenerateActivityLink(activity.ActivityId, activity.Circle != null ? ((int?)activity.Circle.CircleId) : null),
                        Data.Enums.SettingName.ActivitiesReaction);
                }

                this.NotifyUserSuccess(Alert.NeighborhoodMessageReactionCreated);
            }

            if (circleId.HasValue)
            {
                return this.Redirect(
                        this.Url.RouteUrl(
                            "CircleActivitiesDetail",
                            new { id = activityId, circleId }
                            ));
            }
            else
            {
                return this.RedirectToAction("Detail", new { id = activityId });
            }
        }

        [HttpGet]
        public async Task<ActionResult> Detail(int id, int? circleId)
        {
            ActionResult result;
            if (!this.CurrentUserHasCirclePermissions(circleId, out result))
            {
                return result;
            }

            var activity = await this.activityService.GetActivityByIdAsync(id);
            if (activity == null)
            {
                return this.HttpNotFound();
            }

            if (activity.Circle != null)
            {
                if (activity.Circle.CircleId != circleId)
                {
                    return this.HttpNotFound();
                }
            }
            else
            {
                if (circleId.HasValue)
                {
                    return this.HttpNotFound();
                }
            }

            var currentUserIsAttending = activity.Attendees.Any(u => u.User == this.CurrentUser && u.Attending);
            var model = new DetailViewModel
            {
                Activity = activity,
                UserCanEdit = this.CurrentUserCanEdit(activity),
                CurrentUserIsAttending = currentUserIsAttending,
                ShownInCircle = circleId
            };
            return this.View(string.Format("Detail{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id, int? circleId)
        {
            var activity = await this.activityService.GetActivityByIdAsync(id);
            if (activity == null || !this.CurrentUserCanEdit(activity))
            {
                return this.HttpNotFound();
            }

            ActionResult result;
            if (!this.CurrentUserHasCirclePermissions(circleId, out result))
            {
                return result;
            }

            var model = Mapper.Map<PostViewModel>(activity);
            model.StartDateHour = model.StartDateTime.Hour;
            model.StartDateMinute = model.StartDateTime.Minute;
            model.EndDateHour = model.EndDateTime.Hour;
            model.EndDateMinute = model.EndDateTime.Minute;
            model.CircleId = circleId;
            return this.View(string.Format("Edit{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, int? circleId, PostViewModel postModel)
        {
            var activity = await this.activityService.GetActivityByIdAsync(id);
            if (activity == null || !this.CurrentUserCanEdit(activity))
            {
                return this.HttpNotFound();
            }

            ActionResult result;
            if (!this.CurrentUserHasCirclePermissions(circleId, out result))
            {
                return result;
            }

            postModel.StartDateTime =
                postModel.StartDateTime.AddHours(postModel.StartDateHour).AddMinutes(postModel.StartDateMinute);
            postModel.EndDateTime =
                postModel.EndDateTime.AddHours(postModel.EndDateHour).AddMinutes(postModel.EndDateMinute);
            postModel.CircleId = circleId;

            if (postModel.EndDateTime < postModel.StartDateTime)
            {
                this.ModelState.AddModelError("EndDateTime", Error.ActivityEndDateBeforeStartDate);
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(string.Format("Edit{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), postModel);
            }

            await
                this.activityService.UpdateAsync(
                    activity,
                    postModel.Title,
                    postModel.Description,
                    postModel.StartDateTime,
                    postModel.EndDateTime,
                    postModel.Location,
                    postModel.AllDay,
                    postModel.AllowSharing,
                    postModel.AllAges,
                    postModel.AgeFrom ?? 0,
                    postModel.AgeTo ?? 0,
                    postModel.Recurring,
                    postModel.Recurring == Activity.Recurrance.No ? null : postModel.RecurringEnd);
            if (circleId.HasValue)
            {
                return
                    this.Redirect(
                        this.Url.RouteUrl(
                            "CircleActivitiesDetail",
                            new { controller = "Activities", action = "Detail", id = activity.ActivityId, circleId }));
            }
            else
            {
                return
                    this.Redirect(
                        this.Url.RouteUrl(
                            "DefaultDetail",
                            new { controller = "Activities", action = "Detail", id = activity.ActivityId }));
            }
        }

        [HttpGet]
        public async Task<ActionResult> Index(int? circleId, int? age)
        {
            this.sharedService.VisitPage(this.CurrentUser, Data.Enums.PageVisitType.Activities);

            ActionResult result;
            if (!this.CurrentUserHasCirclePermissions(circleId, out result))
            {
                return result;
            }

            List<IActivity> activities = null;
            if (age.HasValue)
            {
                activities = await this.activityService.GetCurrentAndUpcomingFilteredByAgeAsync(this.CurrentUser.Id, circleId, this.CurrentUserPosition, this.CurrentUserNeighborhoodRadius, age.Value);
            }
            else
            {
                activities = await this.activityService.GetCurrentAndUpcomingAsync(this.CurrentUser.Id, circleId, this.CurrentUserPosition, this.CurrentUserNeighborhoodRadius);
            }

            var model =
                GetActivitiesOverviewModel(
                    activities,
                    await this.helpIconService.GetNonShownHelpIconsForUserAsync(this.CurrentUser),
                    this.CurrentUser,
                    circleId);

            return this.View(string.Format("Index{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public ActionResult InviteUser(int? id)
        {
            if (!id.HasValue)
            {
                return this.HttpNotFound();
            }

            return this.RedirectToRoute("DefaultDetail", new { controller = "Activities", action = "Detail", id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InviteUser(int? id, string userId)
        {
            if (this.CurrentUserCanOnlyView())
            {
                return this.Json(new { state = "danger", message = Alert.UsersInvitedByOrganizationUserFailed });
            }

            var activity = await this.activityService.GetActivityByIdForPositionAsync(id, this.CurrentUserPosition, this.CurrentUserNeighborhoodRadius);
            if (activity == null)
            {
                return this.Json(new { state = "danger", message = Alert.UsersInvitedByEmailFailed });
            }

            var user = await this.userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return this.Json(new { state = "danger", message = Alert.UsersInvitedByEmailFailed });
            }

            await this.activityService.InviteUserAsync(activity, user, this.CurrentUser, this.PortalUrl);

            return this.Json(new { state = "success", message = string.Format(Alert.UserInvitedForActivity, user.Name) });
        }

        [HttpGet]
        public ActionResult InviteUsersByEmail(int? id)
        {
            if (!id.HasValue)
            {
                return this.HttpNotFound();
            }

            return this.RedirectToRoute("DefaultDetail", new { controller = "Activities", action = "Detail", id });
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InviteUsersByEmail(int? id, string emailAddresses)
        {
            if (id != null)
            {
                var activity = await this.activityService.GetActivityByIdAsync(id.Value);
                if (activity == null)
                {
                    return this.HttpNotFound();
                }

                var addresses = this.SeparateEmailAddresses(emailAddresses).ToList();
                var filteredAddresses = addresses.Where(x => Utils.IsValidEmail(x)).ToList();
                var filteredEmailAddresses = string.Join(", ", filteredAddresses);
                if (filteredAddresses.Any())
                {
                    await
                        this.activityService.InviteUserByEmailAsync(
                            activity,
                            addresses,
                            this.CurrentUser,
                            this.PortalUrl);

                    return this.Json(new { state = "success", message = string.Format(Alert.UserIsInvitedForTheActivityByEmail, filteredEmailAddresses) });
                }
            }

            return this.Json(new { state = "danger", message = Alert.UsersInvitedByEmailFailed });
        }

        public async Task<ActionResult> MyActivities(int? circleId)
        {
            ActionResult result;
            if (!this.CurrentUserHasCirclePermissions(circleId, out result))
            {
                return result;
            }

            var model =
                GetActivitiesOverviewModel(
                    await this.activityService.GetCurrentAndUpcomingActivitiesCreateByUserAsync(this.CurrentUser, circleId),
                    await this.helpIconService.GetNonShownHelpIconsForUserAsync(this.CurrentUser),
                    this.CurrentUser,
                    circleId);

            return this.View(string.Format("Index{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Remove(int? id, int? circleId)
        {
            if (!id.HasValue)
            {
                return this.HttpNotFound();
            }

            ActionResult result;
            if (!this.CurrentUserHasCirclePermissions(circleId, out result))
            {
                return result;
            }

            var activity = await this.activityService.GetActivityByIdAsync(id.Value);
            if (activity == null || !this.CurrentUserCanEdit(activity))
            {
                return this.HttpNotFound();
            }

            await this.activityService.DeleteAsync(activity);

            if (circleId.HasValue)
            {
                return this.Redirect(this.Url.RouteUrl("CircleActivities", new { controller = "Activities", action = "Index", circleId }));
            }
            else
            {
                return this.Redirect(this.Url.RouteUrl("Default", new { controller = "Activities", action = "Index" }));
            }
        }

        [HttpGet]
        public ActionResult Remove()
        {
            return this.Redirect(this.Url.RouteUrl("Default", new { controller = "Activities", action = "Index" }));
        }

        public async Task<ActionResult> SearchUsers(int? id, string q)
        {
            if (!id.HasValue)
            {
                return this.HttpNotFound();
            }

            var activity = await this.activityService.GetActivityByIdAsync(id.Value);
            if (activity == null)
            {
                return this.HttpNotFound();
            }

            // Get users who are not invited and / or not attending the activity
            var users = await this.activityService.SearchNeighborsAsync(this.CurrentUser.Id, this.CurrentUserPosition, this.CurrentUserNeighborhoodRadius, q);
            var usersInvited =
                users.Where(user => user.ActivityInvitations.Any(a => a.Activity.ActivityId == activity.ActivityId))
                    .ToList();
            var usersAttending =
                users.Where(user => activity.Attendees.Any(a => a.User.Id == user.Id && a.Attending)).ToList();
            var usersNotAttendingAndNotInvited = users.Except(usersInvited).Except(usersAttending);

            var mappedUsers = Mapper.Map<List<SearchUserViewModel>>(usersNotAttendingAndNotInvited);

            foreach (var user in mappedUsers.Where(u => u.HasProfileImage))
            {
                user.ProfileImageUrl = this.Url.RouteUrl(
                    "UserProfileImage",
                    new
                    {
                        controller = "Users",
                        action = "ProfileImage",
                        userId = user.UserId,
                        mediaId = user.ProfileImageId
                    });
            }

            return this.Json(mappedUsers, JsonRequestBehavior.AllowGet);
        }

        private bool CurrentUserCanEdit(Activity activity)
        {
            return (this.CurrentUserInformation.OrganizationMembership != null
                    && activity.OrganizationMembershipId.HasValue
                    && this.CurrentUserInformation.OrganizationMembership.Organization.OrganizationId
                    == activity.OrganizationMembership.Organization.OrganizationId
                    && this.CurrentUserInformation.OrganizationMembership.IsAdministrator)
                   || (this.CurrentUserInformation.OrganizationMembership == null
                       && !activity.OrganizationMembershipId.HasValue && activity.Creator.Id == this.CurrentUser.Id)
                   || this.CurrentUser.Roles.Any(r => r.RoleId == "Admin");
        }

        private bool CurrentUserHasCirclePermissions(int? circleId, out ActionResult result)
        {
            if (circleId.HasValue)
            {
                // No async/await here, because it's not supported by ASP.NET MVC in childactions.
                var circle = this.circleService.GetCircleByIdAsync(circleId).Result;
                if (circle == null)
                {
                    result = this.HttpNotFound();
                    return false;
                }

                var userIsMemberOfCircle = this.circleService.IsUserMemberOfCircle(circle, this.CurrentUser)
                                           && !this.CurrentUserCanOnlyView();
                if (!userIsMemberOfCircle)
                {
                    result = this.HttpForbidden();
                    return false;
                }
            }

            result = null;
            return true;
        }
    }
}