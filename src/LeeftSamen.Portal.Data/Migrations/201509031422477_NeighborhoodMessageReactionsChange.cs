// <copyright file="201509031422477_NeighborhoodMessageReactionsChange.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class NeighborhoodMessageReactionsChange : DbMigration
    {
        public override void Up()
        {
            this.RenameColumn(table: "dbo.NeighborhoodMessageReactions", name: "ParentId", newName: "Reactions_ReactionId");
            this.RenameIndex(table: "dbo.NeighborhoodMessageReactions", name: "IX_ParentId", newName: "IX_Reactions_ReactionId");
        }

        public override void Down()
        {
            this.RenameIndex(table: "dbo.NeighborhoodMessageReactions", name: "IX_Reactions_ReactionId", newName: "IX_ParentId");
            this.RenameColumn(table: "dbo.NeighborhoodMessageReactions", name: "Reactions_ReactionId", newName: "ParentId");
        }
    }
}
