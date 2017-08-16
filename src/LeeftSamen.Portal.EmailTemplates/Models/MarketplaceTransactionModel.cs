// <copyright file="MarketplaceTransactionModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.EmailTemplates.Models
{
    /// <summary>
    /// The marketplace reaction model.
    /// </summary>
    public class MarketplaceTransactionModel : BaseModel
    {
        /// <summary>
        /// Gets or sets the marketplace item title.
        /// </summary>
        public string MarketplaceItemTitle { get; set; }

        /// <summary>
        /// Gets or sets the reaction url.
        /// </summary>
        public string MarketplaceItemUrl { get; set; }

        /// <summary>
        /// Gets or sets the receiver name.
        /// </summary>
        public string ReceiverName { get; set; }

        /// <summary>
        /// Gets or sets the sender name.
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// Gets the template name.
        /// </summary>
        public override string TemplateName
        {
            get
            {
                return "MarketplaceTransaction";
            }
        }
    }
}