// <copyright file="ErrorController.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Controllers
{
    using Common.InterfaceText;
    using System.Web.Mvc;

    /// <summary>
    /// The error controller.
    /// </summary>
    public class ErrorController : Controller
    {
        /// <summary>
        /// The detail.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [AllowAnonymous]
        public ActionResult Detail(int id)
        {
            this.Response.StatusCode = id;

            switch (id)
            {
                case 400:
                    this.ViewBag.Title = Title.BadRequest;
                    break;
                case 403:
                    this.ViewBag.Title = Title.AccessDenied;
                    break;
                case 404:
                    this.ViewBag.Title = Title.FileNotFound;
                    break;
                default:
                    this.ViewBag.Title = Title.InternalServerError;
                    break;
            }

            return this.View("Error");
        }
    }
}