// <copyright file="CreateBulkPostModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Activities
{
    using System.Collections.Generic;

    public class CreateBulkPostModel
    {
        public string Activities { get; set; }

        public bool Shareable { get; set; }

        public class ActivityDate
        {
            public string Date { get; set; }

            public List<Activity> Activities { get; set; }
        }

        public class Activity
        {
            public string StartTimeH { get; set; }

            public string StartTimeM { get; set; }

            public string EndTimeH { get; set; }

            public string EndTimeM { get; set; }

            public bool AllDay { get; set; }

            public string Title { get; set; }

            public string Description { get; set; }

            public string Location { get; set; }
        }
    }
}