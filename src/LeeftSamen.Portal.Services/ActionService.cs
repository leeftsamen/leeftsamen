// <copyright file="ActionService.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System.Data.Entity.Infrastructure;

namespace LeeftSamen.Portal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    using LeeftSamen.Portal.Data;
    using LeeftSamen.Portal.Data.Models;

    public class ActionService : IActionService
    {
        private readonly IApplicationDbContext databaseContext;

        public ActionService(IApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        private IQueryable<Data.Models.Action> ActiveActions
        {
            get
            {
                var now = DateTime.Now;
                return this.databaseContext.Actions.Where(
                    a =>
                    (!a.ActionStart.HasValue || a.ActionStart <= now) &&
                    (!a.ActionEnd.HasValue || a.ActionEnd >= now));
            }
        }

        private IQueryable<Organization> ApprovedOrganizations
        {
            get
            {
                return this.databaseContext.Organizations.Where(o => !o.IsRequestPending);
            }
        }

        //private IQueryable<Data.Models.Action> ActiveActionsForUser(User user)
        //{
        //    return this.ActiveActions.Where(
        //        a =>
        //            a.Zipcodes.Any(z => z.PostalCode == user.PostalCode));
        //}

        //public async Task<List<Data.Models.Action>> GetActionsAsync(User user, int limit = 10)
        //{
        //    return
        //        await
        //        this.ActiveActionsForUser(user).Take(limit)
        //            .ToListAsync()
        //            .ConfigureAwait(false);
        //}

        //public async Task<Data.Models.Action> GetActionByIdAsync(User user, int? actionId)
        //{
        //    return
        //        await
        //        this.ActiveActionsForUser(user)
        //            .FirstOrDefaultAsync(o => o.ActionId == actionId)
        //            .ConfigureAwait(false);
        //}

        public async Task<List<Organization>> GetOrganizationsByActionIdAsync(User user, int? actionId)
        {
            return
                await
                this.ApprovedOrganizations
                    .Where(o => o.Hidden != true && o.Actions.Any(a => a.Action.ActionId == actionId))
                    .ToListAsync()
                    .ConfigureAwait(false);
        }

        public async Task<ActionVote> CreateVote(
            Data.Models.Action action,
            Organization organization,
            User creator)
        {
            var vote = this.databaseContext.ActionVotes.Create();
            vote.Action = action;
            vote.Creator = creator;
            vote.Organization = organization;
            this.databaseContext.ActionVotes.Add(vote);

            try
            {
                await this.databaseContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (DbUpdateException)
            {
                return null;
            }

            return vote;
        }

        public async Task<Data.Models.Action> GetActionByIdAsync(User user, int? actionId)
        {
            var now = DateTime.Now;
            return
                await
                this.databaseContext.Actions.Where(a => a.Zipcodes.Any(z => z.PostalCode == user.PostalCode))
                .FirstOrDefaultAsync(a => a.ActionId == actionId)
                    .ConfigureAwait(false);
        }
    }
}
