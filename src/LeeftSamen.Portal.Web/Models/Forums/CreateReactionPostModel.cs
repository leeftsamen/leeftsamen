// <copyright file="CreateReactionPostModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeeftSamen.Common.InterfaceText;

namespace LeeftSamen.Portal.Web.Models.Forums
{
    public class CreateReactionPostModel
    {
        [Display(ResourceType = typeof(Label), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "TitleIsRequired")]
        public string Title { get; set; }

        [Display(ResourceType = typeof(Label), Name = "Reaction")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "ReactionIsRequired")]
        [AllowHtml]
        public string Text { get; set; }
    }
}