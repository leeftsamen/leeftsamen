// <copyright file="201509100919438_DelNeighborhoodMessageParentReaction.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class DelNeighborhoodMessageParentReaction : DbMigration
    {
        public override void Up()
        {
            this.DropForeignKey("dbo.NeighborhoodMessageReactions", "Reactions_ReactionId", "dbo.NeighborhoodMessageReactions");
            this.DropIndex("dbo.NeighborhoodMessageReactions", new[] { "Reactions_ReactionId" });
            //DropColumn("dbo.NeighborhoodMessageReactions", "Reactions_ReactionId");
        }

        public override void Down()
        {
            //AddColumn("dbo.NeighborhoodMessageReactions", "Reactions_ReactionId", c => c.Int());
            this.CreateIndex("dbo.NeighborhoodMessageReactions", "Reactions_ReactionId");
            //AddForeignKey("dbo.NeighborhoodMessageReactions", "Reactions_ReactionId", "dbo.NeighborhoodMessageReactions", "ReactionId");
        }
    }
}
