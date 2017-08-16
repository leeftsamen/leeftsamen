// <copyright file="201607111216431_NeighborhoodMessageImages.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class NeighborhoodMessageImages : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.NeighborhoodMessages", "Image2Id", c => c.Int());
            this.AddColumn("dbo.NeighborhoodMessages", "Image3Id", c => c.Int());
            this.AddColumn("dbo.NeighborhoodMessages", "Image4Id", c => c.Int());
            this.AddColumn("dbo.NeighborhoodMessages", "Image5Id", c => c.Int());
            this.CreateIndex("dbo.NeighborhoodMessages", "Image2Id");
            this.CreateIndex("dbo.NeighborhoodMessages", "Image3Id");
            this.CreateIndex("dbo.NeighborhoodMessages", "Image4Id");
            this.CreateIndex("dbo.NeighborhoodMessages", "Image5Id");
            this.AddForeignKey("dbo.NeighborhoodMessages", "Image2Id", "dbo.Media", "MediaId");
            this.AddForeignKey("dbo.NeighborhoodMessages", "Image3Id", "dbo.Media", "MediaId");
            this.AddForeignKey("dbo.NeighborhoodMessages", "Image4Id", "dbo.Media", "MediaId");
            this.AddForeignKey("dbo.NeighborhoodMessages", "Image5Id", "dbo.Media", "MediaId");
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.NeighborhoodMessages", "Image5Id", "dbo.Media");
            this.DropForeignKey("dbo.NeighborhoodMessages", "Image4Id", "dbo.Media");
            this.DropForeignKey("dbo.NeighborhoodMessages", "Image3Id", "dbo.Media");
            this.DropForeignKey("dbo.NeighborhoodMessages", "Image2Id", "dbo.Media");
            this.DropIndex("dbo.NeighborhoodMessages", new[] { "Image5Id" });
            this.DropIndex("dbo.NeighborhoodMessages", new[] { "Image4Id" });
            this.DropIndex("dbo.NeighborhoodMessages", new[] { "Image3Id" });
            this.DropIndex("dbo.NeighborhoodMessages", new[] { "Image2Id" });
            this.DropColumn("dbo.NeighborhoodMessages", "Image5Id");
            this.DropColumn("dbo.NeighborhoodMessages", "Image4Id");
            this.DropColumn("dbo.NeighborhoodMessages", "Image3Id");
            this.DropColumn("dbo.NeighborhoodMessages", "Image2Id");
        }
    }
}
