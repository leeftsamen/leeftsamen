// <copyright file="201604151005155_AddCircleEmailGroup.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddCircleEmailGroup : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.CircleEmailGroups",
                c => new
                    {
                        CircleEmailGroupId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Creator_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.CircleEmailGroupId)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id)
                .Index(t => t.Creator_Id);

            this.AddColumn("dbo.CircleEmailGroups", "Circle_CircleId", c => c.Int());
            this.CreateIndex("dbo.CircleEmailGroups", "Circle_CircleId");
            this.AddForeignKey("dbo.CircleEmailGroups", "Circle_CircleId", "dbo.Circles", "CircleId");

            this.AddColumn("dbo.CircleEmailMessageReceivers", "EmailGroup_CircleEmailGroupId", c => c.Int());
            this.AddColumn("dbo.CircleEmailMessageReceivers", "CircleEmailMessage_MessageId", c => c.Int());
            this.AddColumn("dbo.CircleEmailMessages", "Group_CircleEmailGroupId", c => c.Int());
            this.CreateIndex("dbo.CircleEmailMessageReceivers", "EmailGroup_CircleEmailGroupId");
            this.CreateIndex("dbo.CircleEmailMessageReceivers", "CircleEmailMessage_MessageId");
            this.CreateIndex("dbo.CircleEmailMessages", "Group_CircleEmailGroupId");
            this.AddForeignKey("dbo.CircleEmailMessageReceivers", "EmailGroup_CircleEmailGroupId", "dbo.CircleEmailGroups", "CircleEmailGroupId");
            this.AddForeignKey("dbo.CircleEmailMessages", "Group_CircleEmailGroupId", "dbo.CircleEmailGroups", "CircleEmailGroupId");
            this.AddForeignKey("dbo.CircleEmailMessageReceivers", "CircleEmailMessage_MessageId", "dbo.CircleEmailMessages", "MessageId");

            //Add temporary help column
            this.AddColumn("dbo.CircleEmailGroups", "Temp", c => c.String());
            //Create group with temporary value for each unique sender+receiver combination
            this.Sql(@" INSERT INTO dbo.CircleEmailGroups (Temp)
                        SELECT DISTINCT(
                            SELECT Receiver_Id, CircleId FROM
                            (SELECT ReceiverId, Receiver_Id, CircleEmailMessages.CircleId FROM CircleEmailMessageReceivers
	                        LEFT JOIN CircleEmailMessages ON CircleEmailMessages.MessageId = CircleEmailMessageReceivers.ReceiverId
                            UNION
                            SELECT MessageId, Creator_Id, CircleId FROM CircleEmailMessages) ST1 
                            WHERE ST1.ReceiverId = ST2.ReceiverId ORDER BY Receiver_Id FOR XML PATH('')) Receivers
                        FROM[CircleEmailMessageReceivers] ST2;");
            //Update groupId
            this.Sql(@" UPDATE dbo.CircleEmailMessages
                        SET Group_CircleEmailGroupId = 
                        (
	                        SELECT CircleEmailGroupId FROM dbo.CircleEmailGroups
	                        WHERE Temp = 
	                        (
		                        SELECT TOP 1 Receiver_Id FROM 
		                        (
			                        SELECT ReceiverId, CircleId, 
			                        (
				                        SELECT ST1.Receiver_Id, CircleId FROM
				                        (
					                        SELECT ReceiverId, Receiver_Id, CircleEmailMessages.CircleId FROM CircleEmailMessageReceivers
	                                        LEFT JOIN CircleEmailMessages ON CircleEmailMessages.MessageId = CircleEmailMessageReceivers.ReceiverId
                                            UNION
                                            SELECT MessageId, Creator_Id, CircleId FROM CircleEmailMessages
				                        ) ST1 
				                        WHERE ST1.ReceiverId = ST2.ReceiverId ORDER BY Receiver_Id FOR XML PATH ('')
			                        ) Receiver_Id
			                        FROM [CircleEmailMessageReceivers] ST2 WHERE ReceiverId = CircleEmailMessages.MessageId
		                        ) AS Receiver_Id
	                        )
                        )");
            //Update receivers
            this.Sql(@" UPDATE r SET
	                        r.EmailGroup_CircleEmailGroupId = m.Group_CircleEmailGroupId
                        FROM CircleEmailMessageReceivers r
                        JOIN CircleEmailMessages m 
                        ON r.ReceiverId = m.MessageId");

            //Drop temporary column
            this.DropColumn("dbo.CircleEmailGroups", "Temp");

            this.DropForeignKey("dbo.CircleEmailMessageReceivers", "ReceiverId", "dbo.CircleEmailMessages");
            this.DropIndex("dbo.CircleEmailMessageReceivers", new[] { "ReceiverId" });

            //Add missing GroupMembers
            this.Sql(@" INSERT INTO CircleEmailMessageReceivers (EmailGroup_CircleEmailGroupId, Receiver_Id, ReceiverId, HasRemovedMessage)
                        SELECT Group_CircleEmailGroupId, Creator_Id, 0, 0 FROM CircleEmailMessages");

            //Delete obsolete receivers
            this.Sql(@" DELETE FROM CircleEmailMessageReceivers WHERE 
                        EmailMessageId NOT IN
                        (
	                        SELECT MIN(EmailMessageId) FROM CircleEmailMessageReceivers
	                        GROUP BY Receiver_Id, EmailGroup_CircleEmailGroupId
                        )");
            this.Sql("UPDATE dbo.CircleEmailGroups SET Circle_CircleId = (SELECT TOP 1 CircleId FROM dbo.CircleEmailMessages WHERE Group_CircleEmailGroupId = CircleEmailGroups.CircleEmailGroupId ORDER BY MessageId ASC)");
        }

        public override void Down()
        {
            this.CreateIndex("dbo.CircleEmailMessageReceivers", "ReceiverId");
            this.AddForeignKey("dbo.CircleEmailMessageReceivers", "ReceiverId", "dbo.CircleEmailMessages", "MessageId");

            this.Sql(@" INSERT INTO CircleEmailMessageReceivers (ReceiverId, Receiver_Id)
                        SELECT CircleEmailMessages.MessageId, CircleEmailMessageReceivers.Receiver_Id FROM CircleEmailMessages
                        LEFT JOIN CircleEmailMessageReceivers ON CircleEmailMessageReceivers.[EmailGroup_CircleEmailGroupId] = CircleEmailMessages.[Group_CircleEmailGroupId]");

            this.DropForeignKey("dbo.CircleEmailMessageReceivers", "CircleEmailMessage_MessageId", "dbo.CircleEmailMessages");
            this.DropForeignKey("dbo.CircleEmailMessages", "Group_CircleEmailGroupId", "dbo.CircleEmailGroups");
            this.DropForeignKey("dbo.CircleEmailMessageReceivers", "EmailGroup_CircleEmailGroupId", "dbo.CircleEmailGroups");
            this.DropForeignKey("dbo.CircleEmailGroups", "Creator_Id", "dbo.AspNetUsers");
            this.DropIndex("dbo.CircleEmailMessages", new[] { "Group_CircleEmailGroupId" });
            this.DropIndex("dbo.CircleEmailGroups", new[] { "Creator_Id" });
            this.DropIndex("dbo.CircleEmailMessageReceivers", new[] { "CircleEmailMessage_MessageId" });
            this.DropIndex("dbo.CircleEmailMessageReceivers", new[] { "EmailGroup_CircleEmailGroupId" });
            this.DropColumn("dbo.CircleEmailMessages", "Group_CircleEmailGroupId");
            this.DropColumn("dbo.CircleEmailMessageReceivers", "CircleEmailMessage_MessageId");
            this.DropColumn("dbo.CircleEmailMessageReceivers", "EmailGroup_CircleEmailGroupId");
            this.DropTable("dbo.CircleEmailGroups");
        }
    }
}
