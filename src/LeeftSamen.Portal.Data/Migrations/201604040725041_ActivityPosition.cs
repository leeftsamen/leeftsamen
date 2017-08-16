// <copyright file="201604040725041_ActivityPosition.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Spatial;

    public partial class ActivityPosition : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.Activities", "Position", c => c.Geography());
            this.Sql("UPDATE Activities SET Activities.Position = AspNetUsers.Position FROM Activities INNER JOIN AspNetUsers ON Activities.Creator_Id = AspNetUsers.Id");
        }

        public override void Down()
        {
            this.DropColumn("dbo.Activities", "Position");
        }
    }
}
