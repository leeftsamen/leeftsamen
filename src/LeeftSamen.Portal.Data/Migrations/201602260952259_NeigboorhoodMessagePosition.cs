// <copyright file="201602260952259_NeigboorhoodMessagePosition.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Spatial;

    public partial class NeigboorhoodMessagePosition : DbMigration
    {
        public override void Up()
        {
            this.AddColumn("dbo.NeighborhoodMessages", "Position", c => c.Geography());
            this.Sql("UPDATE NeighborhoodMessages SET NeighborhoodMessages.Position = AspNetUsers.Position FROM NeighborhoodMessages INNER JOIN AspNetUsers ON NeighborhoodMessages.Creator_Id = AspNetUsers.Id");
        }

        public override void Down()
        {
            this.DropColumn("dbo.NeighborhoodMessages", "Position");
        }
    }
}
