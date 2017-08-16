// <copyright file="WeeklyActivitiesModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.EmailTemplates.Models
{
    using System.Collections.Generic;

    using LeeftSamen.Portal.Data.Models;

    public class WeeklyActivitiesModel : BaseModel
    {
        public IList<NeighborhoodMessage> Messages { get; set; }

        public IList<Activity> Activities { get; set; }

        public IList<Circle> Circles { get; set; }

        public IList<MarketplaceItem> ForSale { get; set; }

        public IList<MarketplaceItem> ToBorrow { get; set; }

        public IList<MarketplaceItem> Meals { get; set; }

        public IList<MarketplaceItem> NeighborHelp { get; set; }

        //public IList<MarketplaceItem> Items { get; set; }

        public User Recipient { get; set; }

        public override string TemplateName
        {
            get
            {
                return "WeeklyActivities";
            }
        }
    }
}
