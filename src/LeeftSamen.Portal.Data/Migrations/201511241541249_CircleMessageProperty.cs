// <copyright file="201511241541249_CircleMessageProperty.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class CircleMessageProperty : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.CircleMessages", "IsHidden", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            this.DropColumn("dbo.CircleMessages", "IsHidden");
        }
    }
}
