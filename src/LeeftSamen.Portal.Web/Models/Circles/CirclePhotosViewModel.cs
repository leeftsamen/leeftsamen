// <copyright file="CirclePhotosViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Circles
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using System.Web.Mvc;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Data.Models;

    /// <summary>
    /// The detail messages view model.
    /// </summary>
    public class CirclePhotosViewModel
    {
        public string CurrentUserId { get; set; }

        /// <summary>
        /// Gets or sets the circle id.
        /// </summary>
        public int CircleId { get; set; }

        /// <summary>
        /// Gets or sets the circle name.
        /// </summary>
        public string CircleName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether current user is circle administrator.
        /// </summary>
        public bool CurrentUserIsCircleAdministrator { get; set; }

        /// <summary>
        /// Gets or sets the photos for a circle/album.
        /// </summary>
        public ICollection<OwnedMedia> Documents { get; set; }

        /// <summary>
        /// Gets or sets the albums for a circle.
        /// </summary>
        public ICollection<CircleAlbumViewModel> PhotoAlbums { get; set; }

        /// <summary>
        /// Gets or sets the currently viewed album.
        /// </summary>
        public CirclePhotoAlbum CurrentPhotoAlbum { get; set; }

        [Required]
        [MaxLength(200, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Error))]
        public string Title { get; set; }

        public class CircleAlbumViewModel
        {
            /// <summary>
            /// Gets or sets the album id.
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// Gets or sets the album title.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the amount of photos in the album.
            /// </summary>
            public int Count { get; set; }

            /// <summary>
            /// Gets or sets cover image for the album.
            /// </summary>
            public CirclePhoto Cover { get; set; }
        }

        public class OwnedMedia
        {
            public OwnedMedia(Media item, string ownerId)
            {
                this.Item = item;
                this.OwnerId = ownerId;
            }

            public Media Item { get; set; }

            public string OwnerId { get; set; }
        }
    }
}