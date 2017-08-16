// <copyright file="MessageDetailViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.NeighborhoodMessages
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    using LeeftSamen.Portal.Data.Models;

    /// <summary>
    /// The message detail view model.
    /// </summary>
    public class MessageDetailViewModel
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public NeighborhoodMessage Message { get; set; }

        /// <summary>
        /// Gets or sets the message type.
        /// </summary>
        public NeighborhoodMessage.MessageTypes MessageType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user can edit message.
        /// </summary>
        public bool UserCanEditMessage { get; set; }

        public ICollection<NeighborhoodMessageReaction> Reactions { get; set; }

        [Required]
        [AllowHtml]
        public string NewReaction { get; set; }

        public bool UserCanPinMessage { get; set; }

        public string UserRole { get; set; }
    }
}