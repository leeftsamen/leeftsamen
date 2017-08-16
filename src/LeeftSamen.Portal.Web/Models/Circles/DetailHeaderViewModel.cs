// <copyright file="DetailHeaderViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Circles
{
    using System.Collections.Generic;

    using LeeftSamen.Portal.Data.Models;

    /// <summary>
    /// The circle header detail view model.
    /// </summary>
    public class DetailHeaderViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetailHeaderViewModel"/> class.
        /// </summary>
        public DetailHeaderViewModel()
        {
            this.MenuItems = new List<MenuItemModel>();
        }

        /// <summary>
        /// Gets or sets the circle id.
        /// </summary>
        public int CircleId { get; set; }

        /// <summary>
        /// Gets or sets the cover color.
        /// </summary>
        public int CoverColor { get; set; }

        /// <summary>
        /// Gets or sets the cover image id.
        /// </summary>
        public int? CoverImageId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether current user can leave circle.
        /// </summary>
        public bool CurrentUserCanLeaveCircle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether current user can only view.
        /// </summary>
        public bool CurrentUserCanOnlyView { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether current user has requested to join.
        /// </summary>
        public bool CurrentUserHasRequestedToJoin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether current user was invited to join.
        /// </summary>
        public bool CurrentUserIsInvitedToJoin { get; set; }

        /// <summary>
        /// Gets or sets a value for accepting a join invitation.
        /// </summary>
        public string InvitationToken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether current user is circle administrator.
        /// </summary>
        public bool CurrentUserIsCircleAdministrator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether current user id member.
        /// </summary>
        public bool CurrentUserIsMember { get; set; }

        public IEnumerable<HelpIcon> HelpIcons { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is private.
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user receives mail.
        /// </summary>
        public bool ReceiveEmails { get; set; }

        /// <summary>
        /// Gets or sets the menu items.
        /// </summary>
        public List<MenuItemModel> MenuItems { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the profile image id.
        /// </summary>
        public int? ProfileImageId { get; set; }
    }
}