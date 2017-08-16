// <copyright file="201509141339166_CirclePhoto.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class CirclePhoto : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.CirclePhotoes",
                c => new
                    {
                        CirclePhotoId = c.Int(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        PhotoId = c.Int(nullable: false),
                        Circle_CircleId = c.Int(nullable: false),
                        UploadedBy_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.CirclePhotoId)
                .ForeignKey("dbo.Circles", t => t.Circle_CircleId, cascadeDelete: true)
                .ForeignKey("dbo.Media", t => t.PhotoId)
                .ForeignKey("dbo.AspNetUsers", t => t.UploadedBy_Id)
                .Index(t => t.PhotoId)
                .Index(t => t.Circle_CircleId)
                .Index(t => t.UploadedBy_Id);
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.CirclePhotoes", "UploadedBy_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.CirclePhotoes", "PhotoId", "dbo.Media");
            this.DropForeignKey("dbo.CirclePhotoes", "Circle_CircleId", "dbo.Circles");
            this.DropIndex("dbo.CirclePhotoes", new[] { "UploadedBy_Id" });
            this.DropIndex("dbo.CirclePhotoes", new[] { "Circle_CircleId" });
            this.DropIndex("dbo.CirclePhotoes", new[] { "PhotoId" });
            this.DropTable("dbo.CirclePhotoes");
        }
    }
}
