// <copyright file="HelpIconController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using LeeftSamen.Portal.Services;
    using LeeftSamen.Portal.Web.Models;
    using LeeftSamen.Portal.Web.Utils;

    public class HelpIconController : BaseController
    {
        private readonly IHelpIconService helpIconService;

        public HelpIconController(ICurrentUserInformation currentUserInformation, IHelpIconService helpIconService)
            : base(currentUserInformation)
        {
            this.helpIconService = helpIconService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetIconIsShown(int id)
        {
            var icon = await this.helpIconService.GetIconByIdAsync(id);
            if (icon == null)
            {
                return this.HttpNotFound();
            }

            await this.helpIconService.SetHelpIconAsShownAsync(icon, this.CurrentUser);

            return this.SetIconIsShown();
        }

        [HttpGet]
        public ActionResult SetIconIsShown()
        {
            return new EmptyResult();
        }

        [ChildActionOnly]
        public ActionResult HomePageIcon()
        {
            var icons = this.helpIconService.GetNonShownHelpIconsForUserAsync(this.CurrentUser).Result;
            var icon = icons.FirstOrDefault(i => i.Type == "home");
            if (icon == null)
            {
                return new EmptyResult();
            }

            var model = new HelpIconViewModel
                            {
                                Id = icon.HelpIconId,
                                Placement = icon.TextPlacement,
                                HelpText = icon.Text,
                                CssClass = "startpage"
                            };
            return this.PartialView("_HelpIcon", model);
        }
    }
}