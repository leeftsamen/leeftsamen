// <copyright file="201509291457117_ShowLocation.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ShowLocation : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.AspNetUsers", "ShowLocation", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            this.DropColumn("dbo.AspNetUsers", "ShowLocation");
        }
    }
}
