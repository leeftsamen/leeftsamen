// <copyright file="201601210838299_AddActionZipcodes.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddActionZipcodes : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.ActionZipcodes",
                c => new
                    {
                        ActionZipcodeId = c.Int(nullable: false, identity: true),
                        PostalCode = c.String(),
                        Action_ActionId = c.Int(),
                    })
                .PrimaryKey(t => t.ActionZipcodeId)
                .ForeignKey("dbo.Actions", t => t.Action_ActionId)
                .Index(t => t.Action_ActionId);
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.ActionZipcodes", "Action_ActionId", "dbo.Actions");
            this.DropIndex("dbo.ActionZipcodes", new[] { "Action_ActionId" });
            this.DropTable("dbo.ActionZipcodes");
        }
    }
}
