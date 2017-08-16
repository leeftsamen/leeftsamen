// <copyright file="201603011526085_SubjectMainReactions.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class SubjectMainReactions : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.ForumReactions", "SubjectMainReaction", c => c.Boolean(nullable: false));
            this.AddColumn("dbo.ForumReactions", "Title", c => c.String());
            this.DropColumn("dbo.ForumReactions", "SubjectReaction");
        }

        public override void Down()
        {
            this.AddColumn("dbo.ForumReactions", "SubjectReaction", c => c.Boolean(nullable: false));
            this.DropColumn("dbo.ForumReactions", "Title");
            this.DropColumn("dbo.ForumReactions", "SubjectMainReaction");
        }
    }
}
