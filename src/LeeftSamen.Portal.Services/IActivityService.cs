// <copyright file="IActivityService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Spatial;
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data.Models;

    public interface IActivityService
    {
        Task<ActivityReaction> CreateReactionAsync(Activity activity, User currentUser, OrganizationMembership organizationMembership, string text);

        Task DeleteAsync(Activity activity);

        Task<Activity> GetActivityByIdForPositionAsync(int? activityId, DbGeography position, int radius);

        Task<Activity> GetActivityByIdAsync(int activityId);

        Task<List<IActivity>> GetCurrentAndUpcomingActivitiesCreateByUserAsync(User user, int? circleId, int maxCount = 100);

        Task<List<IActivity>> GetCurrentAndUpcomingOrganizationActivitiesAsync(Organization organization, int maxCount = 100);

        Task<List<IActivity>> GetCurrentAndUpcomingFilteredByAgeAsync(string userId, int? circleId, DbGeography position, int radius, int age, int maxCount = 100);

        Task<List<IActivity>> GetCurrentAndUpcomingAsync(string userId, int? circleId, DbGeography position, int radius, int maxCount = 500);

        Task<List<Activity>> GetOrganizationActivitiesAsync(Organization organization);

        Task<Activity> InsertAsync(string title, string description, DateTime startDateTime, DateTime endDateTime, string location, bool allDay, User creator, bool allowSharing, bool allAges, int ageFrom, int ageTo, Activity.Recurrance recurring, DateTime? recurringEnd, OrganizationMembership organizationMembership = null, Circle circle = null);

        Task SetAttendanceAsync(Activity activity, User user, bool attending);

        Task<Activity> UpdateAsync(Activity activity, string title, string description, DateTime startDateTime, DateTime endDateTime, string location, bool allDay, bool allowSharing, bool allAges, int ageFrom, int ageTo, Activity.Recurrance recurring, DateTime? recurringEnd);

        Task InviteUserAsync(Activity activity, User user, User invitedBy, string portalUrl);

        Task InviteUserByEmailAsync(
            Activity activity,
            IEnumerable<string> emailAddresses,
            User invitedBy,
            string portalUrl);

        Task<List<User>> SearchNeighborsAsync(string userId, DbGeography position, int radius, string q);

        Task<ActivityInvitation> GetInvitationByActivityIdAcceptTokenAsync(int? id, string acceptToken);

        Task AcceptInvitationAsync(ActivityInvitation invitation, User user);
    }
}