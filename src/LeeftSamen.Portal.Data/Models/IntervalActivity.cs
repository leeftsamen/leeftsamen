// <copyright file="IntervalActivity.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A class for an interval of a recurring Activity
    /// </summary>
    public class IntervalActivity : IActivity
    {
        public DateTime OriginalEndDateTime { get; set; }

        public DateTime OriginalStartDateTime { get; set; }

        public int ActivityId { get; set; }

        public int? AgeFrom { get; set; }

        public int? AgeTo { get; set; }

        public bool AllAges { get; set; }

        public bool AllDay { get; set; }

        public bool AllowSharing { get; set; }

        public ICollection<ActivityAttendance> Attendees { get; set; }

        public DateTime CreationDate { get; set; }

        public User Creator { get; set; }

        public string Description { get; set; }

        public DateTime EndDateTime { get; set; }

        public ICollection<ActivityInterval> Intervals { get; set; }

        public ICollection<ActivityInvitation> Invitations { get; set; }

        public string Location { get; set; }

        public OrganizationMembership OrganizationMembership { get; set; }

        public int? OrganizationMembershipId { get; set; }

        public ICollection<ActivityReaction> Reactions { get; set; }

        public Activity.Recurrance Recurring { get; set; }

        public DateTime StartDateTime { get; set; }

        public string Title { get; set; }
    }
}