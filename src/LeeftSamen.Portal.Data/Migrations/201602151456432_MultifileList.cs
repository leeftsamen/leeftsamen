// <copyright file="201602151456432_MultifileList.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class MultifileList : DbMigration
    {
        public override void Up()
        {
            this.DropForeignKey("dbo.SquareFacts", "File1_MediaId", "dbo.Media");
            this.DropForeignKey("dbo.SquareFacts", "File2_MediaId", "dbo.Media");
            this.DropForeignKey("dbo.SquareFacts", "File3_MediaId", "dbo.Media");
            this.DropForeignKey("dbo.SquareFacts", "File4_MediaId", "dbo.Media");
            this.DropForeignKey("dbo.SquareFacts", "OverviewImage_MediaId", "dbo.Media");
            this.DropIndex("dbo.SquareFacts", new[] { "File1_MediaId" });
            this.DropIndex("dbo.SquareFacts", new[] { "File2_MediaId" });
            this.DropIndex("dbo.SquareFacts", new[] { "File3_MediaId" });
            this.DropIndex("dbo.SquareFacts", new[] { "File4_MediaId" });
            this.DropIndex("dbo.SquareFacts", new[] { "OverviewImage_MediaId" });
            this.CreateTable(
                "dbo.SquareFactMedias",
                c => new
                    {
                        SquareFactMediaId = c.Int(nullable: false, identity: true),
                        Media_MediaId = c.Int(),
                        SquareFact_SquareFactId = c.Int(),
                    })
                .PrimaryKey(t => t.SquareFactMediaId)
                .ForeignKey("dbo.Media", t => t.Media_MediaId)
                .ForeignKey("dbo.SquareFacts", t => t.SquareFact_SquareFactId)
                .Index(t => t.Media_MediaId)
                .Index(t => t.SquareFact_SquareFactId);

            this.DropColumn("dbo.SquareFacts", "OverviewImageId");
            this.DropColumn("dbo.SquareFacts", "File1Id");
            this.DropColumn("dbo.SquareFacts", "File2Id");
            this.DropColumn("dbo.SquareFacts", "File3Id");
            this.DropColumn("dbo.SquareFacts", "File4Id");
            this.DropColumn("dbo.SquareFacts", "File1_MediaId");
            this.DropColumn("dbo.SquareFacts", "File2_MediaId");
            this.DropColumn("dbo.SquareFacts", "File3_MediaId");
            this.DropColumn("dbo.SquareFacts", "File4_MediaId");
            this.DropColumn("dbo.SquareFacts", "OverviewImage_MediaId");
        }

        public override void Down()
        {
            this.AddColumn("dbo.SquareFacts", "OverviewImage_MediaId", c => c.Int());
            this.AddColumn("dbo.SquareFacts", "File4_MediaId", c => c.Int());
            this.AddColumn("dbo.SquareFacts", "File3_MediaId", c => c.Int());
            this.AddColumn("dbo.SquareFacts", "File2_MediaId", c => c.Int());
            this.AddColumn("dbo.SquareFacts", "File1_MediaId", c => c.Int());
            this.AddColumn("dbo.SquareFacts", "File4Id", c => c.Int());
            this.AddColumn("dbo.SquareFacts", "File3Id", c => c.Int());
            this.AddColumn("dbo.SquareFacts", "File2Id", c => c.Int());
            this.AddColumn("dbo.SquareFacts", "File1Id", c => c.Int());
            this.AddColumn("dbo.SquareFacts", "OverviewImageId", c => c.Int());
            this.DropForeignKey("dbo.SquareFactMedias", "SquareFact_SquareFactId", "dbo.SquareFacts");
            this.DropForeignKey("dbo.SquareFactMedias", "Media_MediaId", "dbo.Media");
            this.DropIndex("dbo.SquareFactMedias", new[] { "SquareFact_SquareFactId" });
            this.DropIndex("dbo.SquareFactMedias", new[] { "Media_MediaId" });
            this.DropTable("dbo.SquareFactMedias");
            this.CreateIndex("dbo.SquareFacts", "OverviewImage_MediaId");
            this.CreateIndex("dbo.SquareFacts", "File4_MediaId");
            this.CreateIndex("dbo.SquareFacts", "File3_MediaId");
            this.CreateIndex("dbo.SquareFacts", "File2_MediaId");
            this.CreateIndex("dbo.SquareFacts", "File1_MediaId");
            this.AddForeignKey("dbo.SquareFacts", "OverviewImage_MediaId", "dbo.Media", "MediaId");
            this.AddForeignKey("dbo.SquareFacts", "File4_MediaId", "dbo.Media", "MediaId");
            this.AddForeignKey("dbo.SquareFacts", "File3_MediaId", "dbo.Media", "MediaId");
            this.AddForeignKey("dbo.SquareFacts", "File2_MediaId", "dbo.Media", "MediaId");
            this.AddForeignKey("dbo.SquareFacts", "File1_MediaId", "dbo.Media", "MediaId");
        }
    }
}
