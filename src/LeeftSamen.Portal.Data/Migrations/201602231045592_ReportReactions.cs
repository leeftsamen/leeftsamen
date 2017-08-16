// <copyright file="201602231045592_ReportReactions.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ReportReactions : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.ForumReactionReports",
                c => new
                    {
                        ForumReactionReportId = c.Int(nullable: false, identity: true),
                        ReportDate = c.DateTime(nullable: false),
                        Remark = c.String(),
                        Reviewed = c.Boolean(nullable: false),
                        Reaction_SquareForumSubjectReactionId = c.Int(),
                        Reporter_Id = c.String(maxLength: 128),
                        ReviewedBy_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ForumReactionReportId)
                .ForeignKey("dbo.SquareForumSubjectReactions", t => t.Reaction_SquareForumSubjectReactionId)
                .ForeignKey("dbo.AspNetUsers", t => t.Reporter_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ReviewedBy_Id)
                .Index(t => t.Reaction_SquareForumSubjectReactionId)
                .Index(t => t.Reporter_Id)
                .Index(t => t.ReviewedBy_Id);

            this.AddColumn("dbo.SquareForumSubjectReactions", "Deleted", c => c.Boolean(nullable: false));
            this.AddColumn("dbo.SquareForumSubjectReactions", "DeletedDate", c => c.DateTime(nullable: false));
            this.AddColumn("dbo.SquareForumSubjectReactions", "DeletedBy_Id", c => c.String(maxLength: 128));
            this.CreateIndex("dbo.SquareForumSubjectReactions", "DeletedBy_Id");
            this.AddForeignKey("dbo.SquareForumSubjectReactions", "DeletedBy_Id", "dbo.AspNetUsers", "Id");
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.ForumReactionReports", "ReviewedBy_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.ForumReactionReports", "Reporter_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.ForumReactionReports", "Reaction_SquareForumSubjectReactionId", "dbo.SquareForumSubjectReactions");
            this.DropForeignKey("dbo.SquareForumSubjectReactions", "DeletedBy_Id", "dbo.AspNetUsers");
            this.DropIndex("dbo.SquareForumSubjectReactions", new[] { "DeletedBy_Id" });
            this.DropIndex("dbo.ForumReactionReports", new[] { "ReviewedBy_Id" });
            this.DropIndex("dbo.ForumReactionReports", new[] { "Reporter_Id" });
            this.DropIndex("dbo.ForumReactionReports", new[] { "Reaction_SquareForumSubjectReactionId" });
            this.DropColumn("dbo.SquareForumSubjectReactions", "DeletedBy_Id");
            this.DropColumn("dbo.SquareForumSubjectReactions", "DeletedDate");
            this.DropColumn("dbo.SquareForumSubjectReactions", "Deleted");
            this.DropTable("dbo.ForumReactionReports");
        }
    }
}
