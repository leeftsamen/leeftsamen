// <copyright file="ApplicationDbContext.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data
{
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Diagnostics;

    using LeeftSamen.Portal.Data.Models;

    using Microsoft.AspNet.Identity.EntityFramework;

    public class ApplicationDbContext : IdentityDbContext<User>, IApplicationDbContext
    {
        public ApplicationDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public ApplicationDbContext()
            : this("DefaultConnection")
        {
        }

        public IDbSet<Action> Actions { get; set; }

        public IDbSet<ActionVote> ActionVotes { get; set; }

        public IDbSet<Activity> Activities { get; set; }

        public IDbSet<ActivityAttendance> ActivityAttendances { get; set; }

        public IDbSet<ActivityInvitation> ActivityInvitations { get; set; }

        public IDbSet<CircleInvitation> CircleInvitations { get; set; }

        public IDbSet<CircleJoinRequest> CircleJoinRequests { get; set; }

        public IDbSet<CircleLabel> CircleLabels { get; set; }

        public IDbSet<CircleSetting> CircleSettings { get; set; }

        public IDbSet<CircleMembership> CircleMemberships { get; set; }

        public IDbSet<CircleMessage> CircleMessages { get; set; }

        public IDbSet<CircleMessageReaction> CircleMessageReactions { get; set; }

        public IDbSet<CircleEmailGroup> CircleEmailGroups { get; set; }

        public IDbSet<CircleEmailMessage> CircleEmailMessages { get; set; }

        public IDbSet<CircleEmailMessageReceiver> CircleEmailMessageReceivers { get; set; }

        public IDbSet<CircleActivity> CircleActivities { get; set; }

        public IDbSet<CircleActivityAttendance> CircleActivityAttendances { get; set; }

        public IDbSet<CircleActivityInvitation> CircleActivityInvitations { get; set; }

        public IDbSet<CircleActivityReaction> CircleActivityReactions { get; set; }

        public IDbSet<CirclePhotoAlbum> CirclePhotoAlbums { get; set; }

        public IDbSet<CirclePhoto> CirclePhotos { get; set; }

        public IDbSet<Circle> Circles { get; set; }

        public IDbSet<FeaturedCircle> FeaturedCircles { get; set; }

        public IDbSet<City> Cities { get; set; }

        public IDbSet<District> Districts { get; set; }

        public IDbSet<HelpIconUser> HelpIconUsers { get; set; }

        public IDbSet<HelpIcon> HelpIcons { get; set; }

        public IDbSet<Job> Jobs { get; set; }

        public IDbSet<MarketplaceItemCategory> MarketplaceItemCategories { get; set; }

        public IDbSet<MarketplaceItemReaction> MarketplaceItemReactions { get; set; }

        public IDbSet<MarketplaceItem> MarketplaceItems { get; set; }

        public IDbSet<MarketplaceItemTransaction> MarketplaceItemTransactions { get; set; }

        public IDbSet<Media> Media { get; set; }

        public IDbSet<NeighborhoodMessage> NeighborhoodMessages { get; set; }

        public IDbSet<NeighborhoodMessageReaction> NeighborhoodMessageReactions { get; set; }

        public IDbSet<Notification> Notifications { get; set; }

        public IDbSet<OrganizationInvitation> OrganizationInvitations { get; set; }

        public IDbSet<OrganizationProduct> OrganizationProducts { get; set; }

        public IDbSet<OrganizationService> OrganizationServices { get; set; }

        public IDbSet<OrganizationMembership> OrganizationMemberships { get; set; }

        public IDbSet<OrganizationTheme> OrganizationThemes { get; set; }

        public IDbSet<OrganizationType> OrganizationTypes { get; set; }

        public IDbSet<Organization> Organizations { get; set; }

        public IDbSet<PushNotification> PushNotifications { get; set; }

        public IDbSet<Setting> Settings { get; set; }

        public IDbSet<Square> Squares { get; set; }

        public IDbSet<SquareZipCode> SquareZipCodes { get; set; }

        public IDbSet<SquareAdmin> SquareAdmins { get; set; }

        public IDbSet<SquareFact> SquareFacts { get; set; }

        public IDbSet<SquareFactMedia> SquareFactMediaList { get; set; }

        public IDbSet<ForumSubject> ForumSubjects { get; set; }

        public IDbSet<ForumReaction> ForumReactions { get; set; }

        public IDbSet<ForumReactionReport> ForumReactionReports { get; set; }

        public IDbSet<ForumReactionMedia> ForumReactionMediaList { get; set; }

        public IDbSet<ActivityReaction> ActivityReactions { get; set; }

        public IDbSet<ActivityInterval> ActivityIntervals { get; set; }

        public IDbSet<Stat> Stats { get; set; }

        public IDbSet<ZuiderlingZipcode> ZuiderlingZipcodes { get; set; }

        public IDbSet<UserDevice> UserDevices { get; set; }

        public IDbSet<UserSetting> UserSettings { get; set; }

        public IDbSet<UserPageVisit> UserPageVisits { get; set; }

        public T Create<T>() where T : class
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateObject<T>();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            var circleEntity = modelBuilder.Entity<Circle>().HasKey(c => c.CircleId);
            circleEntity.HasMany(c => c.Members).WithRequired(u => u.Circle);
            circleEntity.HasMany(c => c.Invitations).WithRequired(u => u.Circle);
            circleEntity.HasMany(c => c.Messages).WithRequired(c => c.Circle);
            circleEntity.HasOptional(c => c.CoverImage).WithMany().HasForeignKey(c => c.CoverImageId);
            circleEntity.HasOptional(c => c.ProfileImage).WithMany().HasForeignKey(c => c.ProfileImageId);
            circleEntity.HasOptional(c => c.Creator).WithMany().WillCascadeOnDelete(false);
            circleEntity.Property(c => c.CreationDateTime).IsRequired().HasColumnType("datetime2");
            circleEntity.Property(c => c.Name).IsRequired();
            circleEntity.Property(c => c.IsPrivate).IsRequired();
            circleEntity.Property(c => c.Position).IsRequired();

            var circleEmailMessage = modelBuilder.Entity<CircleEmailMessage>().HasKey(c => c.MessageId);
            circleEmailMessage.Property(c => c.Text).IsRequired();
            circleEmailMessage.Property(c => c.Subject).IsRequired();
            circleEmailMessage.Property(c => c.CreationDateTime).IsRequired().HasColumnType("datetime2");
            circleEmailMessage.Property(c => c.CircleId).IsRequired();
            circleEmailMessage.HasOptional(c => c.Replies).WithMany().HasForeignKey(c => c.ParentMessageId);
            circleEmailMessage.Property(c => c.CreatorHasRemovedMessage).IsRequired();

            var circleEmailMessageReplies = modelBuilder.Entity<CircleEmailMessageReceiver>().HasKey(c => c.EmailMessageId);
            //circleEmailMessageReplies.HasRequired<CircleEmailMessage>(c => c.EmailMessage).WithMany(m => m.Recipients).HasForeignKey(m => m.ReceiverId);
            circleEmailMessageReplies.Property(m => m.HasRemovedMessage).IsRequired();

            var jobEntity = modelBuilder.Entity<Job>().HasKey(j => j.JobId);
            jobEntity.HasMany(j => j.VisibleToMembers).WithMany();
            jobEntity.HasRequired(j => j.Circle).WithMany(c => c.Jobs);
            jobEntity.HasRequired(j => j.Creator).WithMany();
            jobEntity.HasOptional(j => j.Assignee).WithMany();
            jobEntity.Property(j => j.DueDateTime).IsRequired().HasColumnType("datetime2");
            jobEntity.Property(j => j.CreationDateTime).IsRequired().HasColumnType("datetime2");
            jobEntity.Property(j => j.Title).IsRequired();
            jobEntity.Property(j => j.Description).IsRequired();
            jobEntity.Property(j => j.IsOnlyVisibleToSelectedMembers).IsRequired();

            var userEntity = modelBuilder.Entity<User>();
            userEntity.HasMany(u => u.Circles).WithRequired(c => c.User);
            userEntity.HasMany(u => u.CircleInvitations).WithOptional(c => c.User);
            userEntity.HasMany(u => u.Organizations).WithRequired(m => m.User);
            userEntity.HasMany(u => u.OrganizationInvitations).WithOptional(m => m.User);
            userEntity.HasOptional(u => u.ProfileImage).WithMany().HasForeignKey(u => u.ProfileImageId);
            userEntity.Property(u => u.Name).IsRequired();
            userEntity.Property(u => u.PostalCode).IsRequired();
            userEntity.Property(u => u.HouseNumber).IsRequired();
            userEntity.Property(u => u.Street).IsRequired();
            userEntity.Property(u => u.City).IsRequired();
            userEntity.Property(u => u.NeighborhoodRadius).IsRequired();
            userEntity.Property(u => u.Latitude).IsRequired().HasPrecision(9, 6);
            userEntity.Property(u => u.Longitude).IsRequired().HasPrecision(9, 6);
            userEntity.Property(u => u.Position).IsRequired();
            userEntity.Property(u => u.AuthToken).IsOptional();
            userEntity.Property(u => u.DeviceToken).IsOptional();
            userEntity.Property(u => u.LastSeen).HasColumnType("datetime2");

            var circleInvitationEntity = modelBuilder.Entity<CircleInvitation>().HasKey(i => i.CircleInvitationId);
            circleInvitationEntity.HasRequired(m => m.Circle).WithMany(c => c.Invitations).WillCascadeOnDelete(true);
            circleInvitationEntity.HasOptional(m => m.User).WithMany(u => u.CircleInvitations).WillCascadeOnDelete(true);
            circleInvitationEntity.HasRequired(m => m.InvitedBy).WithMany();
            circleInvitationEntity.Property(m => m.InvitationDateTime).IsRequired().HasColumnType("datetime2");
            circleInvitationEntity.Property(m => m.ExpireDate).IsRequired().HasColumnType("datetime2");
            circleInvitationEntity.Property(m => m.AcceptToken).IsRequired().HasMaxLength(32);
            circleInvitationEntity.Property(m => m.Email).IsOptional();

            var circleMembershipEntity = modelBuilder.Entity<CircleMembership>().HasKey(m => m.CircleMembershipId);
            circleMembershipEntity.HasRequired(m => m.Circle).WithMany(c => c.Members).WillCascadeOnDelete(true);
            circleMembershipEntity.HasRequired(m => m.User).WithMany(u => u.Circles).WillCascadeOnDelete(true);
            circleMembershipEntity.Property(m => m.MemberSinceDateTime).IsRequired().HasColumnType("datetime2");

            var circleJoinRequestEntity = modelBuilder.Entity<CircleJoinRequest>().HasKey(m => m.CircleJoinRequestId);
            circleJoinRequestEntity.HasRequired(m => m.Circle).WithMany(c => c.JoinRequests).WillCascadeOnDelete(true);
            circleJoinRequestEntity.HasRequired(m => m.User)
                .WithMany(u => u.CircleJoinRequests)
                .WillCascadeOnDelete(true);
            circleJoinRequestEntity.Property(m => m.JoinRequestDateTime).IsRequired().HasColumnType("datetime2");
            circleJoinRequestEntity.Property(r => r.AcceptToken).IsRequired().HasMaxLength(32);

            var circleMessageEntity = modelBuilder.Entity<CircleMessage>().HasKey(c => c.MessageId);
            circleMessageEntity.HasRequired(c => c.Circle).WithMany(c => c.Messages).WillCascadeOnDelete(true);
            circleMessageEntity.HasRequired(c => c.Creator).WithMany();
            circleMessageEntity.HasOptional(c => c.Attachment).WithMany().HasForeignKey(c => c.AttachmentId);
            circleMessageEntity.Property(c => c.CreationDateTime).IsRequired().HasColumnType("datetime2");

            var circleMessageReactionEntity = modelBuilder.Entity<CircleMessageReaction>().HasKey(c => c.ReactionId);
            circleMessageReactionEntity.HasRequired(c => c.Message).WithMany(c => c.Reactions).WillCascadeOnDelete(true);
            circleMessageReactionEntity.HasRequired(c => c.Creator).WithMany();
            circleMessageEntity.HasOptional(c => c.Attachment).WithMany().HasForeignKey(c => c.AttachmentId);
            circleMessageReactionEntity.Property(c => c.CreationDateTime).IsRequired().HasColumnType("datetime2");

            var circlePhotoAlbumEntity = modelBuilder.Entity<CirclePhotoAlbum>().HasKey(c => c.CirclePhotoAlbumId);
            circlePhotoAlbumEntity.HasRequired(c => c.Circle).WithMany(c => c.CirclePhotoAlbums).WillCascadeOnDelete(true);
            circlePhotoAlbumEntity.HasRequired(c => c.CreatedBy).WithMany();

            var circlePhotoEntity = modelBuilder.Entity<CirclePhoto>().HasKey(c => c.CirclePhotoId);
            circlePhotoEntity.HasRequired(c => c.Circle).WithMany(c => c.Photos).WillCascadeOnDelete(false);
            circlePhotoEntity.HasOptional(c => c.CirclePhotoAlbum).WithMany(c => c.Photos).WillCascadeOnDelete(true);
            circlePhotoEntity.HasRequired(c => c.UploadedBy).WithMany();
            circlePhotoEntity.HasRequired(c => c.Photo).WithMany().HasForeignKey(c => c.PhotoId);

            var media = modelBuilder.Entity<Media>().HasKey(m => m.MediaId);
            media.Property(m => m.CreationDate).HasColumnType("datetime2");

            var organizationEntity = modelBuilder.Entity<Organization>().HasKey(o => o.OrganizationId);
            organizationEntity.HasMany(o => o.Members).WithRequired(m => m.Organization);
            organizationEntity.HasMany(o => o.Invitations).WithRequired(m => m.Organization);
            organizationEntity.HasMany(o => o.Services).WithRequired(m => m.Organization);
            organizationEntity.HasMany(o => o.Products).WithRequired(m => m.Organization);

            organizationEntity.HasMany(o => o.ActiveInDistricts).WithMany();
            organizationEntity.HasMany(o => o.Themes).WithMany();
            organizationEntity.HasRequired(o => o.OrganizationType).WithMany();
            organizationEntity.HasRequired(o => o.RequestedBy).WithMany();
            organizationEntity.HasOptional(o => o.Logo).WithMany().HasForeignKey(o => o.LogoId);
            organizationEntity.Property(o => o.Name).IsRequired();
            organizationEntity.Property(o => o.RequestDateTime).IsOptional().HasColumnType("datetime2");

            var organizationServiceEntity =
               modelBuilder.Entity<OrganizationService>().HasKey(m => m.OrganizationServiceId);
            organizationServiceEntity.HasRequired(m => m.Organization).WithMany(o => o.Services);
            organizationServiceEntity.Property(t => t.Title).IsRequired();

            var organizationProductEntity =
               modelBuilder.Entity<OrganizationProduct>().HasKey(m => m.OrganizationProductId);
            organizationProductEntity.HasRequired(m => m.Organization).WithMany(o => o.Products);
            organizationProductEntity.Property(t => t.Title).IsRequired();

            var organizationTypeEntity = modelBuilder.Entity<OrganizationType>().HasKey(t => t.OrganizationTypeId);
            organizationTypeEntity.Property(t => t.Name).IsRequired();

            var organizationTheme = modelBuilder.Entity<OrganizationTheme>().HasKey(t => t.ThemeId);
            organizationTheme.Property(t => t.Name).IsRequired();

            var organizationMembershipEntity =
                modelBuilder.Entity<OrganizationMembership>().HasKey(m => m.OrganizationMembershipId);
            organizationMembershipEntity.HasRequired(m => m.Organization).WithMany(o => o.Members);
            organizationMembershipEntity.HasRequired(m => m.User).WithMany(o => o.Organizations);
            organizationMembershipEntity.Property(m => m.MemberSinceDateTime).IsRequired().HasColumnType("datetime2");

            var organizationInvitationEntity =
                modelBuilder.Entity<OrganizationInvitation>().HasKey(m => m.OrganizationInvitationId);
            organizationInvitationEntity.HasRequired(m => m.Organization).WithMany(o => o.Invitations);
            organizationInvitationEntity.HasOptional(m => m.User).WithMany(u => u.OrganizationInvitations);
            organizationInvitationEntity.HasRequired(m => m.InvitedBy).WithMany();
            organizationInvitationEntity.Property(m => m.InvitationDateTime).IsRequired().HasColumnType("datetime2");
            organizationInvitationEntity.Property(m => m.AcceptToken).IsRequired().HasMaxLength(32);
            organizationInvitationEntity.Property(m => m.Email).IsOptional();

            var marketplaceItemEntity = modelBuilder.Entity<MarketplaceItem>().HasKey(i => i.MarketplaceItemId);
            marketplaceItemEntity.HasRequired(i => i.Owner).WithMany().WillCascadeOnDelete(true);
            marketplaceItemEntity.Property(i => i.CreationDateTime).HasColumnType("datetime2");
            marketplaceItemEntity.Property(i => i.Title).IsRequired();
            marketplaceItemEntity.HasOptional(i => i.Image1).WithMany().HasForeignKey(i => i.Image1Id);
            marketplaceItemEntity.HasOptional(i => i.Image2).WithMany().HasForeignKey(i => i.Image2Id);
            marketplaceItemEntity.HasOptional(i => i.Image3).WithMany().HasForeignKey(i => i.Image3Id);
            marketplaceItemEntity.HasOptional(i => i.Image4).WithMany().HasForeignKey(i => i.Image4Id);
            marketplaceItemEntity.HasOptional(i => i.Image5).WithMany().HasForeignKey(i => i.Image5Id);
            marketplaceItemEntity.HasRequired(i => i.Category).WithMany();
            marketplaceItemEntity.HasMany(i => i.Reactions).WithRequired(i => i.MarketplaceItem);
            marketplaceItemEntity.HasOptional(m => m.OrganizationMembership)
                .WithMany()
                .HasForeignKey(m => m.OrganizationMembershipId);
            marketplaceItemEntity.Ignore(x => x.Distance);

            var marketplaceItemCategoryEntity = modelBuilder.Entity<MarketplaceItemCategory>().HasKey(i => i.CategoryId);
            marketplaceItemCategoryEntity.Property(m => m.Name).IsRequired();

            var marketplaceItemReactionEntity = modelBuilder.Entity<MarketplaceItemReaction>().HasKey(i => i.ReactionId);
            marketplaceItemReactionEntity.HasRequired(i => i.Creator).WithMany().WillCascadeOnDelete(false);
            marketplaceItemReactionEntity.HasRequired(i => i.MarketplaceItem)
                .WithMany(i => i.Reactions)
                .WillCascadeOnDelete(true);
            marketplaceItemReactionEntity.Property(i => i.Text).IsRequired();
            marketplaceItemReactionEntity.Property(i => i.CreationDateTime).HasColumnType("datetime2");
            marketplaceItemReactionEntity.HasOptional(i => i.Parent)
                .WithMany()
                .HasForeignKey(i => i.ParentId)
                .WillCascadeOnDelete(false);

            var neighborhoodMessageEntity = modelBuilder.Entity<NeighborhoodMessage>().HasKey(i => i.MessageId);
            neighborhoodMessageEntity.HasRequired(m => m.Creator).WithMany().WillCascadeOnDelete(true);
            neighborhoodMessageEntity.Property(m => m.Title).IsRequired();
            neighborhoodMessageEntity.Property(m => m.CreationDateTime).HasColumnType("datetime2");
            neighborhoodMessageEntity.Property(m => m.ExpirationDateTime).HasColumnType("datetime2");
            neighborhoodMessageEntity.HasOptional(m => m.Image1).WithMany().HasForeignKey(m => m.Image1Id);
            neighborhoodMessageEntity.HasOptional(m => m.Image2).WithMany().HasForeignKey(m => m.Image2Id);
            neighborhoodMessageEntity.HasOptional(m => m.Image3).WithMany().HasForeignKey(m => m.Image3Id);
            neighborhoodMessageEntity.HasOptional(m => m.Image4).WithMany().HasForeignKey(m => m.Image4Id);
            neighborhoodMessageEntity.HasOptional(m => m.Image5).WithMany().HasForeignKey(m => m.Image5Id);
            neighborhoodMessageEntity.HasOptional(m => m.File1).WithMany().HasForeignKey(m => m.File1Id);
            neighborhoodMessageEntity.HasOptional(m => m.OrganizationMembership)
                .WithMany()
                .HasForeignKey(m => m.OrganizationMembershipId);
            neighborhoodMessageEntity.HasMany(i => i.Reactions).WithRequired(i => i.NeighborhoodMessage);

            var neighborhoodMessageReactionEntity = modelBuilder.Entity<NeighborhoodMessageReaction>().HasKey(i => i.ReactionId);
            neighborhoodMessageReactionEntity.HasRequired(i => i.Creator).WithMany().WillCascadeOnDelete(false);
            neighborhoodMessageReactionEntity.HasRequired(i => i.NeighborhoodMessage)
                .WithMany(i => i.Reactions)
                .WillCascadeOnDelete(true);
            neighborhoodMessageReactionEntity.Property(i => i.Text).IsRequired();
            neighborhoodMessageReactionEntity.Property(i => i.CreationDateTime).HasColumnType("datetime2");

            var cityEntity = modelBuilder.Entity<City>().HasKey(c => c.CityId);
            cityEntity.Property(c => c.CityCode).IsRequired().HasColumnName("GM_CODE");
            cityEntity.Property(c => c.CityName).IsRequired().HasColumnName("GM_NAAM");
            cityEntity.Property(c => c.Shape).IsRequired();

            var districtEntity = modelBuilder.Entity<District>().HasKey(c => c.DistrictId);
            districtEntity.Property(c => c.DistrictCode).IsRequired().HasColumnName("WK_CODE");
            districtEntity.Property(c => c.DistrictName).IsRequired().HasColumnName("WK_NAAM");
            districtEntity.Property(c => c.CityCode).IsRequired().HasColumnName("GM_CODE");
            districtEntity.Property(c => c.CityName).IsRequired().HasColumnName("GM_NAAM");
            districtEntity.Property(c => c.Shape).IsRequired();

            var activityEntity = modelBuilder.Entity<Activity>().HasKey(i => i.ActivityId);
            activityEntity.HasMany(a => a.Attendees).WithRequired(a => a.Activity).WillCascadeOnDelete();
            activityEntity.HasRequired(i => i.Creator).WithMany().WillCascadeOnDelete(true);
            activityEntity.HasMany(a => a.Invitations).WithRequired(u => u.Activity);
            activityEntity.HasOptional(i => i.Intervals).WithMany();
            activityEntity.HasOptional(i => i.OrganizationMembership).WithMany().HasForeignKey(i => i.OrganizationMembershipId);
            activityEntity.Property(i => i.StartDateTime).HasColumnType("datetime2");
            activityEntity.Property(i => i.EndDateTime).HasColumnType("datetime2");
            activityEntity.Property(i => i.CreationDate).HasColumnType("datetime2");
            activityEntity.Property(i => i.RecurringEnd).HasColumnType("datetime2");

            var circleActivityEntity = modelBuilder.Entity<CircleActivity>().HasKey(i => i.ActivityId);
            circleActivityEntity.HasMany(a => a.Attendees).WithRequired(a => a.CircleActivity).WillCascadeOnDelete();
            circleActivityEntity.HasRequired(i => i.Creator).WithMany();
            circleActivityEntity.HasMany(a => a.Invitations).WithRequired(u => u.CircleActivity);
            circleActivityEntity.Property(i => i.StartDateTime).HasColumnType("datetime2");
            circleActivityEntity.Property(i => i.EndDateTime).HasColumnType("datetime2");
            circleActivityEntity.Property(i => i.CreationDate).HasColumnType("datetime2");

            var activityInvitationEntity = modelBuilder.Entity<ActivityInvitation>().HasKey(i => i.ActivityInvitationId);
            activityInvitationEntity.HasRequired(m => m.Activity).WithMany(c => c.Invitations).WillCascadeOnDelete();
            activityInvitationEntity.HasOptional(m => m.User).WithMany(u => u.ActivityInvitations).WillCascadeOnDelete(true);
            activityInvitationEntity.HasRequired(m => m.InvitedBy).WithMany();
            activityInvitationEntity.Property(m => m.InvitationDateTime).IsRequired().HasColumnType("datetime2");
            activityInvitationEntity.Property(m => m.AcceptToken).IsRequired().HasMaxLength(32);
            activityInvitationEntity.Property(m => m.Email).IsOptional();

            var circleActivityInvitationEntity = modelBuilder.Entity<CircleActivityInvitation>().HasKey(i => i.CircleActivityInvitationId);
            circleActivityInvitationEntity.HasRequired(m => m.CircleActivity).WithMany(c => c.Invitations).WillCascadeOnDelete();
            circleActivityInvitationEntity.HasOptional(m => m.User).WithMany(u => u.CircleActivityInvitations);
            circleActivityInvitationEntity.HasRequired(m => m.InvitedBy).WithMany();
            circleActivityInvitationEntity.Property(m => m.InvitationDateTime).IsRequired().HasColumnType("datetime2");
            circleActivityInvitationEntity.Property(m => m.AcceptToken).IsRequired().HasMaxLength(32);
            circleActivityInvitationEntity.Property(m => m.Email).IsOptional();

            var activityIntervalEntity = modelBuilder.Entity<ActivityInterval>().HasKey(i => i.IntervalId);
            activityIntervalEntity.HasRequired(i => i.Activity).WithMany(i => i.Intervals).WillCascadeOnDelete();

            var activityAttendanceEntity = modelBuilder.Entity<ActivityAttendance>().HasKey(i => i.ActivityAttendanceId);
            activityAttendanceEntity.HasRequired(e => e.Activity).WithMany(e => e.Attendees).WillCascadeOnDelete();
            activityAttendanceEntity.HasRequired(e => e.User).WithMany();
            activityAttendanceEntity.Property(e => e.Attending).IsRequired();
            activityAttendanceEntity.Property(e => e.ModificationDate).IsRequired().HasColumnType("datetime2");

            var circleActivityAttendanceEntity = modelBuilder.Entity<CircleActivityAttendance>().HasKey(i => i.ActivityAttendanceId);
            circleActivityAttendanceEntity.HasRequired(e => e.CircleActivity).WithMany(e => e.Attendees).WillCascadeOnDelete();
            circleActivityAttendanceEntity.HasRequired(e => e.User).WithMany();
            circleActivityAttendanceEntity.Property(e => e.Attending).IsRequired();
            circleActivityAttendanceEntity.Property(e => e.UserJoinedDate).IsRequired().HasColumnType("datetime2");

            var activityReactionEntity = modelBuilder.Entity<ActivityReaction>().HasKey(i => i.ReactionId);
            activityReactionEntity.HasRequired(i => i.Creator).WithMany().WillCascadeOnDelete(false);
            activityReactionEntity.HasRequired(i => i.Activity).WithMany(i => i.Reactions).WillCascadeOnDelete(true);
            activityReactionEntity.Property(i => i.Text).IsRequired();
            activityReactionEntity.Property(i => i.CreationDateTime).HasColumnType("datetime2");

            var circleActivityReactionEntity = modelBuilder.Entity<CircleActivityReaction>().HasKey(i => i.ReactionId);
            circleActivityReactionEntity.HasRequired(i => i.Creator).WithMany().WillCascadeOnDelete(false);
            circleActivityReactionEntity.HasRequired(i => i.CircleActivity).WithMany(i => i.Reactions).WillCascadeOnDelete(true);
            circleActivityReactionEntity.Property(i => i.Text).IsRequired();
            circleActivityReactionEntity.Property(i => i.CreationDateTime).HasColumnType("datetime2");

            var notificationEntity = modelBuilder.Entity<Notification>().HasKey(i => i.NotificationId);
            notificationEntity.HasRequired(i => i.ForUser)
                .WithMany()
                .HasForeignKey(i => i.ForUserId)
                .WillCascadeOnDelete(true);
            notificationEntity.Property(i => i.CreationDateTime).HasColumnType("datetime2");

            var helpIconEntity = modelBuilder.Entity<HelpIcon>().HasKey(i => i.HelpIconId);
            helpIconEntity.Property(i => i.Text).IsRequired();
            helpIconEntity.Property(i => i.TextPlacement).IsRequired();
            helpIconEntity.Property(i => i.Type).IsRequired();
            helpIconEntity.HasMany(i => i.ShownIcons).WithRequired(r => r.HelpIcon);

            var helpIconUserEntity = modelBuilder.Entity<HelpIconUser>().HasKey(i => i.Id);
            helpIconUserEntity.HasRequired(i => i.HelpIcon);
            helpIconUserEntity.HasRequired(i => i.User);

            var statEntity = modelBuilder.Entity<Stat>().HasKey(i => i.StatId);
            statEntity.Property(i => i.DateTime).HasColumnType("datetime2");

            var squareEntity = modelBuilder.Entity<Square>().HasKey(s => s.SquareId);
            circleEntity.HasOptional(s => s.CoverImage).WithMany().HasForeignKey(s => s.CoverImageId);
            circleEntity.HasOptional(s => s.ProfileImage).WithMany().HasForeignKey(s => s.ProfileImageId);
        }
    }
}