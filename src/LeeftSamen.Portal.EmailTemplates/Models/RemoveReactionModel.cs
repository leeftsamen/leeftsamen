// <copyright file="RemoveReactionModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.EmailTemplates.Models
{
    public class RemoveReactionModel : BaseModel
    {
        /// <summary>
        /// Gets or sets the reaction text.
        /// </summary>
        public string ReactionText { get; set; }

        /// <summary>
        /// Gets or sets the reaction Date.
        /// </summary>
        public DateTime ReactionDate { get; set; }

        /// <summary>
        /// Gets or sets the removed Date.
        /// </summary>
        public DateTime RemovedDate { get; set; }

        /// <summary>
        /// Gets or sets the invited by name.
        /// </summary>
        public string RemovedByName { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the template name.
        /// </summary>
        public override string TemplateName
        {
            get
            {
                return "RemoveReaction";
            }
        }
    }
}
