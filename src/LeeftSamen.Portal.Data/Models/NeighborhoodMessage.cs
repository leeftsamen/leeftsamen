// <copyright file="NeighborhoodMessage.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Spatial;

    public class NeighborhoodMessage
    {
        public const int ImageCount = 1;

        public enum MessageTypes
        {
            /// <summary>
            /// The neighbor messages.
            /// </summary>
            NeighborMessages,

            /// <summary>
            /// The organization messages.
            /// </summary>
            OrganizationMessages,

            /// <summary>
            /// The association messages.
            /// </summary>
            AssociationMessages,

            /// <summary>
            /// The any.
            /// </summary>
            Any
        }

        public virtual bool AllowSharing { get; set; }

        public virtual DateTime CreationDateTime { get; set; }

        public virtual User Creator { get; set; }

        public virtual DbGeography Position { get; set; }

        public virtual DateTime? ExpirationDateTime { get; set; }

        public virtual string FullText { get; set; }

        public virtual Media Image1 { get; set; }

        public virtual int? Image1Id { get; set; }

        public virtual Media Image2 { get; set; }

        public virtual int? Image2Id { get; set; }

        public virtual Media Image3 { get; set; }

        public virtual int? Image3Id { get; set; }

        public virtual Media Image4 { get; set; }

        public virtual int? Image4Id { get; set; }

        public virtual Media Image5 { get; set; }

        public virtual int? Image5Id { get; set; }

        public virtual Media File1 { get; set; }

        public virtual int? File1Id { get; set; }

        public virtual string IntroductionText { get; set; }

        public virtual int MessageId { get; set; }

        public virtual bool IsPinned { get; set; }

        public virtual OrganizationMembership OrganizationMembership { get; set; }

        public virtual int? OrganizationMembershipId { get; set; }

        public virtual ICollection<NeighborhoodMessageReaction> Reactions { get; set; }

        public virtual string Title { get; set; }

        public MessageTypes GetMessageType()
        {
            var messageType = NeighborhoodMessage.MessageTypes.NeighborMessages;
            if (this.OrganizationMembershipId.HasValue)
            {
                switch (this.OrganizationMembership.Organization.OrganizationType.Type)
                {
                    case OrganizationType.Types.Association:
                        messageType = NeighborhoodMessage.MessageTypes.AssociationMessages;
                        break;
                    default:
                        messageType = NeighborhoodMessage.MessageTypes.OrganizationMessages;
                        break;
                }
            }

            return messageType;
        }
    }
}