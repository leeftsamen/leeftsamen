// <copyright file="CircleJobUnassignedModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.EmailTemplates.Models
{
    public class CircleJobUnassignedModel : BaseModel
    {
        /// <summary>
        /// Gets the template name.
        /// </summary>
        public override string TemplateName
        {
            get
            {
                return "CircleJobUnassigned";
            }
        }

        public string JobUrl { get; set; }

        public string JobOverviewUrl { get; set; }

        public string JobName { get; set; }

        /// <summary>
        /// Gets or sets the circle name.
        /// </summary>
        public string CircleName { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        public string Unassignee { get; set; }
    }
}
