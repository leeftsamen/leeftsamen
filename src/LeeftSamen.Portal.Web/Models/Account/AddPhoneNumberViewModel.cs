// <copyright file="AddPhoneNumberViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    using LeeftSamen.Common.InterfaceText;

    /// <summary>
    /// The add phone number view model.
    /// </summary>
    public class AddPhoneNumberViewModel
    {
        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        [Required]
        [Phone]
        [Display(ResourceType = typeof(Label), Name = "PhoneNumber")]
        public string Number { get; set; }
    }
}