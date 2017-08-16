// <copyright file="201604290734193_JobNoTime.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class JobNoTime : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.Jobs", "HasDueTime", c => c.Boolean(nullable: false, defaultValue: true));
        }

        public override void Down()
        {
            this.DropColumn("dbo.Jobs", "HasDueTime");
        }
    }
}
