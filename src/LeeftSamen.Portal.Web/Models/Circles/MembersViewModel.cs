// <copyright file="MembersViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Circles
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
            this.Members = new List<MemberViewModel>();
        }

        public IEnumerable<HelpIcon> HelpIcons { get; set; }

        /// <summary>
        /// Gets or sets the circle id.
        /// </summary>
        public int CircleId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether circle is private.
        /// </summary>
        public bool CircleIsPrivate { get; set; }

        /// <summary>
        /// Gets or sets the circle name.
        /// </summary>
        public string CircleName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether current user is circle administrator.
        /// </summary>
        public bool CurrentUserIsCircleAdministrator { get; set; }

        /// <summary>
        /// Gets or sets the member to remove.
        /// </summary>
        public string MemberToRemove { get; set; }

        /// <summary>
        /// Gets or sets the members.
        /// </summary>
        public List<MemberViewModel> Members { get; set; }

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
        /// The member view model.
        /// </summary>
        public class MemberViewModel
        {
            /// <summary>
            /// Gets or sets the email.
            /// </summary>
            public string Email { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether has been invited.
            /// </summary>
            public bool HasBeenInvited { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether invitation has Expired.
            /// </summary>
            public bool InvitationIsExpired { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether is administrator.
            /// </summary>
            public bool IsAdministrator { get; set; }

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

            /// <summary>
            /// Gets or sets the user profile's description.
            /// </summary>
            public string ProfileDescription { get; set; }
        }
    }
}