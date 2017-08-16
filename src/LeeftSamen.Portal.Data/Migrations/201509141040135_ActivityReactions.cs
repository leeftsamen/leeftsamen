// <copyright file="201509141040135_ActivityReactions.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ActivityReactions : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.ActivityReactions",
                c => new
                    {
                        ReactionId = c.Int(nullable: false, identity: true),
                        CreationDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        OrganizationMembershipId = c.Int(),
                        Text = c.String(nullable: false),
                        Activity_ActivityId = c.Int(nullable: false),
                        Creator_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.ReactionId)
                .ForeignKey("dbo.Activities", t => t.Activity_ActivityId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id)
                .ForeignKey("dbo.OrganizationMemberships", t => t.OrganizationMembershipId)
                .Index(t => t.OrganizationMembershipId)
                .Index(t => t.Activity_ActivityId)
                .Index(t => t.Creator_Id);
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.ActivityReactions", "OrganizationMembershipId", "dbo.OrganizationMemberships");
            this.DropForeignKey("dbo.ActivityReactions", "Creator_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.ActivityReactions", "Activity_ActivityId", "dbo.Activities");
            this.DropIndex("dbo.ActivityReactions", new[] { "Creator_Id" });
            this.DropIndex("dbo.ActivityReactions", new[] { "Activity_ActivityId" });
            this.DropIndex("dbo.ActivityReactions", new[] { "OrganizationMembershipId" });
            this.DropTable("dbo.ActivityReactions");
        }
    }
}
