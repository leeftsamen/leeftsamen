// <copyright file="MarketplaceLendModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.EmailTemplates.Models
{
    public class MarketplaceLendModel : BaseModel
    {
        /// <summary>
        /// Gets or sets the marketplace item title.
        /// </summary>
        public string MarketplaceItemTitle { get; set; }

        /// <summary>
        /// Gets or sets the marketplace item description.
        /// </summary>
        public string MarketplaceItemDescription { get; set; }

        /// <summary>
        /// Gets or sets the receiver name.
        /// </summary>
        public string ReceiverName { get; set; }

        /// <summary>
        /// Gets or sets the creator name.
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// Gets or sets the itemurl.
        /// </summary>
        public string ItemUrl { get; set; }

        public int? CircleId { get; set; }

        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Gets the template name.
        /// </summary>
        public override string TemplateName
        {
            get
            {
                return "MarketplaceLendRequest";
            }
        }
    }
}
