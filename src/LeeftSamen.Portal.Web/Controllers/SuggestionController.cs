// <copyright file="SuggestionController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Common.InterfaceText;
using LeeftSamen.Portal.Web.Models.Suggestion;

namespace LeeftSamen.Portal.Web.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using LeeftSamen.Portal.Services;
    using LeeftSamen.Portal.Web.Utils;

    [Authorize]
    public class SuggestionController : BaseController
    {
        private readonly ISuggestionService _suggestionService;

        public SuggestionController(ICurrentUserInformation currentUserInformation, ISuggestionService suggestionService)
            : base(currentUserInformation)
        {
            this._suggestionService = suggestionService;
        }

        public ActionResult Index()
        {
            if (this.Request.UrlReferrer != null)
            {
                return this.Redirect(this.Request.UrlReferrer.AbsoluteUri);
            }

            return this.RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(SuggestionModalViewModel model)
        {
            await this._suggestionService.SendSuggestion(this.CurrentUser, model.Subject, model.Suggestion);

            this.NotifyUserSuccess(Alert.SuggestionSend);

            return this.Index();
        }
    }
}