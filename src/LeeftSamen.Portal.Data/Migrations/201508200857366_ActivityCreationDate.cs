// <copyright file="201508200857366_ActivityCreationDate.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ActivityCreationDate : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.Activities", "CreationDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }

        public override void Down()
        {
            this.DropColumn("dbo.Activities", "CreationDate");
        }
    }
}
