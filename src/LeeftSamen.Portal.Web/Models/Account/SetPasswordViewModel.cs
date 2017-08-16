// <copyright file="SetPasswordViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    using LeeftSamen.Common.InterfaceText;

    /// <summary>
    /// The set password view model.
    /// </summary>
    public class SetPasswordViewModel
    {
        /// <summary>
        /// Gets or sets the confirm password.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Label), Name = "ConfirmNewPassword")]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(Error),
            ErrorMessageResourceName = "NewPasswordsDoNotMatch")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets the new password.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "PasswordLength",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Label), Name = "NewPassword")]
        public string NewPassword { get; set; }
    }
}