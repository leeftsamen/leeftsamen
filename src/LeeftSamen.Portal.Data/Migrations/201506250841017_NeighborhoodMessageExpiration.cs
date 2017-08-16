// <copyright file="201506250841017_NeighborhoodMessageExpiration.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class NeighborhoodMessageExpiration : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.NeighborhoodMessages", "ExpirationDateTime", c => c.DateTime(precision: 7, storeType: "datetime2"));
            this.DropColumn("dbo.NeighborhoodMessages", "EventDateTime");
        }

        public override void Down()
        {
            this.AddColumn("dbo.NeighborhoodMessages", "EventDateTime", c => c.DateTime(precision: 7, storeType: "datetime2"));
            this.DropColumn("dbo.NeighborhoodMessages", "ExpirationDateTime");
        }
    }
}
