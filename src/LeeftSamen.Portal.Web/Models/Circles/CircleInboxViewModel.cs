// <copyright file="CircleInboxViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Data.Models;
using System.Collections.Generic;

namespace LeeftSamen.Portal.Web.Models.Circles
{
    public class CircleInboxViewModel
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
        /// Gets or sets Email messages.
        /// </summary>
        public ICollection<CircleEmailMessage> EmailMessages { get; set; }

        /// <summary>
        /// Gets or sets the subject text.
        /// </summary>
        public string subjectText { get; set; }

        /// <summary>
        /// Gets or sets the text of the message.
        /// </summary>
        public string messageText { get; set; }

        public string CurrentUserId { get; set; }
    }
}