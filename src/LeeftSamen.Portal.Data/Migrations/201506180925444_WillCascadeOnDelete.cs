// <copyright file="201506180925444_WillCascadeOnDelete.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class WillCascadeOnDelete : DbMigration
    {
        public override void Up()
        {
            this.DropForeignKey("dbo.CircleInvitations", "User_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.CircleJoinRequests", "User_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.CircleMemberships", "User_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.CircleInvitations", "Circle_CircleId", "dbo.Circles");
            this.DropForeignKey("dbo.CircleJoinRequests", "Circle_CircleId", "dbo.Circles");
            this.DropForeignKey("dbo.CircleMemberships", "Circle_CircleId", "dbo.Circles");
            this.DropForeignKey("dbo.CircleMessages", "Circle_CircleId", "dbo.Circles");
            this.DropForeignKey("dbo.CircleMessageReactions", "Message_MessageId", "dbo.CircleMessages");
            this.AddForeignKey("dbo.CircleInvitations", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            this.AddForeignKey("dbo.CircleJoinRequests", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            this.AddForeignKey("dbo.CircleMemberships", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            this.AddForeignKey("dbo.CircleInvitations", "Circle_CircleId", "dbo.Circles", "CircleId", cascadeDelete: true);
            this.AddForeignKey("dbo.CircleJoinRequests", "Circle_CircleId", "dbo.Circles", "CircleId", cascadeDelete: true);
            this.AddForeignKey("dbo.CircleMemberships", "Circle_CircleId", "dbo.Circles", "CircleId", cascadeDelete: true);
            this.AddForeignKey("dbo.CircleMessages", "Circle_CircleId", "dbo.Circles", "CircleId", cascadeDelete: true);
            this.AddForeignKey("dbo.CircleMessageReactions", "Message_MessageId", "dbo.CircleMessages", "MessageId", cascadeDelete: true);
        }

        public override void Down()
        {
            this.DropForeignKey("dbo.CircleMessageReactions", "Message_MessageId", "dbo.CircleMessages");
            this.DropForeignKey("dbo.CircleMessages", "Circle_CircleId", "dbo.Circles");
            this.DropForeignKey("dbo.CircleMemberships", "Circle_CircleId", "dbo.Circles");
            this.DropForeignKey("dbo.CircleJoinRequests", "Circle_CircleId", "dbo.Circles");
            this.DropForeignKey("dbo.CircleInvitations", "Circle_CircleId", "dbo.Circles");
            this.DropForeignKey("dbo.CircleMemberships", "User_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.CircleJoinRequests", "User_Id", "dbo.AspNetUsers");
            this.DropForeignKey("dbo.CircleInvitations", "User_Id", "dbo.AspNetUsers");
            this.AddForeignKey("dbo.CircleMessageReactions", "Message_MessageId", "dbo.CircleMessages", "MessageId");
            this.AddForeignKey("dbo.CircleMessages", "Circle_CircleId", "dbo.Circles", "CircleId");
            this.AddForeignKey("dbo.CircleMemberships", "Circle_CircleId", "dbo.Circles", "CircleId");
            this.AddForeignKey("dbo.CircleJoinRequests", "Circle_CircleId", "dbo.Circles", "CircleId");
            this.AddForeignKey("dbo.CircleInvitations", "Circle_CircleId", "dbo.Circles", "CircleId");
            this.AddForeignKey("dbo.CircleMemberships", "User_Id", "dbo.AspNetUsers", "Id");
            this.AddForeignKey("dbo.CircleJoinRequests", "User_Id", "dbo.AspNetUsers", "Id");
            this.AddForeignKey("dbo.CircleInvitations", "User_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
