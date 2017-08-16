// <copyright file="201505081512487_HelpIcon.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class HelpIcon : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.HelpIcons",
                c => new
                    {
                        HelpIconId = c.Int(nullable: false, identity: true),
                        Text = c.String(nullable: false),
                        TextPlacement = c.String(nullable: false),
                        Type = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.HelpIconId);

            this.CreateTable(
                "dbo.HelpIconUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HelpIcon_HelpIconId = c.Int(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.HelpIcons", t => t.HelpIcon_HelpIconId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.HelpIcon_HelpIconId)
                .Index(t => t.User_Id);
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.HelpIconUsers", "User_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.HelpIconUsers", "HelpIcon_HelpIconId", "dbo.HelpIcons");
            this.DropIndex("dbo.HelpIconUsers", new[] { "User_Id" });
            this.DropIndex("dbo.HelpIconUsers", new[] { "HelpIcon_HelpIconId" });
            this.DropTable("dbo.HelpIconUsers");
            this.DropTable("dbo.HelpIcons");
        }
    }
}
