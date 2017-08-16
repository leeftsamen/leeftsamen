// <copyright file="IndexViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Forums
{
    public class IndexViewModel
    {
        public string Type { get; set; }

        public int? TypeId { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public bool IsCurrentUserAdmin { get; set; }

        public List<ForumSubject> Subjects { get; set; }

        public class ForumSubject
        {
            public int SubjectId { get; set; }

            public string Title { get; set; }

            public string Description { get; set; }

            public int ReactionCount { get; set; }

            public DateTime? LastMessageDate { get; set; }

            public string LastMessageName { get; set; }
        }
    }
}