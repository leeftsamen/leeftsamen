// <copyright file="ReportAbuseController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Services;
    using LeeftSamen.Portal.Web.Models.ReportAbuse;
    using LeeftSamen.Portal.Web.Utils;

    [Authorize]
    public class ReportAbuseController : BaseController
    {
        private readonly IReportAbuseService reportAbuseService;

        public ReportAbuseController(ICurrentUserInformation currentUserInformation, IReportAbuseService reportAbuseService)
            : base(currentUserInformation)
        {
            this.reportAbuseService = reportAbuseService;
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
        public async Task<ActionResult> Index(ReportAbusePostModel model)
        {
            var forUrl = this.Request.UrlReferrer != null ? this.Request.UrlReferrer.AbsoluteUri : string.Empty;
            await this.reportAbuseService.ReportAbuseAsync(
                this.CurrentUser,
                model.ReportDescription,
                forUrl);

            this.NotifyUserSuccess(Alert.ReportAbuseReported);

            return this.Index();
        }
    }
}