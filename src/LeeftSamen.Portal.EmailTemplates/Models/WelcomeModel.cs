// <copyright file="WelcomeModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.EmailTemplates.Models
{
    /// <summary>
    /// The welcome model.
    /// </summary>
    public class WelcomeModel : BaseModel
    {
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
                return "Welcome";
            }
        }

        /// <summary>
        /// Gets or sets the confirm email url.
        /// </summary>
        public string ConfirmEmailUrl { get; set; }
    }
}