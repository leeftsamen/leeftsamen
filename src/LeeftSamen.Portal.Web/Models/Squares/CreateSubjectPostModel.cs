// <copyright file="CreateSubjectPostModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using LeeftSamen.Common.InterfaceText;

namespace LeeftSamen.Portal.Web.Models.Squares
{
    public class CreateSubjectPostModel
    {
        [Display(ResourceType = typeof(Label), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "TitleIsRequired")]
        [MaxLength(40, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Error))]
        public string Title { get; set; }

        [Display(ResourceType = typeof(Label), Name = "Description")]
        //[Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "DescriptionIsRequired")]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Error))]
        public string Description { get; set; }
    }
}