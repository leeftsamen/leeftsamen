// <copyright file="201602161402300_AddSquareRestrictions.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddSquareRestrictions : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.SquareAdmins",
                c => new
                    {
                        SquareAdminId = c.Int(nullable: false, identity: true),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy_Id = c.String(maxLength: 128),
                        Square_SquareId = c.Int(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.SquareAdminId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedBy_Id)
                .ForeignKey("dbo.Squares", t => t.Square_SquareId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.CreatedBy_Id)
                .Index(t => t.Square_SquareId)
                .Index(t => t.User_Id);

            this.CreateTable(
                "dbo.SquareZipCodes",
                c => new
                    {
                        SquareZipCodeId = c.Int(nullable: false, identity: true),
                        ZipCode = c.String(),
                        Square_SquareId = c.Int(),
                    })
                .PrimaryKey(t => t.SquareZipCodeId)
                .ForeignKey("dbo.Squares", t => t.Square_SquareId)
                .Index(t => t.Square_SquareId);
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.SquareAdmins", "User_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.SquareZipCodes", "Square_SquareId", "dbo.Squares");
            this.DropForeignKey("dbo.SquareAdmins", "Square_SquareId", "dbo.Squares");
            this.DropForeignKey("dbo.SquareAdmins", "CreatedBy_Id", "dbo.AspNetUsers");
            this.DropIndex("dbo.SquareZipCodes", new[] { "Square_SquareId" });
            this.DropIndex("dbo.SquareAdmins", new[] { "User_Id" });
            this.DropIndex("dbo.SquareAdmins", new[] { "Square_SquareId" });
            this.DropIndex("dbo.SquareAdmins", new[] { "CreatedBy_Id" });
            this.DropTable("dbo.SquareZipCodes");
            this.DropTable("dbo.SquareAdmins");
        }
    }
}
