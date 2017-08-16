// <copyright file="IHelpIconService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data.Models;

    public interface IHelpIconService
    {
        Task<HelpIcon> GetIconByIdAsync(int id);

        Task<List<HelpIcon>> GetNonShownHelpIconsForUserAsync(User user);

        Task SetHelpIconAsShownAsync(HelpIcon helpIcon, User user);
    }
}