// <copyright file="IEmailTemplateModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.EmailTemplates.Models
{
    /// <summary>
    /// The EmailTemplateModel interface.
    /// </summary>
    public interface IEmailTemplateModel
    {
        /// <summary>
        /// Gets or sets the portal url.
        /// </summary>
        string PortalUrl { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        string Subject { get; set; }

        /// <summary>
        /// Gets the template name.
        /// </summary>
        string TemplateName { get; }

        string Company { get; }

        string Receiver { get; set; }

        string EmailSettingsUrl { get; set; }
    }
}