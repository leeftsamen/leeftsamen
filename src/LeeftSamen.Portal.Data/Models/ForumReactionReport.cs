// <copyright file="ForumReactionReport.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Data.Models
{
    public class ForumReactionReport
    {
        public virtual int ForumReactionReportId { get; set; }

        public ForumReaction Reaction { get; set; }

        public User Reporter { get; set; }

        public DateTime ReportDate { get; set; }

        public string Remark { get; set; }

        public bool Reviewed { get; set; }

        public User ReviewedBy { get; set; }
    }
}
