// <copyright file="201509011335560_EmailSettings.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class EmailSettings : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.AspNetUsers", "ReceivesWeekMail", c => c.Boolean(nullable: false));
            this.AddColumn("dbo.AspNetUsers", "ReceivesMarketplaceMail", c => c.Boolean(nullable: false));
            this.AddColumn("dbo.AspNetUsers", "ReceivesCircleMessageMail", c => c.Boolean(nullable: false));
            this.AddColumn("dbo.AspNetUsers", "ReceivesCircleJobMail", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            this.DropColumn("dbo.AspNetUsers", "ReceivesCircleJobMail");
            this.DropColumn("dbo.AspNetUsers", "ReceivesCircleMessageMail");
            this.DropColumn("dbo.AspNetUsers", "ReceivesMarketplaceMail");
            this.DropColumn("dbo.AspNetUsers", "ReceivesWeekMail");
        }
    }
}
