// <copyright file="Configuration.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Data.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Linq;

    using LeeftSamen.Portal.Data.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        private bool pendingMigrations;

        public Configuration()
        {
            // Don't use auto migrations. Follow these steps instead:
            // * Add migrations when needed
            // * Run Update-Database local
            // To update server databases:
            // * Create a database script by running: Update-Database -Script -SourceMigration [source] -TargetMigration [target]
            // * Review the script
            // * Update the server database using the generated and reviewed script
            this.AutomaticMigrationsEnabled = false;
            this.AutomaticMigrationDataLossAllowed = false;

            // This flag is used to trigger seeding the database only when there is at least one migration. This
            // way we prevent seeding the database every time the applications starts.
            this.pendingMigrations = new DbMigrator(this).GetPendingMigrations().Any();
        }

        protected override void Seed(ApplicationDbContext context)
        {
            if (!this.pendingMigrations)
            {
                return;
            }

            context.MarketplaceItemCategories.AddOrUpdate(
                c => c.Alias,
                new MarketplaceItemCategory { Alias = MarketplaceItemCategory.CategoryAlias.Stuff, SortOrder = 10, Name = "Spullen te koop", Title = "Ik wil spullen kopen of verkopen", Description = string.Empty },
                new MarketplaceItemCategory { Alias = MarketplaceItemCategory.CategoryAlias.Borrowing, SortOrder = 20, Name = "Spullen te leen", Title = "Ik wil spullen lenen of uitlenen", Description = string.Empty },
                new MarketplaceItemCategory { Alias = MarketplaceItemCategory.CategoryAlias.Meals, SortOrder = 30, Name = "Maaltijden", Title = "Ik wil een maaltijd afhalen of aanbieden", Description = string.Empty },
                new MarketplaceItemCategory { Alias = MarketplaceItemCategory.CategoryAlias.HelpNeighborhood, SortOrder = 40, Name = "Burenhulp", Title = "Ik wil buren helpen of hulp hebben", Description = string.Empty });

            context.OrganizationTypes.AddOrUpdate(
                c => c.Name,
                new OrganizationType { Name = "Professionele organisatie", Type = OrganizationType.Types.Professional },
                new OrganizationType { Name = "Vereniging", Type = OrganizationType.Types.Association },
                new OrganizationType { Name = "Vrijwilligersorganisatie", Type = OrganizationType.Types.Volunteer });
        }
    }
}