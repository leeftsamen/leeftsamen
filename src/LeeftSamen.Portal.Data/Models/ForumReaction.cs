// <copyright file="ForumReaction.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Data.Models
{
    public class ForumReaction
    {
        public virtual int ForumReactionId { get; set; }

        public virtual ForumSubject Subject { get; set; }

        public virtual bool SubjectMainReaction { get; set; }

        public virtual ForumReaction ParentReaction { get; set; }

        public virtual User Creator { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public virtual string Title { get; set; }

        public virtual string Message { get; set; }

        public virtual List<ForumReactionMedia> MediaList { get; set; }

        public bool Deleted { get; set; }

        public User DeletedBy { get; set; }

        public DateTime? DeletedDate { get; set; }

        public virtual int EditCount { get; set; }

        public User LastEditBy { get; set; }

        public DateTime? LastEditDate { get; set; }
    }
}
