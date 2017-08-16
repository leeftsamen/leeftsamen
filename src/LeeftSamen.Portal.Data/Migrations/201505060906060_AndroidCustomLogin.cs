// <copyright file="201505060906060_AndroidCustomLogin.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AndroidCustomLogin : DbMigration
    {
        public override void Down()
        {
            this.DropColumn("dbo.AspNetUsers", "AuthToken");
            this.DropColumn("dbo.AspNetUsers", "DeviceToken");
        }

        public override void Up()
        {
            this.AddColumn("dbo.AspNetUsers", "DeviceToken", c => c.String());
            this.AddColumn("dbo.AspNetUsers", "AuthToken", c => c.String());
        }
    }
}