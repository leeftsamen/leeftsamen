// <copyright file="OrganizationInvitationModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.EmailTemplates.Models
{
    /// <summary>
    /// The organization invitation model.
    /// </summary>
    public class OrganizationInvitationModel : BaseModel
    {
        /// <summary>
        /// Gets or sets the accept invitation url.
        /// </summary>
        public string AcceptInvitationUrl { get; set; }

        /// <summary>
        /// Gets or sets the organization name.
        /// </summary>
        public string OrganizationName { get; set; }

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
                return "OrganizationInvitation";
            }
        }
    }
}