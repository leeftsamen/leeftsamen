// <copyright file="201605100958434_CircleEmailSetting.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class CircleEmailSetting : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.CircleMemberships", "ReceiveEmails", c => c.Boolean(nullable: false, defaultValue:true));
        }

        public override void Down()
        {
            this.DropColumn("dbo.CircleMemberships", "ReceiveEmails");
        }
    }
}
