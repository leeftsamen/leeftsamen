// <copyright file="201505081228180_HideOrganization.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class HideOrganization : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.Organizations", "Hidden", c => c.Boolean(defaultValue: false));
        }

        public override void Down()
        {
            this.DropColumn("dbo.Organizations", "Hidden");
        }
    }
}
