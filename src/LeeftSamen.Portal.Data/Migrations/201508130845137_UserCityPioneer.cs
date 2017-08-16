// <copyright file="201508130845137_UserCityPioneer.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class UserCityPioneer : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.AspNetUsers", "IsCityPioneer", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            this.DropColumn("dbo.AspNetUsers", "IsCityPioneer");
        }
    }
}
