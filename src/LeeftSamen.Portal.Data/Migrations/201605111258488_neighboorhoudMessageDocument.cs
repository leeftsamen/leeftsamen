// <copyright file="201605111258488_neighboorhoudMessageDocument.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class neighboorhoudMessageDocument : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.NeighborhoodMessages", "File1Id", c => c.Int());
            this.CreateIndex("dbo.NeighborhoodMessages", "File1Id");
            this.AddForeignKey("dbo.NeighborhoodMessages", "File1Id", "dbo.Media", "MediaId");
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.NeighborhoodMessages", "File1Id", "dbo.Media");
            this.DropIndex("dbo.NeighborhoodMessages", new[] { "File1Id" });
            this.DropColumn("dbo.NeighborhoodMessages", "File1Id");
        }
    }
}
