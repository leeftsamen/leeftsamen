﻿// <copyright file="201509291752228_AddCategoryAlias.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddCategoryAlias : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.MarketplaceItemCategories", "Alias", c => c.String());
        }

        public override void Down()
        {
            this.DropColumn("dbo.MarketplaceItemCategories", "Alias");
        }
    }
}
