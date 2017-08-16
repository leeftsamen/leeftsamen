// <copyright file="JoinRequestsViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Circles
{
    using System.Collections.Generic;

    /// <summary>
    /// The join requests view model.
    /// </summary>
    public class JoinRequestsViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JoinRequestsViewModel"/> class.
        /// </summary>
        public JoinRequestsViewModel()
        {
            this.JoinRequests = new List<JoinRequestViewModel>();
            this.Invitations = new List<MembersViewModel.MemberViewModel>();
        }

        /// <summary>
        /// Gets or sets the circle id.
        /// </summary>
        public int CircleId { get; set; }

        /// <summary>
        /// Gets or sets the circle name.
        /// </summary>
        public string CircleName { get; set; }

        /// <summary>
        /// Gets or sets the members.
        /// </summary>
        public List<JoinRequestViewModel> JoinRequests { get; set; }

        /// <summary>
        /// Gets or sets the members.
        /// </summary>
        public List<MembersViewModel.MemberViewModel> Invitations { get; set; }

        /// <summary>
        /// The join request view model.
        /// </summary>
        public class JoinRequestViewModel
        {
            /// <summary>
            /// Gets or sets the circle join request id.
            /// </summary>
            public int CircleJoinRequestId { get; set; }

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