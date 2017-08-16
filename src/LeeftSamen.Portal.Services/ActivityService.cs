// <copyright file="ActivityService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Threading.Tasks;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Data;
    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.EmailTemplates.Models;
    using Data.Enums;

    public class ActivityService : IActivityService
    {
        private readonly IApplicationDbContext databaseContext;

        private readonly ILinkGenerator linkGenerator;

        private readonly IMailerService mailerService;

        private readonly INotificationService notificationService;

        private readonly IRandomGenerator randomGenerator;

        public ActivityService(
            IApplicationDbContext databaseContext,
            ILinkGenerator linkGenerator,
            INotificationService notificationService,
            IMailerService mailerService,
            IRandomGenerator randomGenerator)
        {
            this.databaseContext = databaseContext;
            this.linkGenerator = linkGenerator;
            this.notificationService = notificationService;
            this.mailerService = mailerService;
            this.randomGenerator = randomGenerator;
        }

        public async Task AcceptInvitationAsync(ActivityInvitation invitation, User user)
        {
            await this.SetAttendanceAsync(invitation.Activity, invitation.User, true);

            await
                this.SendNotificationInvitationAcceptedToInvitingUser(invitation.Activity, user, invitation.InvitedBy)
                    .ConfigureAwait(false);

            this.databaseContext.ActivityInvitations.Remove(invitation);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<ActivityReaction> CreateReactionAsync(
            Activity activity,
            User currentUser,
            OrganizationMembership organizationMembership,
            string text)
        {
            var reaction = this.databaseContext.ActivityReactions.Create();
            reaction.CreationDateTime = DateTime.Now;
            reaction.Activity = activity;
            reaction.Creator = currentUser;
            reaction.Text = text;
            reaction.OrganizationMembership = organizationMembership;

            this.databaseContext.ActivityReactions.Add(reaction);
            activity.Reactions.Add(reaction);

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            return reaction;
        }

        public async Task DeleteAsync(Activity activity)
        {
            var invitations = this.databaseContext.ActivityInvitations.Where(i => i.Activity.ActivityId == activity.ActivityId);
            foreach (var invitation in invitations)
            {
                this.databaseContext.ActivityInvitations.Remove(invitation);
            }

            //var items = this.databaseContext.
            this.databaseContext.Activities.Remove(activity);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<Activity> GetActivityByIdAsync(int activityId)
        {
            return
                await
                this.databaseContext.Activities.FirstOrDefaultAsync(a => a.ActivityId == activityId)
                    .ConfigureAwait(false);
        }

        public async Task<Activity> GetActivityByIdForPositionAsync(int? activityId, DbGeography position, int radius)
        {
            return
                await
                this.databaseContext.Activities.FirstOrDefaultAsync(
                    a =>
                    a.ActivityId == activityId && a.Position.Distance(position) <= radius)
                    .ConfigureAwait(false);
        }

        public async Task<List<IActivity>> GetCurrentAndUpcomingActivitiesCreateByUserAsync(
            User user,
            int? circleId,
            int maxCount = 100)
        {
            var query = this.databaseContext.Activities.Where(a => a.Creator.Id == user.Id);
            if (circleId.HasValue)
            {
                query = query.Where(a => a.Circle.CircleId == circleId.Value);
            }
            else
            {
                query = query.Where(a => a.Circle == null);
            }

            var activities =
                await
                query.Where(a => a.EndDateTime >= DateTime.Today).Take(maxCount).ToListAsync().ConfigureAwait(false);
            var intervalActivities =
                await query.Where(a => a.Recurring != Activity.Recurrance.No).ToListAsync().ConfigureAwait(false);
            return CombineActivitiesWithInterval(activities, intervalActivities, maxCount);
        }

        public async Task<List<IActivity>> GetCurrentAndUpcomingFilteredByAgeAsync(string userId, int? circleId, DbGeography position, int radius, int age, int maxCount = 100)
        {
            var query =
                this.databaseContext.Activities.Where(
                    a =>
                    ((a.Position.Distance(position) <= radius || a.Attendees.Any(at => at.User.Id == userId)) && (a.AgeFrom ?? 0) <= age && ((a.AgeTo ?? int.MaxValue) >= age || a.AgeTo == 0)));
                     //));
            if (circleId.HasValue)
            {
                query = query.Where(a => a.Circle.CircleId == circleId.Value);
            }
            else
            {
                query = query.Where(a => a.Circle == null);
            }

            var activities =
                await
                query.Where(a => a.StartDateTime >= DateTime.Today).Take(maxCount).ToListAsync().ConfigureAwait(false);
            var intervalActivities =
                await query.Where(a => a.Recurring != Activity.Recurrance.No).ToListAsync().ConfigureAwait(false);
            return CombineActivitiesWithInterval(activities, intervalActivities, maxCount);
        }

        public async Task<List<IActivity>> GetCurrentAndUpcomingAsync(string userId, int? circleId, DbGeography position, int radius, int maxCount = 500)
        {
            var query =
                this.databaseContext.Activities.Where(
                    a =>
                    (a.Position.Distance(position) <= radius
                     || a.Attendees.Any(at => at.User.Id == userId)));
            if (circleId.HasValue)
            {
                query = query.Where(a => a.Circle.CircleId == circleId.Value);
            }
            else
            {
                query = query.Where(a => a.Circle == null);
            }

            var activities =
                await
                query.Where(a => a.StartDateTime >= DateTime.Today).Take(maxCount).ToListAsync().ConfigureAwait(false);
            var intervalActivities =
                await query.Where(a => a.Recurring != Activity.Recurrance.No).ToListAsync().ConfigureAwait(false);
            return CombineActivitiesWithInterval(activities, intervalActivities, maxCount);
        }

        public async Task<List<IActivity>> GetCurrentAndUpcomingOrganizationActivitiesAsync(
            Organization organization,
            int maxCount = 100)
        {
            var query = this.databaseContext.Activities.Where(a => a.OrganizationMembership.Organization.OrganizationId == organization.OrganizationId);
            var activities =
                await
                query.Where(a => a.EndDateTime >= DateTime.Today).Take(maxCount).ToListAsync().ConfigureAwait(false);
            var intervalActivities =
                await query.Where(a => a.Recurring != Activity.Recurrance.No).ToListAsync().ConfigureAwait(false);
            return CombineActivitiesWithInterval(activities, intervalActivities, maxCount);
        }

        public async Task<ActivityInvitation> GetInvitationByActivityIdAcceptTokenAsync(int? id, string acceptToken)
        {
            return
                await
                this.databaseContext.ActivityInvitations.FirstOrDefaultAsync(
                    i => i.Activity.ActivityId == id && i.AcceptToken == acceptToken).ConfigureAwait(false);
        }

        public async Task<List<Activity>> GetOrganizationActivitiesAsync(Organization organization)
        {
            return
                await
                this.databaseContext.Activities.Where(
                    a => a.OrganizationMembership.Organization.OrganizationId == organization.OrganizationId)
                    .OrderByDescending(a => a.CreationDate)
                    .Take(100)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<Activity> InsertAsync(string title, string description, DateTime startDateTime, DateTime endDateTime, string location, bool allDay, User creator, bool allowSharing, bool allAges, int ageFrom, int ageTo, Activity.Recurrance recurring, DateTime? recurringEnd, OrganizationMembership organizationMembership = null, Circle circle = null)
        {
            var activity = this.databaseContext.Activities.Create();
            activity.Title = title;
            activity.Description = description;
            activity.StartDateTime = startDateTime;
            activity.EndDateTime = endDateTime;
            activity.Location = location;
            activity.Position = creator.Position;
            activity.AllDay = allDay;
            activity.Creator = creator;
            activity.OrganizationMembership = organizationMembership;
            activity.Circle = circle;
            activity.CreationDate = DateTime.Now;
            activity.AllowSharing = allowSharing;
            activity.AllAges = allAges || (ageFrom == 0 && ageTo == 0);
            activity.AgeFrom = ageFrom > 0 ? (int?)ageFrom : null;
            activity.AgeTo = ageTo > 0 ? (int?)ageTo : null;
            activity.Recurring = recurring;
            activity.RecurringEnd = recurringEnd;
            this.databaseContext.Activities.Add(activity);

            var attendance = this.databaseContext.ActivityAttendances.Create();
            attendance.User = creator;
            attendance.ModificationDate = DateTime.Now;
            attendance.Attending = true;

            activity.Attendees.Add(attendance);

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            await this.SetActivityIntervalsAsync(activity).ConfigureAwait(false);

            return activity;
        }

        public async Task InviteUserAsync(Activity activity, User user, User invitedBy, string portalUrl)
        {
            if (!(await this.ShouldInviteUser(activity, user).ConfigureAwait(false)))
            {
                return;
            }

            var activityInvitation =
                await this.CreateActivityInvitationAsync(activity, invitedBy, user).ConfigureAwait(false);

            await this.SendInvitationEmailAsync(activityInvitation, portalUrl).ConfigureAwait(false);
            await this.SendNotificationInviteUserAsync(user, activity).ConfigureAwait(false);
        }

        public async Task InviteUserByEmailAsync(
            Activity activity,
            IEnumerable<string> emailAddresses,
            User invitedBy,
            string portalUrl)
        {
            foreach (var emailAddress in emailAddresses.Where(this.mailerService.IsValidEmail))
            {
                var email = emailAddress;

                // TODO: Show notification: user already has a pending invitation
                var user =
                    await this.databaseContext.Users.FirstOrDefaultAsync(u => u.Email == email).ConfigureAwait(false);
                if (user != null && !(await this.ShouldInviteUser(activity, user).ConfigureAwait(false)))
                {
                    continue;
                }

                if (!(await this.ShouldInviteEmail(activity, email).ConfigureAwait(false)))
                {
                    continue;
                }

                var activityInvitation =
                    await this.CreateActivityInvitationAsync(activity, invitedBy, user, email).ConfigureAwait(false);

                await this.SendInvitationEmailAsync(activityInvitation, portalUrl).ConfigureAwait(false);
            }
        }

        public async Task<List<User>> SearchNeighborsAsync(string userId, DbGeography position, int radius, string q)
        {
            return
                await
                this.databaseContext.Users.Where(u => u.Position.Distance(position) <= radius)
                    .Where(u => u.Name.Contains(q))
                    .Where(u => u.Id != userId)
                    .OrderBy(u => u.Name)
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task SetAttendanceAsync(Activity activity, User user, bool attending)
        {
            var attendance =
                await
                this.databaseContext.ActivityAttendances.FirstOrDefaultAsync(
                    a => a.User.Id == user.Id && a.Activity.ActivityId == activity.ActivityId).ConfigureAwait(false);
            if (attendance == null)
            {
                attendance = this.databaseContext.ActivityAttendances.Create();
                attendance.Activity = activity;
                attendance.User = user;

                this.databaseContext.ActivityAttendances.Add(attendance);
            }

            attendance.ModificationDate = DateTime.Now;
            attendance.Attending = attending;

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            await this.SendNotificationAttendance(activity, user, attending).ConfigureAwait(false);
        }

        public async Task<Activity> UpdateAsync(Activity activity, string title, string description, DateTime startDateTime, DateTime endDateTime, string location, bool allDay, bool allowSharing, bool allAges, int ageFrom, int ageTo, Activity.Recurrance recurring, DateTime? recurringEnd)
        {
            activity.Title = title;
            activity.Description = description;
            activity.StartDateTime = startDateTime;
            activity.EndDateTime = endDateTime;
            activity.Location = location;
            activity.AllDay = allDay;
            activity.AllowSharing = allowSharing;
            activity.AllAges = allAges || (ageFrom == 0 && ageTo == 0);
            activity.AgeFrom = ageFrom > 0 ? (int?)ageFrom : null;
            activity.AgeTo = ageTo > 0 ? (int?)ageTo : null;
            activity.Recurring = recurring;
            activity.RecurringEnd = recurringEnd;
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            await this.SetActivityIntervalsAsync(activity).ConfigureAwait(false);
            return activity;
        }

        private static List<IActivity> CombineActivitiesWithInterval(
            ICollection<Activity> activities,
            ICollection<Activity> intervalActivities,
            int maxCount = 100)
        {
            var result = new List<IActivity>();
            result.AddRange(activities);

            if (intervalActivities.Count <= 0)
            {
                return result.OrderBy(a => a.StartDateTime).Take(maxCount).ToList();
            }

            var startDate = DateTime.Today.AddDays(1);
            while (result.Count < maxCount && startDate < DateTime.MaxValue.AddDays(-1))
            {
                foreach (var activity in intervalActivities)
                {
                    if (startDate < activity.StartDateTime || (activity.RecurringEnd.HasValue && activity.RecurringEnd.Value < startDate))
                    {
                        continue;
                    }

                    var checkDayOfWeek = (activity.Recurring == Activity.Recurrance.Day
                                          || activity.Recurring == Activity.Recurrance.WorkDay
                                          || activity.Recurring == Activity.Recurrance.Week)
                                         && activity.Intervals.Any(i => i.RepeatWeekDay == (int)startDate.DayOfWeek);

                    var checkDay = activity.Recurring == Activity.Recurrance.Month
                                   && activity.Intervals.Any(i => i.RepeatDay == startDate.Day);

                    // var checkDayOfWeekAndWeekOfMonth = activity.Recurring == Activity.Recurrance.TwoWeek && activity.Intervals.Any(i => i.RepeatWeekDay == (int)startDate.DayOfWeek && i.RepeatMonthWeek == startDate.WeekOfMonth());
                    var checkMonthAndDay = activity.Recurring == Activity.Recurrance.Year
                                           && activity.Intervals.Any(
                                               i => i.RepeatDay == startDate.Day && i.RepeatMonth == startDate.Month);

                    if (!checkDayOfWeek && !checkDay && !checkMonthAndDay)
                    {
                        continue;
                    }

                    var timespan = startDate - activity.StartDateTime;
                    result.Add(
                        new IntervalActivity
                            {
                                ActivityId = activity.ActivityId,
                                Title = activity.Title,
                                Location = activity.Location,
                                StartDateTime =
                                    startDate.AddHours(activity.StartDateTime.Hour)
                                    .AddMinutes(activity.StartDateTime.Minute),
                                EndDateTime = activity.EndDateTime.Add(timespan),
                                OriginalStartDateTime = activity.StartDateTime,
                                OriginalEndDateTime = activity.EndDateTime,
                                Attendees = activity.Attendees,
                                AllDay = activity.AllDay,
                                AllowSharing = activity.AllowSharing,
                                AllAges = activity.AllAges,
                                AgeFrom = activity.AgeFrom,
                                AgeTo = activity.AgeTo
                            });
                }

                startDate = startDate.AddDays(1);
            }

            return result.OrderBy(a => a.StartDateTime).Take(maxCount).ToList();
        }

        private async Task<ActivityInvitation> CreateActivityInvitationAsync(
            Activity activity,
            User invitedBy,
            User user,
            string email = null)
        {
            var activityInvitation = this.databaseContext.ActivityInvitations.Create();

            activityInvitation.Activity = activity;
            activityInvitation.InvitationDateTime = DateTime.Now;
            activityInvitation.InvitedBy = invitedBy;
            activityInvitation.AcceptToken = this.randomGenerator.GenerateRandomToken();
            if (user != null)
            {
                activityInvitation.User = user;
            }
            else
            {
                activityInvitation.Email = email;
            }

            this.databaseContext.ActivityInvitations.Add(activityInvitation);
            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);

            return activityInvitation;
        }

        private async Task SendInvitationEmailAsync(ActivityInvitation invitation, string portalUrl)
        {
            int? circleId = null;
            if (invitation.Activity.Circle != null)
            {
                circleId = invitation.Activity.Circle.CircleId;
            }

            var acceptInvitationUrl =
                this.linkGenerator.GenerateActivityAcceptInvitationLink(
                    invitation.Activity.ActivityId,
                    invitation.AcceptToken);
            var viewInvitationUrl =
                this.linkGenerator.GenerateActivityLink(
                    invitation.Activity.ActivityId, circleId);

            var model = new ActivityInvitationModel
                            {
                                Subject = Subject.ActivityInvitation,
                                AcceptInvitationUrl = acceptInvitationUrl,
                                ViewInvitationUrl = viewInvitationUrl,
                                ActivityTitle = invitation.Activity.Title,
                                InvitedByName = invitation.InvitedBy.Name,
                                PortalUrl = portalUrl
                            };

            if (invitation.User != null)
            {
                model.Name = invitation.User.Name;
                await this.mailerService.SendAsync(model, invitation.User).ConfigureAwait(false);
            }
            else
            {
                await this.mailerService.SendAsync(model, invitation.Email).ConfigureAwait(false);
            }
        }

        private async Task<bool> SendNotificationAttendance(Activity activity, User user, bool attending)
        {
            int? circleId = null;
            if (activity.Circle != null)
            {
                circleId = activity.Circle.CircleId;
            }

            var link = this.linkGenerator.GenerateActivityLink(activity.ActivityId, circleId);
            var message =
                string.Format(
                    attending
                        ? Common.InterfaceText.Notification.ActivityAttending
                        : Common.InterfaceText.Notification.ActivityNotAttending,
                    user.Name,
                    activity.Title);

            var settingType = attending
                        ? SettingName.ActivityAttending
                        : SettingName.ActivityNotAttending;
            await
                this.notificationService.CreateNotificationForUserAsync(activity.Creator, message, link, settingType)
                    .ConfigureAwait(false);
            return true;
        }

        private async Task SendNotificationInvitationAcceptedToInvitingUser(Activity activity, User user, User inviteBy)
        {
            var link = this.linkGenerator.GenerateActivityAcceptedLink(activity.ActivityId);
            var message = string.Format(
                Common.InterfaceText.Notification.ActivityInvitationAccepted,
                user.Name,
                activity.Title);

            //await this.notificationService.CreateNotificationForUserAsync(inviteBy, message, link, SettingName.ActivityInvitationAccepted).ConfigureAwait(false);
        }

        private async Task<bool> SendNotificationInviteUserAsync(User user, Activity activity)
        {
            var link = this.linkGenerator.GenerateActivityAcceptedLink(activity.ActivityId);
            var message = string.Format(Common.InterfaceText.Notification.ActivityInvitation, activity.Title);

            await this.notificationService.CreateNotificationForUserAsync(user, message, link, SettingName.ActivityInvitation).ConfigureAwait(false);

            return true;
        }

        private async Task SetActivityIntervalsAsync(Activity activity)
        {
            foreach (var i in activity.Intervals.ToList())
            {
                this.databaseContext.ActivityIntervals.Remove(i);
            }

            ActivityInterval interval;
            switch (activity.Recurring)
            {
                case Activity.Recurrance.Day:
                    for (var i = 0; i <= 6; i++)
                    {
                        interval = this.databaseContext.Create<ActivityInterval>();
                        interval.RepeatWeekDay = i;
                        activity.Intervals.Add(interval);
                    }

                    break;
                case Activity.Recurrance.WorkDay:
                    for (var i = 0; i <= 4; i++)
                    {
                        interval = this.databaseContext.Create<ActivityInterval>();
                        interval.RepeatWeekDay = i;
                        activity.Intervals.Add(interval);
                    }

                    break;
                case Activity.Recurrance.Week:
                    interval = this.databaseContext.Create<ActivityInterval>();
                    interval.RepeatWeekDay = (int)activity.StartDateTime.DayOfWeek;
                    activity.Intervals.Add(interval);
                    break;

                // case Activity.Recurrance.TwoWeek:
                // var week = activity.StartDateTime.WeekOfMonth();
                // week = week % 2 == 0 ? 2 : 1;
                // var c = week == 1 ? 3 : 2;
                // for (var w = 0; w < c; w++)
                // {
                // interval = this.databaseContext.Create<ActivityInterval>();
                // interval.RepeatMonthWeek = week + (w * 2);
                // interval.RepeatWeekDay = (int)activity.StartDateTime.DayOfWeek;
                // activity.Intervals.Add(interval);
                // }
                // break;
                case Activity.Recurrance.Month:
                    interval = this.databaseContext.Create<ActivityInterval>();
                    interval.RepeatDay = activity.StartDateTime.Day;
                    activity.Intervals.Add(interval);
                    break;
                case Activity.Recurrance.Year:
                    interval = this.databaseContext.Create<ActivityInterval>();
                    interval.RepeatDay = activity.StartDateTime.Day;
                    interval.RepeatMonth = activity.StartDateTime.Month;
                    activity.Intervals.Add(interval);
                    break;
            }

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }

        private async Task<bool> ShouldInviteEmail(Activity activity, string emailAddress)
        {
            var emailIsInvited =
                await
                this.databaseContext.ActivityInvitations.AnyAsync(
                    i => i.Activity.ActivityId == activity.ActivityId && i.Email == emailAddress).ConfigureAwait(false);

            return !emailIsInvited;
        }

        private async Task<bool> ShouldInviteUser(Activity activity, User user)
        {
            var activityEntry = this.databaseContext.Entry(activity);
            var userIsAttending =
                await
                activityEntry.Collection(u => u.Attendees)
                    .Query()
                    .AnyAsync(a => a.User.Id == user.Id && a.Attending)
                    .ConfigureAwait(false);

            var userEntry = this.databaseContext.Entry(user);
            var userIsInvited =
                await
                userEntry.Collection(u => u.ActivityInvitations)
                    .Query()
                    .AnyAsync(a => a.Activity.ActivityId == activity.ActivityId)
                    .ConfigureAwait(false);

            return !userIsInvited && !userIsAttending;
        }
    }
}