// <copyright file="201509290912263_AddPreferences.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddPreferences : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.MarketplaceItems", "PreferenceDelivery", c => c.Boolean(nullable: false));
            this.AddColumn("dbo.MarketplaceItems", "PreferencePickup", c => c.Boolean(nullable: false));
            this.AddColumn("dbo.MarketplaceItems", "PreferenceMail", c => c.Boolean(nullable: false));
            this.AddColumn("dbo.MarketplaceItems", "PreferenceOnline", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            this.DropColumn("dbo.MarketplaceItems", "PreferenceOnline");
            this.DropColumn("dbo.MarketplaceItems", "PreferenceMail");
            this.DropColumn("dbo.MarketplaceItems", "PreferencePickup");
            this.DropColumn("dbo.MarketplaceItems", "PreferenceDelivery");
        }
    }
}
