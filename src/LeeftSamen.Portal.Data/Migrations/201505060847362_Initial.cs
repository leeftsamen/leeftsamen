// <copyright file="201505060847362_Initial.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Down()
        {
            this.DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            this.DropForeignKey("dbo.Notifications", "ForUserId", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.NeighborhoodMessages", "OrganizationMembershipId", "dbo.OrganizationMemberships");
            this.DropForeignKey("dbo.NeighborhoodMessages", "Image1Id", "dbo.Media");
            this.DropForeignKey("dbo.NeighborhoodMessages", "Creator_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.MarketplaceItemReactions", "ParentId", "dbo.MarketplaceItemReactions");
            this.DropForeignKey(
                "dbo.MarketplaceItemReactions",
                "OrganizationMembershipId",
                "dbo.OrganizationMemberships");
            this.DropForeignKey(
                "dbo.MarketplaceItemReactions",
                "MarketplaceItem_MarketplaceItemId",
                "dbo.MarketplaceItems");
            this.DropForeignKey("dbo.MarketplaceItems", "Owner_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.MarketplaceItems", "OrganizationMembershipId", "dbo.OrganizationMemberships");
            this.DropForeignKey("dbo.MarketplaceItems", "Image5Id", "dbo.Media");
            this.DropForeignKey("dbo.MarketplaceItems", "Image4Id", "dbo.Media");
            this.DropForeignKey("dbo.MarketplaceItems", "Image3Id", "dbo.Media");
            this.DropForeignKey("dbo.MarketplaceItems", "Image2Id", "dbo.Media");
            this.DropForeignKey("dbo.MarketplaceItems", "Image1Id", "dbo.Media");
            this.DropForeignKey("dbo.MarketplaceItems", "Category_CategoryId", "dbo.MarketplaceItemCategories");
            this.DropForeignKey("dbo.MarketplaceItemReactions", "Creator_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.Activities", "OrganizationMembershipId", "dbo.OrganizationMemberships");
            this.DropForeignKey("dbo.ActivityIntervals", "Activity_ActivityId", "dbo.Activities");
            this.DropForeignKey("dbo.Activities", "Creator_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.AspNetUsers", "ProfileImageId", "dbo.Media");
            this.DropForeignKey("dbo.OrganizationInvitations", "User_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.OrganizationInvitations", "Organization_OrganizationId", "dbo.Organizations");
            this.DropForeignKey("dbo.Organizations", "RequestedBy_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.Organizations", "OrganizationType_OrganizationTypeId", "dbo.OrganizationTypes");
            this.DropForeignKey("dbo.OrganizationMemberships", "User_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.OrganizationMemberships", "Organization_OrganizationId", "dbo.Organizations");
            this.DropForeignKey("dbo.Organizations", "LogoId", "dbo.Media");
            this.DropForeignKey("dbo.OrganizationDistricts", "District_DistrictId", "dbo.Districts");
            this.DropForeignKey("dbo.OrganizationDistricts", "Organization_OrganizationId", "dbo.Organizations");
            this.DropForeignKey("dbo.OrganizationInvitations", "InvitedBy_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.CircleInvitations", "User_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.CircleInvitations", "InvitedBy_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.CircleInvitations", "Circle_CircleId", "dbo.Circles");
            this.DropForeignKey("dbo.Circles", "ProfileImageId", "dbo.Media");
            this.DropForeignKey("dbo.CircleMessages", "Circle_CircleId", "dbo.Circles");
            this.DropForeignKey("dbo.CircleMessageReactions", "Message_MessageId", "dbo.CircleMessages");
            this.DropForeignKey("dbo.CircleMessageReactions", "Creator_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.CircleMessageReactions", "Circle_CircleId", "dbo.Circles");
            this.DropForeignKey("dbo.CircleMessages", "Creator_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.CircleJoinRequests", "User_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.CircleJoinRequests", "Circle_CircleId", "dbo.Circles");
            this.DropForeignKey(
                "dbo.JobCircleMemberships",
                "CircleMembership_CircleMembershipId",
                "dbo.CircleMemberships");
            this.DropForeignKey("dbo.JobCircleMemberships", "Job_JobId", "dbo.Jobs");
            this.DropForeignKey("dbo.CircleMemberships", "User_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.CircleMemberships", "Circle_CircleId", "dbo.Circles");
            this.DropForeignKey("dbo.Jobs", "Creator_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.Jobs", "Circle_CircleId", "dbo.Circles");
            this.DropForeignKey("dbo.Jobs", "Assignee_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.Circles", "Creator_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.Circles", "CoverImageId", "dbo.Media");
            this.DropIndex("dbo.OrganizationDistricts", new[] { "District_DistrictId" });
            this.DropIndex("dbo.OrganizationDistricts", new[] { "Organization_OrganizationId" });
            this.DropIndex("dbo.JobCircleMemberships", new[] { "CircleMembership_CircleMembershipId" });
            this.DropIndex("dbo.JobCircleMemberships", new[] { "Job_JobId" });
            this.DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            this.DropIndex("dbo.Notifications", new[] { "ForUserId" });
            this.DropIndex("dbo.NeighborhoodMessages", new[] { "Creator_Id" });
            this.DropIndex("dbo.NeighborhoodMessages", new[] { "OrganizationMembershipId" });
            this.DropIndex("dbo.NeighborhoodMessages", new[] { "Image1Id" });
            this.DropIndex("dbo.MarketplaceItems", new[] { "Owner_Id" });
            this.DropIndex("dbo.MarketplaceItems", new[] { "Category_CategoryId" });
            this.DropIndex("dbo.MarketplaceItems", new[] { "OrganizationMembershipId" });
            this.DropIndex("dbo.MarketplaceItems", new[] { "Image5Id" });
            this.DropIndex("dbo.MarketplaceItems", new[] { "Image4Id" });
            this.DropIndex("dbo.MarketplaceItems", new[] { "Image3Id" });
            this.DropIndex("dbo.MarketplaceItems", new[] { "Image2Id" });
            this.DropIndex("dbo.MarketplaceItems", new[] { "Image1Id" });
            this.DropIndex("dbo.MarketplaceItemReactions", new[] { "MarketplaceItem_MarketplaceItemId" });
            this.DropIndex("dbo.MarketplaceItemReactions", new[] { "Creator_Id" });
            this.DropIndex("dbo.MarketplaceItemReactions", new[] { "ParentId" });
            this.DropIndex("dbo.MarketplaceItemReactions", new[] { "OrganizationMembershipId" });
            this.DropIndex("dbo.ActivityIntervals", new[] { "Activity_ActivityId" });
            this.DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            this.DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            this.DropIndex("dbo.OrganizationMemberships", new[] { "User_Id" });
            this.DropIndex("dbo.OrganizationMemberships", new[] { "Organization_OrganizationId" });
            this.DropIndex("dbo.Organizations", new[] { "RequestedBy_Id" });
            this.DropIndex("dbo.Organizations", new[] { "OrganizationType_OrganizationTypeId" });
            this.DropIndex("dbo.Organizations", new[] { "LogoId" });
            this.DropIndex("dbo.OrganizationInvitations", new[] { "User_Id" });
            this.DropIndex("dbo.OrganizationInvitations", new[] { "Organization_OrganizationId" });
            this.DropIndex("dbo.OrganizationInvitations", new[] { "InvitedBy_Id" });
            this.DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            this.DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            this.DropIndex("dbo.CircleMessageReactions", new[] { "Message_MessageId" });
            this.DropIndex("dbo.CircleMessageReactions", new[] { "Creator_Id" });
            this.DropIndex("dbo.CircleMessageReactions", new[] { "Circle_CircleId" });
            this.DropIndex("dbo.CircleMessages", new[] { "Circle_CircleId" });
            this.DropIndex("dbo.CircleMessages", new[] { "Creator_Id" });
            this.DropIndex("dbo.CircleJoinRequests", new[] { "User_Id" });
            this.DropIndex("dbo.CircleJoinRequests", new[] { "Circle_CircleId" });
            this.DropIndex("dbo.CircleMemberships", new[] { "User_Id" });
            this.DropIndex("dbo.CircleMemberships", new[] { "Circle_CircleId" });
            this.DropIndex("dbo.Jobs", new[] { "Creator_Id" });
            this.DropIndex("dbo.Jobs", new[] { "Circle_CircleId" });
            this.DropIndex("dbo.Jobs", new[] { "Assignee_Id" });
            this.DropIndex("dbo.Circles", new[] { "Creator_Id" });
            this.DropIndex("dbo.Circles", new[] { "ProfileImageId" });
            this.DropIndex("dbo.Circles", new[] { "CoverImageId" });
            this.DropIndex("dbo.CircleInvitations", new[] { "User_Id" });
            this.DropIndex("dbo.CircleInvitations", new[] { "InvitedBy_Id" });
            this.DropIndex("dbo.CircleInvitations", new[] { "Circle_CircleId" });
            this.DropIndex("dbo.AspNetUsers", "UserNameIndex");
            this.DropIndex("dbo.AspNetUsers", new[] { "ProfileImageId" });
            this.DropIndex("dbo.Activities", new[] { "Creator_Id" });
            this.DropIndex("dbo.Activities", new[] { "OrganizationMembershipId" });
            this.DropTable("dbo.OrganizationDistricts");
            this.DropTable("dbo.JobCircleMemberships");
            this.DropTable("dbo.AspNetRoles");
            this.DropTable("dbo.Notifications");
            this.DropTable("dbo.NeighborhoodMessages");
            this.DropTable("dbo.MarketplaceItems");
            this.DropTable("dbo.MarketplaceItemReactions");
            this.DropTable("dbo.MarketplaceItemCategories");
            this.DropTable("dbo.Cities");
            this.DropTable("dbo.ActivityIntervals");
            this.DropTable("dbo.AspNetUserRoles");
            this.DropTable("dbo.OrganizationTypes");
            this.DropTable("dbo.OrganizationMemberships");
            this.DropTable("dbo.Districts");
            this.DropTable("dbo.Organizations");
            this.DropTable("dbo.OrganizationInvitations");
            this.DropTable("dbo.AspNetUserLogins");
            this.DropTable("dbo.AspNetUserClaims");
            this.DropTable("dbo.CircleMessageReactions");
            this.DropTable("dbo.CircleMessages");
            this.DropTable("dbo.CircleJoinRequests");
            this.DropTable("dbo.CircleMemberships");
            this.DropTable("dbo.Jobs");
            this.DropTable("dbo.Media");
            this.DropTable("dbo.Circles");
            this.DropTable("dbo.CircleInvitations");
            this.DropTable("dbo.AspNetUsers");
            this.DropTable("dbo.Activities");
        }

        public override void Up()
        {
            this.CreateTable(
                "dbo.Activities",
                c =>
                new
                    {
                        ActivityId = c.Int(nullable: false, identity: true),
                        AllDay = c.Boolean(nullable: false),
                        Description = c.String(),
                        EndDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Location = c.String(),
                        OrganizationMembershipId = c.Int(),
                        StartDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Title = c.String(),
                        Creator_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.ActivityId)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id)
                .ForeignKey("dbo.OrganizationMemberships", t => t.OrganizationMembershipId)
                .Index(t => t.OrganizationMembershipId)
                .Index(t => t.Creator_Id);

            this.CreateTable(
                "dbo.AspNetUsers",
                c =>
                new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        City = c.String(nullable: false),
                        HouseNumber = c.String(nullable: false),
                        Latitude = c.Decimal(nullable: false, precision: 9, scale: 6),
                        Longitude = c.Decimal(nullable: false, precision: 9, scale: 6),
                        Name = c.String(nullable: false),
                        NeighborhoodRadius = c.Int(nullable: false),
                        Position = c.Geography(nullable: false),
                        PostalCode = c.String(nullable: false),
                        ProfileImageId = c.Int(),
                        Street = c.String(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Media", t => t.ProfileImageId)
                .Index(t => t.ProfileImageId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");

            this.CreateTable(
                "dbo.CircleInvitations",
                c =>
                new
                    {
                        CircleInvitationId = c.Int(nullable: false, identity: true),
                        AcceptToken = c.String(nullable: false, maxLength: 32),
                        Email = c.String(),
                        InvitationDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Circle_CircleId = c.Int(nullable: false),
                        InvitedBy_Id = c.String(nullable: false, maxLength: 128),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.CircleInvitationId)
                .ForeignKey("dbo.Circles", t => t.Circle_CircleId)
                .ForeignKey("dbo.AspNetUsers", t => t.InvitedBy_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.Circle_CircleId)
                .Index(t => t.InvitedBy_Id)
                .Index(t => t.User_Id);

            this.CreateTable(
                "dbo.Circles",
                c =>
                new
                    {
                        CircleId = c.Int(nullable: false, identity: true),
                        CoverColor = c.Int(nullable: false),
                        CoverImageId = c.Int(),
                        CreationDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Description = c.String(),
                        IsPrivate = c.Boolean(nullable: false),
                        Name = c.String(nullable: false),
                        Position = c.Geography(nullable: false),
                        ProfileImageId = c.Int(),
                        Creator_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.CircleId)
                .ForeignKey("dbo.Media", t => t.CoverImageId)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id)
                .ForeignKey("dbo.Media", t => t.ProfileImageId)
                .Index(t => t.CoverImageId)
                .Index(t => t.ProfileImageId)
                .Index(t => t.Creator_Id);

            this.CreateTable(
                "dbo.Media",
                c =>
                new
                    {
                        MediaId = c.Int(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Data = c.Binary(),
                        MimeType = c.String(),
                        Name = c.String(),
                        Size = c.Int(nullable: false),
                    }).PrimaryKey(t => t.MediaId);

            this.CreateTable(
                "dbo.Jobs",
                c =>
                new
                    {
                        JobId = c.Int(nullable: false, identity: true),
                        CompletionDateTime = c.DateTime(),
                        CreationDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Description = c.String(nullable: false),
                        DueDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsOnlyVisibleToSelectedMembers = c.Boolean(nullable: false),
                        Title = c.String(nullable: false),
                        Assignee_Id = c.String(maxLength: 128),
                        Circle_CircleId = c.Int(nullable: false),
                        Creator_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.JobId)
                .ForeignKey("dbo.AspNetUsers", t => t.Assignee_Id)
                .ForeignKey("dbo.Circles", t => t.Circle_CircleId)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id)
                .Index(t => t.Assignee_Id)
                .Index(t => t.Circle_CircleId)
                .Index(t => t.Creator_Id);

            this.CreateTable(
                "dbo.CircleMemberships",
                c =>
                new
                    {
                        CircleMembershipId = c.Int(nullable: false, identity: true),
                        IsAdministrator = c.Boolean(nullable: false),
                        MemberSinceDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Circle_CircleId = c.Int(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.CircleMembershipId)
                .ForeignKey("dbo.Circles", t => t.Circle_CircleId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.Circle_CircleId)
                .Index(t => t.User_Id);

            this.CreateTable(
                "dbo.CircleJoinRequests",
                c =>
                new
                    {
                        CircleJoinRequestId = c.Int(nullable: false, identity: true),
                        AcceptToken = c.String(nullable: false, maxLength: 32),
                        JoinRequestDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Circle_CircleId = c.Int(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.CircleJoinRequestId)
                .ForeignKey("dbo.Circles", t => t.Circle_CircleId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.Circle_CircleId)
                .Index(t => t.User_Id);

            this.CreateTable(
                "dbo.CircleMessages",
                c =>
                new
                    {
                        MessageId = c.Int(nullable: false, identity: true),
                        CreationDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsPrivate = c.Boolean(nullable: false),
                        MessageText = c.String(),
                        Creator_Id = c.String(nullable: false, maxLength: 128),
                        Circle_CircleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MessageId)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id)
                .ForeignKey("dbo.Circles", t => t.Circle_CircleId)
                .Index(t => t.Creator_Id)
                .Index(t => t.Circle_CircleId);

            this.CreateTable(
                "dbo.CircleMessageReactions",
                c =>
                new
                    {
                        ReactionId = c.Int(nullable: false, identity: true),
                        CreationDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ReactionText = c.String(),
                        Circle_CircleId = c.Int(),
                        Creator_Id = c.String(nullable: false, maxLength: 128),
                        Message_MessageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ReactionId)
                .ForeignKey("dbo.Circles", t => t.Circle_CircleId)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id)
                .ForeignKey("dbo.CircleMessages", t => t.Message_MessageId)
                .Index(t => t.Circle_CircleId)
                .Index(t => t.Creator_Id)
                .Index(t => t.Message_MessageId);

            this.CreateTable(
                "dbo.AspNetUserClaims",
                c =>
                new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    }).PrimaryKey(t => t.Id).ForeignKey("dbo.AspNetUsers", t => t.UserId).Index(t => t.UserId);

            this.CreateTable(
                "dbo.AspNetUserLogins",
                c =>
                new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);

            this.CreateTable(
                "dbo.OrganizationInvitations",
                c =>
                new
                    {
                        OrganizationInvitationId = c.Int(nullable: false, identity: true),
                        AcceptToken = c.String(nullable: false, maxLength: 32),
                        Email = c.String(),
                        InvitationDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        InvitedBy_Id = c.String(nullable: false, maxLength: 128),
                        Organization_OrganizationId = c.Int(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.OrganizationInvitationId)
                .ForeignKey("dbo.AspNetUsers", t => t.InvitedBy_Id)
                .ForeignKey("dbo.Organizations", t => t.Organization_OrganizationId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.InvitedBy_Id)
                .Index(t => t.Organization_OrganizationId)
                .Index(t => t.User_Id);

            this.CreateTable(
                "dbo.Organizations",
                c =>
                new
                    {
                        OrganizationId = c.Int(nullable: false, identity: true),
                        Address = c.String(),
                        City = c.String(),
                        Description = c.String(),
                        Email = c.String(),
                        IsRequestPending = c.Boolean(nullable: false),
                        LogoId = c.Int(),
                        Name = c.String(nullable: false),
                        Phone = c.String(),
                        PostalCode = c.String(),
                        RequestDateTime = c.DateTime(precision: 7, storeType: "datetime2"),
                        Website = c.String(),
                        OrganizationType_OrganizationTypeId = c.Int(nullable: false),
                        RequestedBy_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.OrganizationId)
                .ForeignKey("dbo.Media", t => t.LogoId)
                .ForeignKey("dbo.OrganizationTypes", t => t.OrganizationType_OrganizationTypeId)
                .ForeignKey("dbo.AspNetUsers", t => t.RequestedBy_Id)
                .Index(t => t.LogoId)
                .Index(t => t.OrganizationType_OrganizationTypeId)
                .Index(t => t.RequestedBy_Id);

            this.CreateTable(
                "dbo.Districts",
                c =>
                new
                    {
                        DistrictId = c.Int(nullable: false, identity: true),
                        GM_CODE = c.String(nullable: false),
                        GM_NAAM = c.String(nullable: false),
                        WK_CODE = c.String(nullable: false),
                        WK_NAAM = c.String(nullable: false),
                        Shape = c.Geography(nullable: false),
                    }).PrimaryKey(t => t.DistrictId);

            this.CreateTable(
                "dbo.OrganizationMemberships",
                c =>
                new
                    {
                        OrganizationMembershipId = c.Int(nullable: false, identity: true),
                        IsAdministrator = c.Boolean(nullable: false),
                        MemberSinceDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Organization_OrganizationId = c.Int(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.OrganizationMembershipId)
                .ForeignKey("dbo.Organizations", t => t.Organization_OrganizationId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.Organization_OrganizationId)
                .Index(t => t.User_Id);

            this.CreateTable(
                "dbo.OrganizationTypes",
                c =>
                new
                    {
                        OrganizationTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Type = c.Int(nullable: false),
                    }).PrimaryKey(t => t.OrganizationTypeId);

            this.CreateTable(
                "dbo.AspNetUserRoles",
                c =>
                new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);

            this.CreateTable(
                "dbo.ActivityIntervals",
                c =>
                new
                    {
                        IntervalId = c.Int(nullable: false, identity: true),
                        RepeatDay = c.Int(),
                        RepeatMonth = c.Int(),
                        RepeatMonthWeek = c.Int(),
                        RepeatWeekDay = c.Int(),
                        RepeatYear = c.Int(),
                        Activity_ActivityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IntervalId)
                .ForeignKey("dbo.Activities", t => t.Activity_ActivityId, cascadeDelete: true)
                .Index(t => t.Activity_ActivityId);

            this.CreateTable(
                "dbo.Cities",
                c =>
                new
                    {
                        CityId = c.Int(nullable: false, identity: true),
                        GM_CODE = c.String(nullable: false),
                        GM_NAAM = c.String(nullable: false),
                        Enabled = c.Boolean(nullable: false),
                        Shape = c.Geography(nullable: false),
                    }).PrimaryKey(t => t.CityId);

            this.CreateTable(
                "dbo.MarketplaceItemCategories",
                c => new { CategoryId = c.Int(nullable: false, identity: true), Name = c.String(nullable: false), })
                .PrimaryKey(t => t.CategoryId);

            this.CreateTable(
                "dbo.MarketplaceItemReactions",
                c =>
                new
                    {
                        ReactionId = c.Int(nullable: false, identity: true),
                        CreationDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        OrganizationMembershipId = c.Int(),
                        ParentId = c.Int(),
                        Text = c.String(nullable: false),
                        Creator_Id = c.String(nullable: false, maxLength: 128),
                        MarketplaceItem_MarketplaceItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ReactionId)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id)
                .ForeignKey("dbo.MarketplaceItems", t => t.MarketplaceItem_MarketplaceItemId, cascadeDelete: true)
                .ForeignKey("dbo.OrganizationMemberships", t => t.OrganizationMembershipId)
                .ForeignKey("dbo.MarketplaceItemReactions", t => t.ParentId)
                .Index(t => t.OrganizationMembershipId)
                .Index(t => t.ParentId)
                .Index(t => t.Creator_Id)
                .Index(t => t.MarketplaceItem_MarketplaceItemId);

            this.CreateTable(
                "dbo.MarketplaceItems",
                c =>
                new
                    {
                        MarketplaceItemId = c.Int(nullable: false, identity: true),
                        CreationDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Description = c.String(),
                        Image1Id = c.Int(),
                        Image2Id = c.Int(),
                        Image3Id = c.Int(),
                        Image4Id = c.Int(),
                        Image5Id = c.Int(),
                        OrganizationMembershipId = c.Int(),
                        Title = c.String(nullable: false),
                        Type = c.Int(nullable: false),
                        Category_CategoryId = c.Int(nullable: false),
                        Owner_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.MarketplaceItemId)
                .ForeignKey("dbo.MarketplaceItemCategories", t => t.Category_CategoryId)
                .ForeignKey("dbo.Media", t => t.Image1Id)
                .ForeignKey("dbo.Media", t => t.Image2Id)
                .ForeignKey("dbo.Media", t => t.Image3Id)
                .ForeignKey("dbo.Media", t => t.Image4Id)
                .ForeignKey("dbo.Media", t => t.Image5Id)
                .ForeignKey("dbo.OrganizationMemberships", t => t.OrganizationMembershipId)
                .ForeignKey("dbo.AspNetUsers", t => t.Owner_Id, cascadeDelete: true)
                .Index(t => t.Image1Id)
                .Index(t => t.Image2Id)
                .Index(t => t.Image3Id)
                .Index(t => t.Image4Id)
                .Index(t => t.Image5Id)
                .Index(t => t.OrganizationMembershipId)
                .Index(t => t.Category_CategoryId)
                .Index(t => t.Owner_Id);

            this.CreateTable(
                "dbo.NeighborhoodMessages",
                c =>
                new
                    {
                        MessageId = c.Int(nullable: false, identity: true),
                        CreationDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        EventDateTime = c.DateTime(precision: 7, storeType: "datetime2"),
                        FullText = c.String(),
                        Image1Id = c.Int(),
                        IntroductionText = c.String(),
                        OrganizationMembershipId = c.Int(),
                        Title = c.String(nullable: false),
                        Creator_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.MessageId)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id, cascadeDelete: true)
                .ForeignKey("dbo.Media", t => t.Image1Id)
                .ForeignKey("dbo.OrganizationMemberships", t => t.OrganizationMembershipId)
                .Index(t => t.Image1Id)
                .Index(t => t.OrganizationMembershipId)
                .Index(t => t.Creator_Id);

            this.CreateTable(
                "dbo.Notifications",
                c =>
                new
                    {
                        NotificationId = c.Int(nullable: false, identity: true),
                        CreationDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ForUserId = c.String(nullable: false, maxLength: 128),
                        Message = c.String(),
                        Read = c.Boolean(nullable: false),
                        Shown = c.Boolean(nullable: false),
                        Url = c.String(),
                    })
                .PrimaryKey(t => t.NotificationId)
                .ForeignKey("dbo.AspNetUsers", t => t.ForUserId, cascadeDelete: true)
                .Index(t => t.ForUserId);

            this.CreateTable(
                "dbo.AspNetRoles",
                c =>
                new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    }).PrimaryKey(t => t.Id).Index(t => t.Name, unique: true, name: "RoleNameIndex");

            this.CreateTable(
                "dbo.JobCircleMemberships",
                c =>
                new
                    {
                        Job_JobId = c.Int(nullable: false),
                        CircleMembership_CircleMembershipId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Job_JobId, t.CircleMembership_CircleMembershipId })
                .ForeignKey("dbo.Jobs", t => t.Job_JobId, cascadeDelete: true)
                .ForeignKey("dbo.CircleMemberships", t => t.CircleMembership_CircleMembershipId, cascadeDelete: true)
                .Index(t => t.Job_JobId)
                .Index(t => t.CircleMembership_CircleMembershipId);

            this.CreateTable(
                "dbo.OrganizationDistricts",
                c =>
                new
                    {
                        Organization_OrganizationId = c.Int(nullable: false),
                        District_DistrictId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Organization_OrganizationId, t.District_DistrictId })
                .ForeignKey("dbo.Organizations", t => t.Organization_OrganizationId, cascadeDelete: true)
                .ForeignKey("dbo.Districts", t => t.District_DistrictId, cascadeDelete: true)
                .Index(t => t.Organization_OrganizationId)
                .Index(t => t.District_DistrictId);
        }
    }
}