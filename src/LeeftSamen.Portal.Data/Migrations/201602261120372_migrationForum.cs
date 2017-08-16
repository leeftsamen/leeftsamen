// <copyright file="201602261120372_migrationForum.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class migrationForum : DbMigration
    {
        public override void Up()
        {
            this.DropForeignKey("dbo.SquareForumSubjectReactions", "Creator_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.SquareForumSubjectReactions", "DeletedBy_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.SquareForumSubjectReactions", "LastEditBy_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.SquareForumSubjectReactions", "Subject_SquareForumSubjectId", "dbo.SquareForumSubjects");
            this.DropForeignKey("dbo.SquareForumReactionMedias", "Media_MediaId", "dbo.Media");
            this.DropForeignKey("dbo.SquareForumReactionMedias", "Reaction_SquareForumSubjectReactionId", "dbo.SquareForumSubjectReactions");
            this.DropForeignKey("dbo.SquareForumSubjects", "Creator_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.SquareForumSubjects", "Square_SquareId", "dbo.Squares");

            this.DropForeignKey("dbo.ForumReactionReports", "Reaction_SquareForumSubjectReactionId", "dbo.SquareForumSubjectReactions");

            this.DropIndex("dbo.SquareForumSubjectReactions", new[] { "Creator_Id" });
            this.DropIndex("dbo.SquareForumSubjectReactions", new[] { "DeletedBy_Id" });
            this.DropIndex("dbo.SquareForumSubjectReactions", new[] { "LastEditBy_Id" });
            this.DropIndex("dbo.SquareForumSubjectReactions", new[] { "Subject_SquareForumSubjectId" });
            this.DropIndex("dbo.SquareForumReactionMedias", new[] { "Media_MediaId" });
            this.DropIndex("dbo.SquareForumReactionMedias", new[] { "Reaction_SquareForumSubjectReactionId" });
            this.DropIndex("dbo.SquareForumSubjects", new[] { "Creator_Id" });
            this.DropIndex("dbo.SquareForumSubjects", new[] { "Square_SquareId" });

            this.Sql("TRUNCATE TABLE dbo.ForumReactionReports");

            this.RenameColumn(table: "dbo.ForumReactionReports", name: "Reaction_SquareForumSubjectReactionId", newName: "Reaction_ForumReactionId");
            this.RenameIndex(table: "dbo.ForumReactionReports", name: "IX_Reaction_SquareForumSubjectReactionId", newName: "IX_Reaction_ForumReactionId");
            this.CreateTable(
                "dbo.ForumReactionMedias",
                c => new
                    {
                        ForumReactionMediaId = c.Int(nullable: false, identity: true),
                        Media_MediaId = c.Int(),
                        Reaction_ForumReactionId = c.Int(),
                    })
                .PrimaryKey(t => t.ForumReactionMediaId)
                .ForeignKey("dbo.Media", t => t.Media_MediaId)
                .ForeignKey("dbo.ForumReactions", t => t.Reaction_ForumReactionId)
                .Index(t => t.Media_MediaId)
                .Index(t => t.Reaction_ForumReactionId);

            this.CreateTable(
                "dbo.ForumReactions",
                c => new
                    {
                        ForumReactionId = c.Int(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false),
                        Message = c.String(),
                        Deleted = c.Boolean(nullable: false),
                        DeletedDate = c.DateTime(),
                        EditCount = c.Int(nullable: false),
                        LastEditDate = c.DateTime(),
                        Creator_Id = c.String(maxLength: 128),
                        DeletedBy_Id = c.String(maxLength: 128),
                        LastEditBy_Id = c.String(maxLength: 128),
                        Subject_ForumSubjectId = c.Int(),
                    })
                .PrimaryKey(t => t.ForumReactionId)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.DeletedBy_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.LastEditBy_Id)
                .ForeignKey("dbo.ForumSubjects", t => t.Subject_ForumSubjectId)
                .Index(t => t.Creator_Id)
                .Index(t => t.DeletedBy_Id)
                .Index(t => t.LastEditBy_Id)
                .Index(t => t.Subject_ForumSubjectId);

            this.CreateTable(
                "dbo.ForumSubjects",
                c => new
                    {
                        ForumSubjectId = c.Int(nullable: false, identity: true),
                        Type = c.String(nullable: false, maxLength: 100),
                        TypeId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        Creator_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ForumSubjectId)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id)
                .Index(t => t.Creator_Id);
            this.CreateIndex("dbo.ForumSubjects",
                new string[] { "Type", "TypeId" },
                unique: false,
                name: "IX_SUBJECTS_TO_FORUM");

            this.DropTable("dbo.SquareForumSubjectReactions");
            this.DropTable("dbo.SquareForumReactionMedias");
            this.DropTable("dbo.SquareForumSubjects");

            this.AddForeignKey("dbo.ForumReactionReports", "Reaction_ForumReactionId", "dbo.ForumReactions");
        }

        public override void Down()
        {
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
                .PrimaryKey(t => t.SquareForumSubjectId);

            this.CreateTable(
                "dbo.SquareForumReactionMedias",
                c => new
                    {
                        SquareForumReactionMediaId = c.Int(nullable: false, identity: true),
                        Media_MediaId = c.Int(),
                        Reaction_SquareForumSubjectReactionId = c.Int(),
                    })
                .PrimaryKey(t => t.SquareForumReactionMediaId);

            this.CreateTable(
                "dbo.SquareForumSubjectReactions",
                c => new
                    {
                        SquareForumSubjectReactionId = c.Int(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false),
                        Message = c.String(),
                        Deleted = c.Boolean(nullable: false),
                        DeletedDate = c.DateTime(),
                        EditCount = c.Int(nullable: false),
                        LastEditDate = c.DateTime(),
                        Creator_Id = c.String(maxLength: 128),
                        DeletedBy_Id = c.String(maxLength: 128),
                        LastEditBy_Id = c.String(maxLength: 128),
                        Subject_SquareForumSubjectId = c.Int(),
                    })
                .PrimaryKey(t => t.SquareForumSubjectReactionId);

            this.DropForeignKey("dbo.ForumReactions", "Subject_ForumSubjectId", "dbo.ForumSubjects");
            this.DropForeignKey("dbo.ForumSubjects", "Creator_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.ForumReactionMedias", "Reaction_ForumReactionId", "dbo.ForumReactions");
            this.DropForeignKey("dbo.ForumReactions", "LastEditBy_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.ForumReactions", "DeletedBy_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.ForumReactions", "Creator_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.ForumReactionMedias", "Media_MediaId", "dbo.Media");

            this.DropForeignKey("dbo.ForumReactionReports", "Reaction_ForumReactionId", "dbo.ForumReactions");

            this.DropIndex("dbo.ForumSubjects", new[] { "Creator_Id" });
            this.DropIndex("dbo.ForumReactions", new[] { "Subject_ForumSubjectId" });
            this.DropIndex("dbo.ForumReactions", new[] { "LastEditBy_Id" });
            this.DropIndex("dbo.ForumReactions", new[] { "DeletedBy_Id" });
            this.DropIndex("dbo.ForumReactions", new[] { "Creator_Id" });
            this.DropIndex("dbo.ForumReactionMedias", new[] { "Reaction_ForumReactionId" });
            this.DropIndex("dbo.ForumReactionMedias", new[] { "Media_MediaId" });
            this.DropIndex("IX_SUBJECTS_TO_FORUM", new[] { "Type", "TypeId" });
            this.DropTable("dbo.ForumSubjects");
            this.DropTable("dbo.ForumReactions");
            this.DropTable("dbo.ForumReactionMedias");
            this.RenameIndex(table: "dbo.ForumReactionReports", name: "IX_Reaction_ForumReactionId", newName: "IX_Reaction_SquareForumSubjectReactionId");
            this.RenameColumn(table: "dbo.ForumReactionReports", name: "Reaction_ForumReactionId", newName: "Reaction_SquareForumSubjectReactionId");
            this.CreateIndex("dbo.SquareForumSubjects", "Square_SquareId");
            this.CreateIndex("dbo.SquareForumSubjects", "Creator_Id");
            this.CreateIndex("dbo.SquareForumReactionMedias", "Reaction_SquareForumSubjectReactionId");
            this.CreateIndex("dbo.SquareForumReactionMedias", "Media_MediaId");
            this.CreateIndex("dbo.SquareForumSubjectReactions", "Subject_SquareForumSubjectId");
            this.CreateIndex("dbo.SquareForumSubjectReactions", "LastEditBy_Id");
            this.CreateIndex("dbo.SquareForumSubjectReactions", "DeletedBy_Id");
            this.CreateIndex("dbo.SquareForumSubjectReactions", "Creator_Id");
            this.AddForeignKey("dbo.SquareForumSubjects", "Square_SquareId", "dbo.Squares", "SquareId");
            this.AddForeignKey("dbo.SquareForumSubjectReactions", "Subject_SquareForumSubjectId", "dbo.SquareForumSubjects", "SquareForumSubjectId");
            this.AddForeignKey("dbo.SquareForumSubjects", "Creator_Id", "dbo.AspNetUsers", "Id");
            this.AddForeignKey("dbo.SquareForumReactionMedias", "Reaction_SquareForumSubjectReactionId", "dbo.SquareForumSubjectReactions", "SquareForumSubjectReactionId");
            this.AddForeignKey("dbo.SquareForumReactionMedias", "Media_MediaId", "dbo.Media", "MediaId");
            this.AddForeignKey("dbo.SquareForumSubjectReactions", "LastEditBy_Id", "dbo.AspNetUsers", "Id");
            this.AddForeignKey("dbo.SquareForumSubjectReactions", "DeletedBy_Id", "dbo.AspNetUsers", "Id");
            this.AddForeignKey("dbo.SquareForumSubjectReactions", "Creator_Id", "dbo.AspNetUsers", "Id");

            this.AddForeignKey("dbo.ForumReactionReports", "Reaction_SquareForumSubjectReactionId", "dbo.SquareForumSubjectReactions");
        }
    }
}
