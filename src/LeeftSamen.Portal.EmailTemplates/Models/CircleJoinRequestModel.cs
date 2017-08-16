// <copyright file="CircleJoinRequestModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.EmailTemplates.Models
{
    /// <summary>
    /// The circle join request model.
    /// </summary>
    public class CircleJoinRequestModel : BaseModel
    {
        /// <summary>
        /// Gets or sets the accept join request url.
        /// </summary>
        public string AcceptJoinRequestUrl { get; set; }

        /// <summary>
        /// Gets or sets the circle name.
        /// </summary>
        public string CircleName { get; set; }

        /// <summary>
        /// Gets the template name.
        /// </summary>
        public override string TemplateName
        {
            get
            {
                return "CircleJoinRequest";
            }
        }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName { get; set; }
    }
}