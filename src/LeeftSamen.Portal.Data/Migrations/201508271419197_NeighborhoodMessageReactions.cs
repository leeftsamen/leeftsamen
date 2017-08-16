// <copyright file="201508271419197_NeighborhoodMessageReactions.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class NeighborhoodMessageReactions : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.NeighborhoodMessageReactions",
                c => new
                    {
                        ReactionId = c.Int(nullable: false, identity: true),
                        CreationDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        OrganizationMembershipId = c.Int(),
                        ParentId = c.Int(),
                        Text = c.String(nullable: false),
                        Creator_Id = c.String(nullable: false, maxLength: 128),
                        NeighborhoodMessage_MessageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ReactionId)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id)
                .ForeignKey("dbo.NeighborhoodMessages", t => t.NeighborhoodMessage_MessageId, cascadeDelete: true)
                .ForeignKey("dbo.OrganizationMemberships", t => t.OrganizationMembershipId)
                .ForeignKey("dbo.NeighborhoodMessageReactions", t => t.ParentId)
                .Index(t => t.OrganizationMembershipId)
                .Index(t => t.ParentId)
                .Index(t => t.Creator_Id)
                .Index(t => t.NeighborhoodMessage_MessageId);
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.NeighborhoodMessageReactions", "ParentId", "dbo.NeighborhoodMessageReactions");
            this.DropForeignKey("dbo.NeighborhoodMessageReactions", "OrganizationMembershipId", "dbo.OrganizationMemberships");
            this.DropForeignKey("dbo.NeighborhoodMessageReactions", "NeighborhoodMessage_MessageId", "dbo.NeighborhoodMessages");
            this.DropForeignKey("dbo.NeighborhoodMessageReactions", "Creator_Id", "dbo.AspNetUsers");
            this.DropIndex("dbo.NeighborhoodMessageReactions", new[] { "NeighborhoodMessage_MessageId" });
            this.DropIndex("dbo.NeighborhoodMessageReactions", new[] { "Creator_Id" });
            this.DropIndex("dbo.NeighborhoodMessageReactions", new[] { "ParentId" });
            this.DropIndex("dbo.NeighborhoodMessageReactions", new[] { "OrganizationMembershipId" });
            this.DropTable("dbo.NeighborhoodMessageReactions");
        }
    }
}
