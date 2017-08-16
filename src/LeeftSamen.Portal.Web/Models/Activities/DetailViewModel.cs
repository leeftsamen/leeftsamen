// <copyright file="DetailViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Activities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    using LeeftSamen.Portal.Data.Models;

    /// <summary>
    /// The detail view model.
    /// </summary>
    public class DetailViewModel
    {
        /// <summary>
        /// Gets or sets the activity.
        /// </summary>
        public Activity Activity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user can edit.
        /// </summary>
        public bool UserCanEdit { get; set; }

        public bool CurrentUserIsAttending { get; set; }

        public int? ShownInCircle { get; set; }

        public ICollection<ActivityReaction> Reactions = new List<ActivityReaction>();

        [Required]
        [AllowHtml]
        public string NewReaction { get; set; }
    }
}