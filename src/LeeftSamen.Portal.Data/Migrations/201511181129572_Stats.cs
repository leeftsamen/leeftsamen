// <copyright file="201511181129572_Stats.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Stats : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.Stats",
                c => new
                    {
                        StatId = c.Int(nullable: false, identity: true),
                        ActivitiesAttendees = c.Int(nullable: false),
                        ActivitiesTotal = c.Int(nullable: false),
                        CircleJobs = c.Int(nullable: false),
                        CircleMembers = c.Int(nullable: false),
                        CircleMessages = c.Int(nullable: false),
                        CirclesPrivate = c.Int(nullable: false),
                        CirclesPublic = c.Int(nullable: false),
                        CirclesTotal = c.Int(nullable: false),
                        DateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        MarketplaceItemsOffered = c.Int(nullable: false),
                        MarketplaceItemsTotal = c.Int(nullable: false),
                        MarketplaceItemsAsked = c.Int(nullable: false),
                        NeighborhoodAssociationMessages = c.Int(nullable: false),
                        NeighborhoodMessagesTotal = c.Int(nullable: false),
                        NeighborhoodNeighborMessages = c.Int(nullable: false),
                        NeighborhoodOrganizationMessages = c.Int(nullable: false),
                        OrganisationsTotal = c.Int(nullable: false),
                        OrganisationVolunteers = c.Int(nullable: false),
                        OrganisationAssociations = c.Int(nullable: false),
                        OrganisationProfessionals = c.Int(nullable: false),
                        UsersActiveDays = c.Int(nullable: false),
                        UsersActiveHours = c.Int(nullable: false),
                        UsersAvgNeighborhoodRadius = c.Int(nullable: false),
                        UsersTotal = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StatId);
        }

        public override void Down()
        {
            this.DropTable("dbo.Stats");
        }
    }
}
