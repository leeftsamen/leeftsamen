// <copyright file="EditReactionModel.cs" company="LeeftSamen B.V.">
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
    public class EditReactionModel
    {
        public int SquareId { get; set; }

        public int SubjectId { get; set; }

        public int ReactionId { get; set; }

        public bool AllowFiles { get; set; }

        public List<Media> Files { get; set; }

        [Display(ResourceType = typeof(Label), Name = "Reaction")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "ReactionIsRequired")]
        [AllowHtml]
        public string Text { get; set; }
    }
}