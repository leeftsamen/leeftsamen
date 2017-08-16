// <copyright file="201512081712072_HomePageRegister2.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class HomePageRegister2 : DbMigration
    {
        public override void Up()
        {
            this.AlterColumn("dbo.AspNetUsers", "DateOfBirth", c => c.DateTime());
        }

        public override void Down()
        {
            this.AlterColumn("dbo.AspNetUsers", "DateOfBirth", c => c.DateTime(nullable: false));
        }
    }
}
