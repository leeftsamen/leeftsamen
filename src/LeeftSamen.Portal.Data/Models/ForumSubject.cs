// <copyright file="ForumSubject.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeeftSamen.Common.InterfaceText;

namespace LeeftSamen.Portal.Data.Models
{
    public class ForumSubject
    {
        public virtual int ForumSubjectId { get; set; }

        public string Type { get; set; }

        public bool Deleted { get; set; }

        public virtual int TypeId { get; set; }

        public virtual User Creator { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public virtual string Title { get; set; }

        public virtual string Description { get; set; }

        public virtual List<ForumReaction> Reactions { get; set; }
    }
}
