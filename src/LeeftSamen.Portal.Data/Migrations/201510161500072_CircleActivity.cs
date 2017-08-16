// <copyright file="201510161500072_CircleActivity.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class CircleActivity : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.CircleActivityInvitations",
                c => new
                    {
                        CircleActivityInvitationId = c.Int(nullable: false, identity: true),
                        AcceptToken = c.String(nullable: false, maxLength: 32),
                        Email = c.String(),
                        InvitationDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CircleActivity_ActivityId = c.Int(nullable: false),
                        InvitedBy_Id = c.String(nullable: false, maxLength: 128),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.CircleActivityInvitationId)
                .ForeignKey("dbo.CircleActivities", t => t.CircleActivity_ActivityId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.InvitedBy_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.CircleActivity_ActivityId)
                .Index(t => t.InvitedBy_Id)
                .Index(t => t.User_Id);

            this.CreateTable(
                "dbo.CircleActivities",
                c => new
                    {
                        ActivityId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Location = c.String(),
                        Title = c.String(),
                        CreationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        StartDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        EndDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        AllDay = c.Boolean(nullable: false),
                        AllAges = c.Boolean(nullable: false),
                        AgeFrom = c.Int(),
                        AgeTo = c.Int(),
                        Circle_CircleId = c.Int(),
                        Creator_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.ActivityId)
                .ForeignKey("dbo.Circles", t => t.Circle_CircleId)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id)
                .Index(t => t.Circle_CircleId)
                .Index(t => t.Creator_Id);

            this.CreateTable(
                "dbo.CircleActivityAttendances",
                c => new
                    {
                        ActivityAttendanceId = c.Int(nullable: false, identity: true),
                        Attending = c.Boolean(nullable: false),
                        UserJoinedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        User_Id = c.String(nullable: false, maxLength: 128),
                        CircleActivity_ActivityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ActivityAttendanceId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .ForeignKey("dbo.CircleActivities", t => t.CircleActivity_ActivityId, cascadeDelete: true)
                .Index(t => t.User_Id)
                .Index(t => t.CircleActivity_ActivityId);

            this.CreateTable(
                "dbo.CircleActivityReactions",
                c => new
                    {
                        ReactionId = c.Int(nullable: false, identity: true),
                        CircleId = c.Int(nullable: false),
                        CreationDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Text = c.String(nullable: false),
                        CircleActivity_ActivityId = c.Int(nullable: false),
                        Creator_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.ReactionId)
                .ForeignKey("dbo.CircleActivities", t => t.CircleActivity_ActivityId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id)
                .Index(t => t.CircleActivity_ActivityId)
                .Index(t => t.Creator_Id);

            this.CreateTable(
                "dbo.CircleEmailMessageReceivers",
                c => new
                    {
                        EmailMessageId = c.Int(nullable: false, identity: true),
                        ReceiverId = c.Int(nullable: false),
                        HasRemovedMessage = c.Boolean(nullable: false),
                        Receiver_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.EmailMessageId)
                .ForeignKey("dbo.CircleEmailMessages", t => t.ReceiverId)
                .ForeignKey("dbo.AspNetUsers", t => t.Receiver_Id)
                .Index(t => t.ReceiverId)
                .Index(t => t.Receiver_Id);

            this.CreateTable(
                "dbo.CircleEmailMessages",
                c => new
                    {
                        MessageId = c.Int(nullable: false, identity: true),
                        Text = c.String(nullable: false),
                        CreationDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Subject = c.String(nullable: false),
                        ParentMessageId = c.Int(),
                        CircleId = c.Int(nullable: false),
                        CreatorHasRemovedMessage = c.Boolean(nullable: false),
                        Creator_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.MessageId)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id)
                .ForeignKey("dbo.CircleEmailMessages", t => t.ParentMessageId)
                .Index(t => t.ParentMessageId)
                .Index(t => t.Creator_Id);
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.CircleEmailMessageReceivers", "Receiver_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.CircleEmailMessageReceivers", "ReceiverId", "dbo.CircleEmailMessages");
            this.DropForeignKey("dbo.CircleEmailMessages", "ParentMessageId", "dbo.CircleEmailMessages");
            this.DropForeignKey("dbo.CircleEmailMessages", "Creator_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.CircleActivityInvitations", "User_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.CircleActivityInvitations", "InvitedBy_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.CircleActivityReactions", "Creator_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.CircleActivityReactions", "CircleActivity_ActivityId", "dbo.CircleActivities");
            this.DropForeignKey("dbo.CircleActivityInvitations", "CircleActivity_ActivityId", "dbo.CircleActivities");
            this.DropForeignKey("dbo.CircleActivities", "Creator_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.CircleActivities", "Circle_CircleId", "dbo.Circles");
            this.DropForeignKey("dbo.CircleActivityAttendances", "CircleActivity_ActivityId", "dbo.CircleActivities");
            this.DropForeignKey("dbo.CircleActivityAttendances", "User_Id", "dbo.AspNetUsers");
            this.DropIndex("dbo.CircleEmailMessages", new[] { "Creator_Id" });
            this.DropIndex("dbo.CircleEmailMessages", new[] { "ParentMessageId" });
            this.DropIndex("dbo.CircleEmailMessageReceivers", new[] { "Receiver_Id" });
            this.DropIndex("dbo.CircleEmailMessageReceivers", new[] { "ReceiverId" });
            this.DropIndex("dbo.CircleActivityReactions", new[] { "Creator_Id" });
            this.DropIndex("dbo.CircleActivityReactions", new[] { "CircleActivity_ActivityId" });
            this.DropIndex("dbo.CircleActivityAttendances", new[] { "CircleActivity_ActivityId" });
            this.DropIndex("dbo.CircleActivityAttendances", new[] { "User_Id" });
            this.DropIndex("dbo.CircleActivities", new[] { "Creator_Id" });
            this.DropIndex("dbo.CircleActivities", new[] { "Circle_CircleId" });
            this.DropIndex("dbo.CircleActivityInvitations", new[] { "User_Id" });
            this.DropIndex("dbo.CircleActivityInvitations", new[] { "InvitedBy_Id" });
            this.DropIndex("dbo.CircleActivityInvitations", new[] { "CircleActivity_ActivityId" });
            this.DropTable("dbo.CircleEmailMessages");
            this.DropTable("dbo.CircleEmailMessageReceivers");
            this.DropTable("dbo.CircleActivityReactions");
            this.DropTable("dbo.CircleActivityAttendances");
            this.DropTable("dbo.CircleActivities");
            this.DropTable("dbo.CircleActivityInvitations");
        }
    }
}
