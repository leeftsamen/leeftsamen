// <copyright file="LinkGenerator.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Utils
{
    using System;
    using System.Web.Mvc;

    using LeeftSamen.Portal.Services;
    using Data.Models;

    public class LinkGenerator : ILinkGenerator
    {
        private const string RequestUrlScheme = "https";
        private readonly UrlHelper urlHelper;

        public LinkGenerator(UrlHelper urlHelper)
        {
            this.urlHelper = urlHelper;
        }

        public string GenerateAccountConfirmEmailLink(string userId, string code)
        {
            return this.Action("ConfirmEmail", "Account", new { userId, code });
        }

        public string GenerateAccountPasswordResetLink(string userId, string code)
        {
            return this.Action("ResetPassword", "Account", new { userId, code });
        }

        public string GenerateActivityAcceptInvitationLink(int id, string code)
        {
            return this.urlHelper.Action("AcceptInvitation", "Activities", new { id, code });
        }

        public string GenerateActivityAcceptedLink(int activityId)
        {
            return this.urlHelper.RouteUrl(
            "DefaultDetail",
                new { action = "Detail", controller = "Activities", id = activityId});
        }

        public string GenerateCircleAcceptInvitationLink(int id, string code)
        {
            return this.Action("AcceptInvitation", "Circles", new { id, code });
        }

        public string GenerateLocalCircleAcceptInvitationLink(int id, string code)
        {
            return this.urlHelper.Action("AcceptInvitation", "Circles", new { id, code });
        }

        public string GenerateCircleAcceptJoinRequestLink(int id, string code)
        {
            return this.Action("AcceptJoinRequest", "Circles", new { id, code });
        }

        public string GenerateCircleJobLink(int circleId, int jobId)
        {
            return this.urlHelper.RouteUrl(
                "CircleSubResources",
                new { action = "Index", controller = "Jobs", circleId });
        }

        public string GenerateCircleJobAcceptLink(int circleId, int jobId)
        {
            return this.urlHelper.RouteUrl(
                "CircleSubResources",
                new { action = "AssignToMe", controller = "Jobs", circleId, id = jobId });
        }

        public string GenerateCircleJoinRequestsLink(int circleId)
        {
            return this.Action("JoinRequests", "Circles", new { id = circleId });
        }

        public string GenerateCircleMembersLink(int circleId)
        {
            return this.Action("Members", "Circles", new { id = circleId });
        }

        public string GenerateCircleMessageLink(int circleId, int messageId)
        {
            return this.Action("Messages", "Circles", new { id = circleId }) + string.Format("#message-{0}", messageId);
        }

        public string GenerateCircleMessageReactionLink(int circleId, int messageReactionId)
        {
            return this.urlHelper.RouteUrl(
                "DefaultDetail",
                new { controller = "Circles", action = "Messages", id = circleId, scrollToReaction = messageReactionId },
                RequestUrlScheme) + string.Format("#reaction-{0}", messageReactionId);
        }

        public string GenerateCirclePhotoLink(int circleId)
        {
            return this.urlHelper.RouteUrl(
                "DefaultDetail",
                new { controller = "Circles", action = "Photos", id = circleId});
        }

        public string GenerateCircleDocLink(int circleId)
        {
            return this.urlHelper.RouteUrl(
                "DefaultDetail",
                new { controller = "Circles", action = "Documents", id = circleId });
        }

        public string GenerateCircleMarketPlaceItem(int itemId, int itemCircleId)
        {
            return this.urlHelper.RouteUrl("CircleMarketplaceDetail",
                new { controller = "Marketplace", action = "Detail", circleId = itemId, id = itemCircleId }, RequestUrlScheme);
        }

        public string GenerateMarketplaceReactionLink(int itemId, int reactionId)
        {
            return this.urlHelper.RouteUrl(
                "DefaultDetail",
                new { controller = "Marketplace", action = "Detail", id = itemId },
                RequestUrlScheme) + string.Format("#reaction-{0}", reactionId);
        }

        public string GenerateMarketplaceItemLink(int itemId)
        {
            return this.urlHelper.RouteUrl(
                "DefaultDetail",
                new { controller = "Marketplace", action = "Detail", id = itemId },
                RequestUrlScheme);
        }

        private string Action(string action, string controller, object routeValues)
        {
            return this.urlHelper.Action(action, controller, routeValues, RequestUrlScheme);
        }

        public string GenerateCircleAcceptedLink(int circleId)
        {
            return this.urlHelper.RouteUrl(
            "DefaultDetail",
                new { action = "Detail", controller = "Circles", id = circleId });
        }

        public string GenerateCircleRequestRejectedLink(int circleId)
        {
            return this.urlHelper.RouteUrl(
            "DefaultDetail",
                new { action = "Detail", controller = "Circles", id = circleId });
        }

        public string GenerateRemovedReactionLink(int subjectId, string type, int typeId)
        {
            return this.urlHelper.RouteUrl(
            "Forum",
                new { action = "Subject", controller = "Forum", type, typeId, id = subjectId });
        }

        public string GenerateActivityLink(int id, int? circleId)
        {
            if (circleId.HasValue)
            {
                return this.urlHelper.RouteUrl(
                "CircleActivitiesDetail",
                    new { action = "Detail", controller = "Activities", id, circleId },
                    RequestUrlScheme);
            }
            else
            {
                return this.urlHelper.RouteUrl(
                "DefaultDetail",
                    new { action = "Detail", controller = "Activities", id },
                    RequestUrlScheme);
            }
        }

        public string GenerateEmailSettingsLink()
        {
            return this.urlHelper.RouteUrl("Default",
                new { action = "ChangeEmailSettings", controller = "Account"},
                RequestUrlScheme);
        }

        public string GenerateNeighborhoodMessageLink(int messageId, NeighborhoodMessage.MessageTypes type)
        {
            return this.urlHelper.RouteUrl(
                "NeighborhoodMessage",
                    new { action = "MessageDetail", controller = "NeighborhoodMessages", messageId, messageType = type },
                    RequestUrlScheme);
        }

        public string GenerateOrganizationInviteLink(int organizationId)
        {
            return this.urlHelper.RouteUrl(
            "DefaultDetail",
                new { action = "Detail", controller = "Organization", id = organizationId },
                RequestUrlScheme);
        }
    }
}