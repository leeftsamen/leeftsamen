// <copyright file="201511180923260_ActivityRecurringEnd.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ActivityRecurringEnd : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.Activities", "RecurringEnd", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }

        public override void Down()
        {
            this.DropColumn("dbo.Activities", "RecurringEnd");
        }
    }
}
