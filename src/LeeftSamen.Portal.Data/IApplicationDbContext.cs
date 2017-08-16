// <copyright file="IApplicationDbContext.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data
{
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data.Models;

    using Microsoft.AspNet.Identity.EntityFramework;

    public interface IApplicationDbContext
    {
        IDbSet<Action> Actions { get; set; }

        IDbSet<ActionVote> ActionVotes { get; set; }

        IDbSet<Activity> Activities { get; set; }

        IDbSet<ActivityAttendance> ActivityAttendances { get; set; }

        IDbSet<ActivityInvitation> ActivityInvitations { get; set; }

        IDbSet<CircleInvitation> CircleInvitations { get; set; }

        IDbSet<CircleJoinRequest> CircleJoinRequests { get; set; }

        IDbSet<CircleLabel> CircleLabels { get; set; }

        IDbSet<CircleSetting> CircleSettings { get; set; }

        IDbSet<CircleMembership> CircleMemberships { get; set; }

        IDbSet<CircleMessage> CircleMessages { get; set; }

        IDbSet<CircleMessageReaction> CircleMessageReactions { get; set; }

        IDbSet<CircleEmailGroup> CircleEmailGroups { get; set; }

        IDbSet<CircleEmailMessage> CircleEmailMessages { get; set; }

        IDbSet<CircleActivity> CircleActivities { get; set; }

        IDbSet<CircleActivityAttendance> CircleActivityAttendances { get; set; }

        IDbSet<CircleActivityInvitation> CircleActivityInvitations { get; set; }

        IDbSet<CircleActivityReaction> CircleActivityReactions { get; set; }

        IDbSet<CircleEmailMessageReceiver> CircleEmailMessageReceivers { get; set; }

        IDbSet<CirclePhotoAlbum> CirclePhotoAlbums { get; set; }

        IDbSet<CirclePhoto> CirclePhotos { get; set; }

        IDbSet<Circle> Circles { get; set; }

        IDbSet<FeaturedCircle> FeaturedCircles { get; set; }

        IDbSet<City> Cities { get; set; }

        IDbSet<District> Districts { get; set; }

        IDbSet<HelpIcon> HelpIcons { get; set; }

        IDbSet<HelpIconUser> HelpIconUsers { get; set; }

        IDbSet<Job> Jobs { get; set; }

        IDbSet<MarketplaceItemCategory> MarketplaceItemCategories { get; set; }

        IDbSet<MarketplaceItemReaction> MarketplaceItemReactions { get; set; }

        IDbSet<MarketplaceItem> MarketplaceItems { get; set; }

        IDbSet<MarketplaceItemTransaction> MarketplaceItemTransactions { get; set; }

        IDbSet<Media> Media { get; set; }

        IDbSet<NeighborhoodMessage> NeighborhoodMessages { get; set; }

        IDbSet<NeighborhoodMessageReaction> NeighborhoodMessageReactions { get; set; }

        IDbSet<Notification> Notifications { get; set; }

        IDbSet<OrganizationInvitation> OrganizationInvitations { get; set; }

        IDbSet<OrganizationProduct> OrganizationProducts { get; set; }

        IDbSet<OrganizationService> OrganizationServices { get; set; }

        IDbSet<OrganizationMembership> OrganizationMemberships { get; set; }

        IDbSet<OrganizationType> OrganizationTypes { get; set; }

        IDbSet<Organization> Organizations { get; set; }

        IDbSet<PushNotification> PushNotifications { get; set; }

        IDbSet<IdentityRole> Roles { get; set; }

        IDbSet<Setting> Settings { get; set; }

        IDbSet<Square> Squares { get; set; }

        IDbSet<SquareZipCode> SquareZipCodes { get; set; }

        IDbSet<SquareAdmin> SquareAdmins { get; set; }

        IDbSet<SquareFact> SquareFacts { get; set; }

        IDbSet<SquareFactMedia> SquareFactMediaList { get; set; }

        IDbSet<ForumSubject> ForumSubjects { get; set; }

        IDbSet<ForumReaction> ForumReactions { get; set; }

        IDbSet<ForumReactionReport> ForumReactionReports { get; set; }

        IDbSet<ForumReactionMedia> ForumReactionMediaList { get; set; }

        IDbSet<UserSetting> UserSettings { get; set; }

        IDbSet<User> Users { get; set; }

        IDbSet<UserPageVisit> UserPageVisits { get; set; }

        IDbSet<OrganizationTheme> OrganizationThemes { get; set; }

        IDbSet<ActivityReaction> ActivityReactions { get; set; }

        IDbSet<ActivityInterval> ActivityIntervals { get; set; }

        IDbSet<Stat> Stats { get; set; }

        IDbSet<ZuiderlingZipcode> ZuiderlingZipcodes { get; set; }

        IDbSet<UserDevice> UserDevices { get; set; }

        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        T Create<T>() where T : class;

        int SaveChanges();

        Task<int> SaveChangesAsync();
    }
}