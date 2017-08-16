// <copyright file="MembersViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Organizations
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The members view model.
    /// </summary>
    public class MembersViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether current user id administrator.
        /// </summary>
        public bool CurrentUserIsOrganizationAdministrator { get; set; }

        /// <summary>
        /// Gets or sets the members.
        /// </summary>
        public List<MemberViewModel> Members { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the number of admins.
        /// </summary>
        public int NumberOfAdmins
        {
            get
            {
                return this.Members.Count(m => m.IsAdministrator);
            }
        }

        /// <summary>
        /// Gets or sets the organization id.
        /// </summary>
        public int OrganizationId { get; set; }

        /// <summary>
        /// The member view model.
        /// </summary>
        public class MemberViewModel
        {
            /// <summary>
            /// Gets or sets the email.
            /// </summary>
            public string Email { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether is administrator.
            /// </summary>
            public bool IsAdministrator { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether has been invited.
            /// </summary>
            public bool HasBeenInvited { get; set; }

            /// <summary>
            /// Gets or sets the user city.
            /// </summary>
            public string UserCity { get; set; }

            /// <summary>
            /// Gets or sets the user id.
            /// </summary>
            public string UserId { get; set; }

            /// <summary>
            /// Gets or sets the user name.
            /// </summary>
            public string UserName { get; set; }

            /// <summary>
            /// Gets or sets the user profile image id.
            /// </summary>
            public int? UserProfileImageId { get; set; }
        }
    }
}