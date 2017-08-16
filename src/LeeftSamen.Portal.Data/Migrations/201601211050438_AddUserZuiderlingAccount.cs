// <copyright file="201601211050438_AddUserZuiderlingAccount.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddUserZuiderlingAccount : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.AspNetUsers", "ZuiderlingAccount", c => c.String());
        }

        public override void Down()
        {
            this.DropColumn("dbo.AspNetUsers", "ZuiderlingAccount");
        }
    }
}
