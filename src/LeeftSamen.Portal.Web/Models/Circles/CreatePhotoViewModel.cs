// <copyright file="CreatePhotoViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Circles
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using System.Web.Mvc;

    using LeeftSamen.Portal.Data.Models;

    /// <summary>
    /// The detail messages view model.
    /// </summary>
    public class CreatePhotoViewModel
    {
        /// <summary>
        /// Gets or sets the circle id.
        /// </summary>
        public int CircleId { get; set; }

        public User UploadedBy { get; set; }

        [Required]
        public HttpPostedFileBase Photo { get; set; }

        public int? PhotoAlbumId { get; set; }
    }
}