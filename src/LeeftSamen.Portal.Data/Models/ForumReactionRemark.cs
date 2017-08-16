// <copyright file="ForumReactionRemark.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Data.Models
{
    public class ForumReactionRemark
    {
        public int ForumReactionRemarkId { get; set; }

        public ForumReaction Reaction { get; set; }

        public User Creator { get; set; }

        public DateTime CreationDate { get; set; }

        public User EditedBy { get; set; }

        public DateTime? LastEditedDate { get; set; }
    }
}
