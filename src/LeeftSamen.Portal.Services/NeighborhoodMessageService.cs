// <copyright file="NeighborhoodMessageService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data;
    using LeeftSamen.Portal.Data.Migrations;
    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.Services.DTO;

    public class NeighborhoodMessageService : INeighborhoodMessageService
    {
        private readonly IApplicationDbContext databaseContext;

        private readonly IMediaService mediaService;

        private readonly INotificationService notificationService;

        private readonly ILinkGenerator linkGenerator;

        public NeighborhoodMessageService(IApplicationDbContext databaseContext, IMediaService mediaService, INotificationService notificationService, ILinkGenerator linkGenerator)
        {
            this.databaseContext = databaseContext;
            this.mediaService = mediaService;
            this.notificationService = notificationService;
            this.linkGenerator = linkGenerator;
        }

        public async Task<NeighborhoodMessageReaction> GetReactionByIdAsync(int reactionId)
        {
            return
                await
                this.databaseContext.NeighborhoodMessageReactions.FirstOrDefaultAsync(r => r.ReactionId == reactionId)
                    .ConfigureAwait(false);
        }

        public async Task<NeighborhoodMessage> CreateNeighborhoodMessageAsync(
            string title,
            string introductionText,
            string fullText,
            User creator,
            OrganizationMembership organizationMembership,
            DateTime? expirationDateTime,
            ImageDto image1,
            ImageDto image2,
            ImageDto image3,
            ImageDto image4,
            ImageDto image5,
            Media file,
            bool allowSharing)
        {
            var message = this.databaseContext.NeighborhoodMessages.Create();
            message.Title = title;
            message.IntroductionText = introductionText;
            message.FullText = fullText;
            message.Creator = creator;
            message.Position = creator.Position;
            message.OrganizationMembership = organizationMembership;
            message.CreationDateTime = DateTime.Now;
            message.ExpirationDateTime = expirationDateTime;
            message.AllowSharing = allowSharing;

            if (image1 != null)
            {
                message.Image1 = this.mediaService.CreateMedia(image1);
            }

            if (image2 != null)
            {
                message.Image2 = this.mediaService.CreateMedia(image2);
            }

            if (image3 != null)
            {
                message.Image3 = this.mediaService.CreateMedia(image3);
            }

            if (image4 != null)
            {
                message.Image4 = this.mediaService.CreateMedia(image4);
            }

            if (image5 != null)
            {
                message.Image5 = this.mediaService.CreateMedia(image5);
            }

            if (file != null)
            {
                message.File1 = file;
            }

            this.databaseContext.NeighborhoodMessages.Add(message);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            return message;
        }

        public async Task<List<NeighborhoodMessage>> GetAllAssociationMessagesAsync(DbGeography position, int radius)
        {
            return
                await
                this.databaseContext.NeighborhoodMessages.Where(
                    m =>
                    m.Position.Distance(position) <= radius && m.OrganizationMembershipId != null
                    && m.OrganizationMembership.Organization.OrganizationType.Type == OrganizationType.Types.Association && (m.ExpirationDateTime == null || m.ExpirationDateTime > DateTime.Now))
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<List<NeighborhoodMessage>> GetAllMessagesAsync(DbGeography position, int radius)
        {
            return
                await
                this.databaseContext.NeighborhoodMessages.Include(m => m.Creator)
                    .Include(m => m.OrganizationMembership)
                    .Include(m => m.OrganizationMembership.Organization)
                    .Where(
                        m =>
                        m.Position.Distance(position) <= radius
                        && (m.ExpirationDateTime == null || m.ExpirationDateTime > DateTime.Now))
                    .OrderByDescending(i => i.IsPinned)
                    .ThenByDescending(i => i.CreationDateTime)
                    .Take(150)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<List<NeighborhoodMessage>> GetAllNeighborMessagesAsync(DbGeography position, int radius)
        {
            return
                await
                this.databaseContext.NeighborhoodMessages.Where(
                    m => m.Position.Distance(position) <= radius && m.OrganizationMembershipId == null && (m.ExpirationDateTime == null || m.ExpirationDateTime > DateTime.Now))
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<List<NeighborhoodMessage>> GetAllOrganizationMessagesAsync(DbGeography position, int radius)
        {
            return
                await
                this.databaseContext.NeighborhoodMessages.Where(
                    m =>
                    m.Position.Distance(position) <= radius && m.OrganizationMembershipId != null
                    && (m.OrganizationMembership.Organization.OrganizationType.Type
                        == OrganizationType.Types.Professional
                        || m.OrganizationMembership.Organization.OrganizationType.Type
                        == OrganizationType.Types.Volunteer) && (m.ExpirationDateTime == null || m.ExpirationDateTime > DateTime.Now)).ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<NeighborhoodMessage>> GetLatestMessagesByOrganizationIdAsync(
            int organizationId,
            int maxCount = 25)
        {
            return
                await
                this.databaseContext.NeighborhoodMessages.Where(
                    m => m.OrganizationMembership.Organization.OrganizationId == organizationId)
                    .OrderByDescending(m => m.CreationDateTime)
                    .Take(maxCount)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<NeighborhoodMessage> GetMessageByIdAsync(int? messageId)
        {
            return
                await
                this.databaseContext.NeighborhoodMessages.FirstOrDefaultAsync(m => m.MessageId == messageId)
                    .ConfigureAwait(false);
        }

        public async Task<Media> GetMessageImageAsync(int? messageId, int? mediaId)
        {
            var message = await
                this.databaseContext.NeighborhoodMessages.Where(m => m.MessageId == messageId && (m.Image1Id == mediaId || m.Image2Id == mediaId || m.Image3Id == mediaId || m.Image4Id == mediaId || m.Image5Id == mediaId))
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

            if (message.Image1Id == mediaId)
            {
                return message.Image1;
            }

            if (message.Image2Id == mediaId)
            {
                return message.Image2;
            }

            if (message.Image3Id == mediaId)
            {
                return message.Image3;
            }

            if (message.Image4Id == mediaId)
            {
                return message.Image4;
            }

            if (message.Image5Id == mediaId)
            {
                return message.Image5;
            }

            return null;
        }

        public async Task RemoveNeighborhoodMessageAsync(NeighborhoodMessage message)
        {
            this.databaseContext.NeighborhoodMessages.Remove(message);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<NeighborhoodMessage> UpdateNeighborhoodMessageAsync(
            NeighborhoodMessage message,
            string title,
            string introductionText,
            string fullText,
            DateTime? expirationDateTime,
            ImageDto image1,
            ImageDto image2,
            ImageDto image3,
            ImageDto image4,
            ImageDto image5,
            Media file,
            bool allowSharing)
        {
            message.Title = title;
            message.IntroductionText = introductionText;
            message.FullText = fullText;
            message.ExpirationDateTime = expirationDateTime;
            message.AllowSharing = allowSharing;

            if (image1 != null)
            {
                if (message.Image1Id.HasValue)
                {
                    this.databaseContext.Media.Remove(message.Image1);
                }

                message.Image1 = this.mediaService.CreateMedia(image1);
            }

            if (image2 != null)
            {
                if (message.Image2Id.HasValue)
                {
                    this.databaseContext.Media.Remove(message.Image2);
                }

                message.Image2 = this.mediaService.CreateMedia(image2);
            }

            if (image3 != null)
            {
                if (message.Image3Id.HasValue)
                {
                    this.databaseContext.Media.Remove(message.Image3);
                }

                message.Image3 = this.mediaService.CreateMedia(image3);
            }

            if (image4 != null)
            {
                if (message.Image4Id.HasValue)
                {
                    this.databaseContext.Media.Remove(message.Image4);
                }

                message.Image4 = this.mediaService.CreateMedia(image4);
            }

            if (image5 != null)
            {
                if (message.Image5Id.HasValue)
                {
                    this.databaseContext.Media.Remove(message.Image5);
                }

                message.Image5 = this.mediaService.CreateMedia(image5);
            }

            if (file != null)
            {
                if (message.File1Id.HasValue)
                {
                    this.databaseContext.Media.Remove(message.File1);
                }

                message.File1 = file;
            }

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            return message;
        }

        public async Task<NeighborhoodMessageReaction> CreateReactionAsync(NeighborhoodMessage message, User creator, OrganizationMembership organizationMembership, string text)
        {
            var reaction = this.databaseContext.NeighborhoodMessageReactions.Create();
            reaction.CreationDateTime = DateTime.Now;
            reaction.Creator = creator;
            reaction.NeighborhoodMessage = message;
            reaction.Text = text;
            reaction.OrganizationMembership = organizationMembership;

            this.databaseContext.NeighborhoodMessageReactions.Add(reaction);

            if (creator.Id != message.Creator.Id)
            {
                await this.notificationService.CreateNotificationForUserAsync(
                    message.Creator,
                    string.Format(Common.InterfaceText.Notification.NeighborhoodMessageReaction, creator.Name, message.Title),
                    this.linkGenerator.GenerateNeighborhoodMessageLink(message.MessageId, message.GetMessageType()),
                    Data.Enums.SettingName.NeighborhoodMessageReaction).ConfigureAwait(false);
            }

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            return reaction;
        }

        public async Task RemoveReactionAsync(NeighborhoodMessageReaction reaction)
        {
            this.databaseContext.NeighborhoodMessageReactions.Remove(reaction);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task PinMessageAsync(int messageId, bool pin)
        {
            var message = await this.databaseContext.NeighborhoodMessages.FirstOrDefaultAsync(m => m.MessageId == messageId).ConfigureAwait(false);
            if (message == null)
            {
                return;
            }

            message.IsPinned = pin;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}