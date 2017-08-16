// <copyright file="ChangeGeneralModel.cs" company="LeeftSamen B.V.">
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

namespace LeeftSamen.Portal.Web.Models.Squares
{
    public class ChangeGeneralModel
    {
        public Square Square { get; set; }

        public int CoverColor { get; set; }

        public int[] CoverColors { get; set; }

        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "NameOfSquareIsRequired")]
        [Display(ResourceType = typeof(Label), Name = "NameOfSquare")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Label), Name = "Title")]
        public string InfoTitle { get; set; }

        [Display(ResourceType = typeof(Label), Name = "Description")]
        [AllowHtml]
        public string InfoText { get; set; }

        public HttpPostedFileBase CoverImage { get; set; }

        public HttpPostedFileBase ProfileImage { get; set; }
    }
}