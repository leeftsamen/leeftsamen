// <copyright file="MembersViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Squares
{
    using System.Collections.Generic;
    using System.Linq;

    using LeeftSamen.Portal.Data.Models;

    /// <summary>
    /// The detail members.
    /// </summary>
    public class MembersViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MembersViewModel"/> class.
        /// </summary>
        public MembersViewModel()
        {
            this.Admins = new List<MemberViewModel>();
        }

        public IEnumerable<HelpIcon> HelpIcons { get; set; }

        /// <summary>
        /// Gets or sets the circle id.
        /// </summary>
        public int SquareId { get; set; }

        /// <summary>
        /// Gets or sets the circle name.
        /// </summary>
        public string SquareName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether current user is circle administrator.
        /// </summary>
        public bool CurrentUserIsAdministrator { get; set; }

        /// <summary>
        /// Gets or sets the members.
        /// </summary>
        public List<MemberViewModel> Admins { get; set; }

        /// <summary>
        /// The member view model.
        /// </summary>
        public class MemberViewModel
        {
            /// <summary>
            /// Gets or sets the user city.
            /// </summary>
            public string City { get; set; }

            /// <summary>
            /// Gets or sets the user id.
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// Gets or sets the user name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the user profile image id.
            /// </summary>
            public int? ProfileImageId { get; set; }
        }
    }
}