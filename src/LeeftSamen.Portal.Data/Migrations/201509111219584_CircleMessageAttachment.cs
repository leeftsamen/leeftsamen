// <copyright file="201509111219584_CircleMessageAttachment.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class CircleMessageAttachment : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.CircleMessages", "AttachmentId", c => c.Int());
            this.CreateIndex("dbo.CircleMessages", "AttachmentId");
            this.AddForeignKey("dbo.CircleMessages", "AttachmentId", "dbo.Media", "MediaId");
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.CircleMessages", "AttachmentId", "dbo.Media");
            this.DropIndex("dbo.CircleMessages", new[] { "AttachmentId" });
            this.DropColumn("dbo.CircleMessages", "AttachmentId");
        }
    }
}
