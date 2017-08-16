// <copyright file="201601291240419_AddJobEndTime.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddJobEndTime : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.Jobs", "DueDateTimeEnd", c => c.DateTime());
        }

        public override void Down()
        {
            this.DropColumn("dbo.Jobs", "DueDateTimeEnd");
        }
    }
}
