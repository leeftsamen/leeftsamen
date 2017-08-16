// <copyright file="EditSubjectModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Common.InterfaceText;
using LeeftSamen.Portal.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeeftSamen.Portal.Web.Models.Forums
{
    public class EditSubjectModel
    {
        public string Type { get; set; }

        public int TypeId { get; set; }

        public int SubjectId { get; set; }

        [Display(ResourceType = typeof(Label), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "TitleIsRequired")]
        [MaxLength(40, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Error))]
        public string Title { get; set; }

        [Display(ResourceType = typeof(Label), Name = "SubTitle")]
        //[Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "DescriptionIsRequired")]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Error))]
        public string Description { get; set; }
    }
}