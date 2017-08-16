// <copyright file="201509300956274_AddMarketplaceItemAliasEnum.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddMarketplaceItemAliasEnum : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.MarketplaceItemCategories", "Alias", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            this.DropColumn("dbo.MarketplaceItemCategories", "Alias");
        }
    }
}
