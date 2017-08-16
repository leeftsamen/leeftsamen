// <copyright file="201509081437233_ActivityInvitation.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ActivityInvitation : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.ActivityInvitations",
                c => new
                    {
                        ActivityInvitationId = c.Int(nullable: false, identity: true),
                        AcceptToken = c.String(nullable: false, maxLength: 32),
                        Email = c.String(),
                        InvitationDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        InvitedBy_Id = c.String(nullable: false, maxLength: 128),
                        User_Id = c.String(maxLength: 128),
                        Activity_ActivityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ActivityInvitationId)
                .ForeignKey("dbo.AspNetUsers", t => t.InvitedBy_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .ForeignKey("dbo.Activities", t => t.Activity_ActivityId, cascadeDelete: false)
                .Index(t => t.InvitedBy_Id)
                .Index(t => t.User_Id)
                .Index(t => t.Activity_ActivityId);
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.ActivityInvitations", "Activity_ActivityId", "dbo.Activities");
            this.DropForeignKey("dbo.ActivityInvitations", "User_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.ActivityInvitations", "InvitedBy_Id", "dbo.AspNetUsers");
            this.DropIndex("dbo.ActivityInvitations", new[] { "Activity_ActivityId" });
            this.DropIndex("dbo.ActivityInvitations", new[] { "User_Id" });
            this.DropIndex("dbo.ActivityInvitations", new[] { "InvitedBy_Id" });
            this.DropTable("dbo.ActivityInvitations");
        }
    }
}
