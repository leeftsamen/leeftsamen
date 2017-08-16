// <copyright file="201603090832266_PushNotifications.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class PushNotifications : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.PushNotifications",
                c => new
                    {
                        PushNotificationId = c.Int(nullable: false, identity: true),
                        Alert = c.String(),
                        Badge = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        IsProcessed = c.Boolean(nullable: false),
                        ProcessedDate = c.DateTime(),
                        Type = c.Int(nullable: false),
                        TypeId = c.Int(),
                    })
                .PrimaryKey(t => t.PushNotificationId);

            this.AddColumn("dbo.AspNetUsers", "IosPushNotificationToken", c => c.String());
            this.AddColumn("dbo.AspNetUsers", "AndroidDeviceRegistrationId", c => c.String());
        }

        public override void Down()
        {
            this.DropColumn("dbo.AspNetUsers", "AndroidDeviceRegistrationId");
            this.DropColumn("dbo.AspNetUsers", "IosPushNotificationToken");
            this.DropTable("dbo.PushNotifications");
        }
    }
}
