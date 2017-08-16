// <copyright file="CreateFactPostModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeeftSamen.Common.InterfaceText;

namespace LeeftSamen.Portal.Web.Models.Squares
{
    public class CreateFactPostModel
    {
        [Display(ResourceType = typeof(Label), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "TitleIsRequired")]
        [MaxLength(40, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Error))]
        public string Title { get; set; }

        [Display(ResourceType = typeof(Label), Name = "IntroductionText")]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Error))]
        public string IntroductionText { get; set; }

        [Display(ResourceType = typeof(Label), Name = "FullText")]
        [AllowHtml]
        public string FullText { get; set; }
    }
}