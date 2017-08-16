// <copyright file="201602291043573_ZuiderlingZipcodes.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ZuiderlingZipcodes : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.ZuiderlingZipcodes",
                c => new
                    {
                        ZuiderlingZipcodeId = c.Int(nullable: false, identity: true),
                        ZipCode = c.String(),
                    })
                .PrimaryKey(t => t.ZuiderlingZipcodeId);
        }

        public override void Down()
        {
            this.DropTable("dbo.ZuiderlingZipcodes");
        }
    }
}
