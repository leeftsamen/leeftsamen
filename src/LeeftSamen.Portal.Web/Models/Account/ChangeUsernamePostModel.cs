// <copyright file="ChangeUsernamePostModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    using LeeftSamen.Common.InterfaceText;

    public class ChangeUsernamePostModel
    {
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "YourUsernameIsRequired")]
        [Display(ResourceType = typeof(Label), Name = "YourUsername")]
        public string Username { get; set; }
    }
}