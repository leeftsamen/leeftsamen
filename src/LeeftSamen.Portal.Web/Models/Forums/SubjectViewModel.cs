// <copyright file="SubjectViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Common.InterfaceText;
using LeeftSamen.Portal.Data.Enums;
using LeeftSamen.Portal.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Forums
{
    public class SubjectViewModel
    {
        public string Type { get; set; }

        public int TypeId { get; set; }

        public int SubjectId { get; set; }

        public string SubjectName { get; set; }

        public string SubjectText { get; set; }

        public DateTime SubjectDate { get; set; }

        public List<ForumSubject> Subjects { get; set; }

        public List<ForumReaction> Reactions { get; set; }

        public int Page { get; set; }

        public int PageCount { get; set; }

        public string CurrentUserId { get; set; }

        public int? ProfileImageId { get; set; }

        public bool IsCurrentUserAdmin { get; set; }

        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "ReactionIsRequired")]
        public string ReactionText { get; set; }

        public class ForumSubject
        {
            public int SubjectId { get; set; }

            public string Title { get; set; }

            public DateTime CreatedDate { get; set; }

            public DateTime? LastReactionDate { get; set; }
        }

        public class ForumReaction
        {
            public int ReactionId { get; set; }

            public string CreatorId { get; set; }

            public int? ProfileImageId { get; set; }

            public string CreatorName { get; set; }

            public int CreatorReactionCount { get; set; }

            public int Distance { get; set; }

            public DateTime CreationDate { get; set; }

            public string Title { get; set; }

            public string Text { get; set; }

            public List<Media> MediaList { get; set; }

            public bool Deleted { get; set; }

            public bool Reported { get; set; }

            public DateTime? LastEditDate { get; set; }

            public List<ForumReaction> ChildReactions { get; set; }
        }
    }
}