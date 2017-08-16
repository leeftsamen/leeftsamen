// <copyright file="201509031219002_AddPinToNeighborhoodMessage.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddPinToNeighborhoodMessage : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.NeighborhoodMessages", "IsPinned", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            this.DropColumn("dbo.NeighborhoodMessages", "IsPinned");
        }
    }
}
