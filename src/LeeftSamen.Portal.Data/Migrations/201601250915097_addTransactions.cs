// <copyright file="201601250915097_addTransactions.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class addTransactions : DbMigration
    {
        public override void Up()
        {
            this.CreateTable(
                "dbo.MarketplaceItemTransactions",
                c => new
                    {
                        MarketplaceItemTransactionId = c.Int(nullable: false, identity: true),
                        SenderZuiderlingAccount = c.String(),
                        ReceiverZuiderlingAccount = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Currency = c.Int(nullable: false),
                        MarketplaceItem_MarketplaceItemId = c.Int(),
                        Receiver_Id = c.String(maxLength: 128),
                        Sender_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.MarketplaceItemTransactionId)
                .ForeignKey("dbo.MarketplaceItems", t => t.MarketplaceItem_MarketplaceItemId)
                .ForeignKey("dbo.AspNetUsers", t => t.Receiver_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Sender_Id)
                .Index(t => t.MarketplaceItem_MarketplaceItemId)
                .Index(t => t.Receiver_Id)
                .Index(t => t.Sender_Id);
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.MarketplaceItemTransactions", "Sender_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.MarketplaceItemTransactions", "Receiver_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.MarketplaceItemTransactions", "MarketplaceItem_MarketplaceItemId", "dbo.MarketplaceItems");
            this.DropIndex("dbo.MarketplaceItemTransactions", new[] { "Sender_Id" });
            this.DropIndex("dbo.MarketplaceItemTransactions", new[] { "Receiver_Id" });
            this.DropIndex("dbo.MarketplaceItemTransactions", new[] { "MarketplaceItem_MarketplaceItemId" });
            this.DropTable("dbo.MarketplaceItemTransactions");
        }
    }
}
