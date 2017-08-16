// <copyright file="ChangeProfileImageViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Account
{
    /// <summary>
    /// The change profile image view model.
    /// </summary>
    public class ChangeProfileImageViewModel : ChangeProfileImagePostModel
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the profile image id.
        /// </summary>
        public int? ProfileImageId { get; set; }
    }
}