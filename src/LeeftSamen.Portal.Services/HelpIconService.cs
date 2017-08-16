// <copyright file="HelpIconService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data;
    using LeeftSamen.Portal.Data.Models;

    public class HelpIconService : IHelpIconService
    {
        private readonly IApplicationDbContext databaseContext;

        public HelpIconService(IApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<HelpIcon> GetIconByIdAsync(int id)
        {
            return await this.databaseContext.HelpIcons.FirstOrDefaultAsync(i => i.HelpIconId == id).ConfigureAwait(false);
        }

        public async Task<List<HelpIcon>> GetNonShownHelpIconsForUserAsync(User user)
        {
            return
                await
                this.databaseContext.HelpIcons.Where(h => h.ShownIcons.All(s => s.User.Id != user.Id))
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task SetHelpIconAsShownAsync(HelpIcon helpIcon, User user)
        {
            var helpIconUser =
                await
                this.databaseContext.HelpIconUsers.FirstOrDefaultAsync(
                    h => h.HelpIcon.HelpIconId == helpIcon.HelpIconId && h.User.Id == user.Id).ConfigureAwait(false);
            if (helpIconUser != null)
            {
                // Already shown
                return;
            }

            helpIconUser = this.databaseContext.HelpIconUsers.Create();
            helpIconUser.HelpIcon = helpIcon;
            helpIconUser.User = user;
            this.databaseContext.HelpIconUsers.Add(helpIconUser);

            await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}