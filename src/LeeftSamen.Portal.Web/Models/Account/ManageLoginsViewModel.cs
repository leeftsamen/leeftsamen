// <copyright file="ManageLoginsViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Account
{
    using System.Collections.Generic;

    using Microsoft.AspNet.Identity;

    /// <summary>
    /// The manage logins view model.
    /// </summary>
    public class ManageLoginsViewModel
    {
        /// <summary>
        /// Gets or sets the current logins.
        /// </summary>
        public IList<UserLoginInfo> CurrentLogins { get; set; }
    }
}