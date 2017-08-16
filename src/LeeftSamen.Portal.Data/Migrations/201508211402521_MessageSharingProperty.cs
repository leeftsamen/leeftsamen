// <copyright file="201508211402521_MessageSharingProperty.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class MessageSharingProperty : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.NeighborhoodMessages", "AllowSharing", c => c.Boolean(nullable: false, defaultValue: true));
        }

        public override void Down()
        {
            this.DropColumn("dbo.NeighborhoodMessages", "AllowSharing");
        }
    }
}
