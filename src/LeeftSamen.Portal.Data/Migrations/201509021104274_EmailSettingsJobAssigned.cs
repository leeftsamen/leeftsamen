// <copyright file="201509021104274_EmailSettingsJobAssigned.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class EmailSettingsJobAssigned : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.AspNetUsers", "ReceivesCircleJobAssigned", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            this.DropColumn("dbo.AspNetUsers", "ReceivesCircleJobAssigned");
        }
    }
}
