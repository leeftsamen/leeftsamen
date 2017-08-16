// <copyright file="201606290903162_AddAccountSettingMarketplaceItem.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddAccountSettingMarketplaceItem : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.AspNetUsers", "ReceivesNewMarketplaceitemMail", c => c.Boolean(nullable: false, defaultValue: true));
        }

        public override void Down()
        {
            this.DropColumn("dbo.AspNetUsers", "ReceivesNewMarketplaceitemMail");
        }
    }
}
