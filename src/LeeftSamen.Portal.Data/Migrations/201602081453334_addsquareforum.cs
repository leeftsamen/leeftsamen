// <copyright file="201602081453334_addsquareforum.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class addsquareforum : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.SquareForumSubjectReactions",
                c => new
                    {
                        SquareForumSubjectReactionId = c.Int(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false),
                        Message = c.String(),
                        Creator_Id = c.String(maxLength: 128),
                        Subject_SquareForumSubjectId = c.Int(),
                    })
                .PrimaryKey(t => t.SquareForumSubjectReactionId)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id)
                .ForeignKey("dbo.SquareForumSubjects", t => t.Subject_SquareForumSubjectId)
                .Index(t => t.Creator_Id)
                .Index(t => t.Subject_SquareForumSubjectId);

            this.CreateTable(
                "dbo.SquareForumSubjects",
                c => new
                    {
                        SquareForumSubjectId = c.Int(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        Creator_Id = c.String(maxLength: 128),
                        Square_SquareId = c.Int(),
                    })
                .PrimaryKey(t => t.SquareForumSubjectId)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id)
                .ForeignKey("dbo.Squares", t => t.Square_SquareId)
                .Index(t => t.Creator_Id)
                .Index(t => t.Square_SquareId);

            this.CreateTable(
                "dbo.Squares",
                c => new
                    {
                        SquareId = c.Int(nullable: false, identity: true),
                        CoverColor = c.Int(nullable: false),
                        CoverImageId = c.Int(),
                        Position = c.Geography(),
                        ProfileImageId = c.Int(),
                        CoverImage_MediaId = c.Int(),
                        ProfileImage_MediaId = c.Int(),
                    })
                .PrimaryKey(t => t.SquareId)
                .ForeignKey("dbo.Media", t => t.CoverImage_MediaId)
                .ForeignKey("dbo.Media", t => t.ProfileImage_MediaId)
                .Index(t => t.CoverImage_MediaId)
                .Index(t => t.ProfileImage_MediaId);
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.SquareForumSubjects", "Square_SquareId", "dbo.Squares");
            this.DropForeignKey("dbo.Squares", "ProfileImage_MediaId", "dbo.Media");
            this.DropForeignKey("dbo.Squares", "CoverImage_MediaId", "dbo.Media");
            this.DropForeignKey("dbo.SquareForumSubjectReactions", "Subject_SquareForumSubjectId", "dbo.SquareForumSubjects");
            this.DropForeignKey("dbo.SquareForumSubjects", "Creator_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.SquareForumSubjectReactions", "Creator_Id", "dbo.AspNetUsers");
            this.DropIndex("dbo.Squares", new[] { "ProfileImage_MediaId" });
            this.DropIndex("dbo.Squares", new[] { "CoverImage_MediaId" });
            this.DropIndex("dbo.SquareForumSubjects", new[] { "Square_SquareId" });
            this.DropIndex("dbo.SquareForumSubjects", new[] { "Creator_Id" });
            this.DropIndex("dbo.SquareForumSubjectReactions", new[] { "Subject_SquareForumSubjectId" });
            this.DropIndex("dbo.SquareForumSubjectReactions", new[] { "Creator_Id" });
            this.DropTable("dbo.Squares");
            this.DropTable("dbo.SquareForumSubjects");
            this.DropTable("dbo.SquareForumSubjectReactions");
        }
    }
}
