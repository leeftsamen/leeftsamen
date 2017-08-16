// <copyright file="CircleInvitationModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.EmailTemplates.Models
{
    using System;

    /// <summary>
    /// The circle invitation model.
    /// </summary>
    public class CircleInvitationModel : BaseModel
    {
        /// <summary>
        /// Gets or sets the accept invitation url.
        /// </summary>
        public string AcceptInvitationUrl { get; set; }

        /// <summary>
        /// Gets or sets the circle name.
        /// </summary>
        public string CircleName { get; set; }

        /// <summary>
        /// Gets or sets the circle description.
        /// </summary>
        public string CircleDescription { get; set; }

        /// <summary>
        /// Gets or sets the invited by name.
        /// </summary>
        public string InvitedByName { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the template name.
        /// </summary>
        public override string TemplateName
        {
            get
            {
                return "CircleInvitation";
            }
        }

        public bool InvitationForUser { get; set; }

        /// <summary>
        /// Gets or Sets the ExpireDate
        /// </summary>
        public DateTime ExpireDate { get; set; }
    }
}