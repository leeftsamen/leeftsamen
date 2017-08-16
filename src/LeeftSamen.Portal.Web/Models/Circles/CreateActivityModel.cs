// <copyright file="CreateActivityModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Circles
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using System.Web.Mvc;

    using LeeftSamen.Common.InterfaceText;

    public class CreateActivityModel
    {
        public int CircleId { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [Display(ResourceType = typeof(Label), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "TitleIsRequired")]
        [MaxLength(40, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Error))]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [Display(ResourceType = typeof(Label), Name = "Description")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "DescriptionIsRequired")]
        [AllowHtml]
        [MaxLength(200, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Error))]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        [Display(ResourceType = typeof(Label), Name = "Location")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "LocationIsRequired")]
        [MaxLength(180, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Error))]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets if the activity is for all ages
        /// </summary>
        public bool AllAges { get; set; }

        [Display(ResourceType = typeof(Label), Name = "AgeFrom")]
        public int? AgeFrom { get; set; }

        [Display(ResourceType = typeof(Label), Name = "AgeTo")]
        public int? AgeTo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all day.
        /// </summary>
        public bool AllDay { get; set; }

        /// <summary>
        /// Gets or sets the start date time.
        /// </summary>
        [Display(ResourceType = typeof(Label), Name = "ActivityStartDate")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "ActivityStartDateIsRequired")]
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// Gets or sets the start date hour.
        /// </summary>
        public int StartDateHour { get; set; }

        /// <summary>
        /// Gets or sets the start date minute.
        /// </summary>
        public int StartDateMinute { get; set; }

        /// <summary>
        /// Gets or sets the end date hour.
        /// </summary>
        public int EndDateHour { get; set; }

        /// <summary>
        /// Gets or sets the end date minute.
        /// </summary>
        public int EndDateMinute { get; set; }

        /// <summary>
        /// Gets or sets the end date time.
        /// </summary>
        [Display(ResourceType = typeof(Label), Name = "ActivityEndDate")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "ActivityEndDateIsRequired")]
        public DateTime EndDateTime { get; set; }

        public bool AllowSharing { get; set; }
    }
}
