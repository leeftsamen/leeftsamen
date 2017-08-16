// <copyright file="201606031413008_CircleSettings.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class CircleSettings : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.CircleSettings",
                c => new
                    {
                        CircleSettingId = c.Int(nullable: false, identity: true),
                        SettingName = c.String(),
                        Value = c.String(),
                        Circle_CircleId = c.Int(),
                    })
                .PrimaryKey(t => t.CircleSettingId)
                .ForeignKey("dbo.Circles", t => t.Circle_CircleId)
                .Index(t => t.Circle_CircleId);
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.CircleSettings", "Circle_CircleId", "dbo.Circles");
            this.DropIndex("dbo.CircleSettings", new[] { "Circle_CircleId" });
            this.DropTable("dbo.CircleSettings");
        }
    }
}
