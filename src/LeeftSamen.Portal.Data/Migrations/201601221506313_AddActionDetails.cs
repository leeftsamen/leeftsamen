// <copyright file="201601221506313_AddActionDetails.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddActionDetails : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.Actions", "Title", c => c.String());
            this.AddColumn("dbo.Actions", "Description", c => c.String());
        }

        public override void Down()
        {
            this.DropColumn("dbo.Actions", "Description");
            this.DropColumn("dbo.Actions", "Title");
        }
    }
}
