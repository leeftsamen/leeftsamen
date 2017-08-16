// <copyright file="ReactionPostModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Marketplace
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    using LeeftSamen.Common.InterfaceText;

    /// <summary>
    /// The reaction post model.
    /// </summary>
    public class ReactionPostModel
    {
        /// <summary>
        /// Gets or sets the new reaction.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "ReactionIsRequired")]
        [AllowHtml]
        public string NewReaction { get; set; }

        /// <summary>
        /// Gets or sets the new reaction parent id.
        /// </summary>
        public int? NewReactionParentId { get; set; }
    }
}