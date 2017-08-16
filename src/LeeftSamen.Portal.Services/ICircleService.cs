// <copyright file="ICircleService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Spatial;
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.Services.DTO;

    public interface ICircleService
    {
        Task<CircleMembership> AcceptInvitationAsync(CircleInvitation invitation, User user);

        Task<CircleMembership> AcceptJoinRequestAsync(CircleJoinRequest joinRequest);

        Task AssignUserToJobAsync(Job job, User user, string portalUrl);

        Task UnassignUserToJobAsync(Job job, User user, string portalUrl);

        Task<CircleActivity> CreateCircleActivityAsync(
            string title,
            string description,
            DateTime startDateTime,
            DateTime endDateTime,
            string location,
            bool allDay,
            User creator,
            Circle circle,
            bool allAges,
            int ageFrom,
            int ageTo);

        Task<Circle> CreateCircleAsync(
            string name,
            string description,
            bool isPrivate,
            User creator,
            int coverColor,
            ImageDto profileImage,
            ImageDto coverImage);

        Task CreateCirclePhotoAlbumAsync(Circle circle, string title, User user);

        Task<CirclePhoto> CreateCirclePhotoAsync(ImageDto photo, Circle circle, User user, int? photoAlbumId);

        Task<CirclePhoto> CreateCircleDocumentAsync(
            Media document,
            Circle circle,
            User user,
            int? photoAlbumId);

        Task<CircleEmailMessage> CreateEmailMessageAsync(
            string messageText,
            string subject,
            User creator,
            List<User> receivers,
            Circle circle,string portalUrl);

        Task<Job> CreateJobAsync(
            string title,
            string description,
            bool hasDueTime,
            DateTime dueDateTime,
            DateTime? dueDateTimeEnd,
            DateTime completionDateTime,
            Circle circle,
            User creator,
            bool isOnlyVisibleToSelectedMembers,
            List<int> selectedMembershipIds,
            string portalUrl,
            bool sendEmail = true);

        Task<Job> SaveJobAsync(
            int jobId,
            string title,
            string description,
            bool hasDueTime,
            DateTime dueDateTime,
            DateTime? dueDateTimeEnd,
            DateTime completionDateTime,
            bool isOnlyVisibleToSelectedMembers,
            List<int> selectedMembershipIds,
            string portalUrl);

        Task<CircleMessage> CreateMessageAsync(
            string messageText,
            ImageDto attachment,
            Circle circle,
            User creator,
            string portalUrl,
            User recipient);

        Task<CircleEmailMessage> CreateGroupEmailMessageAsync(
            CircleEmailGroup group,
            string messageText,
            User creator,
            Circle circle,
            string portalUrl);

        Task<CircleEmailGroup> CreateEmailGroupAsync(
            string Name,
            User Creator,
            List<User> receivers,
            Circle circle,
            string portalUrl);

        Task<CircleMessageReaction> CreateMessageReactionAsync(
            string reactionText,
            ImageDto attachment,
            Circle circle,
            CircleMessage message,
            User creator);

        Task<bool> DeclineInvitationAsync(CircleInvitation invitation);

        Task DeleteMessageAsync(CircleMessage message, Circle circle);

        Task EditCircleAsync(
            Circle circle,
            string name,
            string description,
            bool isPrivate,
            int coverColor,
            ImageDto profileImage,
            ImageDto coverImage);

        Task EditMembershipProfile(
            CircleMembership membership,
            string profileText);

        Task<CircleMessage> EditMessageAsync(
            CircleMessage messageText,
            Circle circle,
            User currentUser,
            string portalUrl,
            string newMessageText);

        Task<List<Circle>> GetAllCirclesAsync();

        Task<List<CirclePhoto>> GetAllCirclePhotosForAlbumAsync(int? id);

        Task<List<CirclePhoto>> GetAllCirclePhotosFromCircleAsync(int? id);

        Task<List<Media>> GetAllMediaFromCircleMessagesAsync(int? id);

        Task<List<Media>> GetAllMediaForAlbumAsync(int? photoAlbumId);

        Task<List<Media>> GetAllMediaForCircleAsync(int? id);

        Task<List<Circle>> GetAllPublicCirclesAsync();

        Task<List<Job>> GetAssignedJobsAsync(Circle circle, User user);

        Task<List<Job>> GetAssignedJobsAsync(Circle circle, User user, DateTime since, int limit = 10);

        Task<Media> GetAttachmentByCircleMessageIdAsync(User user, int? circleMessageId, int? mediaId);

        Task<Media> GetAttachmentByCircleMessageReactionIdAsync(User user, int? circleMessageId, int? mediaId);

        Task<List<CircleActivity>> GetCircleActivitiesByIdAsync(int circleId);

        Task<Circle> GetCircleByIdAsync(int? circleId);

        Task<Circle> GetCircleByIdAsync(int? circleId, User user);

        List<Activity> GetActivitiesByCircle(Circle circle);

        List<CircleLabel> GetCircleLabels(Circle circle);

        List<CircleSetting> GetCircleSettings(Circle circle);

        CircleSetting GetCircleSettingByName(Circle circle, string name);

        Task<CircleMembership> GetCircleMembershipByIdAsync(int? circleId, string userId);

        CircleMembership GetCircleMembershipById(int? circleId, string userId);

        int[] GetCoverColors();

        Task<Media> GetCoverImageByCircleIdAsync(int? circleId, int? mediaId);

        Task<List<CircleEmailGroup>> GetEmailGroupsAsync(User user, Circle circle);

        Task<CircleEmailGroup> GetEmailGroupAsync(int groupId);

        Task<List<CircleEmailMessage>> GetEmailGroupMessagesAsync(CircleEmailGroup group);

        Task<CircleEmailMessage> GetLastEmailMessage(int groupId);

        Task<CircleEmailMessage> GetEmailMessageAsync(int? emailMessageid);

        ICollection<CircleEmailMessage> GetEmailMessages(int circleId, string userId);

        List<Circle> GetFeaturedCircles(User user);

        Task<CircleInvitation> GetInvitationByCircleIdAcceptTokenAsync(int? id, string acceptToken);

        Task<CircleInvitation> GetInvitationByCircleIdAcceptTokenAsync(int? id, string acceptToken, User user);

        Task<List<CircleInvitation>> GetInvitationsAsync(Circle circle);

        CircleInvitation GetUserInvitationForCircle(User user, Circle circle);

        Task<CircleInvitation> GetUserInvitationForCircleAsync(User user, Circle circle);

        Task<List<CircleInvitation>> GetInvitationsByUserAsync(User user);

        Task<CircleJoinRequest> GetJoinRequestByCircleIdAcceptTokenAsync(int? id, string acceptToken);

        Task<CircleJoinRequest> GetJoinRequestByIdAsync(int? id, int? requestId);

        Task<CircleMessage> GetMessageByIdAsync(int circleId, int messageId);

        Task<Media> GetPhotoByMediaId(int circleId, int? mediaId);

        Task<Media> GetProfileImageByCircleIdAsync(int? circleId, int? mediaId);

        Task<List<Circle>> GetPublicCirclesInNeighborhoodAsync(string userId, DbGeography position, int radius);

        ICollection<CircleEmailMessage> GetSendEmailMessages(int circleId, string userId);

        Task<Job> GetUnassignedJobByIdAsync(int? circleId, int? jobId);

        Task<List<Job>> GetUnassignedJobsAsync(Circle circle, User user);

        Task<List<Circle>> GetUserCirclesAsync(User user);

        Task SendNotificationNewCircleMarketPlaceItem(Circle circle, User user, MarketplaceItem marketplaceItem);

        Task SendEmailMarketPlaceItemCreatedToCircleMembers(Circle circle, User creator, string portalUrl,
            MarketplaceItem marketplaceItem);

        Task SendEmailLendMarketPlaceItemCreatedToNeighboorhood(
            User creator,
            string portalUrl,
            MarketplaceItem marketplaceItem);

        Task SendEmailHelpMarketPlaceItemCreatedToNeighboorhood(
            User creator,
            string portalUrl,
            MarketplaceItem marketplaceItem);

        Task<List<Circle>> UserCirclesWhereUserIsOnlyAdmin(User user);

        Task GiveAdminRightsAsync(CircleMembership membership);

        Task InviteUserAsync(Circle circle, User user, User invitedBy, string portalUrl);

        Task InviteUserByEmailAsync(Circle circle, IEnumerable<string> emailAddresses, User invitedBy, string portalUrl);

        bool IsUserAdminOfCircle(Circle circle, User user);

        bool IsUserAllowedToLeaveCircle(Circle circle, User user);

        bool IsUserInvitedToJoinCircle(Circle circle, User user);

        bool IsUserMemberOfCircle(Circle circle, User user);

        Task RejectJoinRequestAsync(CircleJoinRequest joinRequest);

        Task RemoveAttachment(CircleMessage message, int attachmentId);

        Task RemoveAttachment(CircleMessageReaction message, int attachmentId);

        Task RemoveCircleAsync(Circle circle);

        Task RemoveCirclePhoto(int photoId);

        Task RemoveCirclePhotoAlbumAsync(int photoAlbumId);

        Task RemoveEmailGroupReceiver(CircleEmailMessageReceiver receiver);

        Task RemoveInvitationAsync(int circleId, string userId);

        Task RemoveInvitationByEmailAsync(int circleId, string email);

        Task RemoveJobAsync(Circle circle, int? jobId);

        Task RemoveMemberAsync(int circleId, string userId);

        Task RemoveReceivedCircleEmailMessageAsync(string userId, int emailMessageId);

        Task RemoveSendCircleEmailMessageAsync(string userId, int emailMessageId);

        Task<bool> RequestToJoinAsync(Circle circle, User user, string portalUrl);

        Task RevokeAdminRightsAsync(CircleMembership membership);

        Task StopCircleEmailAsync(CircleMembership membership);

        Task StartCircleEmailAsync(CircleMembership membership);

        Task SendInvitationReminderAsync(Circle circle, User user, User invitedBy, string portalUrl);

        Task SendInvitationReminderByEmailAsync(Circle circle, string emailAddress, User invitedBy, string portalUrl);

        Task<List<CircleMembership>> SearchMembersAsync(Circle circle, string q, List<int> excludeMembershipIds);

        Task<List<User>> SearchNeighborsNotInCircleAsync(Circle circle, DbGeography position, int radius, string q);

        Task<Circle> SetCoverImage(int circleId, Media media);

        Task<Circle> SetCoverImageAsync(int circleId, Media media);

        Task<Circle> SetPrivateAsync(int circleId);

        Task<Circle> SetProfileImageAsync(int circleId, Media media);

        Task<Circle> SetPublicAsync(int circleId);

        Task UserLeaveCircle(Circle circle, User user);

        Task<Job> GetJobByIdAsync(int? jobId);
    }
}