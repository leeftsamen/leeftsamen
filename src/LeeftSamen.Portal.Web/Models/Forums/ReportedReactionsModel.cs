// <copyright file="ReportedReactionsModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeeftSamen.Portal.Web.Models.Forums
{
    public class ReportedReactionsModel
    {
        public string Type { get; set; }

        public int TypeId { get; set; }

        public List<ReportedReaction> Reactions { get; set; }

        public class ReportedReaction
        {
            public int ReactionId { get; set; }

            public int ReportId { get; set; }

            public string ReporterName { get; set; }

            public string Title { get; set; }

            public string Reaction { get; set; }

            public DateTime ReportedDate { get; set; }

            public string ReactionByName { get; set; }

            public string ReactionById { get; set; }

            public List<Media> MediaList { get; set; }
        }
    }
}