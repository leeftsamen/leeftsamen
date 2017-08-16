// <copyright file="201602180934246_SquareSettings.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class SquareSettings : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.SquareForumReactionMedias",
                c => new
                    {
                        SquareForumReactionMediaId = c.Int(nullable: false, identity: true),
                        Media_MediaId = c.Int(),
                        Reaction_SquareForumSubjectReactionId = c.Int(),
                    })
                .PrimaryKey(t => t.SquareForumReactionMediaId)
                .ForeignKey("dbo.Media", t => t.Media_MediaId)
                .ForeignKey("dbo.SquareForumSubjectReactions", t => t.Reaction_SquareForumSubjectReactionId)
                .Index(t => t.Media_MediaId)
                .Index(t => t.Reaction_SquareForumSubjectReactionId);

            this.AddColumn("dbo.Squares", "InfoTitle", c => c.String());
            this.AddColumn("dbo.Squares", "InfoText", c => c.String());
            this.AddColumn("dbo.Squares", "ForumTitle", c => c.String());
            this.AddColumn("dbo.Squares", "ForumText", c => c.String());
            this.DropColumn("dbo.Squares", "Position");
        }

        public override void Down()
        {
            this.AddColumn("dbo.Squares", "Position", c => c.Geography());
            this.DropForeignKey("dbo.SquareForumReactionMedias", "Reaction_SquareForumSubjectReactionId", "dbo.SquareForumSubjectReactions");
            this.DropForeignKey("dbo.SquareForumReactionMedias", "Media_MediaId", "dbo.Media");
            this.DropIndex("dbo.SquareForumReactionMedias", new[] { "Reaction_SquareForumSubjectReactionId" });
            this.DropIndex("dbo.SquareForumReactionMedias", new[] { "Media_MediaId" });
            this.DropColumn("dbo.Squares", "ForumText");
            this.DropColumn("dbo.Squares", "ForumTitle");
            this.DropColumn("dbo.Squares", "InfoText");
            this.DropColumn("dbo.Squares", "InfoTitle");
            this.DropTable("dbo.SquareForumReactionMedias");
        }
    }
}
