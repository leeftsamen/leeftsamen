// <copyright file="ChangeZuiderlingSettingsViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    using LeeftSamen.Common.InterfaceText;

    /// <summary>
    /// The change zuiderling view model.
    /// </summary>
    public class ChangeZuiderlingSettingsViewModel
    {
        public bool AccountVerified { get; set; }

        //Name changed to prevent chrome autofill
        [DataType(DataType.Text)]
        [Display(ResourceType = typeof(Label), Name = "ZuiderlingAccount")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "ZuiderlingAccountIsRequired")]
        public string ZA { get; set; }

        //Name changed to prevent chrome autofill
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Label), Name = "ZuiderlingPassword")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "ZuiderlingPasswordIsRequired")]
        public string ZP { get; set; }
    }
}