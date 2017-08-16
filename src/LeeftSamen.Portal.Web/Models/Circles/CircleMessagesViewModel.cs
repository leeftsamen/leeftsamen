// <copyright file="CircleMessagesViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Circles
{
    using System;
    using System.Web;

    using LeeftSamen.Portal.Data.Models;
    using System.ComponentModel.DataAnnotations;
    using Common.InterfaceText;    /// <summary>
                                   /// The circle messages view model.
                                   /// </summary>
    public class CircleMessagesViewModel
    {
        public int MessageId { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public CircleMessage Message { get; set; }

        public DateTime CreationDateTime { get; set; }

        public DateTime? LatestReactionDateTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the current user is the creator of the message
        /// </summary>
        public bool IsCreator { get; set; }

        /// <summary>
        /// Gets or sets the new reaction.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "MessageTextRequired")]
        public string NewReaction { get; set; }

        public HttpPostedFileBase Attachment { get; set; }

        public int? ScrollToReactionId { get; set; }
    }
}