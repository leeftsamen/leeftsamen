// <copyright file="201604140928496_DeviceTypes.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class DeviceTypes : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.UserDevices",
                c => new
                    {
                        UserDeviceId = c.Int(nullable: false, identity: true),
                        Token = c.String(),
                        LastUseDate = c.DateTime(nullable: false),
                        DeviceType = c.Int(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.UserDeviceId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);

            this.DropColumn("dbo.AspNetUsers", "IosPushNotificationToken");
            this.DropColumn("dbo.AspNetUsers", "AndroidDeviceRegistrationId");
        }

        public override void Down()
        {
            this.AddColumn("dbo.AspNetUsers", "AndroidDeviceRegistrationId", c => c.String());
            this.AddColumn("dbo.AspNetUsers", "IosPushNotificationToken", c => c.String());
            this.DropForeignKey("dbo.UserDevices", "User_Id", "dbo.AspNetUsers");
            this.DropIndex("dbo.UserDevices", new[] { "User_Id" });
            this.DropTable("dbo.UserDevices");
        }
    }
}
