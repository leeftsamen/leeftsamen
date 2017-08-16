// <copyright file="ForumSubjectViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Squares
{
    public class ForumSubjectViewModel
    {
        public int SquareId { get; set; }

        public int SubjectId { get; set; }

        public string SubjectName { get; set; }

        public List<ForumReaction> Reactions { get; set; }

        public int Page { get; set; }

        public int PageCount { get; set; }

        public string CurrentUserId { get; set; }

        public bool IsCurrentUserAdmin { get; set; }

        public class ForumReaction
        {
            public int ReactionId { get; set; }

            public string CreatorId { get; set; }

            public int? ProfileImageId { get; set; }

            public string CreatorName { get; set; }

            public int CreatorReactionCount { get; set; }

            public int Distance { get; set; }

            public DateTime CreationDate { get; set; }

            public string Text { get; set; }

            public List<Media> MediaList { get; set; }

            public bool Deleted { get; set; }

            public bool Reported { get; set; }

            public DateTime? LastEditDate { get; set; }
        }
    }
}