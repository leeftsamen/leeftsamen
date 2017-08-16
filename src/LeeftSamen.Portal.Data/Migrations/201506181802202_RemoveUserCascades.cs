// <copyright file="201506181802202_RemoveUserCascades.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class RemoveUserCascades : DbMigration
    {
        public override void Up()
        {
            this.DropForeignKey("dbo.Activities", "Creator_Id", "dbo.AspNetUsers");
            this.DropIndex("dbo.Circles", new[] { "Creator_Id" });
            this.AlterColumn("dbo.Circles", "Creator_Id", c => c.String(maxLength: 128));
            this.CreateIndex("dbo.Circles", "Creator_Id");
            this.AddForeignKey("dbo.Activities", "Creator_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.Activities", "Creator_Id", "dbo.AspNetUsers");
            this.DropIndex("dbo.Circles", new[] { "Creator_Id" });
            this.AlterColumn("dbo.Circles", "Creator_Id", c => c.String(nullable: false, maxLength: 128));
            this.CreateIndex("dbo.Circles", "Creator_Id");
            this.AddForeignKey("dbo.Activities", "Creator_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
