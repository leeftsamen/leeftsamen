// <copyright file="201602241037493_TrackEdits.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class TrackEdits : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.SquareForumSubjectReactions", "EditCount", c => c.Int(nullable: false));
            this.AddColumn("dbo.SquareForumSubjectReactions", "LastEditDate", c => c.DateTime());
            this.AddColumn("dbo.SquareForumSubjectReactions", "LastEditBy_Id", c => c.String(maxLength: 128));
            this.AlterColumn("dbo.SquareForumSubjectReactions", "DeletedDate", c => c.DateTime());
            this.CreateIndex("dbo.SquareForumSubjectReactions", "LastEditBy_Id");
            this.AddForeignKey("dbo.SquareForumSubjectReactions", "LastEditBy_Id", "dbo.AspNetUsers", "Id");
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.SquareForumSubjectReactions", "LastEditBy_Id", "dbo.AspNetUsers");
            this.DropIndex("dbo.SquareForumSubjectReactions", new[] { "LastEditBy_Id" });
            this.AlterColumn("dbo.SquareForumSubjectReactions", "DeletedDate", c => c.DateTime(nullable: false));
            this.DropColumn("dbo.SquareForumSubjectReactions", "LastEditBy_Id");
            this.DropColumn("dbo.SquareForumSubjectReactions", "LastEditDate");
            this.DropColumn("dbo.SquareForumSubjectReactions", "EditCount");
        }
    }
}
