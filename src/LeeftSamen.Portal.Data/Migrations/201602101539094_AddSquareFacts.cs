// <copyright file="201602101539094_AddSquareFacts.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddSquareFacts : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.SquareFacts",
                c => new
                    {
                        SquareFactId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        IntroductionText = c.String(),
                        FullText = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                        OverviewImageId = c.Int(),
                        File1Id = c.Int(),
                        File2Id = c.Int(),
                        File3Id = c.Int(),
                        File4Id = c.Int(),
                        Creator_Id = c.String(maxLength: 128),
                        File1_MediaId = c.Int(),
                        File2_MediaId = c.Int(),
                        File3_MediaId = c.Int(),
                        File4_MediaId = c.Int(),
                        OverviewImage_MediaId = c.Int(),
                        Square_SquareId = c.Int(),
                    })
                .PrimaryKey(t => t.SquareFactId)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id)
                .ForeignKey("dbo.Media", t => t.File1_MediaId)
                .ForeignKey("dbo.Media", t => t.File2_MediaId)
                .ForeignKey("dbo.Media", t => t.File3_MediaId)
                .ForeignKey("dbo.Media", t => t.File4_MediaId)
                .ForeignKey("dbo.Media", t => t.OverviewImage_MediaId)
                .ForeignKey("dbo.Squares", t => t.Square_SquareId)
                .Index(t => t.Creator_Id)
                .Index(t => t.File1_MediaId)
                .Index(t => t.File2_MediaId)
                .Index(t => t.File3_MediaId)
                .Index(t => t.File4_MediaId)
                .Index(t => t.OverviewImage_MediaId)
                .Index(t => t.Square_SquareId);

            this.AddColumn("dbo.Squares", "Name", c => c.String());
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.SquareFacts", "Square_SquareId", "dbo.Squares");
            this.DropForeignKey("dbo.SquareFacts", "OverviewImage_MediaId", "dbo.Media");
            this.DropForeignKey("dbo.SquareFacts", "File4_MediaId", "dbo.Media");
            this.DropForeignKey("dbo.SquareFacts", "File3_MediaId", "dbo.Media");
            this.DropForeignKey("dbo.SquareFacts", "File2_MediaId", "dbo.Media");
            this.DropForeignKey("dbo.SquareFacts", "File1_MediaId", "dbo.Media");
            this.DropForeignKey("dbo.SquareFacts", "Creator_Id", "dbo.AspNetUsers");
            this.DropIndex("dbo.SquareFacts", new[] { "Square_SquareId" });
            this.DropIndex("dbo.SquareFacts", new[] { "OverviewImage_MediaId" });
            this.DropIndex("dbo.SquareFacts", new[] { "File4_MediaId" });
            this.DropIndex("dbo.SquareFacts", new[] { "File3_MediaId" });
            this.DropIndex("dbo.SquareFacts", new[] { "File2_MediaId" });
            this.DropIndex("dbo.SquareFacts", new[] { "File1_MediaId" });
            this.DropIndex("dbo.SquareFacts", new[] { "Creator_Id" });
            this.DropColumn("dbo.Squares", "Name");
            this.DropTable("dbo.SquareFacts");
        }
    }
}
