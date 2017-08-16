// <copyright file="PostMessageViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.NeighborhoodMessages
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using System.Web.Mvc;

    using LeeftSamen.Common.InterfaceText;

    public class PostMessageViewModel
    {
        [Display(ResourceType = typeof(Label), Name = "ExpirationDateTime")]
        public DateTime? ExpirationDateTime { get; set; }

        [Range(0, 23)]
        public int ExpirationDateTimeHour { get; set; }

        [Range(0, 59)]
        public int ExpirationDateTimeMinute { get; set; }

        [Display(ResourceType = typeof(Label), Name = "FullText")]
        [AllowHtml]
//        [MaxLength(200, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Error))]
        public string FullText { get; set; }

        public HttpPostedFileBase Image1 { get; set; }

        public HttpPostedFileBase Image2 { get; set; }

        public HttpPostedFileBase Image3 { get; set; }

        public HttpPostedFileBase Image4 { get; set; }

        public HttpPostedFileBase Image5 { get; set; }

        public int? Image1Id { get; set; }

        public int? Image2Id { get; set; }

        public int? Image3Id { get; set; }

        public int? Image4Id { get; set; }

        public int? Image5Id { get; set; }

        public HttpPostedFileBase File1 { get; set; }

        public int? File1Id { get; set; }

        [Display(ResourceType = typeof(Label), Name = "IntroductionTextNeighborhoodMessage")]
        [MaxLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Error))]
        public string IntroductionText { get; set; }

        public bool Expires { get; set; }

        public int? MessageId { get; set; }

        [Display(ResourceType = typeof(Label), Name = "TitleNeighborhoodMessage")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "TitleIsRequired")]
        [MaxLength(70, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Error))]
        public string Title { get; set; }

        public bool AllowSharing { get; set; }

        public bool HasImage(int index)
        {
            switch (index)
            {
                case 1:
                    return this.Image1Id.HasValue;
                case 2:
                    return this.Image2Id.HasValue;
                case 3:
                    return this.Image3Id.HasValue;
                case 4:
                    return this.Image4Id.HasValue;
                case 5:
                    return this.Image5Id.HasValue;
            }

            return false;
        }
    }
}