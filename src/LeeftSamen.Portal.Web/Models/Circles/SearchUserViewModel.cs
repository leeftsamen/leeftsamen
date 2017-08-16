// <copyright file="SearchUserViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Circles
{
    /// <summary>
    /// The search user view model.
    /// </summary>
    public class SearchUserViewModel
    {
        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether has profile image.
        /// </summary>
        public bool HasProfileImage { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the profile image id.
        /// </summary>
        public int? ProfileImageId { get; set; }

        /// <summary>
        /// Gets or sets the profile image url.
        /// </summary>
        public string ProfileImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public string UserId { get; set; }
    }
}