// <copyright file="201606030727383_ActivityCircle.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ActivityCircle : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.Activities", "Circle_CircleId", c => c.Int());
            this.CreateIndex("dbo.Activities", "Circle_CircleId");
            this.AddForeignKey("dbo.Activities", "Circle_CircleId", "dbo.Circles", "CircleId");
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.Activities", "Circle_CircleId", "dbo.Circles");
            this.DropIndex("dbo.Activities", new[] { "Circle_CircleId" });
            this.DropColumn("dbo.Activities", "Circle_CircleId");
        }
    }
}
