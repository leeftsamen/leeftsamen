// <copyright file="IndexViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Jobs
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The circle jobs view model.
    /// </summary>
    public class IndexViewModel
    {
        /// <summary>
        /// Gets or sets the jobs.
        /// </summary>
        public List<JobViewModel> AssignedJobs { get; set; }

        /// <summary>
        /// Gets or sets the circle id.
        /// </summary>
        public int CircleId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating wether the current circle is private.
        /// </summary>
        public bool CircleIsPrivate { get; set; }

        /// <summary>
        /// Gets or sets the current user id.
        /// </summary>
        public string CurrentUserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether current user is circle administrator.
        /// </summary>
        public bool CurrentUserIsCircleAdministrator { get; set; }

        /// <summary>
        /// Gets or sets the unassigned jobs.
        /// </summary>
        public List<JobViewModel> UnassignedJobs { get; set; }

        /// <summary>
        /// The job view model.
        /// </summary>
        public class JobViewModel
        {
            /// <summary>
            /// Gets or sets the assignee id.
            /// </summary>
            public string AssigneeId { get; set; }

            /// <summary>
            /// Gets or sets the assignee name.
            /// </summary>
            public string AssigneeName { get; set; }

            /// <summary>
            /// Gets or sets the assignee profile image id.
            /// </summary>
            public int? AssigneeProfileImageId { get; set; }

            /// <summary>
            /// Gets or sets the due date time.
            /// </summary>
            public DateTime CreationDateTime { get; set; }

            /// <summary>
            /// Gets or sets the due date time.
            /// </summary>
            public bool HasDueDateTime { get; set; }

            /// <summary>
            /// Gets or sets the due date time.
            /// </summary>
            public DateTime DueDateTime { get; set; }

            /// <summary>
            /// Gets or sets the due date time.
            /// </summary>
            public DateTime? DueDateTimeEnd { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether is only visible to selected members.
            /// </summary>
            public bool IsOnlyVisibleToSelectedMembers { get; set; }

            /// <summary>
            /// Gets or sets the job id.
            /// </summary>
            public int JobId { get; set; }

            /// <summary>
            /// Gets or sets the title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the creator id.
            /// </summary>
            public string CreatorId { get; set; }
        }
    }
}