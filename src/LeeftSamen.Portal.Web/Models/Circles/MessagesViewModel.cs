// <copyright file="MessagesViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Circles
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using System.Web.Mvc;

    using LeeftSamen.Portal.Data.Models;
    using Common.InterfaceText;
    /// <summary>
    /// The detail messages view model.
    /// </summary>
    public class MessagesViewModel
    {
        /// <summary>
        /// Gets or sets the circle id.
        /// </summary>
        public int CircleId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether circle is private.
        /// </summary>
        public bool CircleIsPrivate { get; set; }

        /// <summary>
        /// Gets or sets the circle name.
        /// </summary>
        public string CircleName { get; set; }

        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        public ICollection<CircleMessagesViewModel> Messages { get; set; }

        /// <summary>
        /// Gets or sets the new message.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "MessageTextRequired")]
        [AllowHtml]
        public string NewMessage { get; set; }

        public HttpPostedFileBase Attachment { get; set; }

        public int? ScrollToReactionId { get; set; }
    }
}