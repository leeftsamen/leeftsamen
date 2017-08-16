// <copyright file="INeighborhoodMessageService.cs" company="LeeftSamen B.V.">
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

    public interface INeighborhoodMessageService
    {
        Task<NeighborhoodMessageReaction> GetReactionByIdAsync(int reactionId);

        Task<NeighborhoodMessage> CreateNeighborhoodMessageAsync(
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
            bool allowSharing);

        Task PinMessageAsync(int messageId, bool pin);

        Task<List<NeighborhoodMessage>> GetAllAssociationMessagesAsync(DbGeography position, int radius);

        Task<List<NeighborhoodMessage>> GetAllMessagesAsync(DbGeography position, int radius);

        Task<List<NeighborhoodMessage>> GetAllNeighborMessagesAsync(DbGeography position, int radius);

        Task<List<NeighborhoodMessage>> GetAllOrganizationMessagesAsync(DbGeography position, int radius);

        Task<List<NeighborhoodMessage>> GetLatestMessagesByOrganizationIdAsync(int organizationId, int maxCount = 25);

        Task<NeighborhoodMessage> GetMessageByIdAsync(int? messageId);

        Task<Media> GetMessageImageAsync(int? messageId, int? mediaId);

        Task RemoveNeighborhoodMessageAsync(NeighborhoodMessage message);

        Task<NeighborhoodMessage> UpdateNeighborhoodMessageAsync(
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
            bool allowSharing);

        Task<NeighborhoodMessageReaction> CreateReactionAsync(NeighborhoodMessage message, User creator, OrganizationMembership organizationMembership, string text);

        Task RemoveReactionAsync(NeighborhoodMessageReaction reaction);
    }
}