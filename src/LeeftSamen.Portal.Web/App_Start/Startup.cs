// <copyright file="Startup.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

using LeeftSamen.Portal.Web;

using Microsoft.Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace LeeftSamen.Portal.Web
{
    using System;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    using LeeftSamen.Portal.Data.Models;
    using LeeftSamen.Portal.Services;
    using LeeftSamen.Portal.Web.Helpers;

    using LightInject;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin;
    using Microsoft.Owin.Security.Cookies;

    using Owin;

    /// <summary>
    /// The startup.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// The configuration.
        /// </summary>
        /// <param name="app">
        /// The app.
        /// </param>
        public void Configuration(IAppBuilder app)
        {
            MvcHandler.DisableMvcResponseHeader = true;

            var container = LightInjectConfig.ConfigureContainer(app);

            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());

            FilterConfig.RegisterGlobalFilters(container, GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AutomapperConfig.ConfigureMappings();

            ConfigureAuth(app, container);
        }

        /// <summary>
        /// Configure Authentication.
        /// For more information on configuring authentication, please visit <a href="http://go.microsoft.com/fwlink/?LinkId=301864"></a>
        /// </summary>
        /// <param name="app">
        /// The app.
        /// </param>
        /// <param name="container">
        /// The container.
        /// </param>
        private static void ConfigureAuth(IAppBuilder app, IServiceFactory container)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(container.GetInstance<IUserService>);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(
                new CookieAuthenticationOptions
                    {
                        AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                        LoginPath = new PathString("/Account/Login"),
                        CookieName = "Authentication",
                        CookieHttpOnly = true,
                        CookieSecure = CookieSecureOption.Always,
                        Provider = CreateCookieAuthenticationProvider(),
                        ExpireTimeSpan = TimeSpan.FromDays(365),
                        SlidingExpiration = true
                    });
        }

        /// <summary>
        /// The create cookie authentication provider.
        /// </summary>
        /// <returns>
        /// The <see cref="ICookieAuthenticationProvider"/>.
        /// </returns>
        private static ICookieAuthenticationProvider CreateCookieAuthenticationProvider()
        {
            // Enables the application to validate the security stamp when the user logs in.
            // This is a security feature which is used when you change a password or add an external login to your account.
            // NOTE: Custom OnApplyRedirect is needed to prevent OWIN from redirecting AJAX-requests to the login page. Instead
            // return a 401 response. See: http://brockallen.com/2013/10/27/using-cookie-authentication-middleware-with-web-api-and-401-response-codes/
            return new CookieAuthenticationProvider
                       {
                           OnValidateIdentity =
                               SecurityStampValidator
                               .OnValidateIdentity<UserManager<User>, User>(
                                   TimeSpan.FromMinutes(30),
                                   (manager, user) =>
                                   user.GenerateUserIdentityAsync(manager)),
                           OnApplyRedirect = context =>
                               {
                                   if (!IsAjaxRequest(context.Request))
                                   {
                                       context.Response.Redirect(context.RedirectUri);
                                   }
                               }
                       };
        }

        /// <summary>
        /// The is ajax request.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool IsAjaxRequest(IOwinRequest request)
        {
            if ((request.Query != null) && (request.Query["X-Requested-With"] == "XMLHttpRequest"))
            {
                return true;
            }

            return (request.Headers != null) && (request.Headers["X-Requested-With"] == "XMLHttpRequest");
        }
    }
}