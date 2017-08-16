// <copyright file="ChangeProfileImagePostModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Account
{
    using System.ComponentModel.DataAnnotations;
    using System.Web;

    using LeeftSamen.Common.InterfaceText;

    /// <summary>
    /// The change profile image post model.
    /// </summary>
    public class ChangeProfileImagePostModel
    {
        /// <summary>
        /// Gets or sets the profile image.
        /// </summary>
        [Display(ResourceType = typeof(Label), Name = "NewProfileImage")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "ProfileImageIsRequired")]
        public HttpPostedFileBase ProfileImage { get; set; }
    }
}