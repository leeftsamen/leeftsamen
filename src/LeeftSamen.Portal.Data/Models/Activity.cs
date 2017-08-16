// <copyright file="Activity.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using LeeftSamen.Common.InterfaceText;
    using System.Data.Entity.Spatial;

    public class Activity : IActivity
    {
        public enum Recurrance
        {
            [Display(ResourceType = typeof(Label), Name = "RecurringNo")]
            No,

            [Display(ResourceType = typeof(Label), Name = "RecurringDay")]
            Day,

            [Display(ResourceType = typeof(Label), Name = "RecurringWorkDay")]
            WorkDay,

            [Display(ResourceType = typeof(Label), Name = "RecurringWeek")]
            Week,

            // [Display(ResourceType = typeof(Label), Name = "RecurringTwoWeek")]
            // TwoWeek,
            [Display(ResourceType = typeof(Label), Name = "RecurringMonth")]
            Month,

            [Display(ResourceType = typeof(Label), Name = "RecurringYear")]
            Year
        }

        public virtual int ActivityId { get; set; }

        public virtual int? AgeFrom { get; set; }

        public virtual int? AgeTo { get; set; }

        public virtual bool AllAges { get; set; }

        public virtual bool AllDay { get; set; }

        public virtual bool AllowSharing { get; set; }

        public virtual ICollection<ActivityAttendance> Attendees { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public virtual User Creator { get; set; }

        public virtual DbGeography Position { get; set; }

        public virtual string Description { get; set; }

        public virtual DateTime EndDateTime { get; set; }

        public virtual ICollection<ActivityInterval> Intervals { get; set; }

        public virtual ICollection<ActivityInvitation> Invitations { get; set; }

        public virtual string Location { get; set; }

        public virtual OrganizationMembership OrganizationMembership { get; set; }

        public virtual int? OrganizationMembershipId { get; set; }

        public virtual ICollection<ActivityReaction> Reactions { get; set; }

        public virtual Recurrance Recurring { get; set; }

        public virtual DateTime? RecurringEnd { get; set; }

        public virtual DateTime StartDateTime { get; set; }

        public virtual string Title { get; set; }

        public virtual Circle Circle { get; set; }
    }
}