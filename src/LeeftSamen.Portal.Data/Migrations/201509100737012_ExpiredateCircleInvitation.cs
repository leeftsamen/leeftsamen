// <copyright file="201509100737012_ExpiredateCircleInvitation.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ExpiredateCircleInvitation : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.CircleInvitations", "ExpireDate", c => c.DateTime(nullable: false));
        }

        public override void Down()
        {
            this.DropColumn("dbo.CircleInvitations", "ExpireDate");
        }
    }
}
