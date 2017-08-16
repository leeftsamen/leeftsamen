// <copyright file="201510011420122_ActivityAge.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ActivityAge : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.Activities", "AllAges", c => c.Boolean(nullable: false, defaultValue: true));
            this.AddColumn("dbo.Activities", "AgeFrom", c => c.Int());
            this.AddColumn("dbo.Activities", "AgeTo", c => c.Int());
        }

        public override void Down()
        {
            this.DropColumn("dbo.Activities", "AgeTo");
            this.DropColumn("dbo.Activities", "AgeFrom");
            this.DropColumn("dbo.Activities", "AllAges");
        }
    }
}
