// <copyright file="ChangeMemberProfileModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Common.InterfaceText;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Circles
{
    public class ChangeMemberProfileModel
    {
        public int CircleId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "DescriptionIsRequired")]
        [Display(ResourceType = typeof(Label), Name = "CircleProfile")]
        public string ProfileText { get; set; }
    }
}