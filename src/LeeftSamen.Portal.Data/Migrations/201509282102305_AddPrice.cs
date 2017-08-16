// <copyright file="201509282102305_AddPrice.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddPrice : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.MarketplaceItems", "Price", c => c.Decimal(precision: 18, scale: 2));
        }

        public override void Down()
        {
            this.DropColumn("dbo.MarketplaceItems", "Price");
        }
    }
}
