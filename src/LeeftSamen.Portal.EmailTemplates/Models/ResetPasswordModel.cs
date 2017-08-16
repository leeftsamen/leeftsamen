// <copyright file="ResetPasswordModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.EmailTemplates.Models
{
    /// <summary>
    /// The reset password model.
    /// </summary>
    public class ResetPasswordModel : BaseModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the reset password url.
        /// </summary>
        public string ResetPasswordUrl { get; set; }

        /// <summary>
        /// Gets the template name.
        /// </summary>
        public override string TemplateName
        {
            get
            {
                return "ResetPassword";
            }
        }
    }
}