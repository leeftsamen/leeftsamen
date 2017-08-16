// <copyright file="ValidateProfileAttribute.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Attributes
{
    using LeeftSamen.Portal.Web.Controllers;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    public class ValidateProfileAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                BaseController controller = filterContext.Controller as BaseController;
                if (controller != null)
                {
                    if (!filterContext.IsChildAction && !controller.IsUserProfileValid()
                        && !(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower() == "register")
                        && !(filterContext.ActionDescriptor.ActionName.ToLower() == "logoff" && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower() == "account"))
                    {
                        // Do redirect;
                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(new { action = "Address", controller = "Register" }));
                    }
                }
            }

            base.OnAuthorization(filterContext);
        }
    }
}