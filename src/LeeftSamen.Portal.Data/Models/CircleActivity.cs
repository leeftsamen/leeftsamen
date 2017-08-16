// <copyright file="CircleActivity.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class CircleActivity
    {
        public CircleActivity()
        {
            this.Attendees = new List<CircleActivityAttendance>();
            this.Invitations = new List<CircleActivityInvitation>();
            this.Reactions = new List<CircleActivityReaction>();
        }

        public virtual int ActivityId { get; set; }

        public virtual User Creator { get; set; }

        public virtual Circle Circle { get; set; }

        public virtual string Description { get; set; }

        public virtual string Location { get; set; }

        public virtual string Title { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public virtual DateTime StartDateTime { get; set; }

        public virtual DateTime EndDateTime { get; set; }

        public virtual bool AllDay { get; set; }

        public virtual bool AllAges { get; set; }

        public virtual int? AgeFrom { get; set; }

        public virtual int? AgeTo { get; set; }

        public virtual List<CircleActivityAttendance> Attendees { get; set; }

        public virtual List<CircleActivityReaction> Reactions { get; set; }

        public virtual List<CircleActivityInvitation> Invitations { get; set; }
    }
}
