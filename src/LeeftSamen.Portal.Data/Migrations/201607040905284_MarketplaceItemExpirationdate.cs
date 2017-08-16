// <copyright file="201607040905284_MarketplaceItemExpirationdate.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class MarketplaceItemExpirationdate : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.MarketplaceItems", "ExpirationDate", c => c.DateTime());
        }

        public override void Down()
        {
            this.DropColumn("dbo.MarketplaceItems", "ExpirationDate");
        }
    }
}
