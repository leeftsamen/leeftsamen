// <copyright file="201607071341596_UserSettings.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class UserSettings : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.Settings",
                c => new
                    {
                        SettingId = c.Int(nullable: false, identity: true),
                        Group = c.String(),
                        Name = c.String(),
                        DefaultValue = c.String(),
                    })
                .PrimaryKey(t => t.SettingId);

            this.CreateTable(
                "dbo.UserSettings",
                c => new
                    {
                        UserSettingId = c.Int(nullable: false, identity: true),
                        SettingId = c.Int(nullable: false),
                        UserId = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.UserSettingId);
        }

        public override void Down()
        {
            this.DropTable("dbo.UserSettings");
            this.DropTable("dbo.Settings");
        }
    }
}
