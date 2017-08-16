// <copyright file="UserProfileImage.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Users
{
    /// <summary>
    /// The user profile image.
    /// </summary>
    public class UserProfileImage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileImage"/> class.
        /// </summary>
        /// <param name="mediaId">
        /// The media Id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        public UserProfileImage(int? mediaId, string userId, string size)
        {
            this.MediaId = mediaId;
            this.UserId = userId;
            this.Size = size;
        }

        /// <summary>
        /// Gets a value indicating whether has profile image.
        /// </summary>
        public bool HasProfileImage
        {
            get
            {
                return this.MediaId.HasValue;
            }
        }

        /// <summary>
        /// Gets or sets the media id.
        /// </summary>
        public int? MediaId { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public string UserId { get; set; }
    }
}