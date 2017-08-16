// <copyright file="201606011356300_CircleMembershipProfile.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class CircleMembershipProfile : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.CircleMemberships", "Profile", c => c.String());
        }

        public override void Down()
        {
            this.DropColumn("dbo.CircleMemberships", "Profile");
        }
    }
}
