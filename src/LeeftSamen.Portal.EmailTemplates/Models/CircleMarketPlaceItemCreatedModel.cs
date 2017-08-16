// <copyright file="CircleMarketPlaceItemCreatedModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.EmailTemplates.Models
{
    public class CircleMarketPlaceItemCreatedModel : BaseModel
    {
        /// <summary>
        /// Gets the template name.
        /// </summary>
        public override string TemplateName
        {
            get
            {
                return "CircleMarketPlaceItemCreated";
            }
        }

        public string MessageUrl { get; set; }

        public string MarketPlaceItemName { get; set; }

        /// <summary>
        /// Gets or sets the circle name.
        /// </summary>
        public string CircleName { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
    }
}
