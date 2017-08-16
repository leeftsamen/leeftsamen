// <copyright file="Job.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class Job
    {
        public virtual User Assignee { get; set; }

        public virtual Circle Circle { get; set; }

        public virtual DateTime? CompletionDateTime { get; set; }

        public virtual DateTime CreationDateTime { get; set; }

        public virtual User Creator { get; set; }

        public virtual string Description { get; set; }

        public virtual DateTime DueDateTime { get; set; }

        public virtual DateTime? DueDateTimeEnd { get; set; }

        public virtual bool HasDueTime { get; set; }

        public virtual bool IsOnlyVisibleToSelectedMembers { get; set; }

        public virtual int JobId { get; set; }

        public virtual string Title { get; set; }

        public virtual ICollection<CircleMembership> VisibleToMembers { get; set; }
    }
}