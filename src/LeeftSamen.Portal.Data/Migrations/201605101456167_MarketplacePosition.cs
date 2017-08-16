// <copyright file="201605101456167_MarketplacePosition.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Spatial;

    public partial class MarketplacePosition : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.MarketplaceItems", "Position", c => c.Geography());
            this.Sql("UPDATE MarketplaceItems SET MarketplaceItems.Position = AspNetUsers.Position FROM MarketplaceItems INNER JOIN AspNetUsers ON MarketplaceItems.Owner_Id = AspNetUsers.Id");
        }

        public override void Down()
        {
            this.DropColumn("dbo.MarketplaceItems", "Position");
        }
    }
}
