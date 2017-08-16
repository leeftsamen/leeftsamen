// <copyright file="LoginViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    using LeeftSamen.Common.InterfaceText;

    /// <summary>
    /// The login view model.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Display(ResourceType = typeof(Label), Name = "Email")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "EmailAddressIsRequired")]
        [EmailAddress(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "EmailAddressIsRequired", ErrorMessage = null)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "PasswordIsRequired")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Label), Name = "Password")]
        public string Password { get; set; }
    }
}