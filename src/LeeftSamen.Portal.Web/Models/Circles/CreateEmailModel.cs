// <copyright file="CreateEmailModel.cs" company="LeeftSamen B.V.">
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
    public class CreateEmailModel
    {
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "MessageTextRequired")]
        [Display(ResourceType = typeof(Label), Name = "MessageText")]
        public string messageText { get; set; }
    }
}