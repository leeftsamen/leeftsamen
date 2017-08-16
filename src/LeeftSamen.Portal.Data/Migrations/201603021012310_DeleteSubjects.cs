// <copyright file="201603021012310_DeleteSubjects.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class DeleteSubjects : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.ForumSubjects", "Deleted", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            this.DropColumn("dbo.ForumSubjects", "Deleted");
        }
    }
}
