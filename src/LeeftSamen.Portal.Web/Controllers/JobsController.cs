// <copyright file="JobsController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Data.Models;

namespace LeeftSamen.Portal.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using AutoMapper;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Services;
    using LeeftSamen.Portal.Web.Models.Jobs;
    using LeeftSamen.Portal.Web.Utils;
    using System.Configuration;

    public class JobsController : BaseController
    {
        private readonly ICircleService circleService;

        public JobsController(ICurrentUserInformation currentUserInformation, ICircleService circleService)
            : base(currentUserInformation)
        {
            this.circleService = circleService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignToMe(int? circleId, int? id, FormCollection form)
        {
            var job = await this.circleService.GetUnassignedJobByIdAsync(circleId, id);
            if (job == null)
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserMemberOfCircle(job.Circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            await this.circleService.AssignUserToJobAsync(job, this.CurrentUser, this.PortalUrl);
            this.NotifyUserSuccess(Alert.JobAssignedToCurrentUser);

            return this.RedirectToRoute("CircleSubResources", new { controller = "Jobs", action = "Index", circleId });
        }

        [HttpGet]
        public async Task<ActionResult> AssignToMe(int? circleId, int? id)
        {
            if (!circleId.HasValue)
            {
                return this.HttpNotFound();
            }

            var job = await this.circleService.GetUnassignedJobByIdAsync(circleId, id);
            if (job == null)
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserMemberOfCircle(job.Circle, this.CurrentUser) || job.Circle.CircleId != circleId)
            {
                return this.HttpForbidden();
            }

            if (job.Assignee != null)
            {
                if (job.Assignee.Id != this.CurrentUser.Id)
                {
                    this.NotifyUserDanger(Alert.JobAssignedToOtherUser);
                }
                else
                {
                    this.NotifyUserSuccess(Alert.JobAssignedToCurrentUser);
                }
            }
            else
            {
                await this.circleService.AssignUserToJobAsync(job, this.CurrentUser, this.PortalUrl);
                this.NotifyUserSuccess(Alert.JobAssignedToCurrentUser);
            }

            return this.RedirectToRoute("CircleSubResources", new { controller = "Jobs", action = "Index", job.Circle.CircleId });
        }

        [HttpGet]
        public async Task<ActionResult> Create(int? circleId)
        {
            var circle = await this.circleService.GetCircleByIdAsync(circleId);
            if (circle == null)
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser) && !circle.IsPrivate)
            {
                return this.HttpForbidden();
            }

            return this.View(string.Format("Create{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), new CreateViewModel
                                           {
                                               CircleId = circle.CircleId,
                                               Weekdays = new Dictionary<string, bool>
                                                              {
                                                                  {Label.Monday, false},
                                                                  {Label.Tuesday, false},
                                                                  {Label.Wednesday, false},
                                                                  {Label.Thursday, false},
                                                                  {Label.Friday, false},
                                                                  {Label.Saturday, false},
                                                                  {Label.Sunday, false}
                                                              }
                                           });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(string jobSubmit, int? circleId, CreatePostModel model)
        {
            var circle = await this.circleService.GetCircleByIdAsync(circleId);
            if (circle == null)
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser) && !circle.IsPrivate)
            {
                return this.HttpForbidden();
            }

            if ( ! model.Repeat)
            {
                this.ModelState.Remove("CompletionDateTime");
            }
            else
            {
                if (!model.Weekdays.Any(x => x.Value))
                {
                    this.ModelState.AddModelError("Weekdays", Error.WeekdayIsRequired);
                }

                if (model.CompletionDateTime < model.DueDateTime)
                {
                    this.ModelState.AddModelError("CompletionDateTime", Error.JobEndDateBeforeStartDate);
                }
            }

            if (this.ModelState.IsValid)
            {
                // Hacky
                if (model.DueDateTimeHour.HasValue && model.DueDateTimeMinute.HasValue)
                {
                    model.DueDateTime =
                        model.DueDateTime.Date.AddHours(model.DueDateTimeHour.Value).AddMinutes(model.DueDateTimeMinute.Value);
                }

                model.CompletionDateTime = model.CompletionDateTime.Date.AddHours(model.CompletionDateTimeHour)
                        .AddMinutes(model.CompletionDateTimeMinute);
                DateTime? dueDateTimeEnd = null;
                if (model.DueDateTimeEndHour.HasValue && model.DueDateTimeEndMinute.HasValue)
                {
                    dueDateTimeEnd = model.DueDateTime.Date.AddHours(model.DueDateTimeEndHour.Value).AddMinutes(model.DueDateTimeEndMinute.Value);
                }

                var membershipIds = new List<int>();
                if (model.IsOnlyVisibleToSelectedMembers && !string.IsNullOrWhiteSpace(model.SelectedMembershipIds))
                {
                    try
                    {
                        membershipIds.AddRange(model.SelectedMembershipIds.Split(',').Select(n => Convert.ToInt32(n)));
                    }
                    catch (Exception)
                    {
                        // TODO: Lege try-catch?
                    }
                }

                // Handle repeated Jobs
                var jobDateTimes = new List<DateTime>();
                var start = model.DueDateTime;
                var end = (model.CompletionDateTime < start)
                                ? model.DueDateTime
                                : model.CompletionDateTime;

                if (start == end || ! model.Repeat)
                {
                    jobDateTimes.Add(start);
                }
                else
                {
                    var rm = Label.ResourceManager;

                    // Calculate all the dates that should repeat the Job
                    for (var dt = start; dt <= end; dt = dt.AddDays(1))
                    {
                        if (model.Weekdays.Any(x => x.Key == rm.GetString(dt.DayOfWeek.ToString()) && x.Value))
                        {
                            jobDateTimes.Add(dt);
                        }
                    }
                }

                var sendEmail = true;
                foreach (var jobDateTime in jobDateTimes)
                {
                    // Add each job to the database TODO: keep dates in separate table?
                    await this.circleService.CreateJobAsync(
                                model.Title,
                                model.Description,
                                !model.HasNoDueDate,
                                jobDateTime,
                                dueDateTimeEnd,
                                end,
                                circle,
                                this.CurrentUser,
                                model.IsOnlyVisibleToSelectedMembers,
                                membershipIds,
                                this.PortalUrl,
                                sendEmail);

                    sendEmail = false;
                }

                this.NotifyUserSuccess(Alert.JobCreated);

                // Does the user want to return to the Job form?
                if (jobSubmit == "new")
                {
                    return this.RedirectToAction("Create", new { circleId });
                }

                return this.RedirectToRoute(
                "CircleSubResources",
                new { controller = "Jobs", circleId = circle.CircleId });
            }

            var viewModel = CreateViewModel.FromPostModel(model);
            viewModel.CircleId = circle.CircleId;

            this.SetStatusCode(HttpStatusCode.BadRequest);
            return this.View(string.Format("Create{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? jobId)
        {
            if (!jobId.HasValue || this.CurrentUserCanOnlyView())
            {
                return this.HttpNotFound();
            }

            var job = await this.circleService.GetJobByIdAsync(jobId);
            if (job == null)
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserAdminOfCircle(job.Circle, this.CurrentUser) && (job.Creator != null ? job.Creator.Id : null) != this.CurrentUser.Id)
            {
                return this.HttpForbidden();
            }

            var model = Mapper.Map<EditPostModel>(job);
            model.CircleId = job.Circle.CircleId;
            model.JobId = job.JobId;
            model.Weekdays = new Dictionary<string, bool>
            {
                {Label.Monday, false},
                {Label.Tuesday, false},
                {Label.Wednesday, false},
                {Label.Thursday, false},
                {Label.Friday, false},
                {Label.Saturday, false},
                {Label.Sunday, false}
            };

            return this.View(string.Format("Edit{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty), model);
        }

        [HttpGet]
        public async Task<ActionResult> Index(int? circleId)
        {
            var circle = await this.circleService.GetCircleByIdAsync(circleId);
            if (circle == null)
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserMemberOfCircle(circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            var assignedJobs = await this.circleService.GetAssignedJobsAsync(circle, this.CurrentUser);
            var unassignedJobs = await this.circleService.GetUnassignedJobsAsync(circle, this.CurrentUser);

            this.ViewBag.CircleId = circle.CircleId;

            return this.View(
                string.Format("Index{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty),
                new IndexViewModel
                    {
                        CircleId = circle.CircleId,
                        CircleIsPrivate = circle.IsPrivate,
                        CurrentUserId = this.CurrentUser.Id,
                        AssignedJobs = Mapper.Map<List<IndexViewModel.JobViewModel>>(assignedJobs),
                        UnassignedJobs = Mapper.Map<List<IndexViewModel.JobViewModel>>(unassignedJobs),
                        CurrentUserIsCircleAdministrator =
                            this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser)
                    });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? jobId, EditPostModel editedJob)
        {
            if (!jobId.HasValue || this.CurrentUserCanOnlyView())
            {
                return this.HttpNotFound();
            }

            var job = await this.circleService.GetJobByIdAsync(jobId);
            if (job == null)
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserAdminOfCircle(job.Circle, this.CurrentUser) && (job.Creator != null ? job.Creator.Id : null) != this.CurrentUser.Id)
            {
                return this.HttpForbidden();
            }

            var memberList = new List<int>();

            if (editedJob.SelectedMembershipIds != "")
                try
                {
                    memberList.AddRange(editedJob.SelectedMembershipIds.Split(',').Select(n => Convert.ToInt32(n)));
                }
                catch (Exception e)
                {
                    editedJob.IsOnlyVisibleToSelectedMembers = false;
                }

            if (editedJob.DueDateTimeHour.HasValue && editedJob.DueDateTimeMinute.HasValue)
            {
                editedJob.DueDateTime = editedJob.DueDateTime.Date.AddHours(editedJob.DueDateTimeHour.Value).AddMinutes(editedJob.DueDateTimeMinute.Value);
            }

            DateTime? dueDateTimeEnd = null;
            if (editedJob.DueDateTimeEndHour.HasValue && editedJob.DueDateTimeEndMinute.HasValue)
            {
                dueDateTimeEnd = editedJob.DueDateTime.Date.AddHours(editedJob.DueDateTimeEndHour.Value).AddMinutes(editedJob.DueDateTimeEndMinute.Value);
            }

            editedJob.CompletionDateTime = editedJob.CompletionDateTime.Date.AddHours(editedJob.CompletionDateTimeHour).AddMinutes(editedJob.CompletionDateTimeMinute);

            await this.circleService.SaveJobAsync(
                    jobId.Value,
                    editedJob.Title,
                    editedJob.Description,
                    !editedJob.HasNoDueDate,
                    editedJob.DueDateTime,
                    dueDateTimeEnd,
                    editedJob.CompletionDateTime,
                    editedJob.IsOnlyVisibleToSelectedMembers,
                    memberList,
                    this.PortalUrl
                );

            this.NotifyUserSuccess(Alert.JobEdited);

            return this.RedirectToAction("Index", new {circleId = job.Circle.CircleId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Remove(int? circleId, int? id, FormCollection form)
        {
            var circle = await this.circleService.GetCircleByIdAsync(circleId);
            if (circle == null)
            {
                return this.HttpNotFound();
            }

            var job = await this.circleService.GetJobByIdAsync(id);
            if (job == null)
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserAdminOfCircle(circle, this.CurrentUser) && (job.Creator != null ? job.Creator.Id : null) != this.CurrentUser.Id)
            {
                return this.HttpForbidden();
            }

            await this.circleService.RemoveJobAsync(circle, id);
            this.NotifyUserSuccess(Alert.JobRemoved);

            return this.Remove(circleId, id);
        }

        [HttpGet]
        public ActionResult Remove(int? circleId, int? id)
        {
            if (!circleId.HasValue)
            {
                return this.HttpNotFound();
            }

            return this.RedirectToRoute("CircleSubResources", new { controller = "Jobs", action = "Index", circleId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UnassignToMe(int? circleId, int? id, FormCollection form)
        {
            var job = await this.circleService.GetUnassignedJobByIdAsync(circleId, id);
            if (job == null)
            {
                return this.HttpNotFound();
            }

            if (!this.circleService.IsUserMemberOfCircle(job.Circle, this.CurrentUser))
            {
                return this.HttpForbidden();
            }

            await this.circleService.UnassignUserToJobAsync(job, this.CurrentUser, this.PortalUrl);
            this.NotifyUserSuccess(Alert.JobUnassigned);

            return this.RedirectToRoute("CircleSubResources", new { controller = "Jobs", action = "Index", circleId });
        }
    }
}