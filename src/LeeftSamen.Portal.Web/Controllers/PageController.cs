// <copyright file="PageController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using LeeftSamen.Common.InterfaceText;
    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.Services;
    using LeeftSamen.Portal.Web.Models.Home;
    using LeeftSamen.Portal.Web.Utils;

    /// <summary>
    /// The page controller.
    /// </summary>
    public class PageController : BaseController
    {
        public PageController(ICurrentUserInformation currentUserInformation)
            : base(currentUserInformation)
        {        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult TermsAndConditions()
        {
            return this.View();
        }
    }
}