// <copyright file="OrganizationsController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System.Text.RegularExpressions;

namespace LeeftSamen.Portal.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.ServiceModel.Syndication;
    using System.Threading.Tasks;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.UI;

    using AutoMapper;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.Services;
    using LeeftSamen.Portal.Web.Attributes;
    using LeeftSamen.Portal.Web.Extensions;
    using LeeftSamen.Portal.Web.Helpers;
    using LeeftSamen.Portal.Web.Models;
    using LeeftSamen.Portal.Web.Models.Circles;
    using LeeftSamen.Portal.Web.Models.NeighborhoodMessages;
    using LeeftSamen.Portal.Web.Models.Organizations;
    using LeeftSamen.Portal.Web.Utils;

    using DetailHeaderViewModel = LeeftSamen.Portal.Web.Models.Organizations.DetailHeaderViewModel;
    using DetailViewModel = LeeftSamen.Portal.Web.Models.Organizations.DetailViewModel;
    using IndexViewModel = LeeftSamen.Portal.Web.Models.Organizations.IndexViewModel;
    using MembersViewModel = LeeftSamen.Portal.Web.Models.Organizations.MembersViewModel;

    [CurrentUserMustBeInActiveCity]
    public class OrganizationsController : BaseController
    {
        private readonly IActivityService activityService;

        private readonly IHelpIconService helpIconService;

        private readonly INeighborhoodMessageService neighborhoodMessageService;

        private readonly IOrganizationService organizationService;

        private readonly ISharedService sharedService;

        private readonly IUserService userService;

        public OrganizationsController(
            ICurrentUserInformation currentUserInformation,
            IUserService userService,
            IOrganizationService organizationService,
            INeighborhoodMessageService neighborhoodMessageService,
            IActivityService activityService,
            ISharedService sharedService,
            IHelpIconService helpIconService)
            : base(currentUserInformation)
        {
            this.userService = userService;
            this.organizationService = organizationService;
            this.neighborhoodMessageService = neighborhoodMessageService;
            this.activityService = activityService;
            this.sharedService = sharedService;
            this.helpIconService = helpIconService;
        }

        private async Task<bool> CurrentUserCanEditOrganization(Organization organization)
        {
            if (this.CurrentUser.Roles.Any(r => r.RoleId == "Admin"))
            {
                return true;
            }

            return await this.organizationService.IsUserAdministratorOfOrganizationAsync(organization, this.CurrentUser);
        }

        [HttpGet]
        public async Task<ActionResult> AcceptInvitation(int? id, string code)
        {
            var invitation = await this.organizationService.GetInvitationByOrganizationIdAcceptTokenAsync(id, code);
            if (invitation == null)
            {
                // TODO: Instead show a notification that the invitation has expired / is invalid.
                return this.HttpNotFound();
            }

            if (invitation.User != null && invitation.User.Id != this.CurrentUser.Id)
            {
                // TODO: Instead show a notification that the invitation has expired / is invalid.
                this.NotifyUserDanger(Error.InvitationWrongUser);
                return this.RedirectToAction("Index", "Home");
                //return this.HttpForbidden();
            }

            var membership = await this.organizationService.AcceptInvitationAsync(invitation, this.CurrentUser);
            this.NotifyUserSuccess(Alert.YouAreNowAOrganizationMember, membership.Organization.Name);

            return this.RedirectToAction("Detail", "Organizations", new { id = membership.Organization.OrganizationId });
        }

        [HttpGet]
        public async Task<ActionResult> Activities(int? id)
        {
            var organization = await this.organizationService.GetOrganizationByIdAsync(id);
            if (organization == null)
            {
                return this.HttpNotFound();
            }

            if (!(await this.organizationService.IsUserMemberOfOrganizationAsync(organization, this.CurrentUser)))
            {
                return this.HttpForbidden();
            }

            var model = Mapper.Map<ActivitiesViewModel>(organization);
            model.ActivitiesModel =
                ActivitiesController.GetActivitiesOverviewModel(
                    await this.activityService.GetCurrentAndUpcomingOrganizationActivitiesAsync(organization),
                    await this.helpIconService.GetNonShownHelpIconsForUserAsync(this.CurrentUser),
                    this.CurrentUser,
                    null);

            return this.View(string.Format("Activities{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public async Task<ActionResult> Activity(int? id, int? activityId)
        {
            var organization = await this.organizationService.GetOrganizationByIdAsync(id);
            if (organization == null)
            {
                return this.HttpNotFound();
            }

            var activity = await this.activityService.GetActivityByIdAsync(activityId.Value);
            if (activity == null
                || activity.OrganizationMembership.Organization.OrganizationId != organization.OrganizationId)
            {
                return this.HttpNotFound();
            }

            var model = new Models.Activities.DetailViewModel
            {
                Activity = activity,
                Reactions = activity.Reactions,
                CurrentUserIsAttending =
                                    activity.Attendees.Any(
                                        u => u.User == this.CurrentUser && u.Attending)
            };

            return this.View(string.Format("~/Views/Activities/Detail{0}.cshtml", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ActivityFeed(int? id)
        {
            var organization = await this.organizationService.GetOrganizationByIdAsync(id);
            if (organization == null)
            {
                return this.HttpNotFound();
            }

            var feed = new SyndicationFeed(
                string.Format(Title.OrganizationActivityFeed, organization.Name),
                string.Format(Text.OrganizationActivityFeed, organization.Name),
                new Uri(this.PortalUrl + "organizations/" + organization.OrganizationId),
                "Org" + organization.OrganizationId + "Activities",
                DateTime.Now);

            var items = new List<SyndicationItem>();
            var activities = await this.activityService.GetOrganizationActivitiesAsync(organization);
            items.AddRange(
                activities.Select(
                    a =>
                    new SyndicationItem(
                        a.Title,
                        this.GetActivityContent(a),
                        new Uri(this.PortalUrl + "activities/" + a.ActivityId.ToString()),
                        a.ActivityId.ToString(),
                        a.CreationDate)));
            feed.Items = items;

            return new AtomResult(feed);
        }

        [HttpGet]
        public async Task<ActionResult> Associations()
        {
            return
                this.IndexView(
                    await
                    this.organizationService.GetOrganizationsByTypeActiveInNeighborhoodAsync(
                        OrganizationType.Types.Association,
                        this.CurrentUserPosition,
                        this.CurrentUserNeighborhoodRadius),
                    await this.helpIconService.GetNonShownHelpIconsForUserAsync(this.CurrentUser));
        }

        [HttpGet]
        public async Task<ActionResult> Detail(int? id)
        {
            var organization = await this.organizationService.GetOrganizationByIdAsync(id);
            if (organization == null)
            {
                return this.HttpNotFound();
            }

            var model = Mapper.Map<DetailViewModel>(organization);
            model.CurrentUserIsOrganizationAdministrator =
                await this.CurrentUserCanEditOrganization(organization);
            model.Messages =
                await
                this.neighborhoodMessageService.GetLatestMessagesByOrganizationIdAsync(organization.OrganizationId);

            if (model.HasWebsite && !Regex.IsMatch(model.Website, "^https?://.*"))
            {
                model.Website = "http://" + model.Website;
            }

            return this.View(string.Format("Detail{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [ChildActionOnly]
        public ActionResult DetailHeader(int? id)
        {
            // No async/await here, because it's not supported by ASP.NET MVC in childactions.
            var organization = this.organizationService.GetOrganizationByIdAsync(id).Result;
            if (organization == null)
            {
                return this.HttpNotFound();
            }

            var userIsMemberOfOrganization =
                this.organizationService.IsUserMemberOfOrganizationAsync(organization, this.CurrentUser).Result;

            var menuItems = new List<MenuItemModel>();

            if (userIsMemberOfOrganization)
            {
                menuItems.Add(
                    new MenuItemModel(
                        "DefaultDetail",
                        new RouteValueDictionary(new { id, controller = "Organizations", action = "Detail" }),
                        Button.Info));
                menuItems.Add(
                    new MenuItemModel(
                        "DefaultDetail",
                        new RouteValueDictionary(new { id, controller = "Organizations", action = "Activities" }),
                        Button.OrganizationActivities));
                menuItems.Add(
                    new MenuItemModel(
                        "DefaultDetail",
                        new RouteValueDictionary(new { id, controller = "Organizations", action = "Members" }),
                        Button.OrganizationMembers));
            }

            foreach (var item in menuItems)
            {
                item.Selected = ViewUtils.IsActiveAction(
                    this.ControllerContext.ParentActionViewContext,
                    item.RouteValues["controller"].ToString(),
                    item.RouteValues["action"].ToString());
            }

            var model = Mapper.Map<DetailHeaderViewModel>(organization);
            model.CurrentUserIsOrganizationAdministrator =
                this.CurrentUserCanEditOrganization(organization).Result;
            model.CurrentUserIsMember =
                this.organizationService.IsUserMemberOfOrganizationAsync(organization, this.CurrentUser).Result;
            model.MenuItems = menuItems;

            return this.PartialView(string.Format("_DetailHeader{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public async Task<ActionResult> DistrictsByCity(string city)
        {
            var districts = new List<TownViewModel>();
            var results = await this.organizationService.GetDistrictsAndCitiesByCityAsync(city);

            foreach (var item in results)
            {
                var items = Mapper.Map<List<DistrictViewModel>>(item.Value).OrderBy(d => d.DistrictName).ToList();

                items.ForEach(i => i.Selected = true);

                districts.Add(new TownViewModel() { Name = item.Key, Districts = items });
            }

            return this.Json(districts, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            var organization = await this.organizationService.GetOrganizationByIdAsync(id);
            if (organization == null)
            {
                return this.HttpNotFound();
            }

            if (!(await this.CurrentUserCanEditOrganization(organization)))
            {
                return this.HttpForbidden();
            }

            var model = await this.CreateEditViewModelAsync(organization);

            return this.View(string.Format("Edit{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, EditPostModel model)
        {
            var organization = await this.organizationService.GetOrganizationByIdAsync(id);
            if (organization == null)
            {
                return this.HttpNotFound();
            }

            if (!(await this.CurrentUserCanEditOrganization(organization)))
            {
                return this.HttpForbidden();
            }

            if (model.ActiveInDistricts == null || !model.ActiveInDistricts.Any())
            {
                this.ModelState.AddModelError("ActiveInDistricts", Error.SelectOneNeighborhood);
            }

            if (model.ActiveOrganizationThemes == null || !model.ActiveOrganizationThemes.Any())
            {
                this.ModelState.AddModelError("ActiveOrganizationThemes", Error.SelectOneTheme);
            }

            if (this.ModelState.IsValid && model.Logo.IsImage(true))
            {
                var logo = ImageHelper.GetResizedImage(model.Logo, 1000, 1000);

                await
                    this.organizationService.EditOrganizationAsync(
                        organization,
                        model.Name,
                        model.Description,
                        model.OrganizationTypeType,
                        model.Address,
                        model.PostalCode,
                        model.City,
                        model.ActiveInDistricts,
                        model.Phone,
                        model.Email,
                        model.Website,
                        model.OpeningHours,
                        logo,
                        model.ActiveOrganizationThemes);

                var deletedServices =
                    organization.Services.Where(
                        s => !model.Services.Any(ps => ps.OrganizationServiceId == s.OrganizationServiceId)).ToList();
                foreach (var service in deletedServices)
                {
                    await this.organizationService.DeleteServiceAsync(service);
                }

                foreach (var service in model.Services.Where(s => s.OrganizationServiceId.HasValue))
                {
                    await
                        this.organizationService.UpdateServiceAsync(
                            organization.Services.Single(s => s.OrganizationServiceId == service.OrganizationServiceId),
                            service.Title,
                            service.IntroductionText,
                            service.FullText);
                }

                foreach (var service in model.Services.Where(s => !s.OrganizationServiceId.HasValue))
                {
                    await
                        this.organizationService.AddServiceAsync(
                            organization,
                            service.Title,
                            service.IntroductionText,
                            service.FullText);
                }

                var deletedproducts =
                    organization.Products.Where(
                        s => !model.Products.Any(ps => ps.OrganizationProductId == s.OrganizationProductId)).ToList();
                foreach (var product in deletedproducts)
                {
                    await this.organizationService.DeleteProductAsync(product);
                }

                foreach (var product in model.Products.Where(s => s.OrganizationProductId.HasValue))
                {
                    await
                        this.organizationService.UpdateProductAsync(
                            organization.Products.Single(s => s.OrganizationProductId == product.OrganizationProductId),
                            product.Title,
                            product.IntroductionText,
                            product.FullText);
                }

                foreach (var product in model.Products.Where(s => !s.OrganizationProductId.HasValue))
                {
                    await
                        this.organizationService.AddProductAsync(
                            organization,
                            product.Title,
                            product.IntroductionText,
                            product.FullText);
                }

                this.NotifyUserSuccess(Alert.OrganizationEditSuccess);

                return this.RedirectToAction("Detail", "Organizations", new { id = organization.OrganizationId });
            }

            if (!model.Logo.IsImage(true))
            {
                this.NotifyUser(TempDataHelper.NotificationType.Danger, Alert.IncorrectFileType);
            }

            this.SetStatusCode(HttpStatusCode.BadRequest);
            return this.View(string.Format("Edit{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), await this.CreateEditViewModelAsync(organization, model));
        }

        [HttpGet]
        public async Task<ActionResult> GetCities()
        {
            return this.Json(await this.organizationService.GetCityNamesAsync(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GiveAdmin(int? id, string userId)
        {
            var membership = await this.organizationService.GetOrganizationMembershipByIdAsync(id, userId);
            if (membership == null)
            {
                return this.HttpNotFound();
            }

            if (
                !(await
                  this.organizationService.IsUserAdministratorOfOrganizationAsync(
                      membership.Organization,
                      this.CurrentUser)))
            {
                return this.HttpForbidden();
            }

            await this.organizationService.GiveAdminRightsAsync(membership);
            this.NotifyUserSuccess(Alert.OrganizationUserAdminGiven, membership.User.Name);
            return this.RedirectToAction("Members", "Organizations", new { id });
        }

        [HttpGet]
        public ActionResult GiveAdmin(int? id)
        {
            if (!id.HasValue)
            {
                return this.HttpNotFound();
            }

            return this.RedirectToAction("Members", "Organizations", new { id });
        }

        [HttpGet]
        public async Task<ActionResult> Index(int? theme)
        {
            this.sharedService.VisitPage(this.CurrentUser, Data.Enums.PageVisitType.Organizations);
            var themes =
                Mapper.Map<List<OrganizationThemeViewModel>>(
                    await this.organizationService.GetOrganisationThemesAsync());
            var organizations = new List<IndexViewModel.OrganizationViewModel>();
            if (theme.HasValue)
            {
                var selectedTheme = themes.FirstOrDefault(t => t.ThemeId == theme.Value);
                if (selectedTheme == null)
                {
                    return this.HttpNotFound();
                }

                selectedTheme.Selected = true;
                organizations =
                    Mapper.Map<List<IndexViewModel.OrganizationViewModel>>(
                        await
                        this.organizationService.GetAllOrganizationsActiveInNeighborhoodByThemeAsync(
                            this.CurrentUserPosition,
                            this.CurrentUserNeighborhoodRadius,
                            selectedTheme.ThemeId));
            }
            else
            {
                organizations =
                    Mapper.Map<List<IndexViewModel.OrganizationViewModel>>(
                        await
                        this.organizationService.GetAllOrganizationsActiveInNeighborhoodAsync(
                            this.CurrentUserPosition,
                            this.CurrentUserNeighborhoodRadius));
            }

            return this.View(
                string.Format("Index{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty),
                new IndexViewModel
                {
                    OrganizationThemes = themes,
                    Organizations = organizations,
                    HelpIcons =
                            await this.helpIconService.GetNonShownHelpIconsForUserAsync(this.CurrentUser)
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InviteUser(int? id, string userId)
        {
            var organization = await this.organizationService.GetOrganizationByIdAsync(id);
            if (organization == null)
            {
                return this.Json(new { state = "danger", message = Alert.UsersInvitedByEmailFailed });
            }

            if (!(await this.organizationService.IsUserAdministratorOfOrganizationAsync(organization, this.CurrentUser)))
            {
                return this.Json(new { state = "danger", message = Alert.UsersInvitedByEmailFailed });
            }

            var user = await this.userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return this.Json(new { state = "danger", message = Alert.UsersInvitedByEmailFailed });
            }

            await
                this.organizationService.InviteUserAsync(
                    organization,
                    user,
                    this.CurrentUser,
                    this.GetAcceptInvitationLinkGenerator(organization.OrganizationId),
                    this.PortalUrl);

            return this.Json(new { state = "success", message = String.Format(Alert.UserIsInvitedForTheOrganization, user.Name) });
        }

        [HttpGet]
        public ActionResult InviteUser(int? id)
        {
            if (!id.HasValue)
            {
                return this.HttpNotFound();
            }

            return this.RedirectToRoute("DefaultDetail", new { controller = "Organizations", action = "Members", id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InviteUsersByEmail(int? id, string emailAddresses)
        {
            var organization = await this.organizationService.GetOrganizationByIdAsync(id);
            if (organization == null)
            {
                return this.Json(new { state = "danger", message = Alert.UsersInvitedByEmailFailed });
            }

            if (!(await this.organizationService.IsUserAdministratorOfOrganizationAsync(organization, this.CurrentUser)))
            {
                return this.Json(new { state = "danger", message = Alert.UsersInvitedByEmailFailed });
            }

            var addresses = this.SeparateEmailAddresses(emailAddresses).ToList();
            var filteredAddresses = addresses.Where(x => Utils.IsValidEmail(x)).ToList();
            var filteredEmailAddresses = string.Join(", ", filteredAddresses);
            if (filteredAddresses.Any())
            {
                await
                this.organizationService.InviteUsersByEmailAsync(
                    organization,
                    filteredAddresses,
                    this.CurrentUser,
                    this.GetAcceptInvitationLinkGenerator(organization.OrganizationId),
                    this.PortalUrl);

                return this.Json(new { state = "success", message = String.Format(Alert.UsersInvitedForTheOrganizationByEmail, filteredEmailAddresses) });
            }

            return this.Json(new { state = "danger", message = Alert.UsersInvitedByEmailFailed });
        }

        [HttpGet]
        public ActionResult InviteUsersByEmail(int? id)
        {
            if (!id.HasValue)
            {
                return this.HttpNotFound();
            }

            return this.RedirectToRoute("DefaultDetail", new { controller = "Organizations", action = "Members", id });
        }

        [HttpGet]
        [OutputCache(Duration = 604800, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "")]
        public async Task<ActionResult> Logo(int? id, int? mediaId)
        {
            //var organization = await this.organizationService.GetOrganizationByIdAsync(id);
            //if (organization == null || organization.LogoId != mediaId)
            //{
            //    return this.HttpNotFound();
            //}

            var logo = await this.organizationService.GetLogoByOrganizationIdAsync(id, mediaId);
            if (logo == null)
            {
                return this.HttpNotFound();
            }

            // TODO: Refactor to generic image resize function
            ////const int Size = 120;
            var image = new WebImage(logo.Data);
            image.Resize(302, 77, true, true);
            image.Crop(1, 1, 1, 1); // border bugfix in WebImage

            // return this.File(image.GetBytes(), logo.MimeType);
            return this.File(image.GetBytes("image/jpeg"), "image/jpeg");
        }

        [HttpGet]
        public async Task<ActionResult> Members(int? id)
        {
            var organization = await this.organizationService.GetOrganizationByIdAsync(id);
            if (organization == null)
            {
                return this.HttpNotFound();
            }

            if (!(await this.organizationService.IsUserMemberOfOrganizationAsync(organization, this.CurrentUser)))
            {
                return this.HttpForbidden();
            }

            var model = Mapper.Map<MembersViewModel>(organization);
            model.Members = Mapper.Map<List<MembersViewModel.MemberViewModel>>(organization.Members);
            model.CurrentUserIsOrganizationAdministrator =
                await this.organizationService.IsUserAdministratorOfOrganizationAsync(organization, this.CurrentUser);

            if (model.CurrentUserIsOrganizationAdministrator)
            {
                model.Members.AddRange(
                    Mapper.Map<List<MembersViewModel.MemberViewModel>>(
                        await this.organizationService.GetInvitationsAsync(organization)));
            }

            model.Members =
                model.Members.OrderByDescending(m => m.IsAdministrator).ThenByDescending(m => m.HasBeenInvited).ToList();

            return this.View(string.Format("Members{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public async Task<ActionResult> Message(int? id, int? messageId)
        {
            var organization = await this.organizationService.GetOrganizationByIdAsync(id);
            if (organization == null)
            {
                return this.HttpNotFound();
            }

            var message = await this.neighborhoodMessageService.GetMessageByIdAsync(messageId);
            if (message == null
                || message.OrganizationMembership.Organization.OrganizationId != organization.OrganizationId)
            {
                return this.HttpNotFound();
            }

            var model = new MessageDetailViewModel
            {
                Message = message,
                MessageType = NeighborhoodMessage.MessageTypes.OrganizationMessages,
                UserCanEditMessage = false,
                Reactions = message.Reactions,
            };
            return this.View(string.Format("~/Views/NeighborhoodMessages/MessageDetail{0}.cshtml", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> MessagesFeed(int? id)
        {
            var organization = await this.organizationService.GetOrganizationByIdAsync(id);
            if (organization == null)
            {
                return this.HttpNotFound();
            }

            var feed = new SyndicationFeed(
                string.Format(Title.OrganizationMessagesFeed, organization.Name),
                string.Format(Text.OrganizationMessagesFeed, organization.Name),
                new Uri(this.PortalUrl + "organizations/" + organization.OrganizationId),
                "Org" + organization.OrganizationId + "Messages",
                DateTime.Now);

            var items = new List<SyndicationItem>();
            var messages =
                await
                this.neighborhoodMessageService.GetLatestMessagesByOrganizationIdAsync(organization.OrganizationId, 100);
            items.AddRange(
                messages.Select(
                    m =>
                    new SyndicationItem(
                        m.Title,
                        this.GetMessageContent(m),
                        new Uri(this.PortalUrl + "neighborhoodmessages/organizationmessages/" + m.MessageId.ToString()),
                        m.MessageId.ToString(),
                        m.CreationDateTime)));
            feed.Items = items;

            return new AtomResult(feed);
        }

        [HttpGet]
        public async Task<ActionResult> Professionals()
        {
            return
                this.IndexView(
                    await
                    this.organizationService.GetOrganizationsByTypeActiveInNeighborhoodAsync(
                        OrganizationType.Types.Professional,
                        this.CurrentUserPosition,
                        this.CurrentUserNeighborhoodRadius),
                    await this.helpIconService.GetNonShownHelpIconsForUserAsync(this.CurrentUser));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveMember(int? id, string memberId)
        {
            var organization = await this.organizationService.GetOrganizationByIdAsync(id);
            if (organization == null)
            {
                return this.HttpNotFound();
            }

            if (!(await this.organizationService.IsUserAdministratorOfOrganizationAsync(organization, this.CurrentUser)))
            {
                return this.HttpForbidden();
            }

            if (this.ModelState.IsValid)
            {
                await this.organizationService.RemoveMemberAsync(organization.OrganizationId, memberId);
                this.NotifyUserSuccess(Alert.RemoveMemberFromOrganizationSuccess);
            }

            return this.RedirectToRoute("DefaultDetail", new { controller = "Organizations", id, action = "Members" });
        }

        [HttpGet]
        public ActionResult RemoveMember(int? id)
        {
            if (!id.HasValue)
            {
                return this.HttpNotFound();
            }

            return this.RedirectToRoute("DefaultDetail", new { controller = "Organizations", id, action = "Members" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveOrganization(int? id)
        {
            var organization = await this.organizationService.GetOrganizationByIdAsync(id);
            if (organization == null)
            {
                return this.HttpNotFound();
            }

            await this.organizationService.RemoveOrganizationAsync(organization);

            return this.RedirectToAction("Index", "Organizations");
        }

        [HttpGet]
        public async Task<ActionResult> RequestNew()
        {
            return this.View(
                string.Format("Request{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty),
                new RequestViewModel
                {
                    OrganizationThemes =
                            Mapper.Map<List<OrganizationThemeViewModel>>(
                                await this.organizationService.GetOrganisationThemesAsync())
                            .OrderBy(t => t.ThemeId)
                            .ToList()
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RequestNew(EditPostModel model)
        {
            if (model.ActiveInDistricts == null || !model.ActiveInDistricts.Any())
            {
                this.ModelState.AddModelError("ActiveInDistricts", Error.SelectOneNeighborhood);
            }

            if (this.ModelState.IsValid && model.Logo.IsImage(true))
            {
                var logo = ImageHelper.GetResizedImage(model.Logo, 1000, 1000);

                await
                    this.organizationService.RequestNewOrganizationAsync(
                        model.Name,
                        model.Description,
                        model.OrganizationTypeType,
                        model.Address,
                        model.PostalCode,
                        model.City,
                        model.ActiveInDistricts,
                        model.ActiveOrganizationThemes,
                        model.Phone,
                        model.Email,
                        model.Website,
                        model.OpeningHours,
                        logo,
                        this.CurrentUser);

                this.NotifyUserSuccess(Alert.OrganizationRequestSuccess);
                return this.RedirectToAction("Index", "Organizations");
            }

            if (!model.Logo.IsImage(true))
            {
                this.NotifyUser(TempDataHelper.NotificationType.Danger, Alert.IncorrectFileType);
            }

            var viewModel = RequestViewModel.FromPostModel(model);
            viewModel.OrganizationThemes =
                this.organizationService.GetOrganisationThemesAsync()
                    .Result.Select(
                        t =>
                        new OrganizationThemeViewModel
                        {
                            Name = t.Name,
                            ThemeId = t.ThemeId,
                            Selected = model.ActiveOrganizationThemes.Contains(t.ThemeId)
                        })
                    .ToList();

            this.SetStatusCode(HttpStatusCode.BadRequest);
            return this.View(string.Format("Request{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RevokeAdmin(int? id, string userId)
        {
            var membership = await this.organizationService.GetOrganizationMembershipByIdAsync(id, userId);
            if (membership == null)
            {
                return this.HttpNotFound();
            }

            if (
                !(await
                  this.organizationService.IsUserAdministratorOfOrganizationAsync(
                      membership.Organization,
                      this.CurrentUser)))
            {
                return this.HttpForbidden();
            }

            await this.organizationService.RevokeAdminRightsAsync(membership);
            this.NotifyUserSuccess(Alert.OrganizationUserAdminRevoked, membership.User.Name);
            return this.RedirectToAction("Members", "Organizations", new { id });
        }

        [HttpGet]
        public ActionResult RevokeAdmin(int? id)
        {
            if (!id.HasValue)
            {
                return this.HttpNotFound();
            }

            return this.RedirectToAction("Members", "Organizations", new { id });
        }

        [HttpGet]
        public ActionResult SendInvitationReminder(int? id)
        {
            if (!id.HasValue)
            {
                return this.HttpNotFound();
            }

            return this.RedirectToAction("Members", "Circles", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendInvitationReminder(int? id, string memberId, string email)
        {
            var organization = await this.organizationService.GetOrganizationByIdAsync(id);
            var user = await this.userService.GetUserByIdAsync(memberId);

            if (organization == null || user == null)
            {
                return this.HttpNotFound();
            }

            if (!await this.organizationService.IsUserAdministratorOfOrganizationAsync(organization, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            if (this.ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(memberId))
                {
                    await this.organizationService.SendInvitationReminderAsync(organization, user, this.CurrentUser, this.GetAcceptInvitationLinkGenerator(organization.OrganizationId), this.PortalUrl);
                }
                else
                {
                    await this.organizationService.SendInvitationReminderByEmailAsync(organization, email, this.CurrentUser, this.GetAcceptInvitationLinkGenerator(organization.OrganizationId), this.PortalUrl);
                }

                this.NotifyUserSuccess(String.Format(Alert.UserIsInvitedForTheCircle, user.UserName));
            }

            return this.RedirectToRoute("DefaultDetail", new { controller = "Organizations", id, action = "Members" });
        }

        [HttpGet]
        public async Task<ActionResult> SearchUsers(int? id, string q)
        {
            if (!id.HasValue)
            {
                return this.HttpNotFound();
            }

            var organization = await this.organizationService.GetOrganizationByIdAsync(id.Value);
            if (organization == null)
            {
                return this.HttpNotFound();
            }

            if (!(await this.organizationService.IsUserAdministratorOfOrganizationAsync(organization, this.CurrentUser)))
            {
                return this.HttpForbidden();
            }

            var users =
                Mapper.Map<List<SearchUserViewModel>>(
                    await
                    this.organizationService.SearchNeighborsNotInOrganizationAsync(organization, this.CurrentUser, q));
            foreach (var user in users.Where(u => u.HasProfileImage))
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

            return this.Json(users, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> Volunteers()
        {
            return
                this.IndexView(
                    await
                    this.organizationService.GetOrganizationsByTypeActiveInNeighborhoodAsync(
                        OrganizationType.Types.Volunteer,
                        this.CurrentUserPosition,
                        this.CurrentUserNeighborhoodRadius),
                    await this.helpIconService.GetNonShownHelpIconsForUserAsync(this.CurrentUser));
        }

        private async Task<EditViewModel> CreateEditViewModelAsync(
            Organization organization,
            EditPostModel postModel = null)
        {
            var model = postModel != null
                            ? EditViewModel.FromPostModel(postModel, organization)
                            : Mapper.Map<EditViewModel>(organization);

            model.DistrictsInCity =
                Mapper.Map<List<DistrictViewModel>>(
                    await this.organizationService.GetDistrictsByCityAsync(organization.City))
                    .OrderBy(d => d.DistrictName)
                    .ToList();

            foreach (var district in model.DistrictsInCity)
            {
                district.Selected = model.ActiveInDistricts.Contains(district.DistrictId);
            }

            model.OrganizationThemes =
                Mapper.Map<List<OrganizationThemeViewModel>>(
                    await this.organizationService.GetOrganisationThemesAsync()).OrderBy(t => t.ThemeId).ToList();

            foreach (var theme in model.OrganizationThemes)
            {
                theme.Selected = model.ActiveOrganizationThemes.Contains(theme.ThemeId);
            }

            return model;
        }

        private Func<string, string> GetAcceptInvitationLinkGenerator(int id)
        {
            return code => this.Url.Action("AcceptInvitation", "Organizations", new { id, code }, this.RequestUrlScheme);
        }

        private TextSyndicationContent GetActivityContent(Activity activity)
        {
            var content = activity.Description;
            if (activity.AllDay)
            {
                var startDate = activity.StartDateTime.ToString("dddd d MMMM yyyy");
                var endDate = activity.EndDateTime.ToString("dddd d MMMM yyyy");
                var date = startDate + (startDate.Equals(endDate) ? string.Empty : " - " + endDate);
                content += string.Format(@"<p><b>{0}:</b> {1}</p>", Label.Date, date);
            }
            else
            {
                content += string.Format(
                    @"<p><b>{0}:</b> {1}</p>",
                    Label.Date,
                    activity.StartDateTime.ToString("dddd d MMMM yyyy"));
                content += string.Format(
                    @"<p><b>{0}:</b> {1}</p>",
                    Label.ActivityStart,
                    activity.StartDateTime.ToString("HH:mm"));
                content += string.Format(
                    @"<p><b>{0}:</b> {1}</p>",
                    Label.ActivityEnd,
                    activity.EndDateTime.ToString(
                        activity.EndDateTime.Date != activity.StartDateTime.Date ? "dddd d MMMM yyyy - HH:mm" : "HH:mm"));
            }

            content += string.Format(@"<p><b>{0}:</b> {1}</p>", Label.Location, activity.Location);

            return new TextSyndicationContent(content, TextSyndicationContentKind.Html);
        }

        private TextSyndicationContent GetMessageContent(NeighborhoodMessage message)
        {
            var content = string.Empty;
            if (!string.IsNullOrWhiteSpace(message.IntroductionText))
            {
                content += string.Format("<p><b>{0}</b></p>", message.IntroductionText);
            }

            content += message.FullText;

            return new TextSyndicationContent(content, TextSyndicationContentKind.Html);
        }

        private ActionResult IndexView(List<Organization> organizations, IEnumerable<HelpIcon> helpIcons)
        {
            return this.View(
                string.Format("Index{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty),
                new IndexViewModel
                {
                    Organizations =
                            Mapper.Map<List<IndexViewModel.OrganizationViewModel>>(organizations),
                    HelpIcons = helpIcons
                });
        }
    }
}