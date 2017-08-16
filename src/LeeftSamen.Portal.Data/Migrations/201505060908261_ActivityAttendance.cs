// <copyright file="201505060908261_ActivityAttendance.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class ActivityAttendance : DbMigration
    {
        public override void Down()
        {
            this.DropForeignKey("dbo.ActivityAttendances", "User_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.ActivityAttendances", "Activity_ActivityId", "dbo.Activities");
            this.DropIndex("dbo.ActivityAttendances", new[] { "User_Id" });
            this.DropIndex("dbo.ActivityAttendances", new[] { "Activity_ActivityId" });
            this.DropTable("dbo.ActivityAttendances");
        }

        public override void Up()
        {
            this.CreateTable(
                "dbo.ActivityAttendances",
                c =>
                new
                    {
                        ActivityAttendanceId = c.Int(nullable: false, identity: true),
                        Attending = c.Boolean(nullable: false),
                        ModificationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Activity_ActivityId = c.Int(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.ActivityAttendanceId)
                .ForeignKey("dbo.Activities", t => t.Activity_ActivityId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.Activity_ActivityId)
                .Index(t => t.User_Id);
        }
    }
}