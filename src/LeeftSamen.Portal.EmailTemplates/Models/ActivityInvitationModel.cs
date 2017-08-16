// <copyright file="ActivityInvitationModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.EmailTemplates.Models
{
    /// <summary>
    /// The circle invitation model.
    /// </summary>
    public class ActivityInvitationModel : BaseModel
    {
        /// <summary>
        /// Gets or sets the accept invitation url.
        /// </summary>
        public string AcceptInvitationUrl { get; set; }

        /// <summary>
        /// Gets or sets the view circle url.
        /// </summary>
        public string ViewInvitationUrl { get; set; }

        /// <summary>
        /// Gets or sets the circle name.
        /// </summary>
        public string ActivityTitle { get; set; }

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
                return "ActivityInvitation";
            }
        }
    }
}