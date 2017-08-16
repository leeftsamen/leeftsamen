// <copyright file="Action.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class Action
    {
        public virtual int ActionId { get; set; }

        public virtual string Name { get; set; }

        public virtual string Title { get; set; }

        public virtual string Description { get; set; }

        public virtual string HomeTitle { get; set; }

        public virtual string HomeText { get; set; }

        public virtual string MenuText { get; set; }

        public virtual decimal MoneyPerVote { get; set; }

        public virtual decimal MoneyAvailable { get; set; }

        public virtual DateTime? ActionStart { get; set; }

        public virtual DateTime? ActionEnd { get; set; }

        public virtual ICollection<ActionParticipant> Participants { get; set; }

        public virtual ICollection<ActionVote> Votes { get; set; }

        public virtual ICollection<ActionZipcode> Zipcodes { get; set; }
    }
}
