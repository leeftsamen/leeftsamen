// <copyright file="InvitationsController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeeftSamen.Portal.Web.Controllers
{
    using System.Threading.Tasks;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Services;
    using LeeftSamen.Portal.Web.Managers;
    using LeeftSamen.Portal.Web.Models.Invitations;
    using LeeftSamen.Portal.Web.Utils;

    using Microsoft.Owin.Security;
    using Data.Enums;

    public class InvitationsController : BaseController
    {
        private readonly IAuthenticationManager authenticationManager;

        private readonly INotificationService notificationService;

        private readonly ICircleService circleService;

        private readonly IOrganizationService organizationService;

        public InvitationsController(ICurrentUserInformation currentUserInformation,
            ICircleService circleService,
            IOrganizationService organizationService,
            IAuthenticationManager authenticationManager,
            INotificationService notificationService)
            : base(currentUserInformation)
        {
            this.authenticationManager = authenticationManager;
            this.notificationService = notificationService;
            this.circleService = circleService;
            this.organizationService = organizationService;
        }

        public ActionResult Index()
        {
            if (!this.Request.IsAuthenticated)
            {
                return this.HttpForbidden();
            }

            var circleInvitations = this.circleService.GetInvitationsByUserAsync(this.CurrentUser).Result;
            var organizationInvitations = this.organizationService.GetInvitationsByUserAsync(this.CurrentUser).Result;

            var invitations = new List<InvitationsViewModel>();
            foreach (var circleInvitation in circleInvitations)
            {
                var invite = new InvitationsViewModel()
                                 {
                                     Id = circleInvitation.Circle.CircleId,
                                     AcceptToken = circleInvitation.AcceptToken,
                                     Name = circleInvitation.Circle.Name,
                                     InvitedBy = circleInvitation.InvitedBy.Name,
                                     InvitationDateTime = circleInvitation.InvitationDateTime,
                                     Controller = "Circles"
                                 };
                invitations.Add(invite);
            }

            foreach (var organizationInvitation in organizationInvitations)
            {
                var invite = new InvitationsViewModel()
                {
                    Id = organizationInvitation.Organization.OrganizationId,
                    AcceptToken = organizationInvitation.AcceptToken,
                    Name = organizationInvitation.Organization.Name,
                    InvitedBy = organizationInvitation.InvitedBy.Name,
                    InvitationDateTime = organizationInvitation.InvitationDateTime,
                    Controller = "Organizations"
                };
                invitations.Add(invite);
            }

            var model = new IndexViewModel
                            {
                                Invitations = invitations
                            };
            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeclineInvitation(int? id, string code)
        {
            var invitation = await this.circleService.GetInvitationByCircleIdAcceptTokenAsync(id, code, this.CurrentUser);

            if (invitation != null)
            {
                this.NotifyUserSuccess(Alert.DeclinedCircleRequest, invitation.Circle.Name);

                var message = string.Format(Notification.CircleInvitationDeclined, invitation.User.Name, invitation.Circle.Name);
                await this.notificationService.CreateNotificationForUserAsync(invitation.InvitedBy, message, null, SettingName.CircleInvitationDeclined);

                var x = this.circleService.DeclineInvitationAsync(invitation).Result;
                return this.RedirectToAction("Index", "Invitations");
            }

            var organizationInvitation = await this.organizationService.GetInvitationsByOrganizationIdAcceptTokenAsync(id, code, this.CurrentUser);
            if (organizationInvitation != null)
            {
                this.NotifyUserSuccess(Alert.YouAreNowAOrganizationMember, organizationInvitation.Organization.Name);

                var message = string.Format(Notification.OrganizationInvitationDeclined, organizationInvitation.User.Name, organizationInvitation.Organization.Name);
                //await this.notificationService.CreateNotificationForUserAsync(organizationInvitation.InvitedBy, message, null, SettingName.OrganizationInvitationDeclined);

                await this.organizationService.DeclineInvitationAsync(organizationInvitation, this.CurrentUser);

                return this.RedirectToAction("Index", "Invitations");
            }

            return this.HttpForbidden();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AcceptInvitation(int? id, string code)
        {
            var invitation = await this.circleService.GetInvitationByCircleIdAcceptTokenAsync(id, code, this.CurrentUser);

            if (invitation != null)
            {
                var membership = this.circleService.AcceptInvitationAsync(invitation, this.CurrentUser).Result;
                this.NotifyUserSuccess(Alert.YouAreNowACircleMember, membership.Circle.Name);

                return this.RedirectToAction("Index", "Invitations");
            }

            var organizationInvitation = await this.organizationService.GetInvitationsByOrganizationIdAcceptTokenAsync(id, code, this.CurrentUser);
            if (organizationInvitation != null)
            {
                var membership = await this.organizationService.AcceptInvitationAsync(organizationInvitation, this.CurrentUser);
                this.NotifyUserSuccess(Alert.YouAreNowAOrganizationMember, membership.Organization.Name);

                return this.RedirectToAction("Index", "Invitations");
            }

            // TODO: Instead show a notification that the invitation has expired / is invalid.
            return this.HttpForbidden();
        }
    }
}