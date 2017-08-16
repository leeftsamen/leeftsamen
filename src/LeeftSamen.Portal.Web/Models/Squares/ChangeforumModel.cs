// <copyright file="ChangeForumModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Common.InterfaceText;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeeftSamen.Portal.Web.Models.Squares
{
    public class ChangeForumModel
    {
        public int SquareId { get; set; }

        [Display(ResourceType = typeof(Label), Name = "Title")]
        public string Title { get; set; }

        [Display(ResourceType = typeof(Label), Name = "Text")]
        [AllowHtml]
        public string Text { get; set; }
    }
}