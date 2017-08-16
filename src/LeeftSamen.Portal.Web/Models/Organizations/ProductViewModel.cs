// <copyright file="ProductViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Common.InterfaceText;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace LeeftSamen.Portal.Web.Models.Organizations
{
    /// <summary>
    /// The product view model.
    /// </summary>
    public class ProductViewModel
    {
        /// <summary>
        /// Gets or sets the product id.
        /// </summary>
        public int? OrganizationProductId { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [Display(ResourceType = typeof(Label), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "TitleIsRequired")]
        [MaxLength(40, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Error))]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the IntroductionText.
        /// </summary>
        [Display(ResourceType = typeof(Label), Name = "IntroductionText")]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Error))]
        public string IntroductionText { get; set; }

        [Display(ResourceType = typeof(Label), Name = "FullText")]
        [AllowHtml]
        [MaxLength(600, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Error))]
        public string FullText { get; set; }
    }
}