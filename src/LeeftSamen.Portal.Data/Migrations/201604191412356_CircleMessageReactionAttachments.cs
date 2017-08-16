// <copyright file="201604191412356_CircleMessageReactionAttachments.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class CircleMessageReactionAttachments : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.CircleMessageReactions", "Attachment_MediaId", c => c.Int());
            this.CreateIndex("dbo.CircleMessageReactions", "Attachment_MediaId");
            this.AddForeignKey("dbo.CircleMessageReactions", "Attachment_MediaId", "dbo.Media", "MediaId");
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.CircleMessageReactions", "Attachment_MediaId", "dbo.Media");
            this.DropIndex("dbo.CircleMessageReactions", new[] { "Attachment_MediaId" });
            this.DropColumn("dbo.CircleMessageReactions", "Attachment_MediaId");
        }
    }
}
