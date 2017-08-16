// <copyright file="201509301323554_AddMarketplaceItemPublicCirclePaymentOption.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddMarketplaceItemPublicCirclePaymentOption : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.MarketplaceItems", "PaymentOption", c => c.Int());
            this.AddColumn("dbo.MarketplaceItems", "IsPublic", c => c.Boolean(nullable: false, defaultValue: true));
            this.AddColumn("dbo.MarketplaceItems", "ShowInCircleId", c => c.Int());
        }

        public override void Down()
        {
            this.DropColumn("dbo.MarketplaceItems", "ShowInCircleId");
            this.DropColumn("dbo.MarketplaceItems", "IsPublic");
            this.DropColumn("dbo.MarketplaceItems", "PaymentOption");
        }
    }
}
