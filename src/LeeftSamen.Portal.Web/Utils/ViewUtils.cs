// <copyright file="ViewUtils.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web.Utils
{
    using System;
    using System.EnterpriseServices;
    using System.Security.Policy;
    using System.Web.Mvc;

    using LeeftSamen.Portal.Web.Models;

    /// <summary>
    /// The view utils.
    /// </summary>
    public static class ViewUtils
    {
        /// <summary>
        /// The get action.
        /// </summary>
        /// <param name="viewContext">
        /// The view context.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetAction(ControllerContext viewContext)
        {
            return viewContext.RouteData.Values["action"].ToString();
        }

        /// <summary>
        /// The get controller.
        /// </summary>
        /// <param name="viewContext">
        /// The view context.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetController(ControllerContext viewContext)
        {
            return viewContext.RouteData.Values["controller"].ToString();
        }

        /// <summary>
        /// The is active action.
        /// </summary>
        /// <param name="viewContext">
        /// The view context.
        /// </param>
        /// <param name="controlName">
        /// The control name.
        /// </param>
        /// <param name="actionName">
        /// The action name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsActiveAction(ControllerContext viewContext, string controlName, string actionName = "*")
        {
            return (string.Equals(GetAction(viewContext), actionName, StringComparison.CurrentCultureIgnoreCase)
                    || string.Equals(actionName, "*"))
                   && string.Equals(GetController(viewContext), controlName, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}