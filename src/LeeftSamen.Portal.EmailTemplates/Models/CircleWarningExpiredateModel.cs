// <copyright file="CircleWarningExpiredateModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.EmailTemplates.Models
{
    using System;

    /// <summary>
    /// The circle warning invitation expire model.
    /// </summary>
    public class CircleWarningExpiredateModel : BaseModel
    {
        /// <summary>
        /// Gets or sets the circle name.
        /// </summary>
        public string CircleName { get; set; }

        /// <summary>
        /// Gets or sets the Expiredate
        /// </summary>
        public DateTime ExpireDate { get; set; }

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
                return "CircleWarningExpiredate";
            }
        }

        /// <summary>
        /// Gets or sets the accept url
        /// </summary>
        public string AcceptInvitationUrl { get; set; }
    }
}
