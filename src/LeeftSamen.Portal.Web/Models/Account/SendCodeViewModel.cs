// <copyright file="SendCodeViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Account
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    /// <summary>
    /// The send code view model.
    /// </summary>
    public class SendCodeViewModel
    {
        /// <summary>
        /// Gets or sets the providers.
        /// </summary>
        public ICollection<SelectListItem> Providers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remember me.
        /// </summary>
        public bool RememberMe { get; set; }

        /// <summary>
        /// Gets or sets the return url.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Gets or sets the selected provider.
        /// </summary>
        public string SelectedProvider { get; set; }
    }
}