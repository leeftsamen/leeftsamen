// <copyright file="IndexViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Circles
{
    using System.Collections.Generic;
    using System.EnterpriseServices.Internal;

    using LeeftSamen.Portal.Data.Models;

    /// <summary>
    /// The circle index view model.
    /// </summary>
    public class IndexViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexViewModel"/> class.
        /// </summary>
        public IndexViewModel()
        {
            this.Circles = new List<CircleViewModel>();
        }

        public IEnumerable<HelpIcon> HelpIcons { get; set; }

        /// <summary>
        /// Gets or sets the circles where the user is invited to join.
        /// </summary>
        public List<CircleInvitationViewModel> InvitationCircles { get; set; }

        /// <summary>
        /// Gets or sets the user circles.
        /// </summary>
        public List<CircleViewModel> Circles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether current user can only view.
        /// </summary>
        public bool CurrentUserCanOnlyView { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether my circles.
        /// </summary>
        public bool MyCircles { get; set; }

        /// <summary>
        /// The circle view model.
        /// </summary>
        public class CircleViewModel
        {
            public IEnumerable<HelpIcon> HelpIcons { get; set; }

            /// <summary>
            /// Gets or sets the circle id.
            /// </summary>
            public int CircleId { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether has requested to join.
            /// </summary>
            public bool HasRequestedToJoin { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether is current user admin.
            /// </summary>
            public bool IsCurrentUserAdmin { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether is current user member.
            /// </summary>
            public bool IsCurrentUserMember { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether is invited to join.
            /// </summary>
            public bool IsInvitedToJoin { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether is private.
            /// </summary>
            public bool IsPrivate { get; set; }

            /// <summary>
            /// Gets or sets the member count.
            /// </summary>
            public int MemberCount { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether has profile image.
            /// </summary>
            public int? ProfileImageId { get; set; }
        }

        public class CircleInvitationViewModel
        {
            /// <summary>
            /// Gets or sets the circle id.
            /// </summary>
            public int CircleId { get; set; }

            /// <summary>
            /// Gets or sets the circle id.
            /// </summary>
            public int InvitationId { get; set; }

            /// <summary>
            /// Gets or sets the join token.
            /// </summary>
            public string Token { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether circle is private.
            /// </summary>
            public bool IsPrivate { get; set; }

            /// <summary>
            /// Gets or sets the member count.
            /// </summary>
            public int MemberCount { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether has profile image.
            /// </summary>
            public int? ProfileImageId { get; set; }
        }
    }
}