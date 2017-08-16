// <copyright file="BaseModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Web.Configuration;

namespace LeeftSamen.Portal.EmailTemplates.Models
{
    /// <summary>
    /// The base model.
    /// </summary>
    public abstract class BaseModel : IEmailTemplateModel
    {
        /// <summary>
        /// Gets or sets the portal url.
        /// </summary>
        public string PortalUrl { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets the template name.
        /// </summary>
        public abstract string TemplateName { get; }

        /// <summary>
        /// Gets or sets the email settings url.
        /// </summary>
        public string EmailSettingsUrl { get; set; }

        public string Receiver { get; set; }

        public string Company
        {
            get
            {
                return WebConfigurationManager.AppSettings["Company"];
            }
        }
    }
}