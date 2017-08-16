// <copyright file="201605180845557_CircleLabels.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class CircleLabels : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.CircleLabels",
                c => new
                    {
                        CircleLabelId = c.Int(nullable: false, identity: true),
                        Label = c.String(),
                        Text = c.String(),
                        Circle_CircleId = c.Int(),
                    })
                .PrimaryKey(t => t.CircleLabelId)
                .ForeignKey("dbo.Circles", t => t.Circle_CircleId)
                .Index(t => t.Circle_CircleId);
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.CircleLabels", "Circle_CircleId", "dbo.Circles");
            this.DropIndex("dbo.CircleLabels", new[] { "Circle_CircleId" });
            this.DropTable("dbo.CircleLabels");
        }
    }
}
