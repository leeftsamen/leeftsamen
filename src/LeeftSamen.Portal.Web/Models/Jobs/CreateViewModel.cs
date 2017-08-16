// <copyright file="CreateViewModel.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Models.Jobs
{
    /// <summary>
    /// The create view model.
    /// </summary>
    public class CreateViewModel : CreatePostModel
    {
        /// <summary>
        /// Gets or sets the circle id.
        /// </summary>
        public int CircleId { get; set; }

        /// <summary>
        /// The from post model.
        /// </summary>
        /// <param name="postModel">
        /// The post model.
        /// </param>
        /// <returns>
        /// The <see cref="CreateViewModel"/>.
        /// </returns>
        public static CreateViewModel FromPostModel(CreatePostModel postModel)
        {
            return new CreateViewModel
            {
                Title = postModel.Title,
                Description = postModel.Description,
                CompletionDateTime = postModel.CompletionDateTime,
                CompletionDateTimeHour = postModel.CompletionDateTimeHour,
                CompletionDateTimeMinute = postModel.CompletionDateTimeMinute,
                HasCompletionDateTimeHour= postModel.HasCompletionDateTimeHour,
                HasCompletionDateTimeMinute = postModel.HasCompletionDateTimeMinute,
                HasNoDueDate = postModel.HasNoDueDate,
                DueDateTime = postModel.DueDateTime,
                DueDateTimeHour = postModel.DueDateTimeHour,
                DueDateTimeMinute = postModel.DueDateTimeMinute,
                HasDueDateTimeHour = postModel.HasDueDateTimeHour,
                HasDueDateTimeMinute = postModel.HasDueDateTimeMinute,
                DueDateTimeEndHour = postModel.DueDateTimeHour,
                DueDateTimeEndMinute = postModel.DueDateTimeMinute,
                HasDueDateTimeEndHour = postModel.HasDueDateTimeHour,
                HasDueDateTimeEndMinute = postModel.HasDueDateTimeMinute,
                IsOnlyVisibleToSelectedMembers = postModel.IsOnlyVisibleToSelectedMembers,
                SelectedMembershipIds = postModel.SelectedMembershipIds,
                Repeat = postModel.Repeat,
                Weekdays = postModel.Weekdays
            };
        }
    }
}