// <copyright file="201507231446276_OrganizationTheme.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class OrganizationTheme : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.OrganizationThemes",
                c => new
                    {
                        ThemeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ThemeId);

            this.CreateTable(
                "dbo.OrganizationOrganizationThemes",
                c => new
                    {
                        Organization_OrganizationId = c.Int(nullable: false),
                        OrganizationTheme_ThemeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Organization_OrganizationId, t.OrganizationTheme_ThemeId })
                .ForeignKey("dbo.Organizations", t => t.Organization_OrganizationId, cascadeDelete: true)
                .ForeignKey("dbo.OrganizationThemes", t => t.OrganizationTheme_ThemeId, cascadeDelete: true)
                .Index(t => t.Organization_OrganizationId)
                .Index(t => t.OrganizationTheme_ThemeId);
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.OrganizationOrganizationThemes", "OrganizationTheme_ThemeId", "dbo.OrganizationThemes");
            this.DropForeignKey("dbo.OrganizationOrganizationThemes", "Organization_OrganizationId", "dbo.Organizations");
            this.DropIndex("dbo.OrganizationOrganizationThemes", new[] { "OrganizationTheme_ThemeId" });
            this.DropIndex("dbo.OrganizationOrganizationThemes", new[] { "Organization_OrganizationId" });
            this.DropTable("dbo.OrganizationOrganizationThemes");
            this.DropTable("dbo.OrganizationThemes");
        }
    }
}
