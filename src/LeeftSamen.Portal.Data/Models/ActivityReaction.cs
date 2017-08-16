// <copyright file="ActivityReaction.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Data.Models
{
    public class ActivityReaction
    {
        public virtual DateTime CreationDateTime { get; set; }

        public virtual User Creator { get; set; }

        public virtual Activity Activity { get; set; }

        public virtual OrganizationMembership OrganizationMembership { get; set; }

        public virtual int? OrganizationMembershipId { get; set; }

        public virtual int ReactionId { get; set; }

        public virtual string Text { get; set; }
    }
}
