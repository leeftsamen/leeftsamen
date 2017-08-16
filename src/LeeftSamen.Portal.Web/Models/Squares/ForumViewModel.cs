// <copyright file="ForumViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Squares
{
    public class ForumViewModel
    {
        public int SquareId { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public bool CurrentUserIsAdministrator { get; set; }

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