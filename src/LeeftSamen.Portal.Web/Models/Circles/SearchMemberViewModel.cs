// <copyright file="SearchMemberViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Circles
{
    /// <summary>
    /// The search member view model.
    /// </summary>
    internal class SearchMemberViewModel
    {
        /// <summary>
        /// Gets or sets the circle membership id.
        /// </summary>
        public string CircleMembershipId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether has profile image.
        /// </summary>
        public bool HasProfileImage { get; set; }

        /// <summary>
        /// Gets or sets the user profile image id.
        /// </summary>
        public int? UserProfileImageId { get; set; }

        /// <summary>
        /// Gets or sets the profile image url.
        /// </summary>
        public string ProfileImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName { get; set; }
    }
}