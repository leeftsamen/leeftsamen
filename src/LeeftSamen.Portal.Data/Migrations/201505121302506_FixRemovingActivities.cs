// <copyright file="201505121302506_FixRemovingActivities.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class FixRemovingActivities : DbMigration
    {
        public override void Down()
        {
            this.DropForeignKey("dbo.ActivityAttendances", "Activity_ActivityId", "dbo.Activities");
            this.AddForeignKey("dbo.ActivityAttendances", "Activity_ActivityId", "dbo.Activities", "ActivityId");
        }

        public override void Up()
        {
            this.DropForeignKey("dbo.ActivityAttendances", "Activity_ActivityId", "dbo.Activities");
            this.AddForeignKey(
                "dbo.ActivityAttendances",
                "Activity_ActivityId",
                "dbo.Activities",
                "ActivityId",
                cascadeDelete: true);
        }
    }
}