// <copyright file="RegisterAccountViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Register
{
    using System.ComponentModel.DataAnnotations;

    using LeeftSamen.Common.InterfaceText;

    public class RegisterAccountViewModel
    {
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Label), Name = "ConfirmPassword")]
        [Compare("Password", ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "PasswordsDoNotMatch")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "EmailAddressIsRequired")]
        [EmailAddress(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "EmailAddressIsRequired",
            ErrorMessage = null)]
        [Display(ResourceType = typeof(Label), Name = "RegisterEmail")]
        public string Email { get; set; }

        [Display(ResourceType = typeof(Label), Name = "RegisterName")]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "PasswordIsRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "PasswordLength", MinimumLength = 8)]
        [RegularExpression("^(?=.*[A-Z])(?=.*[0-9]).{8,}$", ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "PasswordStrength")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Label), Name = "RegisterPassword")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(Label), Name = "AcceptTermsName")]
        public bool AcceptTerms { get; set; }
    }
}