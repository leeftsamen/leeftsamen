// <copyright file="ActionController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System.Data.SqlClient;

namespace LeeftSamen.Portal.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.ServiceModel.Syndication;
    using System.Threading.Tasks;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.UI;

    using AutoMapper;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Services;
    using LeeftSamen.Portal.Web.Models.Action;
    using LeeftSamen.Portal.Web.Models.Organizations;
    using LeeftSamen.Portal.Web.Utils;
    using RazorEngine.Compilation.ImpromptuInterface.Dynamic;
    using System.Configuration;

    public class ActionController : BaseController
    {
        private readonly IOrganizationService organizationService;
        private readonly IActionService actionService;

        public ActionController(
            ICurrentUserInformation currentUserInformation,
            IOrganizationService organizationService,
            IActionService actionService)
            : base(currentUserInformation)
        {
            this.organizationService = organizationService;
            this.actionService = actionService;
        }

        // GET: Action
        public async Task<ActionResult> Index(int? id)
        {
            //var action = this.CurrentUserInformation.UserActions.FirstOrDefault(a => a.ActionId == id);
            var action = await this.actionService.GetActionByIdAsync(this.CurrentUser, id);
            //var organizations = new List<IndexVoteModel.OrganizationViewModel>();
            var organizations = await this.actionService.GetOrganizationsByActionIdAsync(this.CurrentUser, id);

            if (action == null)
            {
                return this.HttpNotFound();
            }

            //if (action.ActionEnd.HasValue && action.ActionEnd.Value < DateTime.Now)
            //{
            //    this.NotifyUserDanger(Alert.ActionExpired);
            //    return this.RedirectToAction("Index", "Home");
            //}

            var org = organizations.Select(
                o =>
                new IndexVoteModel.OrganizationViewModel
                {
                    Name = o.Name,
                    Description = o.Description,
                    LogoId = o.LogoId,
                    OrganizationId = o.OrganizationId,
                    OrganizationTypeName = o.OrganizationType.Name,
                    Collected = o.Votes.Count(v => v.Action == action) * action.MoneyPerVote
                }
            ).ToList();
            return this.View(
                new IndexVoteModel
                {
                    ActionId = action.ActionId,
                    Name = action.Name,
                    Title = action.Title,
                    Description = action.Description,
                    ActionEnded = (action.ActionEnd ?? DateTime.MinValue) < DateTime.Now.Date,
                    ActionStarted = (action.ActionStart ?? DateTime.MinValue) < DateTime.Now.Date,
                    MaxVotesReached = action.Votes.Count * action.MoneyPerVote >= action.MoneyAvailable,
                    HasVoted = action.Votes.Any(v => v.Creator == this.CurrentUser),
                    Organizations = org
                });
        }

        [ChildActionOnly]
        public ActionResult MenuActions()
        {
            var actionItems = this.CurrentUserInformation.UserActions.Select(
                a =>
                    new ActionsMenuModel.ActionMenuItem
                    {
                        ActionId = a.ActionId,
                        MenuItem = a.MenuText
                    }
                ).ToList();
            return this.PartialView(
                string.Format("_MenuActions{0}", ConfigurationManager.AppSettings["ShowRestyle"] == "true" ? "Restyle" : string.Empty),
                new ActionsMenuModel
                    {
                        Menus = actionItems
                    });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Vote(int? id, int? organizationId)
        {
            var action = this.CurrentUserInformation.UserActions.FirstOrDefault(a => a.ActionId == id);
            //var action = await this.actionService.GetActionByIdAsync(this.CurrentUser, id);
            var organization = await this.organizationService.GetOrganizationByIdAsync(organizationId);
            if (action == null || organization == null)
            {
                return this.HttpNotFound();
            }

            var vote = await this.actionService.CreateVote(action, organization, this.CurrentUser);
            if (vote == null)
            {
                this.NotifyUserDanger(Alert.VoteActionFailure);
                return this.RedirectToAction("Index", new {id = id});
            }
            else
            {
                this.NotifyUserSuccess(Alert.VoteActionSuccess);
                return this.RedirectToAction("Index", new { id = id });
            }
        }
    }
}