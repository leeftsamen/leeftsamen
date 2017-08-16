// <copyright file="CircleService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System.Web;

namespace LeeftSamen.Portal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Data;
    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.EmailTemplates.Models;
    using LeeftSamen.Portal.Services.DTO;
    using Data.Enums;

    public class CircleService : ICircleService
    {
        private readonly int[] coverColors;

        private readonly IApplicationDbContext databaseContext;

        private readonly ILinkGenerator linkGenerator;

        private readonly IMailerService mailerService;

        private readonly IMediaService mediaService;

        private readonly INotificationService notificationService;

        private readonly IRandomGenerator randomGenerator;

        public CircleService(
            IApplicationDbContext databaseContext,
            IMediaService mediaService,
            IMailerService mailerService,
            IRandomGenerator randomGenerator,
            ILinkGenerator linkGenerator,
            INotificationService notificationService)
        {
            this.databaseContext = databaseContext;
            this.mediaService = mediaService;
            this.mailerService = mailerService;
            this.randomGenerator = randomGenerator;
            this.linkGenerator = linkGenerator;
            this.notificationService = notificationService;
            this.coverColors = new[] { 0xf5736e, 0x2ec5c2, 0x7e66a5, 0xfecb50 };
        }

        public async Task<CircleMembership> AcceptInvitationAsync(CircleInvitation invitation, User user)
        {
            if (invitation.User != null && invitation.User.Id != user.Id)
            {
                return null;
            }

            var membership = this.databaseContext.CircleMemberships.Create();
            membership.Circle = invitation.Circle;
            membership.User = user;
            membership.IsAdministrator = false;
            membership.MemberSinceDateTime = DateTime.Now;
            membership.ReceiveEmails = true;

            this.databaseContext.CircleMemberships.Add(membership);
            this.databaseContext.CircleInvitations.Remove(invitation);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            await this.SendNotificationInvitationAcceptedToCircleAdminsAsync(membership).ConfigureAwait(false);

            return membership;
        }

        public async Task<CircleMembership> AcceptJoinRequestAsync(CircleJoinRequest joinRequest)
        {
            var membership = this.databaseContext.CircleMemberships.Create();
            membership.Circle = joinRequest.Circle;
            membership.User = joinRequest.User;
            membership.IsAdministrator = false;
            membership.MemberSinceDateTime = DateTime.Now;
            membership.ReceiveEmails = true;

            this.databaseContext.CircleMemberships.Add(membership);
            this.databaseContext.CircleJoinRequests.Remove(joinRequest);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            await this.SendNotificationAcceptedJoinCircleRequest(membership).ConfigureAwait(false);

            return membership;
        }

        public async Task AssignUserToJobAsync(Job job, User user, string portalUrl)
        {
            job.Assignee = user;

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            await this.SendNotificationJobAssignedToCreator(job).ConfigureAwait(false);
            await this.SendEmailJobAssignedToCreator(job, portalUrl).ConfigureAwait(false);
        }

        public async Task UnassignUserToJobAsync(Job job, User user, string portalUrl)
        {
            job.Assignee = null;

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            await this.SendNotificationJobUnassigned(job, user).ConfigureAwait(false);
            await this.SendEmailJobUnassigned(job, user, portalUrl).ConfigureAwait(false);
        }

        public async Task<CircleActivity> CreateCircleActivityAsync(
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
            int ageTo)
        {
            var circleActivity = new CircleActivity
                                     {
                                         Title = title,
                                         Description = description,
                                         Location = location,
                                         AllAges = allAges,
                                         AllDay = allDay,
                                         Circle = circle,
                                         Creator = creator,
                                         CreationDate = DateTime.Now,
                                         StartDateTime = startDateTime,
                                         EndDateTime = endDateTime,
                                         AgeFrom = ageFrom > 0 ? (int?)ageFrom : null,
                                         AgeTo = ageTo > 0 ? (int?)ageTo : null
                                     };

            var attendes = new CircleActivityAttendance
                               {
                                   Attending = true,
                                   CircleActivity = circleActivity,
                                   User = circleActivity.Creator,
                                   UserJoinedDate = DateTime.Now
                               };

            circleActivity.Attendees.Add(attendes);

            this.databaseContext.CircleActivities.Add(circleActivity);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            return circleActivity;
        }

        public async Task<Circle> CreateCircleAsync(
            string name,
            string description,
            bool isPrivate,
            User creator,
            int coverColor,
            ImageDto profileImage,
            ImageDto coverImage)
        {
            var membership = this.databaseContext.CircleMemberships.Create();
            membership.User = creator;
            membership.MemberSinceDateTime = DateTime.Now;
            membership.IsAdministrator = true;
            membership.ReceiveEmails = true;

            var circle = this.databaseContext.Circles.Create();
            circle.Name = name;
            circle.IsPrivate = isPrivate;
            circle.Creator = creator;
            circle.CreationDateTime = DateTime.Now;
            circle.Members.Add(membership);
            circle.Position = creator.Position;
            circle.Description = description;
            circle.CoverColor = coverColor;

            if (profileImage != null)
            {
                circle.ProfileImage = this.mediaService.CreateMedia(profileImage);
            }

            if (coverImage != null)
            {
                circle.CoverImage = this.mediaService.CreateMedia(coverImage);
            }

            this.databaseContext.Circles.Add(circle);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            return circle;
        }

        public async Task CreateCirclePhotoAlbumAsync(Circle circle, string title, User user)
        {
            var circlePhotoAlbum = new CirclePhotoAlbum { CreatedBy = user, Circle = circle, Title = title };
            circle.CirclePhotoAlbums.Add(circlePhotoAlbum);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<CirclePhoto> CreateCirclePhotoAsync(
            ImageDto photo,
            Circle circle,
            User user,
            int? photoAlbumId)
        {
            var circlePhoto = new CirclePhoto { UploadedBy = user, Circle = circle };

            if (photo != null)
            {
                circlePhoto.Photo = this.mediaService.CreateMedia(photo);
            }

            var album = circle.CirclePhotoAlbums.FirstOrDefault(a => a.CirclePhotoAlbumId == photoAlbumId);
            if (album != null)
            {
                circlePhoto.CirclePhotoAlbum = album;
            }

            circle.Photos.Add(circlePhoto);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            await this.SendNotificationNewCirclePhoto(circle, user).ConfigureAwait(false);

            return circlePhoto;
        }

        public async Task<CirclePhoto> CreateCircleDocumentAsync(
            Media document,
            Circle circle,
            User user,
            int? photoAlbumId)
        {
            var circlePhoto = new CirclePhoto { UploadedBy = user, Circle = circle };

            if (document != null)
            {
                circlePhoto.Photo = document;
            }

            var album = circle.CirclePhotoAlbums.FirstOrDefault(a => a.CirclePhotoAlbumId == photoAlbumId);
            if (album != null)
            {
                circlePhoto.CirclePhotoAlbum = album;
            }

            circle.Photos.Add(circlePhoto);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            await this.SendNotificationNewCircleDoc(circle, user).ConfigureAwait(false);

            return circlePhoto;
        }

        public async Task<CircleEmailMessage> CreateEmailMessageAsync(
            string messageText,
            string subject,
            User creator,
            List<User> receivers,
            Circle circle,
            string portalUrl)
        {
            var circleEmailMessage = new CircleEmailMessage
                                         {
                                             Creator = creator,
                                             CreationDateTime = DateTime.Now,
                                             Subject = subject,
                                             Text = messageText,
                                             CircleId = circle.CircleId,
                                             CreatorHasRemovedMessage = false,
                                             Recipients = new Collection<CircleEmailMessageReceiver>()
                                         };

            foreach (var receiver in receivers)
            {
                if (receiver.Id != creator.Id)
                {
                    var circleEmailMessageReceiver = new CircleEmailMessageReceiver
                                                         {
                                                             //EmailMessage = circleEmailMessage,
                                                             //EmailMessageId =
                                                             //    circleEmailMessage.MessageId,
                                                             Receiver = receiver,
                                                             HasRemovedMessage = false
                                                         };

                    circleEmailMessage.Recipients.Add(circleEmailMessageReceiver);

                    var membership = this.GetCircleMembershipById(circle.CircleId, receiver.Id);
                    if (membership == null || membership.ReceiveEmails)
                    {
                        await
                            this.mailerService.SendAsync(
                                new CircleEmailMessageModel
                                    {
                                        Subject = Subject.CircleEmailMessageReceived,
                                        CircleName = circle.Name,
                                        InvitedByName = circleEmailMessage.Creator.Name,
                                        ReceiverName = receiver.Name,
                                        PortalUrl = portalUrl,
                                        UrlToGoToWebsite =
                                            string.Format(
                                                "{0}circles/{1}/inboxreceived", portalUrl,
                                                circle.CircleId)
                                    },
                                receiver.Email).ConfigureAwait(false);
                    }
                }
            }

            this.databaseContext.CircleEmailMessages.Add(circleEmailMessage);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            return circleEmailMessage;
        }

        public async Task<CircleEmailGroup> CreateEmailGroupAsync(
            string Name,
            User Creator,
            List<User> receivers,
            Circle circle,
            string portalUrl)
        {
            var circleEmailGroup = new CircleEmailGroup()
            {
                Creator = Creator,
                Name = Name,
                Receivers = new List<CircleEmailMessageReceiver>(),
                Circle = circle
            };

            receivers.Add(Creator);

            foreach (var receiver in receivers)
            {
                var circleEmailMessageReceiver = new CircleEmailMessageReceiver
                {
                    Receiver = receiver,
                    HasRemovedMessage = false
                };
                circleEmailGroup.Receivers.Add(circleEmailMessageReceiver);
            }

            this.databaseContext.CircleEmailGroups.Add(circleEmailGroup);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            return circleEmailGroup;
        }

        public async Task<CircleEmailMessage> CreateGroupEmailMessageAsync(
            CircleEmailGroup group,
            string messageText,
            User creator,
            Circle circle,
            string portalUrl)
        {
            var circleEmailMessage = new CircleEmailMessage
            {
                Creator = creator,
                CreationDateTime = DateTime.Now,
                Subject = "Conversatie",
                Text = messageText,
                CircleId = circle.CircleId,
                CreatorHasRemovedMessage = false,
                Recipients = new Collection<CircleEmailMessageReceiver>(),
                Group = group
            };

            foreach (var receiver in group.Receivers)
            {
                if (receiver.Receiver.Id != creator.Id)
                {
                    var link = string.Format(
                                            "{0}circles/{1}/messagegroup?groupId={2}", portalUrl,
                                            circle.CircleId, group.CircleEmailGroupId);
                    var membership = this.GetCircleMembershipById(circle.CircleId, receiver.Receiver.Id);
                    if (membership == null || membership.ReceiveEmails)
                    {
                        await
                        this.mailerService.SendAsync(
                            new CircleEmailMessageModel
                            {
                                Subject = Subject.CircleEmailMessageReceived,
                                CircleName = circle.Name,
                                InvitedByName = circleEmailMessage.Creator.Name,
                                ReceiverName = receiver.Receiver.Name,
                                PortalUrl = portalUrl,
                                UrlToGoToWebsite = link
                            },
                            receiver.Receiver.Email).ConfigureAwait(false);
                    }

                    var message = string.Format(Common.InterfaceText.Notification.CircleNewEmail, creator.Name, circle.Name);
                    await this.notificationService.CreateNotificationForUserAsync(receiver.Receiver, message, link, SettingName.CircleNewEmail).ConfigureAwait(false);
                }
            }

            this.databaseContext.CircleEmailMessages.Add(circleEmailMessage);
            try
            {
                await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
            }

            return circleEmailMessage;
        }

        public async Task<Job> CreateJobAsync(
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
            bool sendEmail = true)
        {
            var job = this.databaseContext.Jobs.Create();
            job.Title = title;
            job.Description = description ?? string.Empty;
            job.HasDueTime = hasDueTime;
            job.DueDateTime = dueDateTime;
            job.DueDateTimeEnd = dueDateTimeEnd;
            job.CompletionDateTime = completionDateTime;
            job.CreationDateTime = DateTime.Now;
            job.Creator = creator;
            job.IsOnlyVisibleToSelectedMembers = isOnlyVisibleToSelectedMembers;

            var usersToNotify = circle.Members.Where(m => m.User != creator).Select(m => m.User);

            if (isOnlyVisibleToSelectedMembers)
            {
                var members =
                    await
                    this.databaseContext.Entry(circle)
                        .Collection(c => c.Members)
                        .Query()
                        .Where(m => selectedMembershipIds.Contains(m.CircleMembershipId))
                        .ToListAsync()
                        .ConfigureAwait(false);
                foreach (var member in members)
                {
                    job.VisibleToMembers.Add(member);
                }

                usersToNotify = job.VisibleToMembers.Select(m => m.User);
            }

            circle.Jobs.Add(job);

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            if (sendEmail)
            {
                await this.SendNotificationNewJobToCircleMembers(circle, job, creator).ConfigureAwait(false);
                await this.SendJobEmail(circle, job, usersToNotify, portalUrl).ConfigureAwait(false);
            }

            return job;
        }

        public async Task<Job> SaveJobAsync(
            int jobId,
            string title,
            string description,
            bool hasDueTime,
            DateTime dueDateTime,
            DateTime? dueDateTimeEnd,
            DateTime completionDateTime,
            bool isOnlyVisibleToSelectedMembers,
            List<int> selectedMembershipIds,
            string portalUrl)
        {
            var job = await this.GetJobByIdAsync(jobId).ConfigureAwait(false);
            job.Title = title;
            job.Description = description;
            job.HasDueTime = hasDueTime;
            job.DueDateTime = dueDateTime;
            job.DueDateTimeEnd = dueDateTimeEnd;
            job.CompletionDateTime = completionDateTime;
            job.IsOnlyVisibleToSelectedMembers = isOnlyVisibleToSelectedMembers;

            var circle = await this.GetCircleByIdAsync(job.Circle.CircleId).ConfigureAwait(false);

            if (isOnlyVisibleToSelectedMembers)
            {
                var usersToNotify = job.VisibleToMembers.Select(m => m.User).ToList();

                if (usersToNotify.Any())
                {
                    await this.SendJobEmail(circle, job, usersToNotify, portalUrl).ConfigureAwait(false);
                }
            }

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            return job;
        }

        public async Task<CircleMessage> CreateMessageAsync(
            string messageText,
            ImageDto attachment,
            Circle circle,
            User creator,
            string portalUrl,
            User recipient)
        {
            var circleMessage = new CircleMessage
                                    {
                                        MessageText = messageText,
                                        CreationDateTime = DateTime.Now,
                                        Creator = creator,
                                        IsPrivate = circle.IsPrivate
                                    };

            if (attachment != null)
            {
                circleMessage.Attachment = this.mediaService.CreateMedia(attachment);
            }

            circle.Messages.Add(circleMessage);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            await this.SendNotificationMessageToCircleMembers(circle, circleMessage, creator).ConfigureAwait(false);
            await
                this.SendEmailMessageCreatedToCircleMembers(circle, circleMessage, creator, portalUrl, recipient)
                    .ConfigureAwait(false);

            return circleMessage;
        }

        public async Task<CircleMessageReaction> CreateMessageReactionAsync(
            string reactionText,
            ImageDto attachment,
            Circle circle,
            CircleMessage message,
            User creator)
        {
            var circleMessageReaction = new CircleMessageReaction
                                            {
                                                Circle = circle,
                                                Message = message,
                                                Creator = creator,
                                                CreationDateTime = DateTime.Now,
                                                ReactionText = reactionText
                                            };
            if (attachment != null)
            {
                circleMessageReaction.Attachment = this.mediaService.CreateMedia(attachment);
            }

            message.Reactions.Add(circleMessageReaction);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            await
                this.SendNotificationMessageReactionToCircleMembers(circle, circleMessageReaction, creator)
                    .ConfigureAwait(false);

            return circleMessageReaction;
        }

        public async Task<bool> DeclineInvitationAsync(CircleInvitation invitation)
        {
            this.databaseContext.CircleInvitations.Remove(invitation);

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        public async Task DeleteMessageAsync(CircleMessage message, Circle circle)
        {
            message.IsHidden = true;

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task EditCircleAsync(
            Circle circle,
            string name,
            string description,
            bool isPrivate,
            int coverColor,
            ImageDto profileImage,
            ImageDto coverImage)
        {
            circle.Name = name;
            circle.Description = description;
            circle.IsPrivate = isPrivate;
            circle.CoverColor = coverColor;

            if (profileImage != null)
            {
                if (circle.ProfileImage != null)
                {
                    this.databaseContext.Media.Remove(circle.ProfileImage);
                }

                circle.ProfileImage = this.mediaService.CreateMedia(profileImage);
            }

            if (coverImage != null)
            {
                if (circle.CoverImage != null)
                {
                    this.databaseContext.Media.Remove(circle.CoverImage);
                }

                circle.CoverImage = this.mediaService.CreateMedia(coverImage);
            }

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task EditMembershipProfile(
            CircleMembership membership,
            string profileText)
        {
            membership.Profile = profileText;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<CircleMessage> EditMessageAsync(
            CircleMessage message,
            Circle circle,
            User currentUser,
            string portalUrl,
            string newMessageText)
        {
            message.MessageText = newMessageText;

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            return message;
        }

        public async Task<List<Circle>> GetAllCirclesAsync()
        {
            return await this.databaseContext.Circles.ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<CirclePhoto>> GetAllCirclePhotosForAlbumAsync(int? id)
        {
            return await this.databaseContext.CirclePhotos.Where(p => p.CirclePhotoAlbum.CirclePhotoAlbumId == id).ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<CirclePhoto>> GetAllCirclePhotosFromCircleAsync(int? id)
        {
            return await this.databaseContext.CirclePhotos.Where(p => p.Circle.CircleId == id).ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<Media>> GetAllMediaFromCircleMessagesAsync(int? id)
        {
            return
                await
                this.databaseContext.CircleMessages.Where(m => m.Circle.CircleId == id)
                    .Where(message => message.AttachmentId != null)
                    .Select(message => message.Attachment)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<List<Media>> GetAllMediaForAlbumAsync(int? id)
        {
            return
                await
                this.databaseContext.CirclePhotos.Where(p => p.CirclePhotoAlbum.CirclePhotoAlbumId == id)
                    .Where(photo => photo.PhotoId != null)
                    .Select(photo => photo.Photo)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<List<Media>> GetAllMediaForCircleAsync(int? id)
        {
            var uploaded =
                await
                this.databaseContext.CirclePhotos.Where(p => p.Circle.CircleId == id)
                    .Where(photo => photo.PhotoId != null)
                    .Select(photo => photo.Photo)
                    .ToListAsync()
                    .ConfigureAwait(false);
            var attached =
                await
                this.databaseContext.CircleMessages.Where(m => m.Circle.CircleId == id)
                    .Where(message => message.AttachmentId != null)
                    .Select(message => message.Attachment)
                    .ToListAsync()
                    .ConfigureAwait(false);

            return uploaded.Union(attached).ToList();
        }

        public async Task<List<Circle>> GetAllPublicCirclesAsync()
        {
            return await this.databaseContext.Circles.Where(c => !c.IsPrivate).ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<Job>> GetAssignedJobsAsync(Circle circle, User user)
        {
            return await this.GetJobsAsync(circle, user, j => j.Assignee != null).ConfigureAwait(false);
        }

        public async Task<List<Job>> GetAssignedJobsAsync(Circle circle, User user, DateTime since, int limit = 10)
        {
            return
                await
                this.GetJobsAsync(circle, user, j => j.Assignee != null && j.DueDateTime >= since, limit)
                    .ConfigureAwait(false);
        }

        public async Task<Media> GetAttachmentByCircleMessageIdAsync(User user, int? circleMessageId, int? mediaId)
        {
            var message = await this.databaseContext.CircleMessages.FirstOrDefaultAsync(r => r.MessageId == circleMessageId).ConfigureAwait(false);
            if (message != null && message.AttachmentId == mediaId)
            {
                var userCircle = user.Circles.FirstOrDefault(c => c.Circle.CircleId == message.Circle.CircleId);
                if (userCircle != null)
                    return message.Attachment;
            }

            return null;
        }

        public async Task<Media> GetAttachmentByCircleMessageReactionIdAsync(User user, int? circleMessageReactionId, int? mediaId)
        {
            var reaction = await this.databaseContext.CircleMessageReactions.FirstOrDefaultAsync(r => r.ReactionId == circleMessageReactionId).ConfigureAwait(false);
            if (reaction != null && reaction.Attachment.MediaId == mediaId)
            {
                var userCircle = user.Circles.FirstOrDefault(c => c.Circle.CircleId == reaction.Circle.CircleId);
                if (userCircle != null)
                    return reaction.Attachment;
            }

            return null;
        }

        public async Task<List<CircleActivity>> GetCircleActivitiesByIdAsync(int circleId)
        {
            return
                await
                this.databaseContext.CircleActivities.Where(a => a.StartDateTime > DateTime.Now).ToListAsync().ConfigureAwait(false);
        }

        public async Task<Circle> GetCircleByIdAsync(int? circleId)
        {
            return
                await
                this.databaseContext.Circles.FirstOrDefaultAsync(c => c.CircleId == circleId).ConfigureAwait(false);
        }

        public async Task<Circle> GetCircleByIdAsync(int? circleId, User user)
        {
            return
                await
                this.databaseContext.Circles.FirstOrDefaultAsync(c => c.CircleId == circleId).ConfigureAwait(false);
        }

        public List<Activity> GetActivitiesByCircle(Circle circle)
        {
            return
                this.databaseContext.Activities.Where(a => a.Circle.CircleId == circle.CircleId).ToList();
        }

        public List<CircleLabel> GetCircleLabels(Circle circle)
        {
            return
                this.databaseContext.CircleLabels.Where(l => l.Circle.CircleId == circle.CircleId).ToList();
        }

        public List<CircleSetting> GetCircleSettings(Circle circle)
        {
            return
                this.databaseContext.CircleSettings.Where(s => s.Circle.CircleId == circle.CircleId).ToList();
        }

        public CircleSetting GetCircleSettingByName(Circle circle, string name)
        {
            return
                this.databaseContext.CircleSettings.FirstOrDefault(s => s.Circle.CircleId == circle.CircleId && s.SettingName == name);
        }

        public async Task<CircleMembership> GetCircleMembershipByIdAsync(int? circleId, string userId)
        {
            return
                await
                this.databaseContext.CircleMemberships.FirstOrDefaultAsync(
                    m => m.Circle.CircleId == circleId && m.User.Id == userId).ConfigureAwait(false);
        }

        public CircleMembership GetCircleMembershipById(int? circleId, string userId)
        {
            return
                this.databaseContext.CircleMemberships.FirstOrDefault(
                    m => m.Circle.CircleId == circleId && m.User.Id == userId);
        }

        public int[] GetCoverColors()
        {
            return this.coverColors;
        }

        public async Task<Media> GetCoverImageByCircleIdAsync(int? circleId, int? mediaId)
        {
            return
                await
                this.databaseContext.Circles.Where(u => u.CircleId == circleId && u.CoverImageId == mediaId)
                    .Select(u => u.CoverImage)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
        }

        public async Task<List<CircleEmailGroup>> GetEmailGroupsAsync(User user, Circle circle)
        {
            return await this.databaseContext.CircleEmailGroups.Include(g => g.Receivers.Select(r => r.Receiver)).Where(g => g.Circle.CircleId == circle.CircleId)
                .Where(g => g.Receivers.Any(r => r.Receiver.Id == user.Id)).ToListAsync().ConfigureAwait(false);
        }

        public async Task<CircleEmailGroup> GetEmailGroupAsync(int groupId)
        {
            return await this.databaseContext.CircleEmailGroups.Include(g => g.Receivers.Select(r => r.Receiver)).FirstOrDefaultAsync(g => g.CircleEmailGroupId == groupId).ConfigureAwait(false);
        }

        public async Task<List<CircleEmailMessage>> GetEmailGroupMessagesAsync(CircleEmailGroup group)
        {
            return await this.databaseContext.CircleEmailMessages.Where(m => m.Group.CircleEmailGroupId == group.CircleEmailGroupId).ToListAsync().ConfigureAwait(false);
        }

        public async Task<CircleEmailMessage> GetLastEmailMessage(int groupId)
        {
            return await this.databaseContext.CircleEmailMessages.Include(m => m.Creator).OrderByDescending(m => m.MessageId).FirstOrDefaultAsync(m => m.Group.CircleEmailGroupId == groupId).ConfigureAwait(false);
        }

        public async Task<CircleEmailMessage> GetEmailMessageAsync(int? emailMessageid)
        {
            return
                await this.databaseContext.CircleEmailMessages.FirstOrDefaultAsync(m => m.MessageId == emailMessageid).ConfigureAwait(false);
        }

        public ICollection<CircleEmailMessage> GetEmailMessages(int circleId, string userId)
        {
            var emailMessages = new List<CircleEmailMessage>();

            foreach (var message in
                this.databaseContext.CircleEmailMessages.ToList().OrderByDescending(i => i.CreationDateTime))
            {
                if (message.CircleId == circleId)
                {
                    foreach (var receiver in message.Recipients)
                    {
                        if (receiver.Receiver != null && (userId == receiver.Receiver.Id && !receiver.HasRemovedMessage))
                        {
                            emailMessages.Add(message);
                        }
                    }
                }
            }

            return emailMessages;
        }

        public List<Circle> GetFeaturedCircles(User user)
        {
            return this.databaseContext.FeaturedCircles.Select(f => f.Circle).Where(c => c.Members.Any(u => u.User.Id == user.Id)).ToList();
        }

        public async Task<CircleInvitation> GetInvitationByCircleIdAcceptTokenAsync(int? id, string acceptToken)
        {
            return
                await
                this.databaseContext.CircleInvitations.FirstOrDefaultAsync(
                    i => i.Circle.CircleId == id && i.AcceptToken == acceptToken).ConfigureAwait(false);
        }

        public async Task<CircleInvitation> GetInvitationByCircleIdAcceptTokenAsync(
            int? id,
            string acceptToken,
            User user)
        {
            return
                await
                this.databaseContext.CircleInvitations.FirstOrDefaultAsync(
                    i => i.Circle.CircleId == id && i.AcceptToken == acceptToken && i.User.Id == user.Id)
                    .ConfigureAwait(false);
        }

        public async Task<List<CircleInvitation>> GetInvitationsAsync(Circle circle)
        {
            return
                await
                this.databaseContext.Entry(circle)
                    .Collection(c => c.Invitations)
                    .Query()
                    .Include(i => i.User)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public CircleInvitation GetUserInvitationForCircle(User user, Circle circle)
        {
            return
                this.databaseContext.CircleInvitations.FirstOrDefault(i => i.Circle.CircleId == circle.CircleId && i.User.Id == user.Id);
        }

        public async Task<CircleInvitation> GetUserInvitationForCircleAsync(User user, Circle circle)
        {
            return await
                this.databaseContext.CircleInvitations.FirstOrDefaultAsync(i => i.Circle.CircleId == circle.CircleId && i.User.Id == user.Id).ConfigureAwait(false);
        }

        public async Task<CircleInvitation> GetUserInvitationForCircleByEmailAsync(string email, Circle circle)
        {
            return await
                this.databaseContext.CircleInvitations.FirstOrDefaultAsync(i => i.Circle.CircleId == circle.CircleId && i.Email == email).ConfigureAwait(false);
        }

        public async Task<List<CircleInvitation>> GetInvitationsByUserAsync(User user)
        {
            return
                await
                this.databaseContext.CircleInvitations.Where(c => c.User.Id == user.Id)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<CircleJoinRequest> GetJoinRequestByCircleIdAcceptTokenAsync(int? id, string acceptToken)
        {
            return
                await
                this.databaseContext.CircleJoinRequests.FirstOrDefaultAsync(
                    i => i.Circle.CircleId == id && i.AcceptToken == acceptToken).ConfigureAwait(false);
        }

        public async Task<CircleJoinRequest> GetJoinRequestByIdAsync(int? id, int? requestId)
        {
            return
                await
                this.databaseContext.CircleJoinRequests.FirstOrDefaultAsync(
                    r => r.Circle.CircleId == id && r.CircleJoinRequestId == requestId).ConfigureAwait(false);
        }

        public async Task<CircleMessage> GetMessageByIdAsync(int circleId, int messageId)
        {
            var circle = await this.GetCircleByIdAsync(circleId).ConfigureAwait(false);
            if (circle == null)
            {
                return null;
            }

            return circle.Messages.FirstOrDefault(m => m.MessageId == messageId);
        }

        public async Task<Media> GetPhotoByMediaId(int circleId, int? mediaId)
        {
            return
                await
                this.databaseContext.CirclePhotos.Where(p => p.Circle.CircleId == circleId && p.PhotoId == mediaId)
                .Select(p => p.Photo).FirstOrDefaultAsync().ConfigureAwait(false);
                //this.databaseContext.Media.Where(m => m.MediaId == mediaId).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<Media> GetProfileImageByCircleIdAsync(int? circleId, int? mediaId)
        {
            return
                await
                this.databaseContext.Circles.Where(c => c.CircleId == circleId && c.ProfileImageId == mediaId)
                    .Select(c => c.ProfileImage)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
        }

        public async Task<List<Circle>> GetPublicCirclesInNeighborhoodAsync(
            string userId,
            DbGeography position,
            int radius)
        {
            var userCirclesQuery = this.GetUserCirclesQuery(userId);

            return
                await
                this.databaseContext.Circles.Where(c => !c.IsPrivate)
                    .Where(c => c.Position.Distance(position) <= radius)
                    .Where(c => !userCirclesQuery.Contains(c))
                    .OrderBy(c => c.Name)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public ICollection<CircleEmailMessage> GetSendEmailMessages(int circleId, string userId)
        {
            var sendEmailMessages = new List<CircleEmailMessage>();

            foreach (var message in
                this.databaseContext.CircleEmailMessages.ToList().OrderByDescending(i => i.CreationDateTime))
            {
                if (message.Creator != null && (message.CircleId == circleId && message.Creator.Id == userId && !message.CreatorHasRemovedMessage))
                {
                    sendEmailMessages.Add(message);
                }
            }

            return sendEmailMessages;
        }

        public async Task<Job> GetUnassignedJobByIdAsync(int? circleId, int? jobId)
        {
            return
                await
                this.databaseContext.Jobs.FirstOrDefaultAsync(j => j.Circle.CircleId == circleId && j.JobId == jobId)
                    .ConfigureAwait(false);
        }

        public async Task<List<Job>> GetUnassignedJobsAsync(Circle circle, User user)
        {
            return await this.GetJobsAsync(circle, user, j => j.Assignee == null).ConfigureAwait(false);
        }

        public async Task<List<Circle>> GetUserCirclesAsync(User user)
        {
            return await this.GetUserCirclesQuery(user.Id).ToListAsync().ConfigureAwait(false);
        }

        public async Task GiveAdminRightsAsync(CircleMembership membership)
        {
            membership.IsAdministrator = true;

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task InviteUserAsync(Circle circle, User user, User invitedBy, string portalUrl)
        {
            if (!(await this.ShouldInviteUser(circle, user).ConfigureAwait(false)))
            {
                return;
            }

            var circleInvitation = await this.CreateCircleInvitationAsync(circle, invitedBy, user).ConfigureAwait(false);

            await this.SendInvitationEmailAsync(circleInvitation, portalUrl).ConfigureAwait(false);
            await this.SendNotificationInviteUserAsync(user, circle).ConfigureAwait(false);
        }

        public async Task InviteUserByEmailAsync(
            Circle circle,
            IEnumerable<string> emailAddresses,
            User invitedBy,
            string portalUrl)
        {
            foreach (var emailAddress in emailAddresses.Where(this.mailerService.IsValidEmail))
            {
                var email = emailAddress;

                var user =
                    await this.databaseContext.Users.FirstOrDefaultAsync(u => u.Email == email).ConfigureAwait(false);
                if (user != null && !(await this.ShouldInviteUser(circle, user).ConfigureAwait(false)))
                {
                    continue;
                }

                if (!(await this.ShouldInviteEmail(circle, email).ConfigureAwait(false)))
                {
                    continue;
                }

                var circleInvitation =
                    await this.CreateCircleInvitationAsync(circle, invitedBy, user, email).ConfigureAwait(false);

                await this.SendInvitationEmailAsync(circleInvitation, portalUrl).ConfigureAwait(false);
            }
        }

        public bool IsUserAdminOfCircle(Circle circle, User user)
        {
            return
                this.databaseContext.Entry(circle)
                    .Collection(c => c.Members)
                    .Query()
                    .Where(m => m.IsAdministrator)
                    .Any(m => m.User.Id == user.Id);
        }

        public bool IsUserAllowedToLeaveCircle(Circle circle, User user)
        {
            return this.IsUserMemberOfCircle(circle, user)
                   && (!this.IsUserAdminOfCircle(circle, user)
                       || this.databaseContext.Entry(circle)
                              .Collection(c => c.Members)
                              .Query()
                              .Count(m => m.IsAdministrator) > 1);
        }

        public bool IsUserInvitedToJoinCircle(Circle circle, User user)
        {
            return
                this.databaseContext.Entry(circle).Collection(c => c.Invitations).Query().Any(m => m.User.Id == user.Id && m.ExpireDate > DateTime.Now);
        }

        public CircleInvitation UserInvitationToJoinCircle(Circle circle, User user)
        {
            return
                this.databaseContext.Entry(circle).Collection(c => c.Invitations).Query().FirstOrDefault(m => m.User.Id == user.Id);
        }

        public bool IsUserMemberOfCircle(Circle circle, User user)
        {
            return this.databaseContext.Entry(circle).Collection(c => c.Members).Query().Any(m => m.User.Id == user.Id);
        }

        public async Task RejectJoinRequestAsync(CircleJoinRequest joinRequest)
        {
            await this.SendNotificationRequestRejectedAsync(joinRequest.User, joinRequest.Circle).ConfigureAwait(false);
            this.databaseContext.CircleJoinRequests.Remove(joinRequest);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RemoveAttachment(CircleMessage message, int attachmentId)
        {
            message.AttachmentId = null;

            var attachment = this.databaseContext.Media.FirstOrDefault(m => m.MediaId == attachmentId);
            this.databaseContext.Media.Remove(attachment);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RemoveAttachment(CircleMessageReaction message, int attachmentId)
        {
            message.Attachment = null;

            var attachment = this.databaseContext.Media.FirstOrDefault(m => m.MediaId == attachmentId);
            this.databaseContext.Media.Remove(attachment);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RemoveCircleAsync(Circle circle)
        {
            var activityAttendances =
                await
                this.databaseContext.ActivityAttendances.Where(
                    aa => aa.Activity.Circle.CircleId == circle.CircleId)
                    .ToListAsync()
                    .ConfigureAwait(false);
            foreach (var activityAttendance in activityAttendances)
            {
                this.databaseContext.ActivityAttendances.Remove(activityAttendance);
            }

            var activityIntervals =
                await
                this.databaseContext.ActivityIntervals.Where(ai => ai.Activity.Circle.CircleId == circle.CircleId)
                    .ToListAsync()
                    .ConfigureAwait(false);
            foreach (var activityInterval in activityIntervals)
            {
                this.databaseContext.ActivityIntervals.Remove(activityInterval);
            }

            var activityInvitations =
                await
                this.databaseContext.ActivityInvitations.Where(
                    ai => ai.Activity.Circle.CircleId == circle.CircleId)
                    .ToListAsync()
                    .ConfigureAwait(false);
            foreach (var activityInvitation in activityInvitations)
            {
                this.databaseContext.ActivityInvitations.Remove(activityInvitation);
            }

            var activityReactions =
                await
                this.databaseContext.ActivityReactions.Where(
                    ar => ar.Activity.Circle.CircleId == circle.CircleId)
                    .ToListAsync()
                    .ConfigureAwait(false);
            foreach (var activityReaction in activityReactions)
            {
                this.databaseContext.ActivityReactions.Remove(activityReaction);
            }

            var activities =
                await
                this.databaseContext.Activities.Where(a => a.Circle.CircleId == circle.CircleId).ToListAsync().ConfigureAwait(false);
            foreach (var activity in activities)
            {
                this.databaseContext.Activities.Remove(activity);
            }

            var messageReceivers =
                await
                this.databaseContext.CircleEmailMessageReceivers.Where(
                    r => r.EmailGroup.Circle.CircleId == circle.CircleId)
                    .ToListAsync()
                    .ConfigureAwait(false);
            foreach (var circleEmailMessageReceiver in messageReceivers)
            {
                this.databaseContext.CircleEmailMessageReceivers.Remove(circleEmailMessageReceiver);
            }

            var emailMessages =
                await
                this.databaseContext.CircleEmailMessages.Where(
                    m => m.CircleId == circle.CircleId).ToListAsync().ConfigureAwait(false);
            foreach (var circleEmailMessage in emailMessages)
            {
                this.databaseContext.CircleEmailMessages.Remove(circleEmailMessage);
            }

            var emailGroups =
                await
                this.databaseContext.CircleEmailGroups.Where(g => g.Circle.CircleId == circle.CircleId)
                    .ToListAsync()
                    .ConfigureAwait(false);
            foreach (var circleEmailGroup in emailGroups)
            {
                this.databaseContext.CircleEmailGroups.Remove(circleEmailGroup);
            }

            var circleInvitations =
                await
                this.databaseContext.CircleInvitations.Where(c => c.Circle.CircleId == circle.CircleId)
                    .ToListAsync()
                    .ConfigureAwait(false);
            foreach (var invite in circleInvitations)
            {
                this.databaseContext.CircleInvitations.Remove(invite);
            }

            var circleJoinRequests =
                await
                this.databaseContext.CircleJoinRequests.Where(c => c.Circle.CircleId == circle.CircleId)
                    .ToListAsync()
                    .ConfigureAwait(false);
            foreach (var circleJoinRequest in circleJoinRequests)
            {
                this.databaseContext.CircleJoinRequests.Remove(circleJoinRequest);
            }

            var jobs = this.databaseContext.Jobs.Where(j => j.Circle.CircleId == circle.CircleId).ToList();
            foreach (var job in jobs)
            {
                this.databaseContext.Jobs.Remove(job);
            }

            var photos = this.databaseContext.CirclePhotos.Where(p => p.Circle.CircleId == circle.CircleId).Include(p => p.Photo).ToList();
            foreach (var photo in photos)
            {
                this.databaseContext.Media.Remove(photo.Photo);
                this.databaseContext.CirclePhotos.Remove(photo);
            }

            var albums = this.databaseContext.CirclePhotoAlbums.Where(a => a.Circle.CircleId == circle.CircleId).ToList();
            foreach (var album in albums)
            {
                this.databaseContext.CirclePhotoAlbums.Remove(album);
            }

            var messageReactions = this.databaseContext.CircleMessageReactions.Where(m => m.Circle.CircleId == circle.CircleId).ToList();
            foreach (var reaction in messageReactions)
            {
                this.databaseContext.CircleMessageReactions.Remove(reaction);
            }

            var messages = this.databaseContext.CircleMessages.Where(m => m.Circle.CircleId == circle.CircleId).ToList();
            foreach (var message in messages)
            {
                var childReactions = this.databaseContext.CircleMessageReactions.Where(r => r.Message.MessageId == message.MessageId).ToList();
                foreach (var childReaction in childReactions)
                {
                    this.databaseContext.CircleMessageReactions.Remove(childReaction);
                }
                this.databaseContext.CircleMessages.Remove(message);
            }

            this.databaseContext.Circles.Remove(circle);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RemoveCirclePhoto(int photoId)
        {
            var message = await this.databaseContext.CircleMessages.FirstOrDefaultAsync(m => m.AttachmentId == photoId).ConfigureAwait(false);
            if (message != null)
            {
                message.AttachmentId = null;
            }

            var circlePhoto =
                await this.databaseContext.CirclePhotos.FirstOrDefaultAsync(m => m.Photo.MediaId == photoId).ConfigureAwait(false);
            if (circlePhoto != null)
            {
                this.databaseContext.CirclePhotos.Remove(circlePhoto);
            }

            var media = await this.databaseContext.Media.FirstOrDefaultAsync(m => m.MediaId == photoId).ConfigureAwait(false);
            if (media != null)
            {
                this.databaseContext.Media.Remove(media);
            }

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RemoveCirclePhotoAlbumAsync(int photoAlbumId)
        {
            var album =
                await
                this.databaseContext.CirclePhotoAlbums.FirstOrDefaultAsync(a => a.CirclePhotoAlbumId == photoAlbumId)
                    .ConfigureAwait(false);

            this.databaseContext.CirclePhotoAlbums.Remove(album);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RemoveEmailGroupReceiver(CircleEmailMessageReceiver receiver)
        {
            this.databaseContext.CircleEmailMessageReceivers.Remove(receiver);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RemoveInvitationAsync(int circleId, string userId)
        {
            var circleInvitation =
                await
                this.databaseContext.CircleInvitations.FirstOrDefaultAsync(
                    m => m.Circle.CircleId == circleId && m.User.Id == userId).ConfigureAwait(false);
            if (circleInvitation == null)
            {
                return;
            }

            this.databaseContext.CircleInvitations.Remove(circleInvitation);

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RemoveInvitationByEmailAsync(int circleId, string email)
        {
            if (email == null)
            {
                return;
            }

            var circleInvitation =
                await
                this.databaseContext.CircleInvitations.FirstOrDefaultAsync(
                    m => m.Circle.CircleId == circleId && m.Email == email).ConfigureAwait(false);

            if (circleInvitation != null)
            {
                this.databaseContext.CircleInvitations.Remove(circleInvitation);
            }

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RemoveJobAsync(Circle circle, int? jobId)
        {
            var job =
                await
                this.databaseContext.Entry(circle)
                    .Collection(c => c.Jobs)
                    .Query()
                    .FirstOrDefaultAsync(j => j.JobId == jobId)
                    .ConfigureAwait(false);

            if (job != null)
            {
                this.databaseContext.Jobs.Remove(job);
                await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task RemoveMemberAsync(int circleId, string userId)
        {
            var circleMembership =
                await
                this.databaseContext.CircleMemberships.FirstOrDefaultAsync(
                    m => m.Circle.CircleId == circleId && m.User.Id == userId).ConfigureAwait(false);
            var circleInvitation =
                await
                this.databaseContext.CircleInvitations.FirstOrDefaultAsync(
                    m => m.Circle.CircleId == circleId && m.User.Id == userId).ConfigureAwait(false);
            if (circleMembership == null && circleInvitation == null)
            {
                return;
            }

            if (circleMembership != null)
            {
                this.databaseContext.CircleMemberships.Remove(circleMembership);
            }

            if (circleInvitation != null)
            {
                this.databaseContext.CircleInvitations.Remove(circleInvitation);
            }

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RemoveReceivedCircleEmailMessageAsync(string userId, int emailMessageId)
        {
            var circleMessageReceiver =
                await
                this.databaseContext.CircleEmailMessageReceivers.FirstAsync(
                    r => r.Receiver.Id == userId && r.ReceiverId == emailMessageId).ConfigureAwait(false);
            circleMessageReceiver.HasRemovedMessage = true;

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RemoveSendCircleEmailMessageAsync(string userId, int emailMessageId)
        {
            var circleMessage =
                await
                this.databaseContext.CircleEmailMessages.FirstAsync(
                    m => m.Creator.Id == userId && m.MessageId == emailMessageId).ConfigureAwait(false);
            circleMessage.CreatorHasRemovedMessage = true;

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<bool> RequestToJoinAsync(Circle circle, User user, string portalUrl)
        {
            if (circle.IsPrivate)
            {
                return false;
            }

            var invitation = this.UserInvitationToJoinCircle(circle, user);

            var userIsMemberOf = this.IsUserMemberOfCircle(circle, user);
            var userIsInvitedToJoin = invitation != null && invitation.ExpireDate > DateTime.Now;
            var joinRequestIsPending = this.IsCircleJoinRequestPending(circle, user);
            if (userIsMemberOf || userIsInvitedToJoin || joinRequestIsPending)
            {
                return false;
            }

            var joinRequest = this.databaseContext.CircleJoinRequests.Create();
            joinRequest.Circle = circle;
            joinRequest.User = user;
            joinRequest.AcceptToken = this.randomGenerator.GenerateRandomToken();
            joinRequest.JoinRequestDateTime = DateTime.Now;

            this.databaseContext.CircleJoinRequests.Add(joinRequest);
            if (invitation != null)
            {
                this.databaseContext.CircleInvitations.Remove(invitation);
            }

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            var admins = await this.GetAdminsAsync(circle).ConfigureAwait(false);
            var acceptJoinRequestUrl = this.linkGenerator.GenerateCircleAcceptJoinRequestLink(
                circle.CircleId,
                joinRequest.AcceptToken);

            var message = string.Format(
                Common.InterfaceText.Notification.CircleReqestedToJoin,
                HttpUtility.HtmlEncode(joinRequest.User.Name),
                joinRequest.Circle.Name);
            var link = this.linkGenerator.GenerateCircleJoinRequestsLink(joinRequest.Circle.CircleId);
            var members = await this.GetAdminsOfCircleQuery(circle).ToListAsync().ConfigureAwait(false);
            await this.notificationService.CreateNotificationForUsersAsync(members, message, link, SettingName.CircleRequestedToJoin).ConfigureAwait(false);

            await
                this.mailerService.SendAsync(
                    new CircleJoinRequestModel
                    {
                        CircleName = circle.Name,
                        Subject = Subject.CircleJoinRequest,
                        PortalUrl = portalUrl,
                        UserName = user.Name,
                        AcceptJoinRequestUrl = acceptJoinRequestUrl,
                    },
                    admins.ToArray()).ConfigureAwait(false);

            return true;
        }

        public async Task RevokeAdminRightsAsync(CircleMembership membership)
        {
            var numberOfAdminsLeft =
                await
                this.databaseContext.Entry(membership.Circle)
                    .Collection(c => c.Members)
                    .Query()
                    .CountAsync(m => m.IsAdministrator && m.CircleMembershipId != membership.CircleMembershipId)
                    .ConfigureAwait(false);
            if (numberOfAdminsLeft == 0)
            {
                return;
            }

            membership.IsAdministrator = false;

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task StopCircleEmailAsync(CircleMembership membership)
        {
            membership.ReceiveEmails = false;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task StartCircleEmailAsync(CircleMembership membership)
        {
            membership.ReceiveEmails = true;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task SendInvitationReminderAsync(Circle circle, User user, User invitedBy, string portalUrl)
        {
            var circleInvitation = await this.GetUserInvitationForCircleAsync(user, circle).ConfigureAwait(false);

            if (circleInvitation == null)
            {
                circleInvitation = await this.CreateCircleInvitationAsync(circle, invitedBy, user).ConfigureAwait(false);
            }
            else
            {
                circleInvitation.ExpireDate = DateTime.Now.AddDays(7);
                await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            }

            await this.SendInvitationEmailAsync(circleInvitation, portalUrl).ConfigureAwait(false);
            await this.SendNotificationInviteUserAsync(user, circle).ConfigureAwait(false);
        }

        public async Task SendInvitationReminderByEmailAsync(Circle circle, string emailAddress, User invitedBy, string portalUrl)
        {
            var user =
                await this.databaseContext.Users.FirstOrDefaultAsync(u => u.Email == emailAddress).ConfigureAwait(false);

            CircleInvitation circleInvitation = null;
            if (user != null)
            {
                circleInvitation = await this.GetUserInvitationForCircleAsync(user, circle).ConfigureAwait(false);
            }
            else
            {
                circleInvitation = await this.GetUserInvitationForCircleByEmailAsync(emailAddress, circle).ConfigureAwait(false);
            }

            if (circleInvitation == null)
            {
                circleInvitation =
                    await this.CreateCircleInvitationAsync(circle, invitedBy, user, emailAddress).ConfigureAwait(false);
            }
            else
            {
                circleInvitation.ExpireDate = DateTime.Now.AddDays(7);
                await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            }

            await this.SendInvitationEmailAsync(circleInvitation, portalUrl).ConfigureAwait(false);
        }

        public async Task<List<CircleMembership>> SearchMembersAsync(
            Circle circle,
            string q,
            List<int> excludeMembershipIds)
        {
            return
                await
                this.databaseContext.CircleMemberships.Include(m => m.User)
                    .Where(m => m.Circle.CircleId == circle.CircleId)
                    .Where(m => !excludeMembershipIds.Contains(m.CircleMembershipId))
                    .Where(m => m.User.Name.Contains(q))
                    .OrderBy(m => m.User.Name)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<List<User>> SearchNeighborsNotInCircleAsync(
            Circle circle,
            DbGeography position,
            int radius,
            string q)
        {
            return
                await
                this.databaseContext.Users.Where(u => u.Position.Distance(position) <= radius)
                    .Where(u => u.Circles.All(c => c.Circle.CircleId != circle.CircleId))
                    .Where(u => u.CircleInvitations.All(i => i.Circle.CircleId != circle.CircleId))
                    .Where(u => u.Name.Contains(q))
                    .OrderBy(u => u.Name)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<Circle> SetCoverImage(int circleId, Media media)
        {
            var circle = await this.GetCircleByIdAsync(circleId).ConfigureAwait(false);
            if (circle == null)
            {
                return null;
            }

            circle.CoverImage = media;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            return circle;
        }

        public async Task<Circle> SetCoverImageAsync(int circleId, Media media)
        {
            var circle =
                await
                this.databaseContext.Circles.FirstOrDefaultAsync(c => c.CircleId == circleId).ConfigureAwait(false);
            if (circle == null)
            {
                return null;
            }

            circle.CoverImage = media;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            return circle;
        }

        public async Task<Circle> SetPrivateAsync(int circleId)
        {
            var circle = await this.GetCircleByIdAsync(circleId).ConfigureAwait(false);
            if (circle == null)
            {
                return null;
            }

            circle.IsPrivate = true;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            return circle;
        }

        public async Task<Circle> SetProfileImageAsync(int circleId, Media media)
        {
            var circle =
                await
                this.databaseContext.Circles.FirstOrDefaultAsync(c => c.CircleId == circleId).ConfigureAwait(false);
            if (circle == null)
            {
                return null;
            }

            circle.ProfileImage = media;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            return circle;
        }

        public async Task<Circle> SetPublicAsync(int circleId)
        {
            var circle = await this.GetCircleByIdAsync(circleId).ConfigureAwait(false);
            if (circle == null)
            {
                return null;
            }

            circle.IsPrivate = false;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            return circle;
        }

        public async Task UserLeaveCircle(Circle circle, User user)
        {
            await this.RemoveMemberAsync(circle.CircleId, user.Id).ConfigureAwait(false);

            var message = string.Format(Common.InterfaceText.Notification.CircleMemberLeft, user.Name, circle.Name);
            var link = this.linkGenerator.GenerateCircleMembersLink(circle.CircleId);
            var members = this.GetAdminsOfCircleQuery(circle).Where(u => u.Id != user.Id).ToList();

            await this.notificationService.CreateNotificationForUsersAsync(members, message, link, SettingName.CircleMemberLeft).ConfigureAwait(false);
        }

        public async Task<Job> GetJobByIdAsync(int? jobId)
        {
            return await this.databaseContext.Jobs.FirstOrDefaultAsync(j => j.JobId == jobId).ConfigureAwait(false);
        }

        public async Task SendNotificationNewCircleMarketPlaceItem(Circle circle, User user, MarketplaceItem marketplaceItem)
        {
            var link = this.linkGenerator.GenerateCircleMarketPlaceItem(circle.CircleId, marketplaceItem.MarketplaceItemId);
            var message = string.Format(Common.InterfaceText.Notification.CircleNewMarketplaceItem, circle.Name);

            await
                this.notificationService.CreateNotificationForUsersAsync(
                    circle.Members.Where(m => m.User != user).Select(m => m.User),
                    message,
                    link,
                    SettingName.CircleNewMarketplaceItem).ConfigureAwait(false);
        }

        public async Task SendEmailMarketPlaceItemCreatedToCircleMembers(
            Circle circle,
            User creator,
            string portalUrl,
            MarketplaceItem marketplaceItem)
        {
            var link = this.linkGenerator.GenerateCircleMarketPlaceItem(circle.CircleId, marketplaceItem.MarketplaceItemId);

            var model = new CircleMarketPlaceItemCreatedModel
                            {
                                PortalUrl = portalUrl,
                                MessageUrl = link,
                                CircleName = circle.Name,
                                MarketPlaceItemName = marketplaceItem.Title,
                                Subject = Subject.CircleMarketPlaceItemGenerated
                            };
            var membership = this.GetCircleMembershipById(circle.CircleId, creator.Id);
            if (membership == null || membership.ReceiveEmails)
            {
                await
                this.mailerService.SendAsync(
                    model,
                    circle.Members.Where(m => m.User != creator && m.ReceiveEmails)
                        .Select(m => m.User)
                        .ToArray()).ConfigureAwait(false);
            }
        }

        public async Task SendEmailLendMarketPlaceItemCreatedToNeighboorhood(
            User creator,
            string portalUrl,
            MarketplaceItem marketplaceItem)
        {
            var link = this.linkGenerator.GenerateMarketplaceItemLink(marketplaceItem.MarketplaceItemId);

            var receivers = await this.databaseContext.Users.Where(u => u.Id != creator.Id && u.Position.Distance(creator.Position) < u.NeighborhoodRadius && u.Position.Distance(creator.Position) < creator.NeighborhoodRadius).ToArrayAsync().ConfigureAwait(false);

            var model = new MarketplaceLendModel
            {
                PortalUrl = portalUrl,
                ItemUrl = link,
                CreatorName = creator.Name,
                MarketplaceItemTitle = marketplaceItem.Title,
                MarketplaceItemDescription = marketplaceItem.Description,
                Subject = string.Format(Subject.CircleMarketPlaceLendItemGenerated, creator.Name, marketplaceItem.Title),
                EmailSettingsUrl = this.linkGenerator.GenerateEmailSettingsLink(),
                ExpirationDate = marketplaceItem.ExpirationDate
            };

            foreach (var receiver in receivers)
            {
                await this.SendNotificationMarketplaceItemToBorrowCreated(marketplaceItem, receiver).ConfigureAwait(false);
            }

            await
                this.mailerService.SendAsync(
                    model,
                    receivers.Where(u => u.ReceivesNewMarketplaceitemMail).ToArray()).ConfigureAwait(false);
        }

        public async Task SendEmailHelpMarketPlaceItemCreatedToNeighboorhood(
            User creator,
            string portalUrl,
            MarketplaceItem marketplaceItem)
        {
            var link = this.linkGenerator.GenerateMarketplaceItemLink(marketplaceItem.MarketplaceItemId);

            var receivers = await this.databaseContext.Users.Where(u => u.Id != creator.Id && u.Position.Distance(creator.Position) < u.NeighborhoodRadius && u.Position.Distance(creator.Position) < creator.NeighborhoodRadius).ToArrayAsync().ConfigureAwait(false);

            var model = new MarketplaceHelpModel
            {
                PortalUrl = portalUrl,
                ItemUrl = link,
                CreatorName = creator.Name,
                MarketplaceItemTitle = marketplaceItem.Title,
                MarketplaceItemDescription = marketplaceItem.Description,
                Subject = string.Format(Subject.CircleMarketPlaceHelpItemGenerated, creator.Name, marketplaceItem.Title),
                EmailSettingsUrl = this.linkGenerator.GenerateEmailSettingsLink()
            };

            foreach (var receiver in receivers)
            {
                await this.SendNotificationMarketplaceItemHelpCreated(marketplaceItem, receiver).ConfigureAwait(false);
            }

            await
            this.mailerService.SendAsync(
                model,
                receivers.Where(u => u.ReceivesNewMarketplaceitemMail).ToArray()).ConfigureAwait(false);
        }

        public async Task<List<Circle>> UserCirclesWhereUserIsOnlyAdmin(User user)
        {
            var circleIds = user.Circles.Where(c => c.IsAdministrator).Select(c => c.Circle.CircleId).ToList();
            return await this.databaseContext.Circles.Where(c => circleIds.Contains(c.CircleId) && c.Members.Count > 1 && c.Members.Count(m => m.IsAdministrator) == 1).ToListAsync();
        }

        private async Task<CircleInvitation> CreateCircleInvitationAsync(
            Circle circle,
            User invitedBy,
            User user,
            string email = null)
        {
            var circleInvitation = this.databaseContext.CircleInvitations.Create();
            circleInvitation.Circle = circle;
            circleInvitation.InvitationDateTime = DateTime.Now;
            circleInvitation.ExpireDate = DateTime.Now.AddDays(10);

            // ExpireDate set to 10 days after the invitation has been send
            circleInvitation.InvitedBy = invitedBy;
            circleInvitation.AcceptToken = this.randomGenerator.GenerateRandomToken();

            if (user != null)
            {
                circleInvitation.User = user;
            }
            else
            {
                circleInvitation.Email = email;
            }

            this.databaseContext.CircleInvitations.Add(circleInvitation);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            return circleInvitation;
        }

        private async Task<List<User>> GetAdminsAsync(Circle circle)
        {
            return await this.GetAdminsOfCircleQuery(circle).ToListAsync().ConfigureAwait(false);
        }

        private IQueryable<User> GetAdminsOfCircleQuery(Circle circle)
        {
            return
                this.databaseContext.Entry(circle)
                    .Collection(c => c.Members)
                    .Query()
                    .Where(m => m.IsAdministrator)
                    .Select(m => m.User);
        }

        private async Task<List<Job>> GetJobsAsync(
            Circle circle,
            User user,
            Expression<Func<Job, bool>> predicate,
            int limit = 0)
        {
            var query = this.databaseContext.Entry(circle).Collection(c => c.Jobs).Query().Where(predicate);

            if (!this.IsUserAdminOfCircle(circle, user))
            {
                query =
                    query.Where(
                        j => !j.IsOnlyVisibleToSelectedMembers || j.VisibleToMembers.Any(m => m.User.Id == user.Id));
            }

            query = query.Where(j => j.DueDateTime > DateTime.Now).OrderBy(j => j.DueDateTime);

            if (limit > 0)
            {
                query = query.Take(limit);
            }

            return await query.ToListAsync().ConfigureAwait(false);
        }

        private IQueryable<Circle> GetUserCirclesQuery(string userId)
        {
            return this.databaseContext.Circles.Where(c => c.Members.Any(m => m.User.Id == userId)).OrderBy(c => c.Name);
        }

        private bool IsCircleJoinRequestPending(Circle circle, User user)
        {
            return
                this.databaseContext.Entry(circle)
                    .Collection(c => c.JoinRequests)
                    .Query()
                    .Any(r => r.User.Id == user.Id);
        }

        private async Task SendEmailJobAssignedToCreator(Job job, string portalUrl)
        {
            var link = portalUrl + this.linkGenerator.GenerateCircleJobLink(job.Circle.CircleId, job.JobId);
            var model = new CircleJobAssignedModel
                            {
                                PortalUrl = portalUrl,
                                JobUrl = link,
                                Name = job.Creator.Name,
                                Assignee = job.Assignee.Name,
                                CircleName = job.Circle.Name,
                                Subject = Subject.CircleJobAssigned
                            };

            //job.Creator.ReceivesCircleJobAssigned &&
            if (job.Assignee != job.Creator)
            {
                var membership = this.GetCircleMembershipById(job.Circle.CircleId, job.Creator.Id);
                if (membership == null || membership.ReceiveEmails)
                {
                    await this.mailerService.SendAsync(model, job.Creator).ConfigureAwait(false);
                }
            }
        }

        private async Task SendEmailJobUnassigned(Job job, User unassignee, string portalUrl)
        {
            var link = portalUrl + this.linkGenerator.GenerateCircleJobAcceptLink(job.Circle.CircleId, job.JobId);
            var overviewLink = portalUrl + this.linkGenerator.GenerateCircleJobLink(job.Circle.CircleId, job.JobId);
            var model = new CircleJobUnassignedModel
            {
                PortalUrl = portalUrl,
                JobUrl = link,
                JobName = job.Title,
                JobOverviewUrl = overviewLink,
                Unassignee = unassignee.Name,
                CircleName = job.Circle.Name,
                Subject = Subject.CircleJobUnassigned
            };

            await this.mailerService.SendAsync(model, job.Circle.Members.Where(m => m.ReceiveEmails).Select(m => m.User).Where(u => u.Id != unassignee.Id).ToArray()).ConfigureAwait(false);

            //if (job.Creator.ReceivesCircleJobAssigned && job.Assignee != job.Creator)
            //{
            //    await this.mailerService.SendAsync(model, job.Creator).ConfigureAwait(false);
            //}
        }

        private async Task SendEmailMessageCreatedToCircleMembers(
            Circle circle,
            CircleMessage circleMessage,
            User creator,
            string portalUrl,
            User recipient)
        {
            var link = this.linkGenerator.GenerateCircleMessageLink(circle.CircleId, circleMessage.MessageId);
            var model = new CircleMessageCreatedModel
                            {
                                PortalUrl = portalUrl,
                                MessageUrl = link,
                                Name = recipient.Name,
                                CircleName = circle.Name,
                                Subject = Subject.MessageGenerated
                            };

            await
                this.mailerService.SendAsync(
                    model,
                    circle.Members.Where(m => m.User != creator && m.ReceiveEmails)// && m.User.ReceivesCircleMessageMail
                        .Select(m => m.User)
                        .ToArray()).ConfigureAwait(false);
        }

        private async Task SendInvitationEmailAsync(CircleInvitation invitation, string portalUrl)
        {
            var acceptInvitationUrl = this.linkGenerator.GenerateCircleAcceptInvitationLink(
                invitation.Circle.CircleId,
                invitation.AcceptToken);

            var model = new CircleInvitationModel
                            {
                                Subject = Subject.CircleInvitation,
                                AcceptInvitationUrl = acceptInvitationUrl,
                                CircleName = invitation.Circle.Name,
                                CircleDescription = invitation.Circle.Description,
                                InvitedByName = invitation.InvitedBy.Name,
                                ExpireDate = invitation.ExpireDate,
                                PortalUrl = portalUrl,
                            };

            if (invitation.User != null)
            {
                model.Name = invitation.User.Name;
                model.InvitationForUser = true;
                await this.mailerService.SendAsync(model, invitation.User).ConfigureAwait(false);
            }
            else
            {
                await this.mailerService.SendAsync(model, invitation.Email).ConfigureAwait(false);
            }
        }

        private async Task SendJobEmail(Circle circle, Job job, IEnumerable<User> users, string portalUrl)
        {
            var link = portalUrl + this.linkGenerator.GenerateCircleJobAcceptLink(circle.CircleId, job.JobId);
            var link2 = portalUrl + this.linkGenerator.GenerateCircleJobLink(circle.CircleId, job.JobId);
            var model = new CircleJobModel
                            {
                                CircleName = circle.Name,
                                JobUrl = link,
                                JobOverviewUrl = link2,
                                PortalUrl = portalUrl,
                                EmailSettingsUrl = this.linkGenerator.GenerateEmailSettingsLink(),
                                Subject = Subject.CircleJob,
                                JobTitle = job.Title,
                                JobText = job.Description,
                                HasDueTime = job.HasDueTime,
                                JobDueDateTime = job.DueDateTime,
                                JobDueDateTimeEnd = job.DueDateTimeEnd,
                                CreatedBy = job.Creator.Name
                            };
            foreach (var user in users)
            {
                //if (user.ReceivesCircleJobMail)
                //{
                    model.Name = user.Name;
                    var membership = this.GetCircleMembershipById(circle.CircleId, user.Id);
                    if (membership == null || membership.ReceiveEmails)
                    {
                        await this.mailerService.SendAsync(model, user).ConfigureAwait(false);
                    }

                //}
            }
        }

        private async Task<bool> SendNotificationMarketplaceItemToBorrowCreated(MarketplaceItem marketplaceItem, User user)
        {
            var link = this.linkGenerator.GenerateMarketplaceItemLink(marketplaceItem.MarketplaceItemId);
            var message = string.Format(Common.InterfaceText.Notification.MarketplaceItemLend, marketplaceItem.Owner.Name, marketplaceItem.Title);

            await
                this.notificationService.CreateNotificationForUserAsync(user, message, link, SettingName.MarketplaceItemToBorrowCreated)
                    .ConfigureAwait(false);

            return true;
        }

        private async Task<bool> SendNotificationMarketplaceItemHelpCreated(MarketplaceItem marketplaceItem, User user)
        {
            var link = this.linkGenerator.GenerateMarketplaceItemLink(marketplaceItem.MarketplaceItemId);
            var message = string.Format(Common.InterfaceText.Notification.MarketplaceItemHelp, marketplaceItem.Owner.Name, marketplaceItem.Title);

            await
                this.notificationService.CreateNotificationForUserAsync(user, message, link, SettingName.MarketplaceItemHelpCreated)
                    .ConfigureAwait(false);

            return true;
        }

        private async Task<bool> SendNotificationAcceptedJoinCircleRequest(CircleMembership request)
        {
            var link = this.linkGenerator.GenerateCircleAcceptedLink(request.Circle.CircleId);
            var message = string.Format(Common.InterfaceText.Notification.CircleAccepted, request.Circle.Name);

            await
                this.notificationService.CreateNotificationForUserAsync(request.User, message, link, SettingName.CircleAccepted)
                    .ConfigureAwait(false);

            return true;
        }

        private async Task SendNotificationInvitationAcceptedToCircleAdminsAsync(CircleMembership membership)
        {
            var message = string.Format(
                Common.InterfaceText.Notification.CircleInvitationAccepted,
                membership.User.Name,
                membership.Circle.Name);
            var link = this.linkGenerator.GenerateCircleMembersLink(membership.Circle.CircleId);
            var members = membership.Circle.Members.Where(m => m.User != membership.User).Where(m => m.IsAdministrator);

            await
                this.notificationService.CreateNotificationForUsersAsync(members.Select(m => m.User), message, link, SettingName.CircleInvitationAccepted)
                    .ConfigureAwait(false);
        }

        private async Task<bool> SendNotificationInviteUserAsync(User user, Circle circle)
        {
            var link = this.linkGenerator.GenerateCircleAcceptedLink(circle.CircleId);
            var message = string.Format(Common.InterfaceText.Notification.CircleInvitation, circle.Name);

            await this.notificationService.CreateNotificationForUserAsync(user, message, link, SettingName.CircleInvitation).ConfigureAwait(false);

            return true;
        }

        private async Task<bool> SendNotificationRequestRejectedAsync(User user, Circle circle)
        {
            var link = this.linkGenerator.GenerateCircleRequestRejectedLink(circle.CircleId);
            var message = string.Format(Common.InterfaceText.Notification.CircleRequestRejected, circle.Name);

            await this.notificationService.CreateNotificationForUserAsync(user, message, link, SettingName.CircleRequestRejected).ConfigureAwait(false);

            return true;
        }

        private async Task SendNotificationJobAssignedToCreator(Job job)
        {
            var link = this.linkGenerator.GenerateCircleJobLink(job.Circle.CircleId, job.JobId);
            var message = string.Format(
                Common.InterfaceText.Notification.CircleJobAssigned,
                job.Assignee.Name,
                job.Circle.Name);

            await
                this.notificationService.CreateNotificationForUserAsync(job.Creator, message, link, SettingName.CircleJobAssigned)
                    .ConfigureAwait(false);
        }

        private async Task SendNotificationJobUnassigned(Job job, User unassigner)
        {
            var link = this.linkGenerator.GenerateCircleJobLink(job.Circle.CircleId, job.JobId);
            var message = string.Format(
                Common.InterfaceText.Notification.CircleJobUnassigned,
                unassigner.Name,
                job.Circle.Name);

            await
                this.notificationService.CreateNotificationForUsersAsync(job.Circle.Members.Select(m => m.User).Where(u => u.Id != unassigner.Id), message, link, SettingName.CircleJobUnassigned)
                    .ConfigureAwait(false);
        }

        private async Task SendNotificationMessageReactionToCircleMembers(
            Circle circle,
            CircleMessageReaction circleMessageReaction,
            User user)
        {
            var link = this.linkGenerator.GenerateCircleMessageReactionLink(
                circle.CircleId,
                circleMessageReaction.ReactionId);
            var message = string.Format(Common.InterfaceText.Notification.CircleMessageReaction, user.Name, circle.Name);
            var members = circle.Members.Where(m => m.User != user).Select(m => m.User);

            await this.notificationService.CreateNotificationForUsersAsync(members, message, link, SettingName.CircleMessageReaction).ConfigureAwait(false);
        }

        private async Task SendNotificationMessageToCircleMembers(Circle circle, CircleMessage circleMessage, User user)
        {
            var message = string.Format(Common.InterfaceText.Notification.CircleMessage, user.Name, circle.Name);
            var link = this.linkGenerator.GenerateCircleMessageLink(circle.CircleId, circleMessage.MessageId);
            var members = circle.Members.Where(m => m.User != user).Select(m => m.User);

            await this.notificationService.CreateNotificationForUsersAsync(members, message, link, SettingName.CircleMessage).ConfigureAwait(false);
        }

        private async Task SendNotificationNewCirclePhoto(Circle circle, User user)
        {
            var link = this.linkGenerator.GenerateCirclePhotoLink(circle.CircleId);
            var message = string.Format(Common.InterfaceText.Notification.CircleNewPhoto, user.Name, circle.Name);

            await
                this.notificationService.CreateNotificationForUsersAsync(
                    circle.Members.Where(m => m.User != user).Select(m => m.User),
                    message,
                    link,
                    SettingName.CircleNewPhoto).ConfigureAwait(false);
        }

        private async Task SendNotificationNewCircleDoc(Circle circle, User user)
        {
            var link = this.linkGenerator.GenerateCircleDocLink(circle.CircleId);
            var message = string.Format(Common.InterfaceText.Notification.CircleNewDoc, user.Name, circle.Name);

            await
                this.notificationService.CreateNotificationForUsersAsync(
                    circle.Members.Where(m => m.User != user).Select(m => m.User),
                    message,
                    link,
                    SettingName.CircleNewDoc).ConfigureAwait(false);
        }

        private async Task SendNotificationNewJobToCircleMembers(Circle circle, Job job, User user)
        {
            var link = this.linkGenerator.GenerateCircleJobLink(circle.CircleId, job.JobId);
            var message = string.Format(Common.InterfaceText.Notification.CircleNewJob, circle.Name);

            // Notification for all members who can see the notification (except the creator)
            var members = circle.Members.Where(m => m.User != user);
            if (job.IsOnlyVisibleToSelectedMembers)
            {
                members = members.Where(m => job.VisibleToMembers.Contains(m));
            }

            await
                this.notificationService.CreateNotificationForUsersAsync(members.Select(m => m.User), message, link, SettingName.CircleNewJob)
                    .ConfigureAwait(false);
        }

        private async Task<bool> ShouldInviteEmail(Circle circle, string emailAddress)
        {
            var emailIsInvited =
                await
                this.databaseContext.CircleInvitations.AnyAsync(
                    i => i.Circle.CircleId == circle.CircleId && i.Email == emailAddress).ConfigureAwait(false);

            return !emailIsInvited;
        }

        private async Task<bool> ShouldInviteUser(Circle circle, User user)
        {
            var userEntry = this.databaseContext.Entry(user);
            var userIsMember =
                await
                userEntry.Collection(c => c.Circles)
                    .Query()
                    .AnyAsync(c => c.Circle.CircleId == circle.CircleId)
                    .ConfigureAwait(false);
            var userIsInvited =
                await
                userEntry.Collection(c => c.CircleInvitations)
                    .Query()
                    .AnyAsync(c => c.Circle.CircleId == circle.CircleId)
                    .ConfigureAwait(false);

            return !userIsMember && !userIsInvited;
        }
    }
}