// <copyright file="ResetPasswordViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    using LeeftSamen.Common.InterfaceText;

    /// <summary>
    /// The reset password view model.
    /// </summary>
    public class ResetPasswordViewModel
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the confirm password.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Label), Name = "ConfirmNewPassword")]
        [Compare("Password", ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "PasswordsDoNotMatch")
        ]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Label), Name = "Password")]
        [StringLength(100, ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "PasswordLength",
            MinimumLength = 6)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public string UserId { get; set; }
    }
}