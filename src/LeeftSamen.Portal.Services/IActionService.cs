// <copyright file="IActionService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data.Models;

    public interface IActionService
    {
        //Task<List<Data.Models.Action>> GetActionsAsync(User user, int limit = 10);

        Task<Data.Models.Action> GetActionByIdAsync(User user, int? actionId);

        Task<List<Organization>> GetOrganizationsByActionIdAsync(User user, int? actionId);

        Task<ActionVote> CreateVote(Data.Models.Action action, Organization organization, User creator);
    }
}
