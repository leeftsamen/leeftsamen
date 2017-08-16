// <copyright file="201602220855176_AddFeaturedCircles.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddFeaturedCircles : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.FeaturedCircles",
                c => new
                    {
                        FeaturedCircleId = c.Int(nullable: false, identity: true),
                        Circle_CircleId = c.Int(),
                    })
                .PrimaryKey(t => t.FeaturedCircleId)
                .ForeignKey("dbo.Circles", t => t.Circle_CircleId)
                .Index(t => t.Circle_CircleId);
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.FeaturedCircles", "Circle_CircleId", "dbo.Circles");
            this.DropIndex("dbo.FeaturedCircles", new[] { "Circle_CircleId" });
            this.DropTable("dbo.FeaturedCircles");
        }
    }
}
