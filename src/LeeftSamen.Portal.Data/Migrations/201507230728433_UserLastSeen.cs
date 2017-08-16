// <copyright file="201507230728433_UserLastSeen.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class UserLastSeen : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.AspNetUsers", "LastSeen", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }

        public override void Down()
        {
            this.DropColumn("dbo.AspNetUsers", "LastSeen");
        }
    }
}
