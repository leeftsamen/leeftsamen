// <copyright file="IActivity.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    using System;
    using System.Collections.Generic;

    public interface IActivity
    {
        int ActivityId { get; set; }

        int? AgeFrom { get; set; }

        int? AgeTo { get; set; }

        bool AllAges { get; set; }

        bool AllDay { get; set; }

        bool AllowSharing { get; set; }

        ICollection<ActivityAttendance> Attendees { get; set; }

        DateTime CreationDate { get; set; }

        User Creator { get; set; }

        string Description { get; set; }

        DateTime EndDateTime { get; set; }

        ICollection<ActivityInterval> Intervals { get; set; }

        ICollection<ActivityInvitation> Invitations { get; set; }

        string Location { get; set; }

        OrganizationMembership OrganizationMembership { get; set; }

        int? OrganizationMembershipId { get; set; }

        ICollection<ActivityReaction> Reactions { get; set; }

        Activity.Recurrance Recurring { get; set; }

        DateTime StartDateTime { get; set; }

        string Title { get; set; }
    }
}