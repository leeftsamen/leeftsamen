// <copyright file="CircleJobModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;

namespace LeeftSamen.Portal.EmailTemplates.Models
{
    public class CircleJobModel : BaseModel
    {
        /// <summary>
        /// Gets the template name.
        /// </summary>
        public override string TemplateName
        {
            get
            {
                return "CircleNewJob";
            }
        }

        public string JobUrl { get; set; }

        public string JobOverviewUrl { get; set; }

        /// <summary>
        /// Gets or sets the circle name.
        /// </summary>
        public string CircleName { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string CreatedBy { get; set; }

        public string JobTitle { get; set; }

        public string JobText { get; set; }

        public bool HasDueTime { get; set; }

        public DateTime JobDueDateTime { get; set; }

        public DateTime? JobDueDateTimeEnd { get; set; }
    }
}