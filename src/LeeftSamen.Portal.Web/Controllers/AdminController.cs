// <copyright file="AdminController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeeftSamen.Portal.Web.Controllers
{
    using System.Web.Mvc;

    using LeeftSamen.Portal.Services;
    using LeeftSamen.Portal.Web.Models.Admin;
    using LeeftSamen.Portal.Web.Utils;

    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly IOrganizationService organizationService;

        public AdminController(ICurrentUserInformation currentUserInformation, IOrganizationService organizationService)
            : base(currentUserInformation)
        {
            this.organizationService = organizationService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return this.View();
        }

        [HttpGet]
        public async Task<ActionResult> Organizations()
        {
            var organizations = await this.organizationService.GetAllOrganizationsInclPendingAsync();

            return this.View(new OrganizationsViewModel
                                 {
                                     Organizations = organizations.Select(o => new OrganizationsViewModel.Organization
                                                                                   {
                                                                                       Name = o.Name,
                                                                                       City = o.City,
                                                                                       IsRequestPending = o.IsRequestPending,
                                                                                       OrganizationId = o.OrganizationId,
                                                                                       RequestDateTime = o.RequestDateTime,
                                                                                       Email = o.Email,
                                                                                       Phone = o.Phone,
                                                                                       Website = o.Website
                                                                                   }).ToList()
                                 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Organizations(OrganizationsPostModel postModel)
        {
            var organization = await this.organizationService.GetOrganizationByIdInclPendingAsync(postModel.OrganizationId);
            if (organization == null)
            {
                return this.HttpNotFound();
            }

            if (organization.IsRequestPending)
            {
                await this.organizationService.ActivateOrganizationAsync(organization);
            }

            return this.Redirect(this.Url.Action("Organizations", "Admin"));
        }
    }
}
