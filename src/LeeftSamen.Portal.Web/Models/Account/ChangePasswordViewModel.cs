// <copyright file="ChangePasswordViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    using LeeftSamen.Common.InterfaceText;

    /// <summary>
    /// The change password view model.
    /// </summary>
    public class ChangePasswordViewModel
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
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Label), Name = "NewPassword")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "NewPasswordIsRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "PasswordLength",
            MinimumLength = 8)]
        [RegularExpression("^(?=.*[A-Z])(?=.*[0-9]).{8,}$", ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "PasswordStrength")]
        public string NewPassword { get; set; }

        /// <summary>
        /// Gets or sets the old password.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Label), Name = "CurrentPassword")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "CurrentPasswordIsRequired")]
        public string OldPassword { get; set; }
    }
}