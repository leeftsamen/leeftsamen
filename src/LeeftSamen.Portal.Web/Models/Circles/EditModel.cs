// <copyright file="EditModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Circles
{
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using System.Web.Mvc;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Data.Models;

    public class EditModel
    {
        public Circle Circle { get; set; }

        public int CoverColor { get; set; }

        public int[] CoverColors { get; set; }

        public HttpPostedFileBase CoverImage { get; set; }

        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "DescriptionIsRequired")]
        [Display(ResourceType = typeof(Label), Name = "Description")]
        [AllowHtml]
        public string Description { get; set; }

        [Required]
        [Display(ResourceType = typeof(Label), Name = "TypeOfCircle")]
        public bool IsPrivate { get; set; }

        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "NameOfCircleIsRequired")]
        [Display(ResourceType = typeof(Label), Name = "NameOfCircle")]
        public string Name { get; set; }

        public HttpPostedFileBase ProfileImage { get; set; }
    }
}