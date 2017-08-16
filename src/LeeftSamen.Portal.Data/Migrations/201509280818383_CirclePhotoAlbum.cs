// <copyright file="201509280818383_CirclePhotoAlbum.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class CirclePhotoAlbum : DbMigration
    {
        public override void Up()
        {
            this.DropForeignKey("dbo.CirclePhotoes", "Circle_CircleId", "dbo.Circles");
            this.CreateTable(
                "dbo.CirclePhotoAlbums",
                c => new
                    {
                        CirclePhotoAlbumId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Circle_CircleId = c.Int(nullable: false),
                        CreatedBy_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.CirclePhotoAlbumId)
                .ForeignKey("dbo.Circles", t => t.Circle_CircleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedBy_Id)
                .Index(t => t.Circle_CircleId)
                .Index(t => t.CreatedBy_Id);

            this.AddColumn("dbo.CirclePhotoes", "CirclePhotoAlbum_CirclePhotoAlbumId", c => c.Int());
            this.CreateIndex("dbo.CirclePhotoes", "CirclePhotoAlbum_CirclePhotoAlbumId");
            this.AddForeignKey("dbo.CirclePhotoes", "CirclePhotoAlbum_CirclePhotoAlbumId", "dbo.CirclePhotoAlbums", "CirclePhotoAlbumId", cascadeDelete: true);
            this.AddForeignKey("dbo.CirclePhotoes", "Circle_CircleId", "dbo.Circles", "CircleId");
            this.DropColumn("dbo.CirclePhotoes", "CreationDate");
        }

        public override void Down()
        {
            this.AddColumn("dbo.CirclePhotoes", "CreationDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            this.DropForeignKey("dbo.CirclePhotoes", "Circle_CircleId", "dbo.Circles");
            this.DropForeignKey("dbo.CirclePhotoes", "CirclePhotoAlbum_CirclePhotoAlbumId", "dbo.CirclePhotoAlbums");
            this.DropForeignKey("dbo.CirclePhotoAlbums", "CreatedBy_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.CirclePhotoAlbums", "Circle_CircleId", "dbo.Circles");
            this.DropIndex("dbo.CirclePhotoes", new[] { "CirclePhotoAlbum_CirclePhotoAlbumId" });
            this.DropIndex("dbo.CirclePhotoAlbums", new[] { "CreatedBy_Id" });
            this.DropIndex("dbo.CirclePhotoAlbums", new[] { "Circle_CircleId" });
            this.DropColumn("dbo.CirclePhotoes", "CirclePhotoAlbum_CirclePhotoAlbumId");
            this.DropTable("dbo.CirclePhotoAlbums");
            this.AddForeignKey("dbo.CirclePhotoes", "Circle_CircleId", "dbo.Circles", "CircleId", cascadeDelete: true);
        }
    }
}
