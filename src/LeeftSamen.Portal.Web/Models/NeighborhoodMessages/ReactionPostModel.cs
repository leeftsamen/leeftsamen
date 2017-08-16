// <copyright file="ReactionPostModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Web.Models.NeighborhoodMessages
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class ReactionPostModel
    {
        public int? ParentId { get; set; }

        [Required]
        [AllowHtml]
        public string NewReaction { get; set; }
    }
}
