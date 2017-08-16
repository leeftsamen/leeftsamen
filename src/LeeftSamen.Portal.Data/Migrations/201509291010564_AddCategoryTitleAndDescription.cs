// <copyright file="201509291010564_AddCategoryTitleAndDescription.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddCategoryTitleAndDescription : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.MarketplaceItemCategories", "Title", c => c.String());
            this.AddColumn("dbo.MarketplaceItemCategories", "Description", c => c.String());
        }

        public override void Down()
        {
            this.DropColumn("dbo.MarketplaceItemCategories", "Description");
            this.DropColumn("dbo.MarketplaceItemCategories", "Title");
        }
    }
}
