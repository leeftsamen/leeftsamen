// <copyright file="201601220852316_AddCurrencies.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddCurrencies : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.MarketplaceItems", "Currency", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            this.DropColumn("dbo.MarketplaceItems", "Currency");
        }
    }
}
