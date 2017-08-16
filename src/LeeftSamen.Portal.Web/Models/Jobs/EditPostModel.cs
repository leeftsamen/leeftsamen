// <copyright file="EditPostModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LeeftSamen.Common.InterfaceText;

namespace LeeftSamen.Portal.Web.Models.Jobs
{
    public class EditPostModel
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [Display(ResourceType = typeof(Label), Name = "Description")]
        //[Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "DescriptionIsRequired")]
        [MaxLength(4000, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Error))]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the completion date time.
        /// </summary>
        [Display(ResourceType = typeof(Label), Name = "CompletionDateTime")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "CompletionDateTimeIsRequired")]
        public DateTime CompletionDateTime { get; set; }

        /// <summary>
        /// Gets or sets the completion date time hour.
        /// </summary>
        [Required]
        [Range(0, 23)]
        [Display(ResourceType = typeof(Label), Name = "DateTime")]
        public int CompletionDateTimeHour { get; set; }

        /// <summary>
        /// Gets or sets the completion date time minute.
        /// </summary>
        [Required]
        [Range(0, 59)]
        [Display(ResourceType = typeof(Label), Name = "DateTime")]
        public int CompletionDateTimeMinute { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether has completion date time hour.
        /// </summary>
        public bool HasCompletionDateTimeHour { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether has completion date time minute.
        /// </summary>
        public bool HasCompletionDateTimeMinute { get; set; }

        [Display(ResourceType = typeof(Label), Name = "HasNoDueDate")]
        public bool HasNoDueDate { get; set; }

        /// <summary>
        /// Gets or sets the due date time.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "DateIsRequired")]
        [Display(ResourceType = typeof(Label), Name = "DueDateTime")]
        public DateTime DueDateTime { get; set; }

        /// <summary>
        /// Gets or sets the due date time hour.
        /// </summary>
        //[Required]
        [Range(0, 23)]
        [Display(ResourceType = typeof(Label), Name = "DateTime")]
        public int? DueDateTimeHour { get; set; }

        /// <summary>
        /// Gets or sets the due date time minute.
        /// </summary>
        //[Required]
        [Range(0, 59)]
        [Display(ResourceType = typeof(Label), Name = "DateTime")]
        public int? DueDateTimeMinute { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether has due date time hour.
        /// </summary>
        public bool HasDueDateTimeHour { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether has due date time minute.
        /// </summary>
        public bool HasDueDateTimeMinute { get; set; }

        /// <summary>
        /// Gets or sets the due date endtime hour.
        /// </summary>
        [Range(0, 23)]
        [Display(ResourceType = typeof(Label), Name = "DateTime")]
        public int? DueDateTimeEndHour { get; set; }

        /// <summary>
        /// Gets or sets the due date endtime minute.
        /// </summary>
        [Range(0, 59)]
        [Display(ResourceType = typeof(Label), Name = "DateTime")]
        public int? DueDateTimeEndMinute { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether has due date endtime hour.
        /// </summary>
        public bool HasDueDateTimeEndHour { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether has due date endtime minute.
        /// </summary>
        public bool HasDueDateTimeEndMinute { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is only visible to selected members.
        /// </summary>
        [Display(ResourceType = typeof(Label), Name = "JobVisibility")]
        public bool IsOnlyVisibleToSelectedMembers { get; set; }

        /// <summary>
        /// Gets or sets the selected membership ids.
        /// </summary>
        public string SelectedMembershipIds { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [Display(ResourceType = typeof(Label), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "TitleIsRequired")]
        [MaxLength(40, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Error))]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a job should be repeated
        /// </summary>
        [Display(ResourceType = typeof(Label), Name = "RepeatJob")]
        public bool Repeat { get; set; }

        /// <summary>
        /// Gets or sets weekdays to repeat the Job on
        /// </summary>
        [Display(ResourceType = typeof(Label), Name = "RepeatWeekdays")]
        [Required(ErrorMessageResourceType = typeof(Error), ErrorMessageResourceName = "WeekdayIsRequired")]
        public Dictionary<string, bool> Weekdays { get; set; }

        public int CircleId { get; set; }

        public int JobId { get; set; }
    }
}