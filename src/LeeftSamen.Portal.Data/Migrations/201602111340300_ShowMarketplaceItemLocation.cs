// <copyright file="201602111340300_ShowMarketplaceItemLocation.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ShowMarketplaceItemLocation : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.MarketplaceItems", "ShowLocation", c => c.Boolean());
        }

        public override void Down()
        {
            this.DropColumn("dbo.MarketplaceItems", "ShowLocation");
        }
    }
}
