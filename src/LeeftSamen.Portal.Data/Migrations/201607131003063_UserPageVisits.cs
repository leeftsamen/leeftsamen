// <copyright file="201607131003063_UserPageVisits.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class UserPageVisits : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.UserPageVisits",
                c => new
                    {
                        UserPageVisitId = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        Page = c.String(),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserPageVisitId);
        }

        public override void Down()
        {
            this.DropTable("dbo.UserPageVisits");
        }
    }
}
