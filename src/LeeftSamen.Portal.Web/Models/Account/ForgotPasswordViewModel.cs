// <copyright file="ForgotPasswordViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    using LeeftSamen.Common.InterfaceText;

    /// <summary>
    /// The forgot password view model.
    /// </summary>
    public class ForgotPasswordViewModel
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [EmailAddress(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "EmailAddressIsInvalid", ErrorMessage = null)]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "EmailAddressIsRequired")]
        [Display(ResourceType = typeof(Label), Name = "Email")]
        public string Email { get; set; }
    }
}