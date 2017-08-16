// <copyright file="ReactionViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.NeighborhoodMessages
{
    using LeeftSamen.Portal.Data.Models;

    public class ReactionViewModel
    {
        public NeighborhoodMessageReaction Reaction { get; set; }

        public bool UserCanDeleteReaction { get; set; }

        public string NewReaction { get; set; }
    }
}