// <copyright file="ChangeNamePostModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    using LeeftSamen.Common.InterfaceText;

    public class ChangeNamePostModel
    {
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "YourNameIsRequired")]
        [Display(ResourceType = typeof(Label), Name = "YourName")]
        public string Name { get; set; }
    }
}