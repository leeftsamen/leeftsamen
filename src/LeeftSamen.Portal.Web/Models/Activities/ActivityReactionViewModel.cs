// <copyright file="ActivityReactionViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Web.Models.Activities
{
    using LeeftSamen.Portal.Data.Models;

    public class ActivityReactionViewModel
    {
        public ActivityReaction Reaction { get; set; }

        public string NewReaction { get; set; }
    }
}
