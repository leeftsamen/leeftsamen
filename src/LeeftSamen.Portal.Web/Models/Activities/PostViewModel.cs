// <copyright file="PostViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Activities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Data.Models;

    /// <summary>
    /// The post view model.
    /// </summary>
    public class PostViewModel
    {
        /// <summary>
        /// Gets or sets the activity id.
        /// </summary>
        public int? ActivityId { get; set; }

        /// <summary>
        /// Gets or sets the circle id.
        /// </summary>
        public int? CircleId { get; set; }

        [Display(ResourceType = typeof(Label), Name = "AgeFrom")]
        public int? AgeFrom { get; set; }

        [Display(ResourceType = typeof(Label), Name = "AgeTo")]
        public int? AgeTo { get; set; }

        public bool AllAges { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all day.
        /// </summary>
        public bool AllDay { get; set; }

        public bool AllowSharing { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [Display(ResourceType = typeof(Label), Name = "Description")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "DescriptionIsRequired")]
        [AllowHtml]
        public string Description { get; set; }

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

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        [Display(ResourceType = typeof(Label), Name = "Location")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "LocationIsRequired")]
        [MaxLength(180, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Error))]
        public string Location { get; set; }

        [Display(ResourceType = typeof(Label), Name = "Recurring")]
        public Activity.Recurrance Recurring { get; set; }

        [Display(ResourceType = typeof(Label), Name = "RecurringEndsAt")]
        public DateTime? RecurringEnd { get; set; }

        /// <summary>
        /// Gets or sets the start date hour.
        /// </summary>
        public int StartDateHour { get; set; }

        /// <summary>
        /// Gets or sets the start date minute.
        /// </summary>
        public int StartDateMinute { get; set; }

        /// <summary>
        /// Gets or sets the start date time.
        /// </summary>
        [Display(ResourceType = typeof(Label), Name = "ActivityStartDate")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "ActivityStartDateIsRequired")]
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [Display(ResourceType = typeof(Label), Name = "TitleActivity")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "TitleIsRequired")]
        [MaxLength(80, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Error))]
        public string Title { get; set; }
    }
}