// <copyright file="201601191459442_AddActions.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddActions : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.Actions",
                c => new
                    {
                        ActionId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        HomeTitle = c.String(),
                        HomeText = c.String(),
                        MenuText = c.String(),
                        MoneyPerVote = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MoneyAvailable = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ActionStart = c.DateTime(),
                        ActionEnd = c.DateTime(),
                    })
                .PrimaryKey(t => t.ActionId);

            this.CreateTable(
                "dbo.ActionParticipants",
                c => new
                    {
                        ActionParticipantId = c.Int(nullable: false, identity: true),
                        Action_ActionId = c.Int(),
                        Organization_OrganizationId = c.Int(),
                    })
                .PrimaryKey(t => t.ActionParticipantId)
                .ForeignKey("dbo.Actions", t => t.Action_ActionId)
                .ForeignKey("dbo.Organizations", t => t.Organization_OrganizationId)
                .Index(t => t.Action_ActionId)
                .Index(t => t.Organization_OrganizationId);

            this.CreateTable(
                "dbo.ActionVotes",
                c => new
                    {
                        ActionVoteid = c.Int(nullable: false, identity: true),
                        Action_ActionId = c.Int(),
                        Creator_Id = c.String(maxLength: 128),
                        Organization_OrganizationId = c.Int(),
                    })
                .PrimaryKey(t => t.ActionVoteid)
                .ForeignKey("dbo.Actions", t => t.Action_ActionId)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id)
                .ForeignKey("dbo.Organizations", t => t.Organization_OrganizationId)
                .Index(t => t.Action_ActionId)
                .Index(t => t.Creator_Id)
                .Index(t => t.Organization_OrganizationId);
            this.CreateIndex("dbo.ActionVotes",
                new string[] { "Action_ActionId", "Creator_Id"},
                unique: true,
                name: "IX_UNIQUEVOTE");
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.ActionVotes", "Organization_OrganizationId", "dbo.Organizations");
            this.DropForeignKey("dbo.ActionVotes", "Creator_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.ActionVotes", "Action_ActionId", "dbo.Actions");
            this.DropForeignKey("dbo.ActionParticipants", "Organization_OrganizationId", "dbo.Organizations");
            this.DropForeignKey("dbo.ActionParticipants", "Action_ActionId", "dbo.Actions");
            this.DropIndex("dbo.ActionVotes", new[] { "Organization_OrganizationId" });
            this.DropIndex("dbo.ActionVotes", new[] { "Creator_Id" });
            this.DropIndex("dbo.ActionVotes", new[] { "Action_ActionId" });
            this.DropIndex("dbo.ActionVotes", new[] { "IX_UNIQUEVOTE" });
            this.DropIndex("dbo.ActionParticipants", new[] { "Organization_OrganizationId" });
            this.DropIndex("dbo.ActionParticipants", new[] { "Action_ActionId" });
            this.DropTable("dbo.ActionVotes");
            this.DropTable("dbo.ActionParticipants");
            this.DropTable("dbo.Actions");
        }
    }
}
