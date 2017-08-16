// <copyright file="201512081416001_HomePageRegister.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class HomePageRegister : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.AspNetUsers", "DateOfBirth", c => c.DateTime(nullable: false));
            this.AddColumn("dbo.AspNetUsers", "Gender", c => c.String());
        }

        public override void Down()
        {
            this.DropColumn("dbo.AspNetUsers", "Gender");
            this.DropColumn("dbo.AspNetUsers", "DateOfBirth");
        }
    }
}
