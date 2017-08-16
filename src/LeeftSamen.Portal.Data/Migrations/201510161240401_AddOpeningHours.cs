// <copyright file="201510161240401_AddOpeningHours.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddOpeningHours : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.Organizations", "OpeningHours", c => c.String());
        }

        public override void Down()
        {
            this.DropColumn("dbo.Organizations", "OpeningHours");
        }
    }
}
