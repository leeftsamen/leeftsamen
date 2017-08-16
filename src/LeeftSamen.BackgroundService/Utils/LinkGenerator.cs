// <copyright file="LinkGenerator.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.BackgroundService.Utils
{
    using System;
    using Portal.Data.Models;
    using LeeftSamen.Portal.Services;

    public class LinkGenerator : ILinkGenerator
    {
        public string GenerateAccountConfirmEmailLink(string userId, string code)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateAccountPasswordResetLink(string userId, string code)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateCircleAcceptInvitationLink(int id, string code)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateLocalCircleAcceptInvitationLink(int id, string code)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateActivityAcceptedLink(int activityId)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateCircleAcceptJoinRequestLink(int id, string code)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateCircleJobLink(int circleId, int jobId)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateCircleJobAcceptLink(int circleId, int jobId)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateCircleJoinRequestsLink(int circleId)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateCircleMembersLink(int circleId)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateCircleMessageLink(int circleId, int messageId)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateCircleMessageReactionLink(int circleId, int messageReactionId)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateCircleMarketPlaceItem(int itemId, int circleId)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateMarketplaceReactionLink(int itemId, int reactionId)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateMarketplaceItemLink(int itemId)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateCircleAcceptedLink(int circleId)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateRemovedReactionLink(int subjectId, string type, int typeId)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateActivityLink(int id, int? circleId)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateActivityAcceptInvitationLink(int id, string code)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateCirclePhotoLink(int circleId)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateCircleDocLink(int circleId)
        {
            throw new System.NotImplementedException();
        }

        public string GenerateEmailSettingsLink()
        {
            throw new System.NotImplementedException();
        }

        public string GenerateNeighborhoodMessageLink(int messageId, NeighborhoodMessage.MessageTypes type)
        {
            throw new NotImplementedException();
        }

        public string GenerateCircleRequestRejectedLink(int circleId)
        {
            throw new NotImplementedException();
        }

        public string GenerateOrganizationInviteLink(int organizationId)
        {
            throw new NotImplementedException();
        }
    }
}
