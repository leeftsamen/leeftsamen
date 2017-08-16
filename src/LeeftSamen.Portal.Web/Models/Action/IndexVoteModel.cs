// <copyright file="IndexVoteModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Action
{
    using System.Collections.Generic;

    using LeeftSamen.Portal.Data.Models;

    /// <summary>
    /// The index vote model.
    /// </summary>
    public class IndexVoteModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexVoteModel"/> class.
        /// </summary>
        public IndexVoteModel()
        {
            this.Organizations = new List<OrganizationViewModel>();
        }

        public int ActionId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool MaxVotesReached { get; set; }

        public bool ActionStarted { get; set; }

        public bool ActionEnded { get; set; }

        public bool HasVoted { get; set; }

        /// <summary>
        /// Gets or sets the organizations.
        /// </summary>
        public List<OrganizationViewModel> Organizations { get; set; }

        /// <summary>
        /// The organization view model.
        /// </summary>
        public class OrganizationViewModel
        {
            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the logo id.
            /// </summary>
            public int? LogoId { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the organization id.
            /// </summary>
            public int OrganizationId { get; set; }

            public string OrganizationTypeName { get; set; }

            public decimal Collected { get; set; }
        }
    }
}