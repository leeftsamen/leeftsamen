// <copyright file="ILinkGenerator.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Data.Models;

namespace LeeftSamen.Portal.Services
{
    public interface ILinkGenerator
    {
        string GenerateAccountConfirmEmailLink(string userId, string code);

        string GenerateAccountPasswordResetLink(string userId, string code);

        string GenerateActivityAcceptedLink(int activityId);

        string GenerateActivityAcceptInvitationLink(int id, string code);

        string GenerateCircleAcceptInvitationLink(int id, string code);

        string GenerateLocalCircleAcceptInvitationLink(int id, string code);

        string GenerateCircleAcceptJoinRequestLink(int id, string code);

        string GenerateCircleJobLink(int circleId, int jobId);

        string GenerateCircleJobAcceptLink(int circleId, int jobId);

        string GenerateCircleJoinRequestsLink(int circleId);

        string GenerateCircleMembersLink(int circleId);

        string GenerateCircleMessageLink(int circleId, int messageId);

        string GenerateCircleMessageReactionLink(int circleId, int messageReactionId);

        string GenerateCirclePhotoLink(int circleId);

        string GenerateCircleDocLink(int circleId);

        string GenerateCircleMarketPlaceItem(int itemId, int circleId);

        string GenerateMarketplaceReactionLink(int itemId, int reactionId);

        string GenerateMarketplaceItemLink(int itemId);

        string GenerateCircleAcceptedLink(int circleId);

        string GenerateCircleRequestRejectedLink(int circleId);

        string GenerateRemovedReactionLink(int subjectId, string type, int typeId);

        string GenerateActivityLink(int id, int? circleId);

        string GenerateEmailSettingsLink();

        string GenerateNeighborhoodMessageLink(int messageId, NeighborhoodMessage.MessageTypes type);

        string GenerateOrganizationInviteLink(int organizationId);
    }
}