// <copyright file="201601080943558_MissingOrganisationTables.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class MissingOrganisationTables : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.OrganizationProducts",
                c =>
                new
                    {
                        OrganizationProductId = c.Int(false, true),
                        FullText = c.String(true),
                        IntroductionText = c.String(true),
                        Title = c.String(false),
                        Organization_OrganizationId = c.Int(false)
                    })
                .PrimaryKey(t => t.OrganizationProductId)
                .ForeignKey("dbo.Organizations", t => t.Organization_OrganizationId)
                .Index(t => t.Organization_OrganizationId);

            this.CreateTable(
                "dbo.OrganizationServices",
                c =>
                new
                {
                    OrganizationServiceId = c.Int(false, true),
                    FullText = c.String(true),
                    IntroductionText = c.String(true),
                    Title = c.String(false),
                    Organization_OrganizationId = c.Int(false)
                })
                .PrimaryKey(t => t.OrganizationServiceId)
                .ForeignKey("dbo.Organizations", t => t.Organization_OrganizationId)
                .Index(t => t.Organization_OrganizationId);
        }

        public override void Down()
        {
            this.DropTable("dbo.OrganizationProducts");
            this.DropForeignKey("dbo.OrganizationProducts", "Organization_OrganizationId", "dbo.Organizations");

            this.DropTable("dbo.OrganizationServices");
            this.DropForeignKey("dbo.OrganizationServices", "Organization_OrganizationId", "dbo.Organizations");
        }
    }
}