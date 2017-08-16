// <copyright file="201510081305471_ActivityRecurrance.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ActivityRecurrance : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.Activities", "Recurring", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            this.DropColumn("dbo.Activities", "Recurring");
        }
    }
}
