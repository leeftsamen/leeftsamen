// <copyright file="DetailViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Data.Models;
using System;
using System.Collections.Generic;

namespace LeeftSamen.Portal.Web.Models.Circles
{
    /// <summary>
    /// The circle view view model.
    /// </summary>
    public class DetailViewModel
    {
        /// <summary>
        /// Gets or sets the circle id.
        /// </summary>
        public int CircleId { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is private.
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether current user is member of circle.
        /// </summary>
        public bool IsCurrentUserMember { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the timeLineItems
        /// </summary>
        public List<TimelineItem> TimeLineItems { get; set; }

        public class TimelineItem
        {
            public string Action { get; set; }

            public DateTime Date { get; set; }

            public string Url { get; set; }

            public string Category { get; set; }

            public User User { get; set; }
        }
    }
}