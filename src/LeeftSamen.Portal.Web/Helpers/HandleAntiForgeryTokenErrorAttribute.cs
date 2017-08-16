// <copyright file="HandleAntiForgeryTokenErrorAttribute.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Helpers
{
    using System.Web.Mvc;

    using Mindscape.Raygun4Net;

    /// <summary>
    /// The handle anti forgery token error attribute.
    /// </summary>
    public class HandleAntiforgeryTokenErrorAttribute : HandleErrorAttribute
    {
        /// <summary>
        /// The on exception.
        /// </summary>
        /// <param name="filterContext">
        /// The filter context.
        /// </param>
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }

            if (filterContext.Exception == null || filterContext.Exception.GetType() != typeof(HttpAntiForgeryException))
            {
                return;
            }

            // Mark the AntiforgeryTokenException as handled so the user doesn't get a 500 error. Instead
            // redirect the user to the current page.
            filterContext.ExceptionHandled = true;
            filterContext.Result = new RedirectToRouteResult(filterContext.RouteData.Values);

            // Still register the error in Raygun
            new RaygunClient().SendInBackground(filterContext.Exception);
        }
    }
}