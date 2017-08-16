// <copyright file="AssignedJobsViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The assigned jobs view model.
    /// </summary>
    public class AssignedJobsViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssignedJobsViewModel"/> class.
        /// </summary>
        /// <param name="currentUserIsCircleAdministrator">
        /// The current user is circle administrator.
        /// </param>
        /// <param name="currentUserId">
        /// The current User Id.
        /// </param>
        /// <param name="jobs">
        /// The jobs.
        /// </param>
        public AssignedJobsViewModel(
            int circleId,
            bool currentUserIsCircleAdministrator,
            string currentUserId,
            List<IndexViewModel.JobViewModel> jobs)
        {
            this.CircleId = circleId;
            this.CurrentUserIsCircleAdministrator = currentUserIsCircleAdministrator;
            this.CurrentUserId = currentUserId;
            this.Jobs = jobs;
        }

        /// <summary>
        /// Gets or sets the circle id.
        /// </summary>
        public int CircleId { get; set; }

        /// <summary>
        /// Gets or sets the current user id.
        /// </summary>
        public string CurrentUserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether current user is circle administrator.
        /// </summary>
        public bool CurrentUserIsCircleAdministrator { get; set; }

        /// <summary>
        /// Gets or sets the jobs.
        /// </summary>
        public List<IndexViewModel.JobViewModel> Jobs { get; set; }

        /// <summary>
        /// The from.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// The <see cref="AssignedJobsViewModel"/>.
        /// </returns>
        public static AssignedJobsViewModel From(
            IndexViewModel model,
            Func<IndexViewModel.JobViewModel, bool> predicate)
        {
            return new AssignedJobsViewModel(
                model.CircleId,
                model.CurrentUserIsCircleAdministrator,
                model.CurrentUserId,
                model.AssignedJobs.Where(predicate).ToList());
        }
    }
}