// <copyright file="201603010950092_ReactToSubject.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ReactToSubject : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.ForumReactions", "SubjectReaction", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            this.DropColumn("dbo.ForumReactions", "SubjectReaction");
        }
    }
}
