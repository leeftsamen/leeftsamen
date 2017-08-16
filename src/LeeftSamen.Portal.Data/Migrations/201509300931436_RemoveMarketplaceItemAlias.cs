// <copyright file="201509300931436_RemoveMarketplaceItemAlias.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class RemoveMarketplaceItemAlias : DbMigration
    {
        public override void Up()
        {
            this.DropColumn("dbo.MarketplaceItemCategories", "Alias");
        }

        public override void Down()
        {
            this.AddColumn("dbo.MarketplaceItemCategories", "Alias", c => c.String());
        }
    }
}
