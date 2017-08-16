// <copyright file="ActivityInterval.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Models
{
    public class ActivityInterval
    {
        public virtual Activity Activity { get; set; }

        public virtual int IntervalId { get; set; }

        /// <summary>
        /// Gets or sets the interval for a specific day of a month
        /// 1 - 31 for a day
        /// Null for any day
        /// </summary>
        public virtual int? RepeatDay { get; set; }

        /// <summary>
        /// Gets or sets the interval for a specific month
        /// 1 - 12 for a month
        /// Null for any month
        /// </summary>
        public virtual int? RepeatMonth { get; set; }

        /// <summary>
        /// Gets or sets the interval for a specific week of a month
        /// 1 - 4 for a week of a month
        /// Null for any week of a month
        /// </summary>
        public virtual int? RepeatMonthWeek { get; set; }

        /// <summary>
        /// Gets or sets the interval for a specific day of the week
        /// 1 - 7 for a day of the week
        /// Null for any day of the week
        /// </summary>
        public virtual int? RepeatWeekDay { get; set; }

        /// <summary>
        /// Gets or sets the interval for a specific year
        /// Null for any year
        /// </summary>
        public virtual int? RepeatYear { get; set; }
    }
}