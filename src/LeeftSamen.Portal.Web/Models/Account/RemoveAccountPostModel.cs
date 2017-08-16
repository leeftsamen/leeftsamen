// <copyright file="RemoveAccountPostModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    using LeeftSamen.Common.InterfaceText;
    using Attributes;

    public class RemoveAccountPostModel
    {
        // Required does not work here. Previously it was a [Range(typeof(bool), "true", "true"] but it did not work properly
        // We created a custom Mandatory Attribute which does work. We also enabled clientside validation by adding:
        //
        //      (function ($) { $.validator.unobtrusive.adapters.addBool("mandatory", "required"); }(jQuery));
        //
        // to ~\Scripts\leeftsamen.js
        [Mandatory(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "ConfirmRemoveAccount")]
        [Display(ResourceType = typeof(Label), Name = "ConfirmRemoveAccount")]
        public bool RemovalConfirmation { get; set; }
    }
}