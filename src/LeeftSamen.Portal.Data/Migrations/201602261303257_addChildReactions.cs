// <copyright file="201602261303257_addChildReactions.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class addChildReactions : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.ForumReactions", "ParentReaction_ForumReactionId", c => c.Int());
            this.CreateIndex("dbo.ForumReactions", "ParentReaction_ForumReactionId");
            this.AddForeignKey("dbo.ForumReactions", "ParentReaction_ForumReactionId", "dbo.ForumReactions", "ForumReactionId");
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.ForumReactions", "ParentReaction_ForumReactionId", "dbo.ForumReactions");
            this.DropIndex("dbo.ForumReactions", new[] { "ParentReaction_ForumReactionId" });
            this.DropColumn("dbo.ForumReactions", "ParentReaction_ForumReactionId");
        }
    }
}
