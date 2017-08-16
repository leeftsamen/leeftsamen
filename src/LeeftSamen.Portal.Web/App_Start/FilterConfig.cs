// <copyright file="FilterConfig.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Web
{
    using System.Web.Mvc;

    using LeeftSamen.Portal.Web.Helpers;

    using LightInject;
    using Attributes;

    /// <summary>
    /// The filter config.
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// The register global filters.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="filters">
        /// The filters.
        /// </param>
        public static void RegisterGlobalFilters(IServiceFactory container, GlobalFilterCollection filters)
        {
            filters.Add(new AuthorizeAttribute());
            filters.Add(new HandleAntiforgeryTokenErrorAttribute { ExceptionType = typeof(HttpAntiForgeryException) });
            filters.Add(new ValidateProfileAttribute());
        }
    }
}