// <copyright file="ForumReaction.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Forums
{
    public class ForumReaction
    {
        public string Type { get; set; }

        public int TypeId { get; set; }

        public string CurrentUserId { get; set; }

        public bool IsCurrentUserAdmin { get; set; }

        public SubjectViewModel.ForumReaction Reaction { get; set; }
    }
}