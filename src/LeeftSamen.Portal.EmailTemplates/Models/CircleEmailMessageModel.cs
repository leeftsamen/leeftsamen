// <copyright file="CircleEmailMessageModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.EmailTemplates.Models
{
    public class CircleEmailMessageModel : BaseModel
    {
        /// <summary>
        /// Gets or sets the accept invitation url.
        /// </summary>
        public string UrlToGoToWebsite { get; set; }

        /// <summary>
        /// Gets or sets the circle name.
        /// </summary>
        public string CircleName { get; set; }

        /// <summary>
        /// Gets or sets the invited by name.
        /// </summary>
        public string InvitedByName { get; set; }

        /// <summary>
        /// Gets or sets the receiver name.
        /// </summary>
        public string ReceiverName { get; set; }

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
                return "CircleEmailMessage";
            }
        }
    }
}
