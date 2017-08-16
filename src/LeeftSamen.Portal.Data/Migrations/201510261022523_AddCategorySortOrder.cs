// <copyright file="201510261022523_AddCategorySortOrder.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddCategorySortOrder : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.MarketplaceItemCategories", "SortOrder", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            this.DropColumn("dbo.MarketplaceItemCategories", "SortOrder");
        }
    }
}
